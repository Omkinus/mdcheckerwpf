using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;

namespace mdcheckerwpf.MVVM.View
{
    public partial class DrawingsPage : UserControl, INotifyPropertyChanged
    {
        public DrawingsPage()
        {
            InitializeComponent();
            DataContext = this;
            DataItems = new ObservableCollection<DrawingData>();
        }

        public ObservableCollection<DrawingData> DataItems { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataItems.Clear();

            var model = new tsm.Model();
            var drawingHandler = new DrawingHandler();

            if (!model.GetConnectionStatus() || !drawingHandler.GetConnectionStatus())
                return;

            var selectedDrawings = drawingHandler.GetDrawingSelector().GetSelected();

            foreach (Drawing drawing in selectedDrawings)
            {
                string drawingMark = drawing.Mark;
                string drawingName = drawing.Name;

                CheckPrecision(drawing, drawingMark, drawingName);
                CheckReflectedView(drawing, drawingMark, drawingName);
            }
        }

        private void CheckPrecision(Drawing drawing, string drawingMark, string drawingName)
        {
            if (drawing == null) return;

            string errorMessage = string.Empty;

            foreach (Tekla.Structures.Drawing.ViewBase view in drawing.GetSheet().GetAllViews())
            {
                foreach (var obj in view.GetAllObjects())
                {
                    if (obj is StraightDimensionSet dimensionSet)
                    {
                        if (dimensionSet.Attributes.Format.Precision != DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerSixteen)
                        {
                            errorMessage = "Присутствует размер округленный не на 1/16";
                            break;
                        }
                    }
                    else if (obj is AngleDimension angleDimension)
                    {
                        if (angleDimension.Attributes.Format.Precision != AngleDimensionAttributes.DimensionValuePrecisions.OnePerHundred)
                        {
                            errorMessage = "Присутствует угловой размер округленный не на 1/100";
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    AddDrawingError(drawingMark, drawingName, errorMessage);
                    break;
                }
            }
        }

        private void CheckReflectedView(Drawing drawing, string drawingMark, string drawingName)
        {
            if (drawing == null) return;

            string reflectedViewNames = string.Empty;

            foreach (Tekla.Structures.Drawing.View view in drawing.GetSheet().GetAllViews())
            {
                if (view.Attributes.ReflectedView)
                {
                    string viewName = string.IsNullOrWhiteSpace(view.Name) ? "Главный вид" : view.Name;
                    reflectedViewNames += $"{viewName} ";
                }
            }

            if (!string.IsNullOrEmpty(reflectedViewNames))
            {
                AddDrawingError(drawingMark, drawingName, $"Присутствует вид с включенным Reflected View: {reflectedViewNames.Trim()}");
            }
        }


        private void AddDrawingError(string drawingMark, string drawingName, string errorMessage) =>
            DataItems.Add(new DrawingData
            {
                DrawingMark = drawingMark,
                DrawingName = drawingName,
                Details = errorMessage
            });
    }

    public class DrawingData
    {
        public string DrawingMark { get; set; }
        public string DrawingName { get; set; }
        public string Details { get; set; }
    }
}

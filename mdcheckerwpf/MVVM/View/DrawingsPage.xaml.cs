using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
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

        private void SaveDrawingsReportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToTxtFile();
        }

        private void SaveToTxtFile()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Сохранить данные",
                FileName = "DrawingsReport.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                StringBuilder sb = new StringBuilder();

               
                sb.AppendLine("Марка\tИмя\tПодробности");

               
                foreach (var item in DataItems)
                {
                    sb.AppendLine($"{item.DrawingMark}\t{item.DrawingName}\t{item.Details}");
                }

                // Запись данных в файл
                File.WriteAllText(filePath, sb.ToString());
                MessageBox.Show("Данные успешно сохранены", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadDrawingsReportButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFromTxtFile();
        }

        private void LoadFromTxtFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Выберите файл для загрузки"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Читаем все строки из файла
                    string[] lines = File.ReadAllLines(filePath);
                    DataItems.Clear();

                    // Пропустим заголовок и загрузим данные
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] columns = lines[i].Split('\t'); 

                       
                        if (columns.Length >= 3)
                        {
                            var drawingData = new DrawingData
                            {
                                DrawingMark = columns[0],
                                DrawingName = columns[1],
                                Details = columns[2]
                            };

                            DataItems.Add(drawingData); 
                        }
                    }

                    MessageBox.Show("Данные успешно загружены", "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class DrawingData
    {
        public string DrawingMark { get; set; }
        public string DrawingName { get; set; }
        public string Details { get; set; }
    }
}

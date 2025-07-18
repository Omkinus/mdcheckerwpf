using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using tsm = Tekla.Structures.Model;
using System.Threading.Tasks;
using System.Linq;
using DocumentFormat.OpenXml.EMMA;
using Tekla.Structures;
using ModelObject = Tekla.Structures.Model.ModelObject;
using Part = Tekla.Structures.Drawing.Part;
using System.Collections.Generic;

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

        private string modelName;

        public string ModelName
        {
            get { return modelName; }
            set
            {
                if (modelName != value)
                {
                    modelName = value;
                    OnPropertyChanged(nameof(ModelName));
                }
            }
        }

        public ObservableCollection<DrawingData> DataItems { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            DataItems.Clear();

            var model = new tsm.Model();
            var drawingHandler = new DrawingHandler();
            ProjectInfo projectInfo = model.GetProjectInfo();

            if (!model.GetConnectionStatus() || !drawingHandler.GetConnectionStatus())
                return;

            ModelName = model.GetInfo().ModelName.TrimEnd(".db1".ToCharArray());

            var selectedDrawings = drawingHandler.GetDrawingSelector().GetSelected();
            int totalDrawings = selectedDrawings.GetSize();
            int currentDrawing = 0;

            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.Value = 0;
            ProgressText.Visibility = Visibility.Visible;
            ProgressText.Text = "Проверка чертежей...";

            foreach (Drawing drawing in selectedDrawings)
            {
                string drawingMark = drawing.Mark;
                string drawingName = drawing.Name;

                CheckPrecision(drawing, drawingMark, drawingName);
                CheckReflectedView(drawing, drawingMark, drawingName);
                CheckDrawnByCheckBy(drawing, drawingMark, drawingName, projectInfo);
                CheckAssembliesMissingPartMarks(drawing, drawingMark, drawingName);

                currentDrawing++;
                ProgressBar.Value = (double)currentDrawing / totalDrawings * 100;
                ProgressText.Text = $"Проверено {currentDrawing} из {totalDrawings} чертежей";

                await System.Threading.Tasks.Task.Delay(10);
            }

            ProgressText.Text = "Проверка завершена!";
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressText.Visibility = Visibility.Collapsed;
        }

        private void CheckPrecision(Drawing drawing, string drawingMark, string drawingName)
        {
            if (drawing == null) return;

            foreach (Tekla.Structures.Drawing.ViewBase view in drawing.GetSheet().GetAllViews())
            {
                foreach (var obj in view.GetAllObjects())
                {
                    if (obj is StraightDimensionSet dimensionSet &&
                        dimensionSet.Attributes.Format.Precision != DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerSixteen)
                    {
                        AddDrawingError(drawingMark, drawingName, "Присутствует размер округленный не на 1/16","Неправильное округление");
                        return;
                    }

                    if (obj is AngleDimension angleDimension &&
                        angleDimension.Attributes.Format.Precision != AngleDimensionAttributes.DimensionValuePrecisions.OnePerHundred)
                    {
                        AddDrawingError(drawingMark, drawingName, "Присутствует угловой размер округленный не на 1/100","Неправильное округление");
                        return;
                    }
                }
            }
        }

        private void CheckReflectedView(Drawing drawing, string drawingMark, string drawingName)
        {
            if (drawing == null) return;

            foreach (Tekla.Structures.Drawing.View view in drawing.GetSheet().GetAllViews())
            {
                if (view.Attributes.ReflectedView)
                {
                    string viewName = string.IsNullOrWhiteSpace(view.Name) ? "Главный вид" : view.Name;
                    AddDrawingError(drawingMark, drawingName, $"Присутствует вид с включенным Reflected View: {viewName}","Reflected view");
                }
            }
        }

        private void CheckDrawnByCheckBy(Drawing drawing, string drawingMark, string drawingName, ProjectInfo projectInfo)
        {
            string drawnBy = string.Empty;
            string checkedBy = string.Empty;
            string projectDrawnby = string.Empty;
            string projectCheckedby = string.Empty;

            drawing.GetUserProperty("DR_DRAWN_BY", ref drawnBy);
            drawing.GetUserProperty("DR_CHECKED_BY", ref checkedBy);
            projectInfo.GetUserProperty("FF_DR_BY", ref projectDrawnby);
            projectInfo.GetUserProperty("FF_CH_BY", ref projectCheckedby);



            if (string.IsNullOrEmpty(projectDrawnby))
                AddDrawingError(drawingMark, drawingName, "На чертеже не заполнено поле DrawnBy","Незаполненные поля");

            if (string.IsNullOrEmpty(projectCheckedby))
                AddDrawingError(drawingMark, drawingName, "На чертеже не заполнено поле CheckBy","Незаполненные поля");
        }

        private void CheckAssembliesMissingPartMarks(Drawing drawing, string drawingMark, string drawingName)
        {
            if (drawing == null) return;

            if (drawing is AssemblyDrawing)
            {
                var model = new tsm.Model();

                foreach (Tekla.Structures.Drawing.View view in drawing.GetSheet().GetAllViews())
                {
                    foreach (var obj in view.GetAllObjects())
                    {
                        if (obj is Tekla.Structures.Drawing.Part part)
                        {
                            Identifier partIdentifier = part.ModelIdentifier;

                            ModelObject modelObject = model.SelectModelObject(partIdentifier);
                            var modelPart = modelObject as Tekla.Structures.Model.Part;
                            if (modelPart == null)
                                continue;

                            string partMark = modelPart.GetPartMark();
                            bool markFoundForThisPart = false;

                            // Перебираем все марки на виде и ищем связанную с этой конкретной деталью
                            foreach (var objMark in view.GetObjects())
                            {
                                if (objMark is Tekla.Structures.Drawing.Mark mark)
                                {
                                    DrawingObjectEnumerator relatedObjects = mark.GetRelatedObjects();
                                    foreach (var relatedObj in relatedObjects)
                                    {
                                        if (relatedObj is Tekla.Structures.Drawing.Part relatedPart)
                                        {
                                            if (relatedPart.ModelIdentifier.ToString() == partIdentifier.ToString())
                                            {
                                                markFoundForThisPart = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (markFoundForThisPart)
                                        break;
                                }
                            }

                            if (!markFoundForThisPart)
                            {
                                string viewDisplayName = string.IsNullOrWhiteSpace(view.Name)
                                    ? "на главном виде"
                                    : $"на виде \"{view.Name}\"";

                                AddDrawingError(
                                    drawingMark,
                                    drawingName,
                                    $"Марка {partMark} не найдена на виде {viewDisplayName}","Отсутствует марка");
                            }
                        }
                    }
                }
            }
        }



        private void AddDrawingError(string drawingMark, string drawingName, string errorMessage,string errortype)
        {
            DataItems.Add(new DrawingData
            {
                DrawingMark = drawingMark,
                DrawingName = drawingName,
                Details = errorMessage,
                Errortype = errortype
            });
        }

        private void SaveDrawingsReportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Сохранить данные",
                FileName = "DrawingsReport.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Отчет");
                    worksheet.Cell(1, 1).Value = "Марка";
                    worksheet.Cell(1, 2).Value = "Имя";
                    worksheet.Cell(1, 3).Value = "Подробности";

                    int row = 2;
                    foreach (var item in DataItems)
                    {
                        worksheet.Cell(row, 1).Value = item.DrawingMark;
                        worksheet.Cell(row, 2).Value = item.DrawingName;
                        worksheet.Cell(row, 3).Value = item.Details;
                        row++;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(saveFileDialog.FileName);
                }

                MessageBox.Show("Данные успешно сохранены в Excel", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadDrawingsReportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Выберите файл для загрузки"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook(openFileDialog.FileName))
                    {
                        var worksheet = workbook.Worksheets.Worksheet(1);
                        DataItems.Clear();

                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            DataItems.Add(new DrawingData
                            {
                                DrawingMark = row.Cell(1).GetString(),
                                DrawingName = row.Cell(2).GetString(),
                                Details = row.Cell(3).GetString()
                            });
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
        public string Errortype { get; internal set; }
    }
}

using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using tsm = Tekla.Structures.Model;
using System.Threading.Tasks;

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

            if (!model.GetConnectionStatus() || !drawingHandler.GetConnectionStatus())
                return;

            ModelName = model.GetInfo().ModelName.Substring(0, model.GetInfo().ModelName.Length - 4);

            var selectedDrawings = drawingHandler.GetDrawingSelector().GetSelected();
            int totalDrawings = selectedDrawings.GetSize();
            int currentDrawing = 0;

            // Показываем прогресс-бар и текст
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
                CheckDrawnByCheckBy(drawing, drawingMark, drawingName);

                // Обновляем прогресс
                currentDrawing++;
                ProgressBar.Value = (double)currentDrawing / totalDrawings * 100;
                ProgressText.Text = $"Проверено {currentDrawing} из {totalDrawings} чертежей";

                // Даем время UI обновиться
                await System.Threading.Tasks.Task.Delay(10);
            }

            ProgressText.Text = "Проверка завершена!";
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressText.Visibility = Visibility.Collapsed;
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

        private void CheckDrawnByCheckBy(Drawing drawing, string drawingMark, string drawingName)
        {
            string drawnby = string.Empty;
            string checkby = string.Empty;
            string projectdrawnby = string.Empty;
            string projectcheckby = string.Empty;

            drawing.GetUserProperty("DR_DRAWN_BY", ref drawnby);
            drawing.GetUserProperty("DR_CHECKED_BY", ref checkby);

            var model = new tsm.Model();
            ProjectInfo projectInfo = model.GetProjectInfo();

            projectInfo.GetUserProperty("FF_DR_BY", ref projectdrawnby);
            projectInfo.GetUserProperty("FF_CH_BY", ref projectcheckby);

            // Проверка, если поле в проекте не заполнено, проверяем соответствующее поле на чертеже
            if (string.IsNullOrEmpty(projectdrawnby) && string.IsNullOrEmpty(drawnby))
            {
                AddDrawingError(drawingMark, drawingName, "На чертеже не заполнено поле DrawnBy");
            }

            if (string.IsNullOrEmpty(projectcheckby) && string.IsNullOrEmpty(checkby))
            {
                AddDrawingError(drawingMark, drawingName, "На чертеже не заполнено поле CheckBy");
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
            SaveToExcelFile();
        }

        private void SaveToExcelFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Сохранить данные",
                FileName = "DrawingsReport.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Отчет");

                    // Заголовки столбцов
                    worksheet.Cell(1, 1).Value = "Марка";
                    worksheet.Cell(1, 2).Value = "Имя";
                    worksheet.Cell(1, 3).Value = "Подробности";

                    // Заполнение данных из DataItems
                    int row = 2;
                    foreach (var item in DataItems)
                    {
                        worksheet.Cell(row, 1).Value = item.DrawingMark;
                        worksheet.Cell(row, 2).Value = item.DrawingName;
                        worksheet.Cell(row, 3).Value = item.Details;
                        row++;
                    }

                    // Установка ширины колонок по содержимому
                    worksheet.Columns().AdjustToContents();

                    // Сохранение файла
                    workbook.SaveAs(filePath);
                    MessageBox.Show("Данные успешно сохранены в Excel", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void LoadDrawingsReportButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFromExcelFile();
        }

        private void LoadFromExcelFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Выберите файл для загрузки"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    using (var workbook = new XLWorkbook(filePath))
                    {
                        var worksheet = workbook.Worksheets.Worksheet(1); // Открываем первый лист

                        // Читаем данные из Excel
                        var rows = worksheet.RowsUsed();
                        DataItems.Clear();

                        foreach (var row in rows.Skip(1)) // Пропускаем заголовок
                        {
                            var drawingData = new DrawingData
                            {
                                DrawingMark = row.Cell(1).GetString(),
                                DrawingName = row.Cell(2).GetString(),
                                Details = row.Cell(3).GetString()
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

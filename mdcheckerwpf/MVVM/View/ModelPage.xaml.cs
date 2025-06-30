using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using Part = Tekla.Structures.Model.Part;
using tsm = Tekla.Structures.Model;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Tekla.Structures;
using System.Linq;
using static mdcheckerwpf.MVVM.View.ModelPage;
using DocumentFormat.OpenXml.EMMA;
using System.Text.RegularExpressions;


namespace mdcheckerwpf.MVVM.View
{
    public partial class ModelPage : UserControl, INotifyPropertyChanged
    {
        public ModelPage()
        {
            InitializeComponent();
            DataContext = this;
            DataItems = new ObservableCollection<ModelData>();
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


        public ObservableCollection<ModelData> DataItems { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Показываем прогресс-бар
            ProgressBar.Visibility = Visibility.Visible;
            StatusMessage.Visibility = Visibility.Visible;
            ProgressBar.Value = 0; // Начальное значение прогресса

            // Выполнение долгих операций в фоновом потоке
            await Task.Run(() =>
            {
                var model = new tsm.Model();
                ModelName = model.GetInfo().ModelName.Substring(0, model.GetInfo().ModelName.Length - 4);

                if (!model.GetConnectionStatus()) return;

                var projectInfo = model.GetProjectInfo();
                var numericFields = new Dictionary<string, double>();
                var stringFields = new Dictionary<string, string>();

                // Числовые поля (длины)
                string[] numericFieldNames = {
                    "ANGLES_LENGTH",
                    "PLATES_LENGTH",
                    "BENTPLATES_LENGTH",
                    "ESDBOLTLENGTH"
                };

                string[] stringFieldNames = {
                    "PROJECT_USERFIELD_1", "PROJECT_USERFIELD_2", "PROJECT_USERFIELD_3", "PROJECT_USERFIELD_4",
                    "PROJECT_USERFIELD_5", "PROJECT_USERFIELD_6", "PROJECT_USERFIELD_7", "PROJECT_USERFIELD_8"
                };

                // Обработка числовых полей
                foreach (var fieldName in numericFieldNames)
                {
                    double fieldValue = double.NaN;
                    if (projectInfo.GetUserProperty(fieldName, ref fieldValue))
                    {
                        numericFields[fieldName] = fieldValue;
                    }
                    else
                    {
                        MessageBox.Show($"Поле {fieldName} в Project properties не заполнено или имеет неверный формат.", "Ошибка");
                        return;
                    }
                    
                }

                // Обработка строковых полей
                foreach (var fieldName in stringFieldNames)
                {
                    string fieldValue = string.Empty;
                    if (projectInfo.GetUserProperty(fieldName, ref fieldValue))
                    {
                        stringFields[fieldName] = fieldValue;
                    }
                    else
                    {
                        MessageBox.Show($"Поле {fieldName} в Project properties не заполнено.", "Ошибка");
                        return;
                    }
                   
                }

                // Загружаем список чертежей один раз
                var drawingHandler = new DrawingHandler();
                var allDrawings = new List<Drawing>();
                var drawingsEnum = drawingHandler.GetDrawings();
                while (drawingsEnum.MoveNext())
                {
                    allDrawings.Add(drawingsEnum.Current);

                    UpdateProgressBar(20); 
                }

                // Обрабатываем выбранные объекты
                var selectedModelObjects = new Tekla.Structures.Model.UI.ModelObjectSelector().GetSelectedObjects();
                int totalObjects = selectedModelObjects.GetSize();
                int processedObjects = 0;

                foreach (var item in selectedModelObjects)
                {
                    if (item is Part part)
                    {
                        string objectName = part.Name;
                        string objectNumber = part.GetPartMark();
                        string guid = part.Identifier.GUID.ToString();

                        CheckPartLength(part, objectName, objectNumber, numericFields,guid);
                        CheckMaterialPart(part, objectName, objectNumber, stringFields,guid);
                        CheckDrawingsForPart(part, objectName, objectNumber, allDrawings,guid);
                    }
                    else if (item is BoltGroup bolt)
                    {
                        string guid = bolt.Identifier.GUID.ToString();
                        CheckBoltsLength(bolt,guid);
                        CheckScrewsWithScrewsAndWashers(bolt,guid);
                    }

                    processedObjects++;
                    // Обновление прогресса
                    double progress = (processedObjects / (double)totalObjects) * 100;
                    UpdateProgressBar(progress);
                }
            });

        
            ProgressBar.Visibility = Visibility.Collapsed;
            StatusMessage.Visibility = Visibility.Collapsed;
        }

        // Метод для обновления прогресса
        private void UpdateProgressBar(double value)
        {
            // Проверяем, что мы находимся в UI-потоке
            if (ProgressBar.Dispatcher.CheckAccess())
            {
                ProgressBar.Value = value;
            }
            else
            {
                ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = value);
            }
        }

        private void CheckDrawingsForPart(Part part, string objectName, string objectNumber, List<Drawing> allDrawings,string guid)
        {
            bool singlePartDrawingFound = false;
            bool assemblyDrawingFound = false;
            bool isMainPart = part.GetAssembly().GetMainPart().Identifier.Equals(part.Identifier);

            foreach (var drawing in allDrawings)
            {
                if (drawing is SinglePartDrawing)
                {
                    SinglePartDrawing singlePartDrawing = drawing as SinglePartDrawing;
                   
                    if (Regex.Replace(singlePartDrawing.Mark.Trim(), @"[^a-zA-Z0-9]", "")
                        .Equals(
                            Regex.Replace(part.GetPartMark().ToString().Trim(), @"[^a-zA-Z0-9]", ""),
                            StringComparison.OrdinalIgnoreCase
                        ))
                    {
                        singlePartDrawingFound = true;
                    }    
                }
                else if (isMainPart && drawing is AssemblyDrawing)
                {
                    AssemblyDrawing assemblyDrawing = drawing as AssemblyDrawing;

                    if (Regex.Replace(assemblyDrawing.Mark.Trim(), @"[^a-zA-Z0-9]", "")
                        .Equals(
                            Regex.Replace(part.GetPartMark().ToString().Trim(), @"[^a-zA-Z0-9]", ""),
                            StringComparison.OrdinalIgnoreCase
                        ))
                    {
                        assemblyDrawingFound = true;
                    }
                }

                if (singlePartDrawingFound && (assemblyDrawingFound || !isMainPart))
                {
                    break;
                }
            }

            if (!singlePartDrawingFound)
            {
                AddModelError(objectName, objectNumber, "Отсутствует Single Part чертёж",guid,"Отсутствует чертеж Single Part" );
            }

            if (isMainPart && !assemblyDrawingFound)
            {
                AddModelError(objectName, objectNumber, "Отсутствует Assembly чертёж",guid, "Отсутствует чертеж Assembly");
            }
        }

         private void CheckMaterialPart(Part part, string objectName, string objectNumber, Dictionary<string, string> userFields,string guid)
        {
            string profileType = string.Empty;
            part.GetReportProperty("PROFILE_TYPE", ref profileType);
            string material = part.Material.MaterialString;

            bool isMaterialCorrect = true;
            string expectedMaterial = string.Empty;

            switch (profileType)
            {
                case "I":
                case "T":
                    expectedMaterial = userFields["PROJECT_USERFIELD_1"].ToString();
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "L":
                case "C":
                    expectedMaterial = userFields["PROJECT_USERFIELD_2"].ToString();
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "U":
                    expectedMaterial = userFields["PROJECT_USERFIELD_3"].ToString();
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "B":
                case "RU":
                    expectedMaterial = userFields["PROJECT_USERFIELD_4"].ToString();
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "M":
                    expectedMaterial = userFields["PROJECT_USERFIELD_5"].ToString();
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "RO":
                    expectedMaterial = userFields["PROJECT_USERFIELD_6"].ToString();
                    isMaterialCorrect = material == expectedMaterial;
                    break;
            }

            if (!isMaterialCorrect)
            {
                AddModelError(objectName, objectNumber, $"Неверный материал: должно быть '{expectedMaterial}', а сейчас '{material}'",guid,"Неправильный материал");
            }
        }

        private void CheckPartLength(Part part, string objectName, string objectNumber, Dictionary<string, double> userFields,string guid)
        {
            string profileType = part.Profile.ProfileString;
            double partLength = double.NaN;
           
            part.GetReportProperty("LENGTH", ref partLength);

            double maxLength = profileType.StartsWith("L") ? userFields["ANGLES_LENGTH"] :
                               profileType.StartsWith("PL") ? userFields["PLATES_LENGTH"] :
                               double.NaN;
          

            if (Math.Round(maxLength,1) < Math.Round(partLength,1))
            {
                string partLengthstring = Tekla.Structures.Datatype.Distance
               .FromDecimalString(partLength.ToString())
               .ToFractionalInchesString()
               .ToString();

                string partmaxLengthstring = Tekla.Structures.Datatype.Distance
             .FromDecimalString(maxLength.ToString())
             .ToFractionalInchesString()
             .ToString();

                AddModelError(objectName, objectNumber, $"Превышена длина: {partLengthstring} (макс: {partmaxLengthstring})",guid,"Неправильная длина детали");
            }
        }

        private void CheckBoltsLength(BoltGroup bolt,string guid)
        {
            var model = new tsm.Model();
            var projectInfo = model.GetProjectInfo();

            // Получаем длину болта и ожидаемую длину
            double boltLength = double.NaN;
            double expectedBoltLength = double.NaN;

            bolt.GetReportProperty("LENGTH", ref boltLength);
            projectInfo.GetUserProperty("ESDBOLTLENGTH", ref expectedBoltLength);

            // Проверяем стандарт и длину болта
            if ((bolt.BoltStandard == "A325N" || bolt.BoltStandard == "A490N" || bolt.BoltStandard == "A307") &&
                Math.Round(boltLength, 2) < Math.Round(expectedBoltLength, 2))
            {
                // Формируем сообщения только в случае ошибки
                string boltLengthString = ConvertToInchesString(boltLength);
                string expectedBoltLengthString = ConvertToInchesString(expectedBoltLength);
                string boltSizeString = ConvertToFeetAndInchesString(bolt.BoltSize);

                AddModelError(bolt.BoltStandard, boltSizeString,
                    $"Длина болта меньше ожидаемой: {boltLengthString} (по полю MINIMALBOLTLENGTH надо: {expectedBoltLengthString}).",guid,"Неправильная длина болта");
            }
        }

        private void CheckScrewsWithScrewsAndWashers(BoltGroup bolt,string guid)
        {
            // Проверяем наличие гайки или шайбы для SCREW
            if (bolt.BoltStandard.Contains("SCREW") &&
                (bolt.Nut1 || bolt.Nut2 || bolt.Washer1 || bolt.Washer2 || bolt.Washer3))
            {
                string boltSizeString = ConvertToFeetAndInchesString(bolt.BoltSize);

                AddModelError(bolt.BoltStandard, boltSizeString, "Тип болта - Screw, но в нем есть гайка/шайба",guid,"Неправильный комплект болта");
            }
        }



        // Вспомогательные функции для конвертации
        private string ConvertToInchesString(double value)
        {
            return Tekla.Structures.Datatype.Distance
                .FromDecimalString(value.ToString())
                .ToFractionalInchesString()
                .ToString();
        }

        private string ConvertToFeetAndInchesString(double value)
        {
            return Tekla.Structures.Datatype.Distance
                .FromDecimalString(value.ToString())
                .ToFractionalFeetAndInchesString();
        }

        private void AddModelError(string objectName, string objectNumber, string errorMessage,string guid, string errortype)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DataItems.Add(new ModelData
                {
                    ObjectNumber = objectNumber,
                    ObjectName = objectName,
                    Description = errorMessage,
                    Guid = guid,
                    Errortype = errortype
                });
            });
        }

        public class ModelData
        {
            public string ObjectNumber { get; set; }
            public string ObjectName { get; set; }
            public string Description { get; set; }
            public string Guid { get; set; }
            public string Errortype { get; set; }
        }

        private void SaveModelReportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToExcelFile();
        }

        private void SaveToExcelFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Сохранить данные",
                FileName = "DataExport.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Отчет");

                    // Заголовки столбцов
                    worksheet.Cell(1, 1).Value = "Марка";
                    worksheet.Cell(1, 2).Value = "Имя";
                    worksheet.Cell(1, 3).Value = "Значение";
                    worksheet.Cell(1, 4).Value = "Guid"; // Добавляем заголовок для Guid

                    // Заполнение данных из DataGrid
                    int row = 2; // начинаем со второй строки, так как первая - заголовок
                    foreach (var item in DataItems)
                    {
                        if (item is ModelData dataItem)
                        {
                            worksheet.Cell(row, 1).Value = dataItem.ObjectNumber;
                            worksheet.Cell(row, 2).Value = dataItem.ObjectName;
                            worksheet.Cell(row, 3).Value = dataItem.Description;
                            worksheet.Cell(row, 4).Value = dataItem.Guid; // Записываем Guid в Excel
                            row++;
                        }
                    }

                    // Установка ширины колонок по содержимому
                    worksheet.Columns().AdjustToContents();

                    // Сохранение файла
                    workbook.SaveAs(filePath);
                    MessageBox.Show("Данные успешно сохранены в Excel", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void LoadModelReportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Выберите файл для загрузки"
            };

            DataItems.Clear();

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Чтение данных из Excel
                    using (var workbook = new XLWorkbook(filePath))
                    {
                        var worksheet = workbook.Worksheet(1); // Получаем первый лист

                        var rows = worksheet.RowsUsed(); // Получаем все использованные строки

                        foreach (var row in rows)
                        {
                            // Пропускаем первую строку (заголовки)
                            if (row.RowNumber() == 1) continue;

                            // Чтение данных из ячеек
                            string objectNumber = row.Cell(1).GetValue<string>();
                            string objectName = row.Cell(2).GetValue<string>();
                            string description = row.Cell(3).GetValue<string>();
                            string guid = row.Cell(4).GetValue<string>();

                            // Добавляем данные в DataItems
                            DataItems.Add(new ModelData
                            {
                                ObjectNumber = objectNumber,
                                ObjectName = objectName,
                                Description = description,
                                Guid = guid ?? Guid.NewGuid().ToString() // Генерация GUID, если его нет
                            });
                        }
                    }

                    MessageBox.Show("Данные успешно загружены из Excel.", "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_SelectModel_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, есть ли выбранные строки в DataGrid
            var selectedItems = ModelDataGrid.SelectedItems;
            if (selectedItems == null || selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите строки в таблице, чтобы выделить объекты в модели.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем подключение к модели Tekla
            var model = new tsm.Model();
            if (!model.GetConnectionStatus())
            {
                MessageBox.Show("Не удалось подключиться к модели Tekla.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();

            // Список объектов для выделения
            var selectedModelObjects = new System.Collections.ArrayList();

            // Проходим по выбранным строкам в таблице
            foreach (var selectedItem in selectedItems)
            {
                if (selectedItem is ModelData modelData && !string.IsNullOrEmpty(modelData.Guid) && Guid.TryParse(modelData.Guid, out Guid objectGuid))
                {
                    var modelObject = model.SelectModelObject(new Identifier(objectGuid));
                    if (modelObject != null)
                    {
                        selectedModelObjects.Add(modelObject);
                    }
                }
            }

            // Если найдены объекты, выделяем их в модели Tekla
            if (selectedModelObjects.Count > 0)
            {
                selector.Select(selectedModelObjects);
                
            }
            else
            {
                MessageBox.Show("Не удалось найти соответствующие объекты в модели Tekla.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


    }
}

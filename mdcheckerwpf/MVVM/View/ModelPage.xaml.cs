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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DataItems.Clear();
            var model = new tsm.Model();
            ModelName = model.GetInfo().ModelName.Substring(0, model.GetInfo().ModelName.Length - 4);

            ModelObjectEnumerator selectedModelObjects = new Tekla.Structures.Model.UI.ModelObjectSelector().GetSelectedObjects();

            if (!model.GetConnectionStatus()) return;

           
                foreach (var item in selectedModelObjects)
                {
                    if (item is Part part)
                    {
                        string objectName = part.Name;
                        string objectNumber = part.GetPartMark();
                        CheckDrawingsForPart(part, objectName, objectNumber);
                        CheckMaterialPart(part, objectName, objectNumber);
                    }
                    if (item is BoltGroup bolt)
                    {

                        CheckBoltsLength(bolt);
                        CheckScrewsWithScrewsAndWashers(bolt);
                    }
                }
        }


        private void CheckDrawingsForPart(Part part, string objectName, string objectNumber)
        {
            var drawingHandler = new DrawingHandler();
            var drawings = drawingHandler.GetDrawings();
            bool singlePartDrawingFound = false;
            bool assemblyDrawingFound = false;

            string partMark = part.GetPartMark().Trim('[', ']');
            bool isMainPart = part.GetAssembly().GetMainPart().Identifier.Equals(part.Identifier);

            while (drawings.MoveNext())
            {
                if (drawings.Current is SinglePartDrawing singlePartDrawing)
                {
                    if (singlePartDrawing.Mark.Trim('[', ']') == partMark)
                    {
                        singlePartDrawingFound = true;
                    }
                }
                else if (isMainPart && drawings.Current is AssemblyDrawing assemblyDrawing)
                {
                    if (assemblyDrawing.Mark.Trim('[', ']') == partMark)
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
                AddModelError(objectName, objectNumber, "Отсутствует Single Part чертёж");
            }

            if (isMainPart && !assemblyDrawingFound)
            {
                AddModelError(objectName, objectNumber, "Отсутствует Assembly чертёж");
            }
        }

        private void CheckMaterialPart(Part part, string objectName, string objectNumber)
        {
            string profileType = string.Empty;
            part.GetReportProperty("PROFILE_TYPE", ref profileType);
            string material = part.Material.MaterialString;
            string partName = string.Empty;
            string partProfile = string.Empty;

            part.GetReportProperty("NAME", ref partName);
            part.GetReportProperty("PROFILE", ref partProfile);

            var userFields = new Dictionary<string, string>();

            void AddUserField(Dictionary<string, string> dictionary, string key, string reportPropertyName)
            {
                string value = string.Empty;
                var model = new tsm.Model();
                ProjectInfo projectInfo = model.GetProjectInfo();
                projectInfo.GetUserProperty(reportPropertyName, ref value);
                dictionary[key] = value;
            }

            AddUserField(userFields, "PROJECT_USERFIELD_1", "PROJECT_USERFIELD_1");
            AddUserField(userFields, "PROJECT_USERFIELD_2", "PROJECT_USERFIELD_2");
            AddUserField(userFields, "PROJECT_USERFIELD_3", "PROJECT_USERFIELD_3");
            AddUserField(userFields, "PROJECT_USERFIELD_4", "PROJECT_USERFIELD_4");
            AddUserField(userFields, "PROJECT_USERFIELD_5", "PROJECT_USERFIELD_5");
            AddUserField(userFields, "PROJECT_USERFIELD_6", "PROJECT_USERFIELD_6");
            AddUserField(userFields, "PROJECT_USERFIELD_7", "PROJECT_USERFIELD_7");

            bool isMaterialCorrect = true;
            string expectedMaterial = string.Empty;

            switch (profileType)
            {
                case "I":
                    expectedMaterial = userFields["PROJECT_USERFIELD_1"];
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "L":
                case "C":
                    expectedMaterial = userFields["PROJECT_USERFIELD_2"];
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "U":
                    expectedMaterial = userFields["PROJECT_USERFIELD_3"];
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "B":
                    expectedMaterial = userFields["PROJECT_USERFIELD_4"];
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "RU":
                    expectedMaterial = partName.Contains("ANCHOR") ? userFields["PROJECT_USERFIELD_7"] : userFields["PROJECT_USERFIELD_4"];
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "RO":
                    expectedMaterial = partProfile.Contains("HSS") ? userFields["PROJECT_USERFIELD_6"] : userFields["PROJECT_USERFIELD_5"];
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "M":
                    expectedMaterial = userFields["PROJECT_USERFIELD_5"];
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                case "T":
                    expectedMaterial = userFields["PROJECT_USERFIELD_1"];
                    isMaterialCorrect = material == expectedMaterial;
                    break;
                default:
                    isMaterialCorrect = true;
                    break;
            }

            if (!isMaterialCorrect)
            {
                AddModelError(objectName, objectNumber, $"Неверный материал: должно быть '{expectedMaterial}', а сейчас '{material}'");
            }
        }

        private void CheckPartLength(Part part, string objectName, string objectNumber)
        {
            string profileType = part.Profile.ProfileString;
            string minAngleLength = string.Empty;
            string minBentPlateLength = string.Empty;
            string minPlateLength = string.Empty;

            var model = new tsm.Model();
            ProjectInfo projectInfo = model.GetProjectInfo();
            projectInfo.GetUserProperty("ANGLES_LENGTH", ref minAngleLength);
            projectInfo.GetUserProperty("PLATES_LENGTH", ref minBentPlateLength);
            projectInfo.GetUserProperty("BENTPLATES_LENGTH", ref minPlateLength);

        }

        private void CheckBoltsLength(BoltGroup bolt)
        {
            var model = new tsm.Model();
            ProjectInfo projectInfo = model.GetProjectInfo();

            double boltLength = double.NaN;
            bolt.GetReportProperty("LENGTH", ref boltLength);
            double boltLengthDouble = Convert.ToDouble(boltLength);
            string boltLengthString = Tekla.Structures.Datatype.Distance
                .FromDecimalString(Convert.ToString(boltLengthDouble))
                .ConvertTo(Tekla.Structures.Datatype.Distance.UnitType.Inch)
                .ToString();


            double expectedBoltLength = double.NaN;
            projectInfo.GetUserProperty("ESDBOLTLENGTH", ref expectedBoltLength);
            double expectedBoltLength2 = Convert.ToDouble(expectedBoltLength);
            string expectedboltLengthString = Tekla.Structures.Datatype.Distance
                .FromDecimalString(Convert.ToString(expectedBoltLength))
                .ConvertTo(Tekla.Structures.Datatype.Distance.UnitType.Inch)
                .ToString();

            string boltSizeString = Tekla.Structures.Datatype.Distance
                .FromDecimalString(Convert.ToString(bolt.BoltSize))
                .ToFractionalFeetAndInchesString();

            if ((bolt.BoltStandard == "A325N" || bolt.BoltStandard == "A490N" || bolt.BoltStandard == "A307") && boltLength < expectedBoltLength2)
            {
                AddModelError(bolt.BoltStandard, boltSizeString, $"Длина болта меньше ожидаемой: {boltLengthString} (по полю MINIMALBOLTLENGTH надо: {expectedboltLengthString}\").");
            }
        }

        private void CheckScrewsWithScrewsAndWashers(BoltGroup bolt)
        {
            var model = new tsm.Model();
            ProjectInfo projectInfo = model.GetProjectInfo();

            if (bolt.BoltStandard.Contains("SCREW") &&
            (bolt.Nut1 || bolt.Nut2 || bolt.Washer1 || bolt.Washer2 || bolt.Washer3))
            {
                string boltSizeString = Tekla.Structures.Datatype.Distance
                .FromDecimalString(Convert.ToString(bolt.BoltSize))
                .ToFractionalFeetAndInchesString();

                AddModelError(bolt.BoltStandard, boltSizeString,"Тип болта - Screw, но в нем есть гайка/шайба");
            }
        }

        private void AddModelError(string objectName, string objectNumber, string errorMessage)
        {
            DataItems.Add(new ModelData
            {
                ObjectNumber = objectNumber,
                ObjectName = objectName,
                Description = errorMessage,
                Guid = Guid.NewGuid().ToString() // Генерация нового GUID
            });
        }

        public class ModelData
        {
            public string ObjectNumber { get; set; }
            public string ObjectName { get; set; }
            public string Description { get; set; }
            public string Guid { get; set; }
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

    }
}

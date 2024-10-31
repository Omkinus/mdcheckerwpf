using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using Part = Tekla.Structures.Model.Part;
using tsm = Tekla.Structures.Model;

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

        public ObservableCollection<ModelData> DataItems { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DataItems.Clear();
            var model = new tsm.Model();
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
                var model = new Model();
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
                AddModelError(objectName, objectNumber, $"Неверный материал: должно быть'{expectedMaterial}',а сейчас '{material}'");
            }
        }

        private void AddModelError(string objectName, string objectNumber, string errorMessage)
        {
            DataItems.Add(new ModelData
            {
                ObjectNumber = objectNumber,
                ObjectName = objectName,
                Description = errorMessage
            });
        }

        public class ModelData
        {
            public string ObjectNumber { get; set; }
            public string ObjectName { get; set; }
            public string Description { get; set; }
        }

        private void SaveModelReportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToTxtFile();
        }

        private void SaveToTxtFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Сохранить данные",
                FileName = "DataExport.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;
                StringBuilder sb = new StringBuilder();

                // Заголовки столбцов
                sb.AppendLine("Марка\tИмя\tЗначение");

                // Получаем данные из DataGrid
                foreach (var item in DataItems)
                {
                    if (item is ModelData dataItem)  // Замените DataItem на фактический тип ваших данных
                    {
                        sb.AppendLine($"{dataItem.ObjectNumber}\t{dataItem.ObjectName}\t{dataItem.Description}");
                    }
                }

                // Запись данных в файл
                File.WriteAllText(filePath, sb.ToString());
                MessageBox.Show("Данные успешно сохранены", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadModelReportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Выберите файл для загрузки"
            };

            DataItems.Clear();

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Читаем все строки из файла
                    string[] lines = File.ReadAllLines(filePath);
                    DataItems.Clear(); 

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] columns = lines[i].Split('\t'); 
                       
                        if (columns.Length >= 3)
                        {
                            var modelData = new ModelData
                            {
                                ObjectNumber = columns[0],
                                ObjectName = columns[1],
                                Description = columns[2]
                            };

                            DataItems.Add(modelData); 
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
}

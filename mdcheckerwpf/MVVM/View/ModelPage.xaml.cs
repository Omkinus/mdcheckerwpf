using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                    string guid = part.Identifier.GUID.ToString();

                    //Список проверок

                    CheckDrawingsForPart(part, objectName, objectNumber, guid);

                }
            }
        }

        private void CheckDrawingsForPart(Part part, string objectName, string objectNumber, string guid)
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
                else if (isMainPart && drawings.Current is AssemblyDrawing assemblyDrawing) // Проверка AssemblyDrawing только если деталь - главная
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
                AddModelError(objectName, objectNumber, "Отсутствует Single Part чертёж", guid);
            }

            if (isMainPart && !assemblyDrawingFound)
            {
                AddModelError(objectName, objectNumber, "Отсутствует Assembly чертёж", guid);
            }
        }

        private void CheckMaterialPart(Part part, string objectName, string objectNumber, string guid)
        {
            string profileType = string.Empty;
            part.GetReportProperty("PROFILE_TYPE", ref profileType);
            string material = part.Material.MaterialString;
            string partName = string.Empty;
            string partProfile = string.Empty;

            part.GetReportProperty("NAME", ref partName);
            part.GetReportProperty("PROFILE", ref partProfile);

            var userFields = new Dictionary<string, string>();

            // Метод для добавления значений user-defined полей проекта в словарь
            void AddUserField(Dictionary<string, string> dictionary, string key, string reportPropertyName)
            {
                string value = string.Empty;
                var model = new Model();
                ProjectInfo projectInfo = model.GetProjectInfo();
                projectInfo.GetUserProperty(reportPropertyName, ref value);
                dictionary[key] = value;
            }

            AddUserField(userFields, "PROJECT_USERFIELD_1", "PROJECT.USERDEFINED.PROJECT_USERFIELD_1");
            AddUserField(userFields, "PROJECT_USERFIELD_2", "PROJECT.USERDEFINED.PROJECT_USERFIELD_2");
            AddUserField(userFields, "PROJECT_USERFIELD_3", "PROJECT.USERDEFINED.PROJECT_USERFIELD_3");
            AddUserField(userFields, "PROJECT_USERFIELD_4", "PROJECT.USERDEFINED.PROJECT_USERFIELD_4");
            AddUserField(userFields, "PROJECT_USERFIELD_5", "PROJECT.USERDEFINED.PROJECT_USERFIELD_5");
            AddUserField(userFields, "PROJECT_USERFIELD_6", "PROJECT.USERDEFINED.PROJECT_USERFIELD_6");
            AddUserField(userFields, "PROJECT_USERFIELD_7", "PROJECT.USERDEFINED.PROJECT_USERFIELD_7");

            bool isMaterialCorrect = true;

           
            switch (profileType)
            {
                case "I":
                    isMaterialCorrect = material == userFields["PROJECT_USERFIELD_1"];
                    break;
                case "L":
                case "C":
                    isMaterialCorrect = material == userFields["PROJECT_USERFIELD_2"];
                    break;
                case "U":
                    isMaterialCorrect = material == userFields["PROJECT_USERFIELD_3"];
                    break;
                case "B":
                    isMaterialCorrect = material == userFields["PROJECT_USERFIELD_4"];
                    break;
                case "RU":
                    if (partName.Contains("ANCHOR"))
                        isMaterialCorrect = material == userFields["PROJECT_USERFIELD_7"];
                    else
                        isMaterialCorrect = material == userFields["PROJECT_USERFIELD_4"];
                    break;
                case "RO":
                    if (partProfile.Contains("HSS"))
                        isMaterialCorrect = material == userFields["PROJECT_USERFIELD_6"];
                    else
                        isMaterialCorrect = material == userFields["PROJECT_USERFIELD_5"];
                    break;
                case "M":
                    isMaterialCorrect = material == userFields["PROJECT_USERFIELD_5"];
                    break;
                case "T":
                    isMaterialCorrect = material == userFields["PROJECT_USERFIELD_1"];
                    break;
                default:
                    isMaterialCorrect = true;
                    break;
            }

           
            if (!isMaterialCorrect)
            {
                AddModelError(objectName, objectNumber, guid, $"Неверный материал для типа профиля {profileType}: ожидалось {userFields[$"PROJECT_USERFIELD_{profileType}"]}, но задано {material}");
            }
        }


        private void AddModelError(string objectName, string objectNumber, string errorMessage, string guid) =>
            DataItems.Add(new ModelData
            {
                ObjectNumber = objectNumber,
                ObjectName = objectName,
                Description = errorMessage,
                Guid = guid
            });
    }

    public class ModelData
    {
        public string ObjectNumber { get; set; }
        public string ObjectName { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
    }
}

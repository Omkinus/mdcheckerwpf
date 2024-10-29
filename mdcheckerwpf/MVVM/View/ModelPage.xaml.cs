using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            LoadData();
            Loaded += ModelPage_Loaded;
        }

        private ObservableCollection<ModelData> _dataItems;
        public ObservableCollection<ModelData> DataItems
        {
            get => _dataItems;
            set
            {
                _dataItems = value;
                OnPropertyChanged(nameof(DataItems));
            }
        }

        private void LoadData()
        {
            DataItems = new ObservableCollection<ModelData>
            {
                
            };
        }

        private void ModelPage_Loaded(object sender, RoutedEventArgs e) { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var model = new tsm.Model();
            ModelObjectEnumerator selectedModelObjects = new Tekla.Structures.Model.UI.ModelObjectSelector().GetSelectedObjects();

            if (!model.GetConnectionStatus()) return;

            DataItems.Clear(); // Очистка таблицы перед новой проверкой

            foreach (var item in selectedModelObjects)
            {
                if (item is Part part)
                {
                    string objectName = part.Name;
                    string objectNumber = part.GetPartMark();
                    string guid = part.Identifier.GUID.ToString();

                    DataItems.Add(new ModelData
                    {
                        ObjectName = objectName,
                        ObjectNumber = objectNumber,
                        Guid = guid,
                        Description = "Описание детали" // Замените на подходящее описание, если нужно
                    });
                }
            }
        }


        private void CheckSinglePartsWithoutDrawing()
        { 
            
        }
    }

    public class ModelData
    {
        public string ObjectNumber { get; set; }
        public string ObjectName { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
    }
}

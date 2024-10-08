using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace mdcheckerwpf.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для ModelPage.xaml
    /// </summary>
    public partial class ModelPage : UserControl, INotifyPropertyChanged
    {
        public ModelPage()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadData();
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
                new ModelData { Number = "1", Name = "Деталь 1", Description = "Описание 1", Guid = Guid.NewGuid().ToString() },
                new ModelData { Number = "2", Name = "Деталь 2", Description = "Описание 2", Guid = Guid.NewGuid().ToString() }
                // Добавьте дополнительные элементы по мере необходимости
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Прекращаем дальнейшую обработку события для предотвращения выделения текста
            var dataGridRow = FindAncestor<DataGridRow>((DependencyObject)e.OriginalSource);
            if (dataGridRow != null)
            {
                e.Handled = true;
            }
        }

        // Метод для поиска родителя нужного типа
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }

    public class ModelData
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
    }
}

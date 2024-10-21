using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace mdcheckerwpf.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для DrawingsPage.xaml
    /// </summary>
    public partial class DrawingsPage : UserControl, INotifyPropertyChanged
    {
        public DrawingsPage()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadData();
           
        }

        private ObservableCollection<DrawingData> _dataItems;
        public ObservableCollection<DrawingData> DataItems
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
            DataItems = new ObservableCollection<DrawingData>
            {
                new DrawingData { DrawingName = "Чертеж 1", Details = "Детали 1", Guid = Guid.NewGuid().ToString() },
                new DrawingData { DrawingName = "Чертеж 2", Details = "Детали 2", Guid = Guid.NewGuid().ToString() }
                // Добавьте дополнительные элементы по мере необходимости
            };
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DrawingData
    {
        public string DrawingName { get; set; }
        public string Details { get; set; }
        public string Guid { get; set; }
    }
}

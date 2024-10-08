using mdcheckerwpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdcheckerwpf.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public ObservableCollection<ModelData> DataItems { get; set; }

        public MainViewModel()
        {
            DataItems = new ObservableCollection<ModelData>
        {
            new ModelData { Number = "001", Name = "Модель 1", Description = "Описание модели 1", Guid = "GUID-001" },
            new ModelData { Number = "002", Name = "Модель 2", Description = "Описание модели 2", Guid = "GUID-002" }
            // Добавьте больше элементов по мере необходимости
        };
        }
    }
}

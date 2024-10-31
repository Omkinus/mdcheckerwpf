using System.ComponentModel;

namespace mdcheckerwpf.MVVM.Model
{
    public class SettingsModel : INotifyPropertyChanged
    {
        private bool _isMainPartsCheckEnabled;
        private bool _isSetting2Enabled;
        private bool _isSetting3Enabled;
        private bool _isSetting4Enabled;

        public bool IsMainPartsCheckEnabled
        {
            get => _isMainPartsCheckEnabled;
            set
            {
                _isMainPartsCheckEnabled = value;
                OnPropertyChanged(nameof(IsMainPartsCheckEnabled));
            }
        }

        public bool IsSetting2Enabled
        {
            get => _isSetting2Enabled;
            set
            {
                _isSetting2Enabled = value;
                OnPropertyChanged(nameof(IsSetting2Enabled));
            }
        }

        public bool IsSetting3Enabled
        {
            get => _isSetting3Enabled;
            set
            {
                _isSetting3Enabled = value;
                OnPropertyChanged(nameof(IsSetting3Enabled));
            }
        }

        public bool IsSetting4Enabled
        {
            get => _isSetting4Enabled;
            set
            {
                _isSetting4Enabled = value;
                OnPropertyChanged(nameof(IsSetting4Enabled));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

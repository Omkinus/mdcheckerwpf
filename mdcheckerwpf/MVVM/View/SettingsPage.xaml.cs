using mdcheckerwpf.MVVM.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace mdcheckerwpf.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private readonly SettingsService _settingsService;
        private mdcheckerwpf.MVVM.Model.SettingsModel _settings;

        public SettingsPage()
        {
            InitializeComponent();
            _settingsService = new SettingsService();

            Loaded += async (s, e) =>
            {
                _settings = await _settingsService.LoadSettingsAsync();
                DataContext = _settings;  // Настройки подгружаются в DataContext
            };

            Unloaded += async (s, e) =>
            {
                if (_settings != null)
                {
                    await _settingsService.SaveSettingsAsync(_settings);  // Настройки сохраняются
                }
            };
        }

    }

}

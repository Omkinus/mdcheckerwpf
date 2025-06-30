using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace mdcheckerwpf.MVVM.View
{
    public partial class SettingsPage : UserControl
    {
        private const string FileName = "settings.json";
        private readonly string _filePath;
        private Settings _settings;

        public SettingsPage()
        {
            InitializeComponent();

            // Путь к папке с .exe
            var exeDir = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location
            ) ?? AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(exeDir, FileName);

            LoadSettings();
            ApplyToUi();
        }

        private void CheckBox_Toggled(object sender, RoutedEventArgs e)
        {
            // Защита от null при загрузке компонента
            if (_settings == null) return;

            _settings.CheckMainParts = chkMainParts.IsChecked == true;
            SaveSettings();
        }

        private void StartPage_Toggled(object sender, RoutedEventArgs e)
        {
            if (_settings == null) return;

            if (rbStartMain.IsChecked == true)
                _settings.StartPage = "main";
            else if (rbStartModel.IsChecked == true)
                _settings.StartPage = "model";
            else if (rbStartDrawings.IsChecked == true)
                _settings.StartPage = "drawings";

            SaveSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _settings = JsonSerializer.Deserialize<Settings>(json)
                                ?? new Settings();
                }
                else
                {
                    _settings = new Settings();
                    SaveSettings();
                }
            }
            catch
            {
                // Если чтение/десериализация не удалась, используем настройки по умолчанию
                _settings = new Settings();
            }
        }

        private void ApplyToUi()
        {
            // Устанавливаем состояние чекбокса
            chkMainParts.IsChecked = _settings.CheckMainParts;

            // Устанавливаем выбранный RadioButton
            switch (_settings.StartPage)
            {
                case "main":
                    rbStartMain.IsChecked = true; break;
                case "drawings":
                    rbStartDrawings.IsChecked = true; break;
                default:
                    rbStartModel.IsChecked = true; break;
            }
        }

        private void SaveSettings()
        {
            try
            {
                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_settings, opts);
                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // Игнорируем ошибки записи
            }
        }

        private class Settings
        {
            [JsonPropertyName("checkMainParts")]
            public bool CheckMainParts { get; set; }

            [JsonPropertyName("startPage")]
            public string StartPage { get; set; } = "model";
        }
    }
}

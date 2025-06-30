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

            // Получаем путь к папке с .exe
            string exeDir = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location
            ) ?? AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(exeDir, FileName);

            LoadSettings();
            ApplyToUi();
        }

        // При каждом клике по чекбоксу Main Parts
        private void CheckBox_Toggled(object sender, RoutedEventArgs e)
        {
            _settings.CheckMainParts = chkMainParts.IsChecked == true;
            SaveSettings();
        }

        // При выборе одной из радиокнопок
        private void StartPage_Toggled(object sender, RoutedEventArgs e)
        {
            if (rbStartModel.IsChecked == true)
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
                    string json = File.ReadAllText(_filePath);
                    _settings = JsonSerializer.Deserialize<Settings>(json)
                                ?? new Settings();
                }
                else
                {
                    _settings = new Settings();
                    SaveSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить настройки:\n{ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                _settings = new Settings();
            }
        }

        private void ApplyToUi()
        {
            // Main Parts
            chkMainParts.IsChecked = _settings.CheckMainParts;

            // Стартовая страница
            if (_settings.StartPage == "drawings")
                rbStartDrawings.IsChecked = true;
            else
                rbStartModel.IsChecked = true;  // по умолчанию model
        }

        private void SaveSettings()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(_settings, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить настройки:\n{ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private class Settings
        {
            [JsonPropertyName("checkMainParts")]
            public bool CheckMainParts { get; set; }

            // "model" или "drawings"
            [JsonPropertyName("startPage")]
            public string StartPage { get; set; } = "model";
        }
    }
}

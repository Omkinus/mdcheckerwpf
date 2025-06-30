using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using mdcheckerwpf.MVVM.View;

namespace mdcheckerwpf
{
    public partial class MainWindow : Window
    {
        private const string FileName = "settings.json";
        private readonly string _settingsPath;
        private Settings _settings;

        public MainWindow()
        {
            InitializeComponent();

            // Позволяем тянуть окно за любую область
            this.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    try { DragMove(); } catch { }
            };

            // Определяем путь к settings.json рядом с .exe
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                         ?? AppDomain.CurrentDomain.BaseDirectory;
            _settingsPath = Path.Combine(exeDir, FileName);

            LoadSettings();

            // При старте отмечаем нужную кнопку — это тут же вызовет RadioButton_Checked
            if (_settings.StartPage.Equals("drawings", StringComparison.OrdinalIgnoreCase))
                rbDrawings.IsChecked = true;
            else
                rbModel.IsChecked = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = (RadioButton)sender;
            string tag = rb.Tag as string;

            // Загружаем нужный UserControl
            UserControl pageToLoad;
            switch (tag)
            {
                case "DrawingsPage":
                    pageToLoad = new DrawingsPage();
                    _settings.StartPage = "drawings";
                    break;

                case "SettingsPage":
                    pageToLoad = new SettingsPage();
                    break;

                case "HelpPage":
                    pageToLoad = new HelpPage();
                    break;

                case "ModelPage":
                default:
                    pageToLoad = new ModelPage();
                    _settings.StartPage = "model";
                    break;
            }

            // Вставляем в ContentArea и сохраняем новый старт
            ContentArea.Content = pageToLoad;
            SaveSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
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
                MessageBox.Show(
                    $"Не удалось загрузить настройки:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                _settings = new Settings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_settings, opts);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось сохранить настройки:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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

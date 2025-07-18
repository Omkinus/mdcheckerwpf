using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using mdcheckerwpf.MVVM;
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

            this.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    try { DragMove(); } catch { }
            };

            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                         ?? AppDomain.CurrentDomain.BaseDirectory;
            _settingsPath = Path.Combine(exeDir, FileName);

            LoadSettings();

            switch (_settings.StartPage?.ToLowerInvariant())
            {
                case "drawings":
                    rbDrawings.IsChecked = true;
                    ContentArea.Content = new DrawingsPage();
                    break;

                case "model":
                    rbModel.IsChecked = true;
                    ContentArea.Content = new ModelPage();
                    break;

                default:
                    ContentArea.Content = new HomePage();
                    break;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = (RadioButton)sender;
            UserControl page;

            switch (rb.Tag as string)
            {
                case "DrawingsPage":
                    page = new DrawingsPage();
                    _settings.StartPage = "drawings";
                    break;
                case "SettingsPage":
                    page = new SettingsPage();
                    break;
                case "HomePage":
                    page = new HomePage();
                    _settings.StartPage = "main";
                    break;
                case "HelpPage":
                    page = new HelpPage();
                    break;
                case "ModelPage":
                default:
                    page = new ModelPage();
                    _settings.StartPage = "model";
                    break;
            }

            ContentArea.Content = page;
            SaveSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    _settings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
                else
                {
                    _settings = new Settings();
                    SaveSettings();
                }
            }
            catch
            {
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
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

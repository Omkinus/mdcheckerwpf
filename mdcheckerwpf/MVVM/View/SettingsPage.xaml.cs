using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using mdcheckerwpf.MVVM;

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

            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                         ?? AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(exeDir, FileName);

            LoadSettings();
            ApplyToUi();
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) return;

            bool isChecked = checkBox.IsChecked == true;

            if (checkBox.Name == "chkMainParts") _settings.CheckMainParts = isChecked;
            else if (checkBox.Name == "chkLength") _settings.CheckLength = isChecked;
            else if (checkBox.Name == "chkMaterial") _settings.CheckMaterial = isChecked;
            else if (checkBox.Name == "chkDetailDrawings") _settings.CheckDetailDrawings = isChecked;
            else if (checkBox.Name == "chkBoltLength") _settings.CheckBoltLength = isChecked;
            else if (checkBox.Name == "chkScrewAssembly") _settings.CheckScrewAssembly = isChecked;
            else if (checkBox.Name == "chkRounding") _settings.CheckRounding = isChecked;
            else if (checkBox.Name == "chkReflectedView") _settings.CheckReflectedView = isChecked;
            else if (checkBox.Name == "chkDrawnCheckedBy") _settings.CheckDrawnCheckedBy = isChecked;
            else if (checkBox.Name == "chkPartMarkMissing") _settings.CheckPartMarkMissing = isChecked;

            SaveSettings();
        }

        private void StartPage_Toggled(object sender, RoutedEventArgs e)
        {
            if (_settings == null) return;

            if (rbStartMain.IsChecked == true) _settings.StartPage = "main";
            else if (rbStartModel.IsChecked == true) _settings.StartPage = "model";
            else if (rbStartDrawings.IsChecked == true) _settings.StartPage = "drawings";

            SaveSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
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

        private void ApplyToUi()
        {
            chkMainParts.IsChecked = _settings.CheckMainParts;
            chkLength.IsChecked = _settings.CheckLength;
            chkMaterial.IsChecked = _settings.CheckMaterial;
            chkDetailDrawings.IsChecked = _settings.CheckDetailDrawings;
            chkBoltLength.IsChecked = _settings.CheckBoltLength;
            chkScrewAssembly.IsChecked = _settings.CheckScrewAssembly;
            chkRounding.IsChecked = _settings.CheckRounding;
            chkReflectedView.IsChecked = _settings.CheckReflectedView;
            chkDrawnCheckedBy.IsChecked = _settings.CheckDrawnCheckedBy;
            chkPartMarkMissing.IsChecked = _settings.CheckPartMarkMissing;

            switch (_settings.StartPage)
            {
                case "main": rbStartMain.IsChecked = true; break;
                case "drawings": rbStartDrawings.IsChecked = true; break;
                default: rbStartModel.IsChecked = true; break;
            }

            SaveSettings(); // гарантируем запись
        }

        private void SaveSettings()
        {
            if (string.IsNullOrEmpty(_filePath)) return;

            try
            {
                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_settings, opts);
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }
    }
}

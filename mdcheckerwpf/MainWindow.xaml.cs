using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using mdcheckerwpf.MVVM.View;


namespace mdcheckerwpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetInitialPage();

            this.MouseDown += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left) // Проверяем, что нажата левая кнопка мыши
                {
                    try
                    {
                        DragMove();
                    }
                    catch (InvalidOperationException)
                    {
                        // Игнорируем ошибку, если DragMove вызывается некорректно
                    }
                }
            };
        }

        private void SetInitialPage()
        {
            // Устанавливаем начальную страницу при загрузке
            ContentArea.Content = new HomePage(); 
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                switch (radioButton.Tag.ToString())
                {
                    case "HomePage":
                        ContentArea.Content = new HomePage();
                        break;
                    case "ModelPage":
                        ContentArea.Content = new ModelPage();
                        break;
                    case "DrawingsPage":
                        ContentArea.Content = new DrawingsPage();
                        break;
                    case "SettingsPage":
                        ContentArea.Content = new SettingsPage();
                        break;
                    case "HelpPage":
                        ContentArea.Content = new HelpPage();
                        break;

                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}

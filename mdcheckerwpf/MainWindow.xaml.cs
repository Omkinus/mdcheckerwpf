using System.Windows;
using System.Windows.Controls;
using mdcheckerwpf.MVVM.View;


namespace mdcheckerwpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetInitialPage();
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
    }
}

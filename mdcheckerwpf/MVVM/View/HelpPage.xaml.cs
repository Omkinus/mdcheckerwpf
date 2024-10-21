using System.Windows;
using System.Windows.Controls;

namespace mdcheckerwpf.MVVM.View
{
    public partial class HelpPage : UserControl
    {
        public HelpPage()
        {
            InitializeComponent();
        }

        private void HelpList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HelpList.SelectedItem is ListViewItem selectedItem)
            {
                string page = selectedItem.Tag.ToString();
                LoadPage(page);
            }
        }

        private void LoadPage(string pageName)
        {
            UserControl newPage = null;

            switch (pageName)
            {
                case "IntroductionPage":
                    newPage = new Question1(); // Создайте этот UserControl
                    break;
                case "FeaturesPage":
                    newPage = new Question2(); // Создайте этот UserControl
                    break;
            
            }

            if (newPage != null)
            {
                ContentArea.Content = newPage; // Отображаем выбранную страницу
            }
        }
    }
}

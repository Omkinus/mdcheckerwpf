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

        // Обработчик для изменения выбранного пункта в ListView
        private void HelpList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HelpList.SelectedItem is ListViewItem selectedItem)
            {
                string page = selectedItem.Tag.ToString();
                LoadPage(page);
            }
        }

        // Метод для загрузки нужной страницы
        private void LoadPage(string pageName)
        {
            UserControl newPage = null;

            switch (pageName)
            {
                case "Question1":
                    newPage = new Question1(); 
                    break;
                case "Question2":
                    newPage = new Question2();
                    break;
                case "Question3":
                    newPage = new Question3();
                    break;
                case "Question4":
                    newPage = new Question4();
                    break;
                case "Question5":
                    newPage = new Question5();
                    break;
                case "Question6":
                    newPage = new Question6();
                    break;

            }

            if (newPage != null)
            {
                ContentArea.Content = newPage; 
            }
        }
    }
}

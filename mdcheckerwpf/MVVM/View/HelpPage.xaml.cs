using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace mdcheckerwpf.MVVM.View
{
    public partial class HelpPage : UserControl
    {
        public HelpPage()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Навигация к секциям на странице
            var sectionName = e.Uri.ToString().TrimStart('#');
            var targetElement = (TextBlock)FindName(sectionName);
            if (targetElement != null)
            {
                // Перемещение к элементу
                ScrollToElement(targetElement);
            }
        }

        private void ScrollToElement(UIElement element)
        {
            if (element != null)
            {
                var scrollViewer = GetScrollViewer(this);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(0); // Сброс позиции
                    var transform = element.TransformToAncestor(scrollViewer);
                    var offset = transform.Transform(new Point(0, 0)).Y;
                    scrollViewer.ScrollToVerticalOffset(offset);
                }
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject parent)
        {
            if (parent is ScrollViewer)
            {
                return (ScrollViewer)parent;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}

using Avalonia.Controls;
using Avalonia.Layout;

namespace Perch
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            Title = "Perch";
            Width = 800;
            Height = 600;

            Content = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children = { new TextBlock { Text = "Welcome to Perch!" } },
            };
        }
    }
}

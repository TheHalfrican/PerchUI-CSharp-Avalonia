using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Perch.Views;

namespace Perch
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            Title = "Perch";
            Width = 800;
            Height = 600;
            // Settings button
            var settingsBtn = new Button { Content = "Settings…" };
            settingsBtn.Click += async (sender, e) =>
            {
                var dlg = new SettingsDialogView();
                await dlg.ShowDialog(this);
            };

            // Main panel with Settings button and welcome text
            var mainPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 10,
                Children =
                {
                    settingsBtn,
                    new TextBlock { Text = "Welcome to Perch!" },
                },
            };

            Content = mainPanel;
        }
    }
}

// File: Views/CustomThemeEditorDialog.cs
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Perch.ViewModels;

namespace Perch.Views
{
    public class CustomThemeEditorDialog : ReactiveWindow<SettingsDialogViewModel>
    {
        // Helper to create a color picker row
        private static StackPanel CreateColorPickerRow(
            string labelText,
            string initialHex,
            Action<string> onColorChanged
        )
        {
            var picker = new ColorPicker { Color = Color.Parse(initialHex) };
            picker
                .GetObservable(ColorView.ColorProperty)
                .Subscribe(c => onColorChanged(c.ToString()));
            return new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 5,
                Children =
                {
                    new TextBlock { Text = labelText },
                    picker,
                },
            };
        }

        public CustomThemeEditorDialog(Window owner, SettingsDialogViewModel vm)
        {
            Owner = owner;
            ViewModel = vm;
            DataContext = vm;

            Title = "Custom Theme Editor";
            Width = 400;
            Height = 250;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var main = new StackPanel { Margin = new Thickness(10), Spacing = 8 };

            // Color pickers
            main.Children.Add(
                CreateColorPickerRow(
                    "Background:",
                    vm.CustomBgColor ?? "#FFFFFF",
                    hex => vm.CustomBgColor = hex
                )
            );
            main.Children.Add(
                CreateColorPickerRow(
                    "Text:",
                    vm.CustomTextColor ?? "#000000",
                    hex => vm.CustomTextColor = hex
                )
            );
            main.Children.Add(
                CreateColorPickerRow(
                    "Accent:",
                    vm.CustomAccentColor ?? "#0078D7",
                    hex => vm.CustomAccentColor = hex
                )
            );

            // OK / Cancel
            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 10,
            };
            var ok = new Button { Content = "OK" };
            var cancel = new Button { Content = "Cancel" };
            buttons.Children.Add(ok);
            buttons.Children.Add(cancel);
            main.Children.Add(buttons);

            ok.Click += (_, __) => Close();
            cancel.Click += (_, __) => Close();

            Content = main;
        }
    }
}

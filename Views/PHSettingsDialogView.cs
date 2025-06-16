using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Perch.ViewModels;
using ReactiveUI;

namespace Perch.Views
{
    public class SettingsDialogView : ReactiveWindow<SettingsDialogViewModel>
    {
        private ComboBox licenseCombo;
        private TextBox emuEdit;
        private ComboBox themeCombo;

        public SettingsDialogView()
        {
            ViewModel = new SettingsDialogViewModel();
            DataContext = ViewModel;

            Title = "Settings";
            Width = 500;
            Height = 600;
            MinWidth = 500;
            MinHeight = 600;

            // TabControl
            var tabs = new TabControl();

            // — General Tab —
            var generalTab = new TabItem { Header = "General" };
            var basicPanel = new StackPanel { Spacing = 10 };

            // Emulator Path
            var emuRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
            emuRow.Children.Add(new TextBlock { Text = "Emulator Path:" });
            emuEdit = new TextBox();
            emuRow.Children.Add(emuEdit);
            var emuBrowse = new Button { Content = "Browse..." };
            emuRow.Children.Add(emuBrowse);
            basicPanel.Children.Add(emuRow);

            // Bind EmulatorPath <-> TextBox
            this.Bind(ViewModel, vm => vm.EmulatorPath, v => v.emuEdit.Text);

            emuBrowse.Click += async (_, __) =>
            {
                var options = new FilePickerOpenOptions
                {
                    Title = "Select Emulator Executable",
                    FileTypeFilter =
                    [
                        new FilePickerFileType("Executable") { Patterns = ["*.exe"] },
                    ],
                };
                var files = await this.StorageProvider.OpenFilePickerAsync(options);
                var path = files?.FirstOrDefault();
                if (path != null)
                    ViewModel.EmulatorPath = path.Path.LocalPath;
            };

            // Scan Folders
            basicPanel.Children.Add(new TextBlock { Text = "Scan Folders:" });
            var folderList = new ListBox { ItemsSource = ViewModel.ScanFolders };
            basicPanel.Children.Add(folderList);

            var folderBtnRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
            var addFolderBtn = new Button { Content = "Add Folder" };
            var removeFolderBtn = new Button { Content = "Remove Selected" };
            folderBtnRow.Children.Add(addFolderBtn);
            folderBtnRow.Children.Add(removeFolderBtn);
            basicPanel.Children.Add(folderBtnRow);

            addFolderBtn.Click += async (_, __) =>
            {
                var options = new FolderPickerOpenOptions { Title = "Select Scan Folder" };
                var dirs = await StorageProvider.OpenFolderPickerAsync(options);
                var folder = dirs?.FirstOrDefault()?.Path.LocalPath;
                if (!string.IsNullOrEmpty(folder) && !ViewModel.ScanFolders.Contains(folder))
                {
                    ViewModel.ScanFolders.Add(folder);
                    ViewModel.SaveAllSettings();
                }
            };

            removeFolderBtn.Click += (_, __) =>
            {
                if (folderList.SelectedItem is string sel)
                {
                    ViewModel.ScanFolders.Remove(sel);
                    ViewModel.SaveAllSettings();
                }
            };

            // Theme Selector
            basicPanel.Children.Add(new TextBlock { Text = "Theme:" });
            themeCombo = new ComboBox
            {
                ItemsSource = new[]
                {
                    "System Default",
                    "Light",
                    "Dark",
                    "Xbox 360",
                    "Lavender Teal",
                    "Custom",
                },
            };
            basicPanel.Children.Add(themeCombo);
            this.Bind(ViewModel, vm => vm.Theme, v => v.themeCombo.SelectedItem);

            var editCustomBtn = new Button { Content = "Edit Custom Theme…", IsEnabled = false };
            basicPanel.Children.Add(editCustomBtn);
            editCustomBtn.Click += (_, __) =>
            {
                var dlg = new CustomThemeEditorDialog(this, ViewModel);
                dlg.ShowDialog(this);
            };
            themeCombo.SelectionChanged += (_, __) =>
            {
                editCustomBtn.IsEnabled = (themeCombo.SelectedItem as string) == "Custom";
            };

            generalTab.Content = basicPanel;

            // — Emulator Tab —
            // (Repeat similar pattern: wrap in ScrollViewer, build
            //  Form with ComboBoxes, CheckBoxes, NumericUpDowns,
            //  bind each control to its VM property:)
            var emulatorTab = new TabItem { Header = "Emulator" };
            var scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            };
            var emuPanel = new StackPanel { Spacing = 10 };

            // Example: License Mode
            emuPanel.Children.Add(new TextBlock { Text = "General (Master)" });
            licenseCombo = new ComboBox
            {
                ItemsSource = new[] { "Deactivated (0)", "Activated (1)" },
            };
            this.Bind(ViewModel, vm => vm.LicenseMask, v => v.licenseCombo.SelectedIndex);
            emuPanel.Children.Add(
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new TextBlock { Text = "License Mode:" },
                        licenseCombo,
                    },
                }
            );

            // …and so on for each section (languageCombo, mountCacheCheckbox, rendererCombo, vsyncCheckbox, etc.)
            // Bind each:
            // this.Bind(ViewModel, vm=>vm.Property, v=>v.Control.Property);
            // For CheckBox:
            // this.Bind(ViewModel, vm=>vm.MountCache, v=>v.mountCacheCheckbox.IsChecked);

            scroll.Content = emuPanel;
            emulatorTab.Content = scroll;

            tabs.ItemsSource = new[] { generalTab, emulatorTab };

            // Main layout
            var main = new StackPanel { Spacing = 10 };
            main.Children.Add(tabs);

            // OK / Cancel buttons
            var buttonBar = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            var ok = new Button { Content = "OK" };
            var cancel = new Button { Content = "Cancel" };
            buttonBar.Children.Add(ok);
            buttonBar.Children.Add(cancel);
            main.Children.Add(buttonBar);

            ok.Click += (_, __) =>
            {
                ViewModel.SaveAllSettings();
                Close();
            };
            cancel.Click += (_, __) => Close();

            Content = main;

            // Finally—load into controls
            ViewModel.LoadSettings();
        }
    }
}

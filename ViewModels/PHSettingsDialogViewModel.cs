using System;
using System.Collections.ObjectModel;
using System.IO;
using IniParser;
using IniParser.Model;
using ReactiveUI;

namespace Perch.ViewModels
{
    public class SettingsDialogViewModel : ReactiveObject
    {
        readonly string _configPath;
        readonly FileIniDataParser _parser;
        IniData _data;

        public SettingsDialogViewModel()
        {
            // Determine a per-user config location:
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Perch"
            );
            Directory.CreateDirectory(dir);
            _configPath = Path.Combine(dir, "config.ini");

            _parser = new FileIniDataParser();
            _data = File.Exists(_configPath) ? _parser.ReadFile(_configPath) : new IniData();

            // Initialize all properties from INI
            LoadSettings();
        }

        private void WriteConfig()
        {
            _parser.WriteFile(_configPath, _data);
        }

        public void LoadSettings()
        {
            // Emulator Path
            EmulatorPath = _data["paths"]?["xenia_path"] ?? "";

            // Scan Folders
            ScanFolders.Clear();
            var raw = _data["library"]?["scan_folders"] ?? "";
            foreach (var f in raw.Split(';', StringSplitOptions.RemoveEmptyEntries))
                ScanFolders.Add(f);

            // Theme
            Theme = _data["appearance"]?["theme"] ?? "System Default";

            // Emulator Master
            LicenseMask = int.TryParse(_data["emulator_master"]?["license_mask"], out var lm)
                ? lm
                : 0;
            UserLanguage = int.TryParse(_data["emulator_master"]?["user_language"], out var ul)
                ? ul
                : 1;
            MountCache = bool.TryParse(_data["emulator_master"]?["mount_cache"], out var mc) && mc;

            // GPU
            Renderer = _data["gpu"]?["renderer"] ?? "any";
            AllowVariableRefresh =
                bool.TryParse(_data["gpu"]?["allow_variable_refresh"], out var avr) && avr;
            BlackBars = bool.TryParse(_data["gpu"]?["black_bars"], out var bb) && bb;

            // Input
            KeyboardMode = _data["input"]?["keyboard_mode"] ?? "XInput";
            KeyboardSlot = int.TryParse(_data["input"]?["keyboard_slot"], out var ks) ? ks : 0;

            // Hacks
            ProtectZero = bool.TryParse(_data["hacks"]?["protect_zero"], out var pz) && pz;
            BreakOnUnimplemented =
                bool.TryParse(_data["hacks"]?["break_on_unimplemented"], out var bu) && bu;

            // Canary Video
            VsyncFps = _data["canary_video"]?["vsync_fps"] ?? "off";
            InternalResolution = _data["canary_video"]?["internal_resolution"] ?? "720p";
            Avpack = _data["canary_video"]?["avpack"] ?? "";

            // Canary Hacks
            MaxQueuedFrames = int.TryParse(_data["canary_hacks"]?["max_queued_frames"], out var mq)
                ? mq
                : 1;
        }

        // ——— PROPERTIES ——————————————————————————————————————————————————————

        private string _emulatorPath = string.Empty;
        public string EmulatorPath
        {
            get => _emulatorPath;
            set => this.RaiseAndSetIfChanged(ref _emulatorPath, value);
        }

        // Using ObservableCollection so UI ListBox will update automatically
        public ObservableCollection<string> ScanFolders { get; } = [];

        private string _theme = string.Empty;
        public string Theme
        {
            get => _theme;
            set => this.RaiseAndSetIfChanged(ref _theme, value);
        }

        private int _licenseMask;
        public int LicenseMask
        {
            get => _licenseMask;
            set => this.RaiseAndSetIfChanged(ref _licenseMask, value);
        }

        private int _userLanguage;
        public int UserLanguage
        {
            get => _userLanguage;
            set => this.RaiseAndSetIfChanged(ref _userLanguage, value);
        }

        private bool _mountCache;
        public bool MountCache
        {
            get => _mountCache;
            set => this.RaiseAndSetIfChanged(ref _mountCache, value);
        }

        private string _renderer = string.Empty;
        public string Renderer
        {
            get => _renderer;
            set => this.RaiseAndSetIfChanged(ref _renderer, value);
        }

        private bool _allowVariableRefresh;
        public bool AllowVariableRefresh
        {
            get => _allowVariableRefresh;
            set => this.RaiseAndSetIfChanged(ref _allowVariableRefresh, value);
        }

        private bool _blackBars;
        public bool BlackBars
        {
            get => _blackBars;
            set => this.RaiseAndSetIfChanged(ref _blackBars, value);
        }

        private string _keyboardMode = string.Empty;
        public string KeyboardMode
        {
            get => _keyboardMode;
            set => this.RaiseAndSetIfChanged(ref _keyboardMode, value);
        }

        private int _keyboardSlot;
        public int KeyboardSlot
        {
            get => _keyboardSlot;
            set => this.RaiseAndSetIfChanged(ref _keyboardSlot, value);
        }

        private bool _protectZero;
        public bool ProtectZero
        {
            get => _protectZero;
            set => this.RaiseAndSetIfChanged(ref _protectZero, value);
        }

        private bool _breakOnUnimplemented;
        public bool BreakOnUnimplemented
        {
            get => _breakOnUnimplemented;
            set => this.RaiseAndSetIfChanged(ref _breakOnUnimplemented, value);
        }

        private string _vsyncFps = string.Empty;
        public string VsyncFps
        {
            get => _vsyncFps;
            set => this.RaiseAndSetIfChanged(ref _vsyncFps, value);
        }

        private string _internalResolution = string.Empty;
        public string InternalResolution
        {
            get => _internalResolution;
            set => this.RaiseAndSetIfChanged(ref _internalResolution, value);
        }

        private string _avpack = string.Empty;
        public string Avpack
        {
            get => _avpack;
            set => this.RaiseAndSetIfChanged(ref _avpack, value);
        }

        private int _maxQueuedFrames;
        public int MaxQueuedFrames
        {
            get => _maxQueuedFrames;
            set => this.RaiseAndSetIfChanged(ref _maxQueuedFrames, value);
        }

        private string _customBgColor = string.Empty;
        public string CustomBgColor
        {
            get => _customBgColor;
            set => this.RaiseAndSetIfChanged(ref _customBgColor, value);
        }

        private string _customTextColor = string.Empty;
        public string CustomTextColor
        {
            get => _customTextColor;
            set => this.RaiseAndSetIfChanged(ref _customTextColor, value);
        }

        private string _customAccentColor = string.Empty;
        public string CustomAccentColor
        {
            get => _customAccentColor;
            set => this.RaiseAndSetIfChanged(ref _customAccentColor, value);
        }

        // ——— SAVE / RESET METHODS ————————————————————————————————————————————

        public void SaveAllSettings()
        {
            // Mirror exactly your Python sections:
            if (!_data.Sections.ContainsSection("paths"))
                _data.Sections.AddSection("paths");
            _data["paths"]["xenia_path"] = EmulatorPath;

            if (!_data.Sections.ContainsSection("library"))
                _data.Sections.AddSection("library");
            _data["library"]["scan_folders"] = string.Join(";", ScanFolders);

            if (!_data.Sections.ContainsSection("appearance"))
                _data.Sections.AddSection("appearance");
            _data["appearance"]["theme"] = Theme;

            if (!_data.Sections.ContainsSection("emulator_master"))
                _data.Sections.AddSection("emulator_master");
            _data["emulator_master"]["license_mask"] = LicenseMask.ToString();
            _data["emulator_master"]["user_language"] = UserLanguage.ToString();
            _data["emulator_master"]["mount_cache"] = MountCache.ToString();

            if (!_data.Sections.ContainsSection("gpu"))
                _data.Sections.AddSection("gpu");
            _data["gpu"]["renderer"] = Renderer;
            _data["gpu"]["allow_variable_refresh"] = AllowVariableRefresh.ToString();
            _data["gpu"]["black_bars"] = BlackBars.ToString();

            if (!_data.Sections.ContainsSection("input"))
                _data.Sections.AddSection("input");
            _data["input"]["keyboard_mode"] = KeyboardMode;
            _data["input"]["keyboard_slot"] = KeyboardSlot.ToString();

            if (!_data.Sections.ContainsSection("hacks"))
                _data.Sections.AddSection("hacks");
            _data["hacks"]["protect_zero"] = ProtectZero.ToString();
            _data["hacks"]["break_on_unimplemented"] = BreakOnUnimplemented.ToString();

            if (!_data.Sections.ContainsSection("canary_video"))
                _data.Sections.AddSection("canary_video");
            _data["canary_video"]["vsync_fps"] = VsyncFps;
            _data["canary_video"]["internal_resolution"] = InternalResolution;
            _data["canary_video"]["avpack"] = Avpack;

            if (!_data.Sections.ContainsSection("canary_hacks"))
                _data.Sections.AddSection("canary_hacks");
            _data["canary_hacks"]["max_queued_frames"] = MaxQueuedFrames.ToString();

            if (!_data.Sections.ContainsSection("CustomTheme"))
                _data.Sections.AddSection("CustomTheme");
            _data["CustomTheme"]["bg_color"] = CustomBgColor;
            _data["CustomTheme"]["text_color"] = CustomTextColor;
            _data["CustomTheme"]["accent_color"] = CustomAccentColor;

            WriteConfig();
        }

        public void ResetEmulatorSettings()
        {
            LicenseMask = 0;
            UserLanguage = 1;
            MountCache = false;
            Renderer = "any";
            AllowVariableRefresh = false;
            BlackBars = false;
            KeyboardMode = "XInput";
            KeyboardSlot = 0;
            ProtectZero = false;
            BreakOnUnimplemented = false;
            VsyncFps = "off";
            InternalResolution = "720p";
            Avpack = string.Empty;
            MaxQueuedFrames = 1;

            SaveAllSettings();
            LoadSettings();
        }
    }
}

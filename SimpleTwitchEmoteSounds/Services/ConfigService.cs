using System;
using System.Collections.Generic;
using System.IO;
using SharpHook.Native;
using Newtonsoft.Json;
using Serilog;
using SimpleTwitchEmoteSounds.Common;
using SimpleTwitchEmoteSounds.Models;
using System.Runtime.InteropServices;

namespace SimpleTwitchEmoteSounds.Services;

public static class ConfigService
{
    private static readonly JsonSerializerSettings Options = new()
    {
        Formatting = Formatting.Indented
    };

    public static readonly AppSettings Settings = InitConfig<AppSettings>("sounds");
    public static readonly UserState State = InitConfig<UserState>("user_state");

    static ConfigService()
    {
        SubscribeToChanges();
        Settings.RefreshSubscriptions();
    }

    private static void SubscribeToChanges()
    {
        State.PropertyChanged += (_, _) =>
        {
            Debouncer.Debounce("User Property Changed", () =>
            {
                Log.Information("User Property Changed");
                SaveConfig("user_state", State);
            });
        };
        Settings.PropertyChanged += (_, _) =>
        {
            Debouncer.Debounce("Sound Prop Changed", () =>
            {
                Log.Information("Sound Property Changed");
                SaveConfig("sounds", Settings);
            });
        };
        Settings.CollectionChanged += (_, _) =>
        {
            Debouncer.Debounce("Sound Collection Changed", () =>
            {
                Log.Information("Sound Settings Collection Changed");
                SaveConfig("sounds", Settings);
            });
        };
        Settings.SoundCommandPropertyChanged += (_, _) =>
        {
            Debouncer.Debounce("Sound Property Changed", () =>
            {
                Log.Information("Sound Property Changed");
                SaveConfig("sounds", Settings);
            });
        };
        Settings.SoundCommands.CollectionChanged += (_, _) =>
        {
            Debouncer.Debounce("Sound Sub Collection Changed", () =>
            {
                Log.Information("Sound Collection Changed");
                SaveConfig("sounds", Settings);
            });
        };
    }

    private static string SettingsFolder()
    {
        var appLocation = AppDomain.CurrentDomain.BaseDirectory;
        var settingsFolder = Path.Combine(appLocation, "Settings");
        #if CUSTOM_FEATURE_INSTALLED
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // On Windows local config files are typically stored in %AppData%/program
            settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleTwitchEmoteSounds", "Settings");
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // On Linux local config files are typically stored in ~/.config/program
            settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "SimpleTwitchEmoteSounds", "Settings");
        }
        #endif
        return settingsFolder;
    }

    private static T InitConfig<T>(string name) where T : class, new()
    {
        var settingsFolder = SettingsFolder();
        var configFilePath = Path.Combine(settingsFolder, $"{name}.json");

        if (!File.Exists(configFilePath))
        {
            Directory.CreateDirectory(settingsFolder);
            var defaultConfig = new T();
            var defaultConfigJson = JsonConvert.SerializeObject(defaultConfig, Options);
            File.WriteAllText(configFilePath, defaultConfigJson);
            return defaultConfig;
        }

        var configJson = File.ReadAllText(configFilePath);
        return JsonConvert.DeserializeObject<T>(configJson) ?? new T();
    }

    private static void SaveConfig<T>(string name, T config) where T : class
    {
        var settingsFolder = SettingsFolder();
        var configFilePath = Path.Combine(settingsFolder, $"{name}.json");
        var configJson = JsonConvert.SerializeObject(config, Options);
        File.WriteAllText(configFilePath, configJson);
    }

    public class NumberStringVersionSoundCommandsWrapper
    {
        public KeyCode? EnableKey { get; set; }
        public required List<NumberStringVersionSoundCommand> SoundCommands { get; set; }
    }

    public class NumberStringVersionSoundCommand
    {
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required List<NumberStringVersionSoundFile> SoundFiles { get; set; }
        public required bool Enabled { get; set; }
        public required bool IsExpanded { get; set; }
        public required string PlayChance { get; set; }
        public required Models.MatchType SelectedMatchType { get; set; }
        public required string Volume { get; set; }
    }

    public class NumberStringVersionSoundFile
    {
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required string Percentage { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using SharpHook.Native;
using Serilog;
using SimpleTwitchEmoteSounds.Common;
using SimpleTwitchEmoteSounds.Models;

namespace SimpleTwitchEmoteSounds.Services;

public static class ConfigService
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
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

    private static T InitConfig<T>(string name) where T : class, new()
    {
        var appLocation = AppDomain.CurrentDomain.BaseDirectory;
        var settingsFolder = Path.Combine(appLocation, "Settings");
        var configFilePath = Path.Combine(settingsFolder, $"{name}.json");

        var defaultConfig = new T();
        if (!File.Exists(configFilePath))
        {
            Directory.CreateDirectory(settingsFolder);
            SaveConfig(name, defaultConfig);
            return defaultConfig;
        }

        try {
            var configJson = File.ReadAllText(configFilePath);
            var config = JsonSerializer.Deserialize<T>(configJson);
            return config ?? defaultConfig;
        }  catch (Exception e)
        {
            Log.Information($"Error when trying to read configuration file: {e}");
            var dateString = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            var configFilePathBackup = Path.Combine(settingsFolder, $"{name}_backup_{dateString}.json");
            File.Copy(configFilePath, configFilePathBackup);
            // Try to migrate otherwise use the default configuration
            var configMigrated = MigrateConfig<T>(name, configFilePathBackup);
            if (configMigrated == null) {
                SaveConfig(name, defaultConfig);
                return defaultConfig;
            } else {
                SaveConfig(name, configMigrated);
                return configMigrated;
            }
        }
    }

    private static T? MigrateConfig<T>(string name, string configFilePath) where T : class, new()
    {
        if (name != "sounds" || typeof(T) != typeof(AppSettings)) {
            Log.Information($"Unable to migrate {name} from {configFilePath}: No migrations available");
            return null;
        }
        try {
            var defaultConfig = new AppSettings();
            var configJson = File.ReadAllText(configFilePath);
            var soundCommands = JsonSerializer.Deserialize<NumberStringVersionSoundCommandsWrapper>(configJson);
            Log.Information($"Detected old AppSettings [NumberStringVersion], try to migrate to latest config version...");
            if (soundCommands == null) {
                return null;
            }
            if (soundCommands.EnableKey is not null) {
                defaultConfig.EnableKey = soundCommands.EnableKey.Value;
            }
            foreach (var soundCommand in soundCommands.SoundCommands)
            {
                var soundCommandMigrated = new SoundCommand
                {
                    Category = soundCommand.Category,
                    SoundFiles = new ObservableCollection<SoundFile>(),  // Initialize empty collection
                    Enabled = soundCommand.Enabled,
                    IsExpanded = soundCommand.IsExpanded,
                    PlayChance = Convert.ToSingle(soundCommand.PlayChance),
                    SelectedMatchType = soundCommand.SelectedMatchType,
                    Name = soundCommand.Name,
                    Volume = Convert.ToSingle(soundCommand.Volume),
                };
                foreach (var soundFile in soundCommand.SoundFiles)
                {
                    var soundFileMigrated = new SoundFile
                    {
                        FileName = soundFile.FileName,
                        FilePath = soundFile.FilePath,
                        Percentage = Convert.ToSingle(soundFile.Percentage),
                    };
                    soundCommandMigrated.SoundFiles.Add(soundFileMigrated);
                }
                defaultConfig.SoundCommands.Add(soundCommandMigrated);
            }
            return defaultConfig as T;
        }  catch (Exception e)
        {
            Log.Information($"Unable to migrate {name} from {configFilePath}, Migration failed: {e}");
            return null;
        }
    }

    private static void SaveConfig<T>(string name, T config) where T : class
    {
        var appLocation = AppDomain.CurrentDomain.BaseDirectory;
        var settingsFolder = Path.Combine(appLocation, "Settings");
        var configFilePath = Path.Combine(settingsFolder, $"{name}.json");
        var configJson = JsonSerializer.Serialize(config, Options);
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

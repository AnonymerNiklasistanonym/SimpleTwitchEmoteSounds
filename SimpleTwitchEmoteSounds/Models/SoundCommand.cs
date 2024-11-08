﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace SimpleTwitchEmoteSounds.Models;

public partial class SoundCommand : ObservableObject
{
    private string _name = string.Empty;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private ObservableCollection<SoundFile> _soundFiles = [];
    [ObservableProperty] private bool _enabled = true;
    [ObservableProperty] private bool _isExpanded = true;
    [ObservableProperty] private float? _playChance = 1;
    [ObservableProperty] private MatchType _selectedMatchType = MatchType.StartsWith;
    [ObservableProperty] private float? _volume = 0.5f;
    [JsonIgnore] public string DisplayName => Category == string.Empty ? $"{Name}" : $"({Category}) {Name}";
    [JsonIgnore] public ObservableCollection<MatchType> MatchTypes => new(Enum.GetValues<MatchType>());
    [JsonIgnore] public string[] Names => Name.Split(',').Select(n => n.Trim()).ToArray();

    partial void OnVolumeChanged(float? value)
    {
        if (value == null)
        {
            Volume = 0.5f;
        }
    }

    partial void OnPlayChanceChanged(float? value)
    {
        if (value == null)
        {
            PlayChance = 1;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (!SetProperty(ref _name, value)) return;
            OnPropertyChanged(nameof(Names));
            OnPropertyChanged(nameof(DisplayName));
        }
    }
}

public enum MatchType
{
    Equals,
    StartsWith,
    StartsWithWord,
    ContainsWord
}

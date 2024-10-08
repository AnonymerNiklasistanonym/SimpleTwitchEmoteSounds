using System;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleTwitchEmoteSounds.Models;

public partial class SoundFile : ObservableObject
{
    [ObservableProperty] private string _fileName = string.Empty;
    [ObservableProperty] private string _filePath = string.Empty;
    [ObservableProperty] private float? _percentage = 1;

    partial void OnPercentageChanged(float? value)
    {
        if (value == null)
        {
            Percentage = 1;
        }
    }
}

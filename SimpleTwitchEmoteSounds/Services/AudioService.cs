﻿using SimpleTwitchEmoteSounds.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Serilog;
using System.Runtime.InteropServices;

namespace SimpleTwitchEmoteSounds.Services;

public static class AudioService
{
    public static async Task PlaySound(SoundCommand soundCommand)
    {
        if (soundCommand.SoundFiles.Count == 0)
        {
            return;
        }

        var selectedFile = SelectRandomSoundFile(soundCommand);

        if (selectedFile == null ||
            string.IsNullOrEmpty(selectedFile.FilePath) ||
            !File.Exists(selectedFile.FilePath))
        {
            return;
        }

        try
        {
            soundCommand.IncrementTimesPlayed();
            await PlayAudioFile(selectedFile.FilePath, soundCommand.Volume ?? 0.5f);
        }
        catch (Exception ex)
        {
            Log.Error($"Error previewing sound: {ex.Message}");
        }
    }

    private static SoundFile? SelectRandomSoundFile(SoundCommand soundCommand)
    {
        var random = new Random();
        var totalProbability = soundCommand.SoundFiles.Sum(sf => sf.Percentage);
        var randomValue = (float)(random.NextDouble() * (totalProbability ?? 1));
        var cumulativeProbability = 0f;

        Log.Debug($"Sound selection: Total probability: {totalProbability:F4}, Random value: {randomValue:F4}");

        foreach (var soundFile in soundCommand.SoundFiles)
        {
            var probability = soundFile.Percentage;
            cumulativeProbability += probability ?? 1;

            Log.Debug(
                $"Checking sound file: {soundFile.FileName}, Probability: {probability:F4}, Cumulative: {cumulativeProbability:F4}");

            if (!(randomValue <= cumulativeProbability)) continue;
            Log.Debug($"Selected sound file: {soundFile.FileName}");
            return soundFile;
        }

        Log.Warning("No sound file selected");
        return null;
    }

    private static async Task PlayAudioFileWindows(string filePath, float volume)
    {
        await using var audioFile = new AudioFileReader(filePath);
        using var outputDevice = new WaveOutEvent();
        var volumeProvider = new VolumeSampleProvider(audioFile)
        {
            Volume = volume
        };

        outputDevice.Init(volumeProvider);
        outputDevice.Play();

        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            await Task.Delay(100);
        }
    }

    private static async Task PlayAudioFile(string filePath, float volume)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Log.Information($"Platform Linux");
            await AudioServiceLinux.PlayAudioFile(filePath, volume);
        }
        else
        {
            Log.Information($"Platform Windows");
            await PlayAudioFileWindows(filePath, volume);
        }
    }

    public static bool DoesSoundExist(SoundFile soundFile)
    {
        return !string.IsNullOrEmpty(soundFile.FilePath) &&
               File.Exists(soundFile.FilePath);
    }
}
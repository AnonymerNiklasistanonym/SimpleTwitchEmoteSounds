using SimpleTwitchEmoteSounds.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using LibVLCSharp.Shared;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SimpleTwitchEmoteSounds.Services;

public static class AudioServiceLinux
{
    public static async Task PlayAudioFile(string filePath, float volume)
    {
        Log.Information($"Platform Linux");
        // Initialize the VLC libraries
        Core.Initialize();
        await PlayAudioWavAsync(filePath, volume);
    }

    public static bool DoesSoundExist(SoundFile soundFile)
    {
        return !string.IsNullOrEmpty(soundFile.FilePath) &&
               File.Exists(soundFile.FilePath);
    }

    private static Task PlayAudioWavAsync(string filePath, float volume)
    {
        return Task.Run(() =>
        {
            // Create VLC instance and media player
            using (var libVLC = new LibVLC())
            using (var mediaPlayer = new MediaPlayer(libVLC))
            {
                // Create a new media object
                string resolvedFilePath = GetOrCreateWavFileCached(filePath);
                using (var media = new Media(libVLC, resolvedFilePath, FromType.FromPath))
                {
                    // Set the media on the media player
                    mediaPlayer.Media = media;
                    mediaPlayer.Volume = (int)(volume * 100);

                    // Play the media
                    Log.Information($"Playing audio... {resolvedFilePath} ({mediaPlayer.Volume})");

                    mediaPlayer.Play();

                    while (mediaPlayer.State != VLCState.Playing)
                    {
                        Log.Information($"Wait for playback to start.... {resolvedFilePath} ({mediaPlayer.Volume}) [{mediaPlayer.State}]");
                        Task.Delay(1).Wait();
                    }

                    // Keep checking if the media is playing
                    while (mediaPlayer.State != VLCState.Ended)
                    {
                        Task.Delay(1000).Wait(); // Wait asynchronously for 1 second

                    }

                    Log.Information($"Finished playing {resolvedFilePath} [{mediaPlayer.Position}].");
                    return true;
                }
            }
        });
    }

    private static string GetOrCreateWavFileCached(string filePath)
    {
        if (Path.GetExtension(filePath) == ".wav") {
            Log.Information($"{filePath} is already a WAV file, do not convert it");
            return filePath;
        }
        // Get the system temp folder and create a "cache" directory inside it
        string tempPath = Path.GetTempPath();
        string cacheDir = Path.Combine(tempPath, "cacheSimpleTwitchEmoteSounds");

        // Ensure the cache directory exists
        Directory.CreateDirectory(cacheDir);

        // Compute the MD5 hash of the file
        string fileHash = ComputeMD5(filePath);

        // Target file path in the cache (hashed file name)
        string cachedFilePath = Path.Combine(cacheDir, fileHash + ".wav");

        // Check if the file already exists in the cache
        if (File.Exists(cachedFilePath))
        {
            Log.Information($"Found converted wav file {cachedFilePath} (of {filePath})");
            return cachedFilePath; // File already cached, no need to copy
        }

        // If the file is not cached or the hashes don't match, copy the new file
        Log.Information($"Converted wav file {cachedFilePath} not found, create from {filePath}...");
        ConvertToWav(filePath, cachedFilePath);
        Log.Information($"Converted {filePath} to wav file {cachedFilePath}");
        return cachedFilePath;
    }

    private static string ConvertToWav(string inputFilePath, string outputFilePath)
    {
        // Use FFmpeg to convert the input audio file to a WAV format
        var processInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{inputFilePath}\" -ar 44100 -ac 2 -f wav \"{outputFilePath}\"", // Conversion to WAV
            RedirectStandardOutput = false,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (var process = Process.Start(processInfo))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start the ffmpeg process.");
                }
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    Log.Information($"Conversion complete: {outputFilePath}");
                    return outputFilePath;
                }
                else
                {
                    Log.Information("Error during conversion:");
                    string errorOutput = process.StandardError.ReadToEnd();
                    Log.Information(errorOutput);
                    return string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Information($"FFmpeg error: {ex.Message}");
            return string.Empty;
        }
    }

    private static string ComputeMD5(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}

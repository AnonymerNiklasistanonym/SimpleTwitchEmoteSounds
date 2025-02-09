using System;
using System.Threading.Tasks;
using Serilog;
using System.Diagnostics;

namespace SimpleTwitchEmoteSounds.Services;

public static class AudioServiceLinux
{
    // Use mpv from system PATH
    public static string mpvPath = "mpv";

    public static async Task PlayAudioFile(string filePath, float volume)
    {
        ProcessStartInfo psi = new()
        {
            FileName = mpvPath,
            Arguments = $"--no-video --volume={volume * 100} \"{filePath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Process? process = null;
        try
        {
            process = Process.Start(psi);

            // Await process exit
            if (process != null)
            {
                await process.WaitForExitAsync();
            }
            else
            {
                Log.Error($"mpv for {filePath} failed to start a process");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"mpv for {filePath} failed with an unexpected error: {ex.Message}");
        }
        finally
        {
            // Ensure the process is killed if the program exits
            if (process != null && !process.HasExited)
            {
                Log.Information($"Terminating mpv process for {filePath} that has not yet exited...");
                process.Kill();
            }
        }
    }
}

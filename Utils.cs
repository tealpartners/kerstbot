using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MargieBot;

public static class Utils
{
    public static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }

    public static async Task<bool> TryConnect(this Bot bot, string token)
    {
        try
        {
            await bot.Connect(token);
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("Could not connect, double-check your API-key.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unknown exception:\r\n" + ex.ToString());
            return false;
        }

        return true;
    }
}
using LocalWifiEnableAndConnect;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var wifiSettings = config.GetSection("WifiSettings").Get<WifiSettings>();

        Logger.Init();
        Console.SetOut(new LoggerTextWriter());

        Console.WriteLine("WiFi Monitor Started...");

        while (true)
        {
            try
            {
                // ✅ STEP 1: Ensure WiFi is ON
                if (!IsWifiEnabled(wifiSettings.WifiInterfaceName))
                {
                    EnableWifi(wifiSettings.WifiInterfaceName);
                }

                // ✅ STEP 2: Check SSID
                string currentSsid = GetCurrentSSID();
                Console.WriteLine($"Current SSID: {currentSsid}");

                if (string.IsNullOrEmpty(currentSsid) ||
                    (currentSsid != wifiSettings.PrimarySSID &&
                     currentSsid != wifiSettings.BackupSSID))
                {
                    Console.WriteLine("Not connected to configured SSIDs. Fixing...");

                    DisconnectWifi();

                    if (!ConnectToSSID(wifiSettings.PrimarySSID))
                    {
                        Console.WriteLine("Primary failed. Trying backup...");
                        ConnectToSSID(wifiSettings.BackupSSID);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Thread.Sleep(wifiSettings.CheckIntervalSeconds * 1000);
        }
    }

    static string GetCurrentSSID()
    {
        string output = RunCommand("netsh", "wlan show interfaces");

        foreach (var line in output.Split('\n'))
        {
            if (line.Trim().StartsWith("SSID") && !line.Contains("BSSID"))
            {
                return line.Split(':')[1].Trim();
            }
        }

        return "";
    }

    static bool ConnectToSSID(string ssid)
    {
        Console.WriteLine($"Connecting to {ssid}...");

        string result = RunCommand("netsh", $"wlan connect name=\"{ssid}\"");

        return result.Contains("completed successfully", StringComparison.OrdinalIgnoreCase);
    }

    static void DisconnectWifi()
    {
        Console.WriteLine("Disconnecting current WiFi...");
        RunCommand("netsh", "wlan disconnect");
    }

    static string RunCommand(string file, string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = file,
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        var output = new StringBuilder();

        process.Start();
        while (!process.StandardOutput.EndOfStream)
        {
            output.AppendLine(process.StandardOutput.ReadLine());
        }
        process.WaitForExit();

        return output.ToString();
    }

    static bool IsWifiEnabled(string interfaceName)
    {
        string output = RunCommand("netsh", "interface show interface");

        foreach (var line in output.Split('\n'))
        {
            if (line.Contains(interfaceName))
            {
                // Example line:
                // Enabled        Connected      Dedicated        Wi-Fi
                return line.Trim().StartsWith("Enabled");
            }
        }

        return false;
    }


    static void EnableWifi(string interfaceName)
    {
        Console.WriteLine("WiFi is OFF. Enabling adapter...");
        RunCommand("netsh", $"interface set interface \"{interfaceName}\" admin=ENABLED");

        // Give Windows a few seconds to bring it up
        Thread.Sleep(5000);
    }

}

class WifiSettings
{
    public string PrimarySSID { get; set; }
    public string BackupSSID { get; set; }
    public int CheckIntervalSeconds { get; set; }
    public string WifiInterfaceName { get; set; }
}
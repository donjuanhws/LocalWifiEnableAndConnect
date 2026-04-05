using System;
using System.Collections.Generic;
using System.Text;

namespace LocalWifiEnableAndConnect
{
    using System;
    using System.IO;
    using System.Linq;

    public static class Logger
    {
        private static readonly object _lock = new object();
        private static string _logDirectory = "logs";
        private static string _logFile = "app.log";
        private static long _maxFileSizeBytes = 2 * 1024 * 1024; // 2MB
        private static int _maxFiles = 2;
        private static TextWriter _originalConsole;

        public static void Init(string logDirectory = "logs")
        {
            _logDirectory = logDirectory;

            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            _originalConsole = Console.Out; // store original console
        }

        public static void Log(string message)
        {
            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string fullMessage = $"[{timestamp}] {message}";

                // Write to console
                _originalConsole.WriteLine(fullMessage);

                string path = Path.Combine(_logDirectory, _logFile);

                // Rotate if needed
                if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    if (fileInfo.Length >= _maxFileSizeBytes)
                    {
                        RotateLogs();
                    }
                }

                File.AppendAllText(path, fullMessage + Environment.NewLine);
            }
        }

        private static void RotateLogs()
        {
            string basePath = Path.Combine(_logDirectory, _logFile);
            string backupPath = basePath + ".1";

            // Delete oldest if exists
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }

            // Move current to backup
            if (File.Exists(basePath))
            {
                File.Move(basePath, backupPath);
            }
        }
    }
}

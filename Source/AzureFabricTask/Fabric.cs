namespace AzureFabricTask
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Win32;
    
    public static class Fabric
    {
        public static string DefaultInstallPath
        {
            get { return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows Azure Emulator", "InstallPath", string.Empty); }
        }

        public static bool IsLoaded
        {
            get { return Process.GetProcesses().Where(p => p.ProcessName.ToUpperInvariant().Contains("DFSERVICE")).Any(); }
        }

        public static bool IsInstallPathValid(string path)
        {
            string csrunPath;
            return Fabric.IsInstallPathValid(path, out csrunPath);
        }

        public static bool Start(string path = null)
        {
            bool success = Fabric.IsLoaded;

            if (!success)
            { 
                string csrunPath;

                if (string.IsNullOrWhiteSpace(path))
                {
                    path = Fabric.DefaultInstallPath;
                }

                success = Fabric.IsInstallPathValid(path, out csrunPath)
                    && Fabric.StartProcess(csrunPath, "/devfabric:start")
                    && Fabric.StartProcess(csrunPath, "/devstore:start");
            }

            return success;
        }

        public static bool Stop(string path = null)
        {
            string csrunPath;

            if (string.IsNullOrWhiteSpace(path))
            {
                path = Fabric.DefaultInstallPath;
            }

            return Fabric.IsInstallPathValid(path, out csrunPath)
                && Fabric.StartProcess(csrunPath, "/devfabric:start")
                && Fabric.StartProcess(csrunPath, "/devstore:start");
        }

        private static bool IsInstallPathValid(string path, out string csrunPath)
        {
            path = !string.IsNullOrWhiteSpace(path) ? Path.GetFullPath(path) : Environment.CurrentDirectory;
            csrunPath = Path.Combine(path, "Emulator", "csrun.exe");
            return File.Exists(csrunPath);
        }

        private static bool StartProcess(string path, string arguments)
        {
            Process process = null;

            try
            {
                process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = arguments;
                process.Start();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            finally
            {
                if (process != null)
                {
                    process.Dispose();
                    process = null;
                }
            }
        }
    }
}
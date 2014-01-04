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

        public static ProcessResult Initialize(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Fabric.DefaultInstallPath;
            }

            string csrunPath;

            if (Fabric.IsInstallPathValid(path, out csrunPath))
            {
                path = Path.Combine(Path.GetDirectoryName(csrunPath), "devstore", "DSInit.exe");

                using (ProcessWrapper pw = new ProcessWrapper(path, "/silent /forceCreate"))
                {
                    return pw.Execute();
                }
            }

            return new ProcessResult(-1, path);
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
            using (ProcessWrapper p = new ProcessWrapper(path, arguments))
            {
                ProcessResult result = p.Execute();
                return result.ExitCode == 0;
            }
        }
    }
}
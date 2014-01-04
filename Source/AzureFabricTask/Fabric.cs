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
        private static readonly object InitializeLock = new object();

        public static string DefaultInstallPath
        {
            get { return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows Azure Emulator", "InstallPath", string.Empty); }
        }

        public static bool IsComputeEmulatorStarted
        {
            get { return Process.GetProcesses().Where(p => p.ProcessName.ToUpperInvariant().Contains("DFSERVICE")).Any(); }
        }

        public static bool IsStorageEmulatorStarted
        {
            get { return Process.GetProcesses().Where(p => p.ProcessName.ToUpperInvariant().Contains("DSSERVICELDB")).Any(); }
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

            if (!Fabric.IsComputeEmulatorStarted && !Fabric.IsStorageEmulatorStarted)
            {
                string csrunPath;

                if (Fabric.IsInstallPathValid(path, out csrunPath))
                {
                    path = Path.Combine(Path.GetDirectoryName(csrunPath), "devstore", "DSInit.exe");

                    lock (Fabric.InitializeLock)
                    {
                        if (!Fabric.IsComputeEmulatorStarted && !Fabric.IsStorageEmulatorStarted)
                        {
                            using (ProcessWrapper pw = new ProcessWrapper(path, "/silent /forceCreate"))
                            {
                                return pw.Execute(20000);
                            }
                        }
                    }
                }
            }

            return new ProcessResult(-1, path);
        }

        public static ProcessResult Start(string path = null)
        {
            ProcessResult result = Fabric.Initialize(path);

            if (result.ExitCode <= 0)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = Fabric.DefaultInstallPath;
                }

                string csrunPath;

                if (Fabric.IsInstallPathValid(path, out csrunPath))
                {
                    path = csrunPath;

                    using (ProcessWrapper pw = new ProcessWrapper(path, "/devfabric:start"))
                    {
                        result = pw.Execute();
                    }

                    if (result.ExitCode <= 0)
                    {
                        using (ProcessWrapper pw = new ProcessWrapper(path, "/devstore:start"))
                        {
                            result = pw.Execute();
                        }
                    }
                }
            }

            return result ?? new ProcessResult(-1, path);
        }

        public static ProcessResult Stop(string path = null)
        {
            ProcessResult result = null;

            if (string.IsNullOrWhiteSpace(path))
            {
                path = Fabric.DefaultInstallPath;
            }

            string csrunPath;

            if (Fabric.IsInstallPathValid(path, out csrunPath))
            {
                path = csrunPath;

                using (ProcessWrapper pw = new ProcessWrapper(path, "/devfabric:shutdown"))
                {
                    result = pw.Execute();
                }

                if (result.ExitCode <= 0)
                {
                    using (ProcessWrapper pw = new ProcessWrapper(path, "/devstore:shutdown"))
                    {
                        result = pw.Execute();
                    }
                }
            }

            return result ?? new ProcessResult(-1, path);
        }

        private static bool IsInstallPathValid(string path, out string csrunPath)
        {
            path = !string.IsNullOrWhiteSpace(path) ? Path.GetFullPath(path) : Environment.CurrentDirectory;
            csrunPath = Path.Combine(path, "Emulator", "csrun.exe");
            return File.Exists(csrunPath);
        }
    }
}
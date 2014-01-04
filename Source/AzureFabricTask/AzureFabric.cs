namespace AzureFabricTask
{
    using System;
    using System.IO;
    using System.Security;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    [SecurityCritical]
    public sealed class AzureFabric : Task
    {
        public ITaskItem SdkDir { get; set; }

        public override bool Execute()
        {
            bool success = Fabric.IsComputeEmulatorStarted && Fabric.IsStorageEmulatorStarted;

            if (!success)
            {
                this.EnsureSdkDir();

                if (this.SdkDir != null)
                {
                    string dir = this.SdkDir.GetMetadata("FullPath");

                    if (Directory.Exists(dir))
                    {
                        if (Fabric.IsInstallPathValid(dir))
                        {
                            ProcessResult result = Fabric.Start(dir);
                            success = result.ExitCode <= 0;

                            if (!string.IsNullOrWhiteSpace(result.StandardOut))
                            {
                                this.Log.LogError(result.StandardOut);
                            }

                            if (!string.IsNullOrWhiteSpace(result.StandardError))
                            {
                                this.Log.LogError(result.StandardError);
                            }
                        }
                        else
                        {
                            this.Log.LogError("The specified SdkDir does not contain a valid Windows Azure SDK installation.");
                        }
                    }
                    else
                    {
                        this.Log.LogError("The specified SdkDir does not exist ({0}).", dir);
                    }
                }
                else
                {
                    this.Log.LogError("The Windows Azure SDK could not be found. You can set a custom install location using the SdkDir property.");
                }
            }

            return success;
        }

        private bool EnsureSdkDir()
        {
            if (this.SdkDir == null)
            {
                string installPath = Fabric.DefaultInstallPath;

                if (!string.IsNullOrWhiteSpace(installPath))
                {
                    this.SdkDir = new TaskItem(installPath);
                }
            }

            return this.SdkDir != null;
        }
    }
}
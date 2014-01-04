namespace AzureFabricTask
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public sealed class ProcessWrapper : IDisposable
    {
        private readonly object locker = new object();
        private string standardError, standardOut;
        private Process process;
        private bool disposed;

        public ProcessWrapper(string path, string args = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path", "path must contain a value.");
            }

            this.process = new Process();
            this.process.StartInfo.Arguments = args ?? string.Empty;
            this.process.StartInfo.CreateNoWindow = true;
            this.process.StartInfo.FileName = path;
            this.process.StartInfo.RedirectStandardError = true;
            this.process.StartInfo.RedirectStandardInput = true;
            this.process.StartInfo.RedirectStandardOutput = true;
            this.process.StartInfo.UseShellExecute = false;
            this.process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            this.process.ErrorDataReceived += this.ProcessErrorDataReceived;
            this.process.OutputDataReceived += this.ProcessOutputDataReceived;
            this.standardError = string.Empty;
            this.standardOut = string.Empty;
        }

        public ProcessResult Execute(int timeout = 0)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.process.Start();
            this.process.BeginErrorReadLine();
            this.process.BeginOutputReadLine();
            bool exited = this.process.WaitForExit(timeout > 0 ? timeout : int.MaxValue);

            if (!exited)
            {
                this.process.Kill();
            }

            return new ProcessResult(this.process.ExitCode, this.process.StartInfo.FileName, this.standardError, this.standardOut);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.process != null)
                    {
                        this.process.Dispose();
                        this.process = null;
                    }
                }

                this.disposed = true;
            }
        }

        private void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (this.locker)
            {
                if (!string.IsNullOrWhiteSpace(this.standardError))
                {
                    this.standardError += "\r\n";
                }

                this.standardError += e.Data;
            }
        }

        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (this.locker)
            {
                if (!string.IsNullOrWhiteSpace(this.standardOut))
                {
                    this.standardOut += "\r\n";
                }

                this.standardOut += e.Data;
            }
        }
    }
}

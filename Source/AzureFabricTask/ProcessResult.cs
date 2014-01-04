namespace AzureFabricTask
{
    using System;

    public sealed class ProcessResult
    {
        public ProcessResult(
            int exitCode,
            string path,
            string standardError = null,
            string standardOut = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path", "path must contain a value.");
            }

            this.ExitCode = exitCode;
            this.Path = path;
            this.StandardError = standardError ?? string.Empty;
            this.StandardOut = standardOut ?? string.Empty;
        }

        public int ExitCode { get; private set; }

        public string Path { get; private set; }

        public string StandardError { get; private set; }

        public string StandardOut { get; private set; }
    }
}

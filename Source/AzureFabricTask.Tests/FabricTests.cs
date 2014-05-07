namespace AzureFabricTask.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public sealed class FabricTests
    {
        public FabricTests()
        {
            Fabric.Stop();
        }

        [Fact]
        public void FabricInstallPathExists()
        {
            Assert.True(Directory.Exists(Fabric.DefaultInstallPath));
        }

        [Fact]
        public void FabricInitialize()
        {
            ProcessResult result = Fabric.Initialize();
            Assert.NotNull(result);
        }

        [Fact]
        public void EmulatorVersion()
        {
            Version version = Fabric.EmulatorVersion();
            Assert.NotNull(version);
        }

        [Fact]
        public void FabricStart()
        {
            ProcessResult result = Fabric.Start();
            Assert.NotNull(result);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public async Task FabricStartParallel()
        {
            ProcessResult[] results = await Task.WhenAll(Enumerable.Range(0, 10).Select(i => Task.Run(() => Fabric.Start())));

            foreach (ProcessResult result in results)
            {
                Assert.Equal(0, result.ExitCode);
            }
        }

        [Fact]
        public void FabricStop()
        {
            Fabric.Start();
            ProcessResult result = Fabric.Stop();
            Assert.NotNull(result);
            Assert.Equal(0, result.ExitCode);
            Assert.False(Fabric.IsComputeEmulatorStarted);
            Assert.False(Fabric.IsStorageEmulatorStarted);
        }
    }
}

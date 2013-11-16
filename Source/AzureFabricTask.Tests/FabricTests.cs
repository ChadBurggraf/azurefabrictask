namespace AzureFabricTask.Tests
{
    using System;
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
        public void FabricStart()
        {
            Assert.True(Fabric.Start());
        }

        [Fact]
        public async Task FabricStartParallel()
        {
            bool[] success = await Task.WhenAll(Enumerable.Range(0, 10).Select(i => Task.Run(() => Fabric.Start())));
            Assert.True(success.All(s => s));
        }

        [Fact]
        public void FabricStop()
        {
            Assert.True(Fabric.Stop());
        }
    }
}

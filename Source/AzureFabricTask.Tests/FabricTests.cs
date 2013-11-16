namespace AzureFabricTask.Tests
{
    using System;
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
        public void FabricStop()
        {
            Assert.True(Fabric.Stop());
        }
    }
}

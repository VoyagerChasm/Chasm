using Chasm.Models.Sockets;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Chasm.Models.Test.Sockets
{
    public class SocketAddressTest
    {

        [ExcludeFromCodeCoverage]
        private class MockSocketAddressClass : SocketAddress
        {
            public MockSocketAddressClass(string host, int port) : base(host, port)
            {
            }
        }

        [Theory]
        [InlineData(null, 0)]
        public void TestConstructorThrowsArgumentNullException(string host, int port)
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() => new MockSocketAddressClass(host, port));
        }


        [Theory]
        [InlineData("", -1)]
        [InlineData("", 0xFFFF0)]
        [InlineData("", 0)]
        [InlineData(" ", -1)]
        [InlineData(" ", 0xFFFF0)]
        [InlineData(" ", 0)]
        public void TestConstructorThrowsArgumentException(string host, int port)
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new MockSocketAddressClass(host, port));
        }

        [Theory]
        [InlineData("valid_host", -1)]
        [InlineData("valid_host", 0xFFFF0)]
        public void TestConstructorThrowsArgumentOuOfRangeException(string host, int port)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => new MockSocketAddressClass(host, port));
        }


        [Theory]
        [InlineData("host", 0)]
        public void TestValidConstructor(string host, int port)
        {
            MockSocketAddressClass mockSocketAddressClass = null;
            Assert.Null(mockSocketAddressClass);
            mockSocketAddressClass = new MockSocketAddressClass(host, port);
            Assert.NotNull(mockSocketAddressClass);
            Assert.IsType<MockSocketAddressClass>(mockSocketAddressClass);
        }

        [Fact]
        public void TestValidEqualsAndHashCodeAndToString()
        {
            var mockSocketAddressClass = new MockSocketAddressClass("host", 0);
            var mockSocketAddressClass2 = new MockSocketAddressClass("host", 0);
            var mockSocketAddressClass3 = new MockSocketAddressClass("host2", 0);

            Assert.Equal(mockSocketAddressClass, mockSocketAddressClass2);
            Assert.NotEqual(mockSocketAddressClass, mockSocketAddressClass3);
            Assert.NotEqual(mockSocketAddressClass2, mockSocketAddressClass3);

            Assert.Equal(mockSocketAddressClass.GetHashCode(), mockSocketAddressClass2.GetHashCode());
            Assert.NotEqual(mockSocketAddressClass.GetHashCode(), mockSocketAddressClass3.GetHashCode());
            Assert.NotEqual(mockSocketAddressClass2.GetHashCode(), mockSocketAddressClass3.GetHashCode());

            Assert.Equal(mockSocketAddressClass.ToString(), mockSocketAddressClass2.ToString());
            Assert.NotEqual(mockSocketAddressClass.ToString(), mockSocketAddressClass3.ToString());
            Assert.NotEqual(mockSocketAddressClass2.ToString(), mockSocketAddressClass3.ToString());
        }
    }
}

using System;
using Xunit;

namespace Chasm.Models.Test
{
    public class CredentialTest
    {

        [Theory]
        [InlineData(null, "")]
        [InlineData("username", null)]
        public void TestConstructorThrowsArgumentNullException(string username, string password)
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() => new Credential(username, password));
        }


        [Theory]
        [InlineData("", "")]
        [InlineData(" ", "")]
        public void TestConstructorThrowsArgumentException(string username, string password)
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new Credential(username, password));
        }

        [Theory]
        [InlineData("username", "")]
        [InlineData("username", "password")]
        public void TestValidConstructor(string username, string password)
        {
            Credential mockSocketAddressClass = null;
            Assert.Null(mockSocketAddressClass);
            mockSocketAddressClass = new Credential(username, password);
            Assert.NotNull(mockSocketAddressClass);
            Assert.IsType<Credential>(mockSocketAddressClass);
        }

        [Fact]
        public void TestValidEqualsAndHashCodeAndToString()
        {
            var mockSocketAddressClass = new Credential("username", "password");
            var mockSocketAddressClass2 = new Credential("username", "password");
            var mockSocketAddressClass3 = new Credential("username2", "password");

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

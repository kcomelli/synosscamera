using FluentAssertions;
using synosscamera.core.Extensions;
using synosscamera.testsuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace synosscamera.core.Tests.UnitTests.Extensions
{
    public class UrlExtensionsTests : TestBase
    {
        const string Category = "Url string extensions";
        const string TestType = "Unit";

        protected override void TestInitialize()
        {
        }
        protected override void TestCleanUp()
        {
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AppendEndpoint_ShouldAppend()
        {
            // arrange
            var url = "http://localhost/myapp";
            var endpoint = "/connect/authorize";
            var expected = $"{url}{endpoint}";

            // act
            var result = url.AppendEndpoint(endpoint);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AppendEndpoint_WithPort_ShouldAppend()
        {
            // arrange
            var url = "http://localhost:43026/myapp";
            var endpoint = "/connect/authorize";
            var expected = $"{url}{endpoint}";

            // act
            var result = url.AppendEndpoint(endpoint);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AppendEndpoint_To_Empty_ShouldAppend()
        {
            // arrange
            var url = string.Empty;
            var endpoint = "/connect/authorize";
            var expected = endpoint;

            // act
            var result = url.AppendEndpoint(endpoint);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AppendEmptyEndpoint_ShouldDoNothing()
        {
            // arrange
            var url = "http://localhost:43026/myapp";
            var endpoint = string.Empty;
            var expected = url;

            // act
            var result = url.AppendEndpoint(endpoint);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AppendEndpoint_Slashes_ShouldAppend()
        {
            // arrange
            var url = "http://localhost/myapp/";
            var endpoint = "/connect/authorize";
            var expected = "http://localhost/myapp/connect/authorize";

            // act
            var result = url.AppendEndpoint(endpoint);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void EnsureLeadingSlash_ShouldBeOk()
        {
            // arrange
            var url = "/connect/authorize";
            var expected = "/connect/authorize";

            // act
            var result = url.EnsureLeadingSlash();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void EnsureLeadingSlash_ShouldAdd()
        {
            // arrange
            var url = "connect/authorize";
            var expected = "/connect/authorize";

            // act
            var result = url.EnsureLeadingSlash();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void EnsureTrailingSlash_ShouldBeOk()
        {
            // arrange
            var url = "connect/authorize/";
            var expected = "connect/authorize/";

            // act
            var result = url.EnsureTrailingSlash();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void EnsureTrailingSlash_ShouldAdd()
        {
            // arrange
            var url = "connect/authorize";
            var expected = "connect/authorize/";

            // act
            var result = url.EnsureTrailingSlash();

            //assert
            result.Should().BeEquivalentTo(expected);
        }


        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void RemoveLeadingSlash_ShouldBeOk()
        {
            // arrange
            var url = "connect/authorize";
            var expected = "connect/authorize";

            // act
            var result = url.RemoveLeadingSlash();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void RemoveLeadingSlash_ShouldRemove()
        {
            // arrange
            var url = "/connect/authorize";
            var expected = "connect/authorize";

            // act
            var result = url.RemoveLeadingSlash();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void RemoveTrailingSlash_ShouldBeOk()
        {
            // arrange
            var url = "connect/authorize";
            var expected = "connect/authorize";

            // act
            var result = url.RemoveTrailingSlash();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void RemoveTrailingSlash_ShouldRemove()
        {
            // arrange
            var url = "connect/authorize/";
            var expected = "connect/authorize";

            // act
            var result = url.RemoveTrailingSlash();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CleanUrlPath_FullUrl_ShouldDoNothing()
        {
            // arrange
            var url = "http://localhost/myapp";
            var expected = "http://localhost/myapp";

            // act
            var result = url.CleanUrlPath();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CleanUrlPath_Root_ShouldDoNothing()
        {
            // arrange
            var url = "/";
            var expected = "/";

            // act
            var result = url.CleanUrlPath();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CleanUrlPath_Empty_ShouldAddSlash()
        {
            // arrange
            var url = string.Empty;
            var expected = "/";

            // act
            var result = url.CleanUrlPath();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CleanUrlPath_FullUrl_ShouldRemove()
        {
            // arrange
            var url = "http://localhost/myapp/";
            var expected = "http://localhost/myapp";

            // act
            var result = url.CleanUrlPath();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("/")]
        [InlineData("~/")]
        [InlineData("/images/test.png")]
        [InlineData("~/images/test.png")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLocalUrl_ShouldBeLocal(string url)
        {
            // arrange

            // act
            var result = url.IsLocalUrl();

            //assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("//")]
        [InlineData("/\\")]
        [InlineData("")]
        [InlineData("http://localhost/myapp/")]
        [InlineData(null)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLocalUrl_ShouldNotBeLocal(string url)
        {
            // arrange

            // act
            var result = url.IsLocalUrl();

            //assert
            result.Should().BeFalse();
        }


        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AddQueryString_NothingYet_ShouldAdd()
        {
            // arrange
            var url = "http://localhost/myapp/";
            var query = "test=123&company=awanto";
            var expected = $"{url}?{query}";

            // act
            var result = url.AddQueryString(query);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AddQueryString_To_Existing_ShouldAdd()
        {
            // arrange
            var url = "http://localhost/myapp/route?existing=344";
            var query = "test=123&company=awanto";
            var expected = $"{url}&{query}";

            // act
            var result = url.AddQueryString(query);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AddHashFragment_NothingYet_ShouldAdd()
        {
            // arrange
            var url = "http://localhost/myapp/";
            var query = "newfragment";
            var expected = $"{url}#{query}";

            // act
            var result = url.AddHashFragment(query);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void AddHashFragment_To_Existing_ShouldAdd()
        {
            // arrange
            var url = "http://localhost/myapp/route#existingfragment";
            var query = "/additional";
            var expected = $"{url}{query}";

            // act
            var result = url.AddHashFragment(query);

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ReadQueryStringAsNameValueCollection_ShouldFind()
        {
            // arrange
            var url = "http://localhost/myapp/route?param1=123&param2=thisisatest&param3=test";
            var expectedCount = 3;
            var checkKey = "param2";
            var expectedValue = "thisisatest";

            // act
            var result = url.ReadQueryStringAsNameValueCollection();

            //assert
            result.Count.Should().Be(expectedCount);
            result[checkKey].Should().Be(expectedValue);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ReadQueryStringAsNameValueCollection_NoQuery_ShouldBeOk()
        {
            // arrange
            var url = "http://localhost/myapp/route";
            var expectedCount = 0;

            // act
            var result = url.ReadQueryStringAsNameValueCollection();

            //assert
            result.Count.Should().Be(expectedCount);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("http://localhost/myapp/", "")]
        [InlineData("http://localhost/myapp/?test=123&company=awanto", "test=123&company=awanto")]
        [InlineData("http://localhost/myapp/#test=123&company=awanto", "test=123&company=awanto")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetQueryString_ShouldBeOk(string url, string expected)
        {
            // arrange
            // act
            var result = url.GetQueryString();

            //assert
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("file://C:/test/test.xml", null)]
        [InlineData("http://localhost/myapp/", "http://localhost")]
        [InlineData("https://localhost/myapp/", "https://localhost")]
        [InlineData("HTTP://awanto.com/myapp/", "HTTP://awanto.com")]
        [InlineData("Https://www.awanto.com", "Https://www.awanto.com")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetOrigin_ShouldBeOk(string url, string expected)
        {
            // arrange
            // act
            var result = url.GetOrigin();

            //assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}

using FluentAssertions;
using synosscamera.core.Diagnostics;
using synosscamera.core.Resources;
using synosscamera.testsuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace synosscamera.core.Tests.UnitTests.Diagnostics
{
    public class GuardsTests : TestBase
    {
        const string Category = "Guards";
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
        public void CheckArgumentNull_ShouldThrow()
        {
            // arrange
            object check = null;

            //assert
            var result = Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                check.CheckArgumentNull(nameof(check));
            });
            result.ParamName.Should().BeEquivalentTo(nameof(check));
        }

        [Theory]
        [InlineData("")]
        [InlineData(12)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CheckArgumentNull_ShouldNotThrow(object check)
        {
            // arrange

            // act
            check.CheckArgumentNull(nameof(check));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CheckArgumentNullOrEmpty_ShouldThrow(string check)
        {
            // arrange

            //assert
            var result = Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                check.CheckArgumentNullOrEmpty(nameof(check));
            });
            result.ParamName.Should().BeEquivalentTo(nameof(check));
        }

        [Theory]
        [InlineData("test")]
        [InlineData("another test")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CheckArgumentNullOrEmpty_ShouldNotThrow(string check)
        {
            // arrange

            // act
            check.CheckArgumentNullOrEmpty(nameof(check));
        }


        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CheckNull_ShouldThrow()
        {
            // arrange
            object check = null;

            //assert
            var result = Assert.Throws<NullReferenceException>(() =>
            {
                // act
                check.CheckNull(nameof(check));
            });
            result.Message.Should().BeEquivalentTo(nameof(check));
        }

        [Theory]
        [InlineData("")]
        [InlineData(12)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CheckNull_ShouldNotThrow(object check)
        {
            // arrange

            // act
            check.CheckNull(nameof(check));
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CheckMandatoryOption_ShouldThrow(string check)
        {
            // arrange

            //assert
            var result = Assert.Throws<ArgumentException>(() =>
            {
                // act
                check.CheckMandatoryOption(nameof(check));
            });

            result.Message.Should().BeEquivalentTo(string.Format(Errors.Exception_OptionMustBeProvided, nameof(check)));
        }

        [Theory]
        [InlineData("test")]
        [InlineData("another test")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CheckMandatoryOption_ShouldNotThrow(string check)
        {
            // arrange

            // act
            check.CheckMandatoryOption(nameof(check));
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CheckMandatoryOption_CustomError_ShouldThrow(string check)
        {
            // arrange
            var errormessage = "This error occured: {0}";

            //assert
            var result = Assert.Throws<ArgumentException>(() =>
            {
                // act
                check.CheckMandatoryOption(nameof(check), errormessage);
            });

            result.Message.Should().BeEquivalentTo(string.Format(errormessage, nameof(check)));
        }

    }
}

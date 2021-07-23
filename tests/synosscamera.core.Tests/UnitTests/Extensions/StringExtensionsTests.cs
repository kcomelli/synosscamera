using FluentAssertions;
using synosscamera.core.Extensions;
using synosscamera.testsuite;
using System;
using System.Globalization;
using Xunit;

namespace synosscamera.core.Tests.UnitTests.Extensions
{
    public class StringExtensionsTests : TestBase
    {
        const string Category = "String extensions";
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
        public void IsEqualNoCase_ShouldBeOk()
        {
            // arrange
            string str1 = "String one 123!";
            string str2 = "String one 123!";

            // act
            var areEqual = str1.IsEqualNoCase(str2);

            // assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualNoCase_DifferentCase_ShouldBeOk()
        {
            // arrange
            string str1 = "String one 123!";
            string str2 = "string one 123!";

            // act
            var areEqual = str1.IsEqualNoCase(str2);

            // assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualNoCase_ShouldFail()
        {
            // arrange
            string str1 = "String one 123!";
            string str2 = "String one 14553!";

            // act
            var areEqual = str1.IsEqualNoCase(str2);

            // assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualNoCase_List_ShouldBeOk()
        {
            // arrange
            string str1 = "String one 123!";
            string[] str2 = new string[] { "Check string 1", "Check string position 2", "String one 123!" };

            // act
            var areEqual = str1.IsEqualNoCase(str2);

            // assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualNoCase_ListDifferentCase_ShouldBeOk()
        {
            // arrange
            string str1 = "String one 123!";
            string[] str2 = new string[] { "Check string 1", "Check string position 2", "string one 123!" };

            // act
            var areEqual = str1.IsEqualNoCase(str2);

            // assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualNoCase_List_ShouldFail()
        {
            // arrange
            string str1 = "String one 123!";
            string[] str2 = new string[] { "Check string 1", "Check string position 2", "String one 123456!" };

            // act
            var areEqual = str1.IsEqualNoCase(str2);

            // assert
            areEqual.Should().BeFalse();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("123", "123")]
        [InlineData("\u2000\u2000", "\u2000\u2000")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualTreatNullAsEmpty_ShouldBeTrue(string str1, string str2)
        {
            // act
            var result = str1.IsEqualTreatNullAsEmpty(str2);

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(" ", null)]
        [InlineData(null, "123")]
        [InlineData(" ", "")]
        [InlineData("123", "123456789")]
        [InlineData("\u2000\u2000", "  ")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualTreatNullAsEmpty_ShouldBeFalse(string str1, string str2)
        {
            // act
            var result = str1.IsEqualTreatNullAsEmpty(str2);

            // assert
            result.Should().BeFalse();
        }


        [Theory]
        [InlineData("", null)]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("123asb", "123Asb")]
        [InlineData("abc", "abc")]
        [InlineData("\u2000\u2000", "\u2000\u2000")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualTreatNullAsEmptyNoCase_ShouldBeTrue(string str1, string str2)
        {
            // act
            var result = str1.IsEqualTreatNullAsEmptyNoCase(str2);

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(" ", null)]
        [InlineData(null, "123")]
        [InlineData(" ", "")]
        [InlineData("123dfg", "123456789ddf")]
        [InlineData("\u2000\u2000", "  ")]
        [InlineData("\u00A0\u00A0", "  ")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsEqualTreatNullAsEmptyNoCase_ShouldBeFalse(string str1, string str2)
        {
            // act
            var result = str1.IsEqualTreatNullAsEmptyNoCase(str2);

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void FormatWith_ShouldBeOk()
        {
            // arrange
            string str1 = "This is the output: \"{0}\"";
            string val1 = "String one 123!";

            string expected = "This is the output: \"String one 123!\"";

            // act
            var result = str1.FormatWith(val1);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void FormatWith_ShouldThrow()
        {
            // arrange
            string str1 = "This is the output: \"{0}\"";

            //assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                ((string)null).FormatWith(str1);
            });
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void FormatWith_enUs_ShouldBeOk()
        {
            // arrange
            var culture = new CultureInfo("en-US");

            string str1 = "This is the output: \"{0}\"";
            double val1 = 1234.50;

            string expected = "This is the output: \"1234.5\"";

            // act
            var result = str1.FormatWith(culture.NumberFormat, val1);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void FormatWith_deDe_ShouldBeOk()
        {
            // arrange
            var culture = new CultureInfo("de-DE");

            string str1 = "This is the output: \"{0}\"";
            double val1 = 1234.50;

            string expected = "This is the output: \"1234,5\"";

            // act
            var result = str1.FormatWith(culture.NumberFormat, val1);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void TruncateAtWord_ShouldBeOk()
        {
            // arrange
            string str1 = "This is the output of the test!";
            int truncateIdx = 12;

            string expected = "This is the...";

            // act
            var result = str1.TruncateAtWord(truncateIdx);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void TruncateAtWord_WordIndex_ShouldBeOk()
        {
            // arrange
            string str1 = "This is the output of the test!";
            int truncateIdx = 20;

            string expected = "This is the output...";

            // act
            var result = str1.TruncateAtWord(truncateIdx);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void TruncateAtWord_All_ShouldDoNothing()
        {
            // arrange
            string str1 = "This is the output of the test!";
            int truncateIdx = 250;

            string expected = "This is the output of the test!";

            // act
            var result = str1.TruncateAtWord(truncateIdx);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void TruncateAtWord_Negative_ShouldDoNothing()
        {
            // arrange
            string str1 = "This is the output of the test!";
            int truncateIdx = -3;

            string expected = "This is the output of the test!";

            // act
            var result = str1.TruncateAtWord(truncateIdx);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToInt_ShouldBeOk()
        {
            // arrange
            string str1 = "123";
            int expected = 123;

            // act
            var result = str1.ToInt();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("This is text", 0)]
        [InlineData("123.56", 10)]
        [InlineData("", 0)]
        [InlineData(null, 25)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToInt_ShouldReturnDefault(string str1, int defaultValue = 0)
        {
            // arrange
            int expected = defaultValue;

            // act
            var result = str1.ToInt(defaultValue);

            // assert
            result.Should().Be(expected);
        }



        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToNullableInt_ShouldBeOk()
        {
            // arrange
            string str1 = "123";
            int? expected = 123;

            // act
            var result = str1.ToNullableInt();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("This is a text")]
        [InlineData("1234.5")]
        [InlineData(null)]
        [InlineData("")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToNullableInt_ShouldBeNull(string str1)
        {
            // arrange
            int? expected = null;

            // act
            var result = str1.ToNullableInt();

            // assert
            result.Should().Be(expected);
        }




        [Theory]
        [InlineData("true")]
        [InlineData("True")]
        [InlineData("yes")]
        [InlineData("YES")]
        [InlineData("ja")]
        [InlineData("Ja")]
        [InlineData("ui")]
        [InlineData("Ui")]
        [InlineData("1")]
        [InlineData("t")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToBool_ShouldBeTrue(string str1)
        {
            // arrange
            bool expected = true;

            // act
            var result = str1.ToBool();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("false")]
        [InlineData("False")]
        [InlineData("no")]
        [InlineData("NO")]
        [InlineData("0")]
        [InlineData("f")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToBool_ShouldBeFalse(string str1)
        {
            // arrange
            bool expected = false;

            // act
            var result = str1.ToBool();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("This is text", false)]
        [InlineData("123.56", true)]
        [InlineData("", true)]
        [InlineData(null, false)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToBool_ShouldReturnDefault(string str1, bool defaultValue = false)
        {
            // arrange
            bool expected = defaultValue;

            // act
            var result = str1.ToBool(defaultValue);

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("true")]
        [InlineData("True")]
        [InlineData("yes")]
        [InlineData("YES")]
        [InlineData("1")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToNullableBool_ShouldBeTrue(string str1)
        {
            // arrange
            bool? expected = true;

            // act
            var result = str1.ToNullableBool();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("false")]
        [InlineData("False")]
        [InlineData("no")]
        [InlineData("NO")]
        [InlineData("0")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToNullableBool_ShouldBeFalse(string str1)
        {
            // arrange
            bool? expected = false;

            // act
            var result = str1.ToNullableBool();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("This is a text")]
        [InlineData("1234.5")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToNullableBool_ShouldBeNull(string str1)
        {
            // arrange
            bool? expected = null;

            // act
            var result = str1.ToNullableBool();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("Not convertible text", "*")]
        [InlineData("B345", "B???")]
        [InlineData("B345", "B###")]
        [InlineData("test.domain.com", "*.domain.com")]
        [InlineData("test.sub.domain.com", "*.domain.com")]
        [InlineData("test.domain.com", "test.domain.com")]
        [InlineData("test.domain.com", "test.*")]
        [InlineData("test.sub.domain.com", "test.*")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLike_ShouldBeTrue(string str1, string pattern)
        {
            // arrange

            // act
            var result = str1.IsLike(pattern);

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Not convertible text", "B???")]
        [InlineData("B345", "B??56")]
        [InlineData("BCDE", "B###")]
        [InlineData("test.domain.de", "*.domain.com")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLike_ShouldBeFalse(string str1, string pattern)
        {
            // arrange

            // act
            var result = str1.IsLike(pattern);

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLike_ApplyMask1_ShouldBeOk()
        {
            // arrange
            string str1 = "Not convertible text";
            string mask = "....            .......";

            var expected = "Not text";

            // act
            var result = str1.ApplyMask(mask);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLike_ApplyMask2_ShouldBeOk()
        {
            // arrange
            string str1 = "Not convertible text";
            string mask = "....................";

            var expected = "Not convertible text";

            // act
            var result = str1.ApplyMask(mask);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLike_ApplyMask3_ShouldBeOk()
        {
            // arrange
            string str1 = "Not convertible text";
            string mask = "..........";

            var expected = "Not conver";

            // act
            var result = str1.ApplyMask(mask);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLike_ApplyMask4_ShouldBeOk()
        {
            // arrange
            string str1 = "Not convertible text";
            string mask = " ";

            var expected = "";

            // act
            var result = str1.ApplyMask(mask);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsLike_ApplyMask_ShouldNoApply(string mask)
        {
            // arrange
            string str1 = "Not convertible text";

            var expected = "Not convertible text";

            // act
            var result = str1.ApplyMask(mask);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("workplaceX")]
        [InlineData("")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ToUniqueFilename_ShouldBeOk(string prefix)
        {
            // arrange
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;

            var expected = location.Substring(0, location.Length - 4) + prefix;

            // act
            var result = location.ToUniqueFileName(prefix);

            // assert
            result.Should().StartWith(expected);
        }


        [Theory]
        [InlineData("This is a test string", new string[] { "*" })]
        [InlineData("This is a test string", new string[] { "B??", "*" })]
        [InlineData("B3455", new string[] { "BC??", "C3??", "B##", "B??##" })]
        [InlineData("B3455", new string[] { "BC??", "C3??", "XB##", "B??##" })]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ValidateAgainstPattern_ShouldValidate(string input, string[] patterns)
        {
            // arrange
            // act
            var result = input.ValidateAgainstPatterns(patterns);

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("This is a test string", new string[] { "B??" })]
        [InlineData("This is a test string", new string[] { "B??", "C??" })]
        [InlineData("B3455", new string[] { "BC??", "C3??", "XB##", "BX??##" })]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void ValidateAgainstPattern_ShouldNotValidate(string input, string[] patterns)
        {
            // arrange
            // act
            var result = input.ValidateAgainstPatterns(patterns);

            // assert
            result.Should().BeFalse();
        }



        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void SplitPascalCase_ShouldBeOk()
        {
            // arrange
            var input = "ThisIsAnExample";
            var expected = "This Is An Example";

            // act
            var result = input.SplitPascalCaseText();

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void SplitPascalCase1_ShouldBeOk()
        {
            // arrange
            var input = "ThisIsAExample";
            var expected = "This Is A Example";

            // act
            var result = input.SplitPascalCaseText();

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("                ")]
        [InlineData("  \t  ")]
        [InlineData("\u2000\u2000")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsMissing_ShouldBeTrue(string input)
        {
            // arrange
            // act
            var result = input.IsMissing();

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("1 This is not whitespace")]
        [InlineData("2 This is not whitespace     ")]
        [InlineData("3 This\u2000is\u2000not\u2000whitespace")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsMissing_ShouldBeFalse(string input)
        {
            // arrange
            // act
            var result = input.IsMissing();

            // assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(null, 5)]
        [InlineData("", 5)]
        [InlineData("                ", 5)]
        [InlineData("  \t  ", 5)]
        [InlineData("\u2000\u2000", 5)]
        [InlineData("1 This is not whitespace but too long", 5)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsMissingOrTooLong_ShouldBeTrue(string input, int maxLength)
        {
            // arrange
            // act
            var result = input.IsMissingOrTooLong(maxLength);

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("1 This is not whitespace", 30)]
        [InlineData("2 This is not whitespace     ", 30)]
        [InlineData("3 This\u2000is\u2000not\u2000whitespace", 30)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsMissingOrTooLong_ShouldBeFalse(string input, int maxLength)
        {
            // arrange
            // act
            var result = input.IsMissingOrTooLong(maxLength);

            // assert
            result.Should().BeFalse();
        }


        [Theory]
        [InlineData("1 This is not whitespace")]
        [InlineData("2 This is not whitespace     ")]
        [InlineData("3 This\u2000is\u2000not\u2000whitespace")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsPresent_ShouldBeTrue(string input)
        {
            // arrange
            // act
            var result = input.IsPresent();

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("                ")]
        [InlineData("  \t  ")]
        [InlineData("\u2000\u2000")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsPresent_ShouldBeFalse(string input)
        {
            // arrange
            // act
            var result = input.IsPresent();

            // assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void NullIfEmpty_ShouldBeNull(string input)
        {
            // arrange
            // act
            var result = input.NullIfEmpty();

            // assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("Testinput")]
        [InlineData("matching case")]
        [InlineData("                ")]
        [InlineData("  \t  ")]
        [InlineData("\u2000\u2000")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void NullIfEmpty_ShouldNotBeNull(string input)
        {
            // arrange
            // act
            var result = input.NullIfEmpty();

            // assert
            result.Should().BeEquivalentTo(input);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("this is a test", "this is a test")]
        [InlineData("{removed", "removed")]
        [InlineData("removed}", "removed")]
        [InlineData("\u2000\u2000", "\u2000\u2000")]
        [InlineData("{removed}", "removed")]
        [InlineData("{", "")]
        [InlineData("}", "")]
        [InlineData("{re{{mo}}{}}ved}", "removed")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void RemoveCurlyBrackets_ShouldBeOk(string str1, string expected)
        {
            // act
            var result = str1.RemoveCurlyBrackets();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("this is a test", "thisisatest")]
        [InlineData("{removed", "removed")]
        [InlineData("property-Name", "propertyName")]
        [InlineData("property_Name", "property_Name")]
        [InlineData("property23_Name", "property23_Name")]
        [InlineData("property+*#Name", "propertyName")]
        [InlineData("property\u2000\u2000Name", "propertyName")]
        public void SanitizePropertyName_ShouldBeOk(string source, string expected)
        {
            // act
            var result = source.SanitizePropertyName();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData(null, 0)]
        [InlineData("AD-AJ2-15", 15)]
        [InlineData("AD-X-2", 2)]
        [InlineData("A-345", 345)]
        [InlineData("213", 213)]
        [InlineData("A123", 0)]
        [InlineData("AS-XD-123-SD", 0)]
        public void ExtractBuildNumber_ShouldBeOk(string source, int expected)
        {
            // act
            var result = source.ExtractBuildNumber();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("This is a test", "this is a test")]
        [InlineData("{removed", "{removed")]
        [InlineData("Removed}", "removed}")]
        [InlineData("\u2000\u2000", "\u2000\u2000")]
        [InlineData("MyNameIs", "myNameIs")]
        [InlineData("myNameIs", "myNameIs")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void FirstCharacterToLower_ShouldBeOk(string str1, string expected)
        {
            // act
            var result = str1.FirstCharacterToLower();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("This is a test", "This is a test")]
        [InlineData("{removed", "{removed")]
        [InlineData("Removed}", "Removed}")]
        [InlineData("\u2000\u2000", "\u2000\u2000")]
        [InlineData("MyNameIs", "MyNameIs")]
        [InlineData("myNameIs", "MyNameIs")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void FirstCharacterToUpper_ShouldBeOk(string str1, string expected)
        {
            // act
            var result = str1.FirstCharacterToUpper();

            // assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("{\"test\": 1}")]
        [InlineData("[{\"test\": 1}]")]
        [InlineData("{}")]
        public void IsValidJson_ShouldBeTrue(string source)
        {
            // act
            var result = source.IsValidJson();

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("AD-AJ2-15")]
        [InlineData("{ test: test")]
        [InlineData("test: }")]
        [InlineData("\"test\": 1 }")]
        [InlineData("{ \"test\": test")]
        [InlineData("\"test\": test")]
        [InlineData("{test: 1}")]
        public void IsValidJson_ShouldBeFalse(string source)
        {
            // act
            var result = source.IsValidJson();

            // assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("dGhpcyBpcyBhIHRlc3Q=")]
        [InlineData("dGhpcyBpcyBhIHRlc3Qgd2l0aCBhbm90aGVyIGludXQgd2hpY2ggaXMgbG9uZ2VyIGFuZCBzaG91bGQgcHJvZHVjZSBhIGxvbmdlciBsb25nZXIgYmFzZTY0IHN0cmluZyBvdXRwdXQ=")]
        [InlineData("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")]
        public void IsBase64_ShouldBeTrue(string source)
        {
            // act
            var result = source.IsBase64();

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("AD-AJ2-15")]
        [InlineData("{ test: test")]
        [InlineData("simple test test")]
        [InlineData("dGhpcyBAbNdrPpcyBhIHRlc3Q=")]
        public void IsBase64_ShouldBeFalse(string source)
        {
            // act
            var result = source.IsBase64();

            // assert
            result.Should().BeFalse();
        }
    }
}

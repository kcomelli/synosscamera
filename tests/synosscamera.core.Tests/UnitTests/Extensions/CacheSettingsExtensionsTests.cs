using FluentAssertions;
using synosscamera.core.Extensions;
using synosscamera.core.Infrastructure.Cache;
using synosscamera.testsuite;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace synosscamera.core.Tests.UnitTests.Extensions
{
    public class CacheSettingsExtensionsTests : TestBase
    {
        const string Category = "String extensions";
        const string TestType = "Unit";

        private Dictionary<string, CacheSettings> _cacheSettings;

        protected override void TestInitialize()
        {
            _cacheSettings = new Dictionary<string, CacheSettings>()
            {
                {
                    "cache1",
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Minutes, AbsoluteExpiration = 6, SlidingExpiration = 2 }
                },
                {
                    "cache2",
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Milliseconds, AbsoluteExpiration = 6, SlidingExpiration = 2 }
                },
                {
                    "cache3",
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Seconds, AbsoluteExpiration = 6, SlidingExpiration = 2 }
                },
                {
                    "cache4",
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Hours, AbsoluteExpiration = 6, SlidingExpiration = 2 }
                },
                {
                    "cache5",
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Hours, SlidingExpiration = 2 }
                },
                {
                    "cache6",
                    new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Hours, AbsoluteExpiration = 6 }
                }
            };
        }
        protected override void TestCleanUp()
        {
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void EnsureSettings_ShouldInitNew()
        {
            // arrange
            string cacheKey = "unknown key";

            // act
            var cacheSettings = _cacheSettings.EnsureSettings(cacheKey);

            // assert
            cacheSettings.Should().NotBeNull();
            cacheSettings.Enabled.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void EnsureSettings_ShouldReturnExisting()
        {
            // arrange
            string cacheKey = "cache1";

            // act
            var cacheSettings = _cacheSettings.EnsureSettings(cacheKey);

            // assert
            cacheSettings.Should().NotBeNull();
            cacheSettings.Should().Be(_cacheSettings[cacheKey]);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetAbsoluteExpiration_ShouldReturnNull()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache5"]; // AbsoluteExpiration not set

            // act
            var absolutExpiration = cacheSettings.GetAbsoluteExpiration();

            // assert
            absolutExpiration.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetAbsoluteExpiration_ShouldBeHaveMinutesAbsolute()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache1"];

            // act
            var absolutExpiration = cacheSettings.GetAbsoluteExpiration();

            // assert
            absolutExpiration.Should().NotBeNull();
            absolutExpiration.Value.TotalMinutes.Should().Be(cacheSettings.AbsoluteExpiration.Value);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetAbsoluteExpiration_ShouldBeHaveMillisecondsAbsolute()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache2"];

            // act
            var absolutExpiration = cacheSettings.GetAbsoluteExpiration();

            // assert
            absolutExpiration.Should().NotBeNull();
            absolutExpiration.Value.TotalMilliseconds.Should().Be(cacheSettings.AbsoluteExpiration.Value);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetAbsoluteExpiration_ShouldBeHaveSecondsAbsolute()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache3"];

            // act
            var absolutExpiration = cacheSettings.GetAbsoluteExpiration();

            // assert
            absolutExpiration.Should().NotBeNull();
            absolutExpiration.Value.TotalSeconds.Should().Be(cacheSettings.AbsoluteExpiration.Value);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetAbsoluteExpiration_ShouldBeHaveHoursAbsolute()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache4"];

            // act
            var absolutExpiration = cacheSettings.GetAbsoluteExpiration();

            // assert
            absolutExpiration.Should().NotBeNull();
            absolutExpiration.Value.TotalHours.Should().Be(cacheSettings.AbsoluteExpiration.Value);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetSlidingExpiration_ShouldReturnNull()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache6"]; // SlidingExpiration not set

            // act
            var absolutExpiration = cacheSettings.GetSlidingExpiration();

            // assert
            absolutExpiration.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetSlidingExpiration_ShouldBeHaveMinutesAbsolute()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache1"];

            // act
            var absolutExpiration = cacheSettings.GetSlidingExpiration();

            // assert
            absolutExpiration.Should().NotBeNull();
            absolutExpiration.Value.TotalMinutes.Should().Be(cacheSettings.SlidingExpiration.Value);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetSlidingExpiration_ShouldBeHaveMillisecondsAbsolute()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache2"];

            // act
            var absolutExpiration = cacheSettings.GetSlidingExpiration();

            // assert
            absolutExpiration.Should().NotBeNull();
            absolutExpiration.Value.TotalMilliseconds.Should().Be(cacheSettings.SlidingExpiration.Value);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetSlidingExpiration_ShouldBeHaveSecondsAbsolute()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache3"];

            // act
            var absolutExpiration = cacheSettings.GetSlidingExpiration();

            // assert
            absolutExpiration.Should().NotBeNull();
            absolutExpiration.Value.TotalSeconds.Should().Be(cacheSettings.SlidingExpiration.Value);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void GetSlidingExpiration_ShouldBeHaveHoursAbsolute()
        {
            // arrange
            var cacheSettings = _cacheSettings["cache4"];

            // act
            var absolutExpiration = cacheSettings.GetSlidingExpiration();

            // assert
            absolutExpiration.Should().NotBeNull();
            absolutExpiration.Value.TotalHours.Should().Be(cacheSettings.SlidingExpiration.Value);
        }
    }
}

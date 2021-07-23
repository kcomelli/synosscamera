using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using synosscamera.core.Abstractions;
using synosscamera.core.Extensions;
using synosscamera.core.Infrastructure.Cache;
using synosscamera.testsuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace synosscamera.core.Tests.UnitTests.Infrastructure.Cache
{
    public class DefaultMemoryCacheWrapperTests : TestBase
    {
        const string Category = "Default MemoryCache wrapper";
        const string TestType = "Unit";

        const string CacheGroup1 = "CacheGroup1";
        const string CacheGroup2 = "CacheGroup2";
        const string CacheGroup3 = "CacheGroup3";

        public enum ConstructorFailTestModes
        {
            NoLoggerFactory,
            NoSettingsProvider,
            NoCache
        }

        private class TestCacheSettingsProvider : ICacheSettingsProvider
        {
            public TestCacheSettingsProvider()
            {
                CacheSettings = new Dictionary<string, CacheSettings>()
                {
                    {
                        CacheGroup1,
                        new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Seconds, AbsoluteExpiration = 15, SlidingExpiration = 5 }
                    },
                    {
                        CacheGroup2,
                        new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Seconds, AbsoluteExpiration = 15, SlidingExpiration = 5 }
                    },
                    {
                        CacheGroup3,
                        new CacheSettings() { Enabled = true, TimeoutUnit = CacheTimeoutUnit.Seconds, AbsoluteExpiration = 30, SlidingExpiration = 5 }
                    }
                };
            }

            public Dictionary<string, CacheSettings> CacheSettings { get; set; }

            public CacheSettings GetSettings(string settingsKey)
            {
                var ret = new CacheSettings() { Enabled = false }; ;

                if (settingsKey.IsPresent() && CacheSettings.ContainsKey(settingsKey))
                    ret = CacheSettings[settingsKey];

                ret.GroupId = settingsKey;
                return ret;
            }
        }



        private InspectableTestLoggerFactory<DefaultMemoryCacheWrapper> _inspectableLogger;
        private IMemoryCache _internalCache;
        private TestCacheSettingsProvider _cacheSettings;

        protected override void TestInitialize()
        {
            _inspectableLogger = new InspectableTestLoggerFactory<DefaultMemoryCacheWrapper>();
            _internalCache = new MemoryCache(Options.Create(new MemoryCacheOptions() { Clock = new SystemClock() }));
            _cacheSettings = new TestCacheSettingsProvider();
        }
        protected override void TestCleanUp()
        {
            _internalCache.Dispose();
            _internalCache = null;
        }

        private void BuildSampleCache(DefaultMemoryCacheWrapper cache)
        {
            cache.Set(CacheGroup1, "key1", "this is a value 1");
            cache.Set(CacheGroup1, "key2", "this is a value 2");
            cache.Set(CacheGroup1, "key3", "this is a value 3");

            cache.Set(CacheGroup2, "key4", "this is a value 4");
            cache.Set(CacheGroup2, "key5", "this is a value 5");
            cache.Set(CacheGroup2, "key6", "this is a value 6");

            cache.Set(CacheGroup3, "key7", "this is a value 7");
            cache.Set(CacheGroup3, "key8", "this is a value 8");
            cache.Set(CacheGroup3, "key9", "this is a value 9");
        }

        [Theory]
        [InlineData(ConstructorFailTestModes.NoLoggerFactory)]
        [InlineData(ConstructorFailTestModes.NoSettingsProvider)]
        [InlineData(ConstructorFailTestModes.NoCache)]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void Constructor_ShouldThrow(ConstructorFailTestModes mode)
        {
            // arrange
            var factory = _inspectableLogger.Factory;
            var cache = _internalCache;
            var settingsProvider = _cacheSettings;

            switch (mode)
            {
                case ConstructorFailTestModes.NoLoggerFactory:
                    factory = null;
                    break;
                case ConstructorFailTestModes.NoSettingsProvider:
                    settingsProvider = null;
                    break;
                case ConstructorFailTestModes.NoCache:
                    cache = null;
                    break;
            }

            // act & assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                new DefaultMemoryCacheWrapper(factory, settingsProvider, cache);
            });
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CreateEntry_ShouldHaveTwoExpirationTokens()
        {
            // arrange
            var useGroup = CacheGroup1;
            using (var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache))
            {
                // act
                using (var entry = sut.CreateEntry(useGroup, "myNewKey"))
                {
                    // assert
                    entry.ExpirationTokens.Should().HaveCount(2);
                }
            }
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void Clear_ExistingGroup_ShouldBeOk()
        {
            // arrange
            var useGroup = CacheGroup1;
            using (var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache))
            {
                BuildSampleCache(sut);

                // assert
                sut.Get(useGroup, "key1").Should().NotBeNull();
                sut.Get(useGroup, "key2").Should().NotBeNull();
                sut.Get(useGroup, "key3").Should().NotBeNull();
                // even though key4 is part of another group
                // the cache should return its value 
                sut.Get(useGroup, "key4").Should().NotBeNull();

                // act
                var result = sut.Clear(CacheGroup1);

                // assert
                result.Should().BeTrue();

                sut.Get(useGroup, "key1").Should().BeNull();
                sut.Get(useGroup, "key2").Should().BeNull();
                sut.Get(useGroup, "key3").Should().BeNull();
                // even though key4 is part of another group
                // the cache should return its value 
                sut.Get(useGroup, "key4").Should().NotBeNull();
            }
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void Clear_AllGroup_ShouldBeOk()
        {
            // arrange
            var useGroup = CacheGroup1;
            using (var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache))
            {
                BuildSampleCache(sut);

                // assert
                sut.Get(useGroup, "key1").Should().NotBeNull();
                sut.Get(useGroup, "key2").Should().NotBeNull();
                sut.Get(useGroup, "key3").Should().NotBeNull();
                // even though key4 is part of another group
                // the cache should return its value 
                sut.Get(useGroup, "key4").Should().NotBeNull();

                // act
                var result = sut.Clear();

                // assert
                result.Should().BeTrue();

                sut.Get(useGroup, "key1").Should().BeNull();
                sut.Get(useGroup, "key2").Should().BeNull();
                sut.Get(useGroup, "key3").Should().BeNull();
                sut.Get(useGroup, "key4").Should().BeNull();
                sut.Get(useGroup, "key5").Should().BeNull();
                sut.Get(useGroup, "key6").Should().BeNull();
                sut.Get(useGroup, "key7").Should().BeNull();
            }
        }

        #region dispose tests
        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void Clear_Disposed_ShouldThrow()
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);
            sut.Dispose();

            // act & assert
            Assert.Throws<ObjectDisposedException>(() =>
            {
                sut.Clear(CacheGroup1);
            });
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CreateEntry_Disposed_ShouldThrow()
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);
            sut.Dispose();

            // act & assert
            Assert.Throws<ObjectDisposedException>(() =>
            {
                sut.CreateEntry(CacheGroup1, "new key");
            });
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void DefaultOptionsOfCache_Disposed_ShouldThrow()
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);
            sut.Dispose();

            // act & assert
            Assert.Throws<ObjectDisposedException>(() =>
            {
                sut.DefaultOptionsOfCache(CacheGroup1);
            });
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void Remove_Disposed_ShouldThrow()
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);
            sut.Dispose();

            // act & assert
            Assert.Throws<ObjectDisposedException>(() =>
            {
                sut.Remove(CacheGroup1, "new key");
            });
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void TryGetValue_Disposed_ShouldThrow()
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);
            sut.Dispose();

            // act & assert
            Assert.Throws<ObjectDisposedException>(() =>
            {
                sut.TryGetValue(CacheGroup1, "key1", out object val1);
            });
        }
        #endregion

        #region invalide cache group id
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("CompleteCache")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void CreateEntry_InvalidGroupId_ShouldThrow(string invalidCacheGroupId)
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);

            // act & assert
            Assert.Throws<ArgumentException>(() =>
            {
                sut.CreateEntry(invalidCacheGroupId, "new key");
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("CompleteCache")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void DefaultOptionsOfCache_InvalidGroupId_ShouldThrow(string invalidCacheGroupId)
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);

            // act & assert
            Assert.Throws<ArgumentException>(() =>
            {
                sut.DefaultOptionsOfCache(invalidCacheGroupId);
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("CompleteCache")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void Remove_InvalidGroupId_ShouldThrow(string invalidCacheGroupId)
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);

            // act & assert
            Assert.Throws<ArgumentException>(() =>
            {
                sut.Remove(invalidCacheGroupId, "rem key");
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("CompleteCache")]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void TryGetValue_InvalidGroupId_ShouldThrow(string invalidCacheGroupId)
        {
            // arrange
            var sut = new DefaultMemoryCacheWrapper(_inspectableLogger.Factory, _cacheSettings, _internalCache);

            // act & assert
            Assert.Throws<ArgumentException>(() =>
            {
                sut.TryGetValue(invalidCacheGroupId, "key1", out object val1);
            });
        }
        #endregion
    }
}

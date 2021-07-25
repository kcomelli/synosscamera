using FluentAssertions;
using synosscamera.core.Extensions;
using synosscamera.core.Tests.Helpers;
using synosscamera.testsuite;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Xunit;

namespace synosscamera.core.Tests.UnitTests.Extensions
{
    public class TypeExtensionsTests : TestBase
    {
        const string Category = "Type Extensions";
        const string TestType = "Unit";

        #region test classes for type extensions
        [CustomOne("Test")]
        public class SampleClass1
        {
            public string Test { get; set; }
            public int TestInt { get; set; }
            [CustomOne("Test2")]
            public decimal TestDec { get; set; }

            public string SampleString { get; set; }

            [CustomTwo(nameof(TestInt))]
            public object SampleRef { get; set; }
        }

        public class Class1 { }
        public class BaseGeneric<T> : IBaseGeneric<T> { }
        public class BaseGeneric2<T> : IBaseGeneric<T>, IInterfaceBidon { }
        public interface IBaseGeneric<T> { }
        public class ChildGeneric : BaseGeneric<Class1> { }
        public interface IChildGeneric : IBaseGeneric<Class1> { }
        public class ChildGeneric2<Class1> : BaseGeneric<Class1> { }
        public interface IChildGeneric2<Class1> : IBaseGeneric<Class1> { }

        public class WrongBaseGeneric<T> { }
        public interface IWrongBaseGeneric<T> { }

        public interface IInterfaceBidon { }

        public class ClassA { }
        public class ClassB { }
        public class ClassC { }
        public class ClassB2 : ClassB { }
        public class BaseGenericA<T, U> : IBaseGenericA<T, U> { }
        public class BaseGenericB<T, U, V> { }
        public interface IBaseGenericB<ClassA, ClassB, ClassC> { }
        public class BaseGenericA2<T, U> : IBaseGenericA<T, U>, IInterfaceBidonA { }
        public interface IBaseGenericA<T, U> { }
        public class ChildGenericA : BaseGenericA<ClassA, ClassB> { }
        public interface IChildGenericA : IBaseGenericA<ClassA, ClassB> { }
        public class ChildGenericA2<ClassA, ClassB> : BaseGenericA<ClassA, ClassB> { }
        public class ChildGenericA3<ClassA, ClassB> : BaseGenericB<ClassA, ClassB, ClassC> { }
        public class ChildGenericA4<ClassA, ClassB> : IBaseGenericB<ClassA, ClassB, ClassC> { }
        public interface IChildGenericA2<ClassA, ClassB> : IBaseGenericA<ClassA, ClassB> { }

        public class WrongBaseGenericA<T, U> { }
        public interface IWrongBaseGenericA<T, U> { }

        public interface IInterfaceBidonA { }
        #endregion

        /// <summary>
        /// This method will be executed for each test and you may initialize test or fake data
        /// </summary>
        protected override void TestInitialize()
        {

        }
        /// <summary>
        /// This method will be executed after each test. Use it to free resources.
        /// </summary>
        protected override void TestCleanUp()
        {
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void LoadCustomAttributes_Type_NullType_ShouldReturnNull()
        {
            // arrange

            // act 
            var attribs = ((Type)null).LoadCustomAttributes<CustomOneAttribute>();

            // assert
            attribs.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void LoadCustomAttributes_Type_ShouldReturnData()
        {
            // arrange

            // act 
            var attribs = typeof(SampleClass1).LoadCustomAttributes<CustomOneAttribute>();

            // assert
            attribs.Should().NotBeNull();
            attribs.Should().HaveCount(1);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void LoadCustomAttributes_Type_ShouldBeEmpty()
        {
            // arrange

            // act 
            var attribs = typeof(SampleClass1).LoadCustomAttributes<RequiredAttribute>();

            // assert
            attribs.Should().NotBeNull();
            attribs.Should().BeEmpty();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void HasCustomAttribute_Type_NullType_ShouldBeFalse()
        {
            // arrange

            // act 
            var hasAttribs = ((Type)null).HasCustomAttribute<CustomOneAttribute>();

            // assert
            hasAttribs.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void HasCustomAttribute_Type_ShouldBeTrue()
        {
            // arrange

            // act 
            var hasAttribs = typeof(SampleClass1).HasCustomAttribute<CustomOneAttribute>();

            // assert
            hasAttribs.Should().BeTrue();

        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void HasCustomAttribute_Type_ShouldBeFalse()
        {
            // arrange

            // act 
            var hasAttribs = typeof(SampleClass1).HasCustomAttribute<RequiredAttribute>();

            // assert
            hasAttribs.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void LoadCustomAttributes_PropertyInfo_Null_ShouldReturnNull()
        {
            // arrange

            // act 
            var attribs = ((PropertyInfo)null).LoadCustomAttributes<CustomOneAttribute>();

            // assert
            attribs.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void LoadCustomAttributes_PropertyInfo_ShouldReturnData()
        {
            // arrange
            var propInfo = typeof(SampleClass1).GetProperty(nameof(SampleClass1.TestDec));

            // act 
            var attribs = propInfo.LoadCustomAttributes<CustomOneAttribute>();

            // assert
            attribs.Should().NotBeNull();
            attribs.Should().HaveCount(1);
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void LoadCustomAttributes_PropertyInfo_ShouldBeEmpty()
        {
            // arrange
            var propInfo = typeof(SampleClass1).GetProperty(nameof(SampleClass1.Test));

            // act 
            var attribs = propInfo.LoadCustomAttributes<RequiredAttribute>();

            // assert
            attribs.Should().NotBeNull();
            attribs.Should().BeEmpty();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void HasCustomAttribute_PropertyInfo_Null_ShouldBeFalse()
        {
            // arrange

            // act 
            var hasAttribs = ((PropertyInfo)null).HasCustomAttribute<CustomOneAttribute>();

            // assert
            hasAttribs.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void HasCustomAttribute_PropertyInfo_ShouldBeTrue()
        {
            // arrange
            var propInfo = typeof(SampleClass1).GetProperty(nameof(SampleClass1.TestDec));

            // act 
            var hasAttribs = propInfo.HasCustomAttribute<CustomOneAttribute>();

            // assert
            hasAttribs.Should().BeTrue();

        }

        [Fact]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void HasCustomAttribute_PropertyInfo_ShouldBeFalse()
        {
            // arrange
            var propInfo = typeof(SampleClass1).GetProperty(nameof(SampleClass1.Test));

            // act 
            var hasAttribs = propInfo.HasCustomAttribute<RequiredAttribute>();

            // assert
            hasAttribs.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(ChildGeneric), typeof(BaseGeneric<>))]
        [InlineData(typeof(ChildGeneric), typeof(IBaseGeneric<>))]
        [InlineData(typeof(IChildGeneric), typeof(IBaseGeneric<>))]
        [InlineData(typeof(ChildGeneric2<>), typeof(BaseGeneric<>))]
        [InlineData(typeof(ChildGeneric2<Class1>), typeof(BaseGeneric<>))]
        [InlineData(typeof(ChildGeneric), typeof(BaseGeneric<Class1>))]
        [InlineData(typeof(ChildGeneric), typeof(IBaseGeneric<Class1>))]
        [InlineData(typeof(IChildGeneric), typeof(IBaseGeneric<Class1>))]
        [InlineData(typeof(ChildGeneric2<Class1>), typeof(BaseGeneric<Class1>))]
        [InlineData(typeof(IChildGeneric2<>), typeof(IBaseGeneric<>))]
        [InlineData(typeof(IChildGeneric2<Class1>), typeof(IBaseGeneric<>))]
        [InlineData(typeof(IChildGeneric2<Class1>), typeof(IBaseGeneric<Class1>))]
        [InlineData(typeof(BaseGeneric<Class1>), typeof(IBaseGeneric<Class1>))]
        [InlineData(typeof(BaseGeneric<>), typeof(IBaseGeneric<>))]
        [InlineData(typeof(BaseGeneric<Class1>), typeof(IBaseGeneric<>))]
        [InlineData(typeof(BaseGeneric2<Class1>), typeof(IBaseGeneric<Class1>))]
        [InlineData(typeof(BaseGeneric2<>), typeof(IBaseGeneric<>))]
        [InlineData(typeof(BaseGeneric2<Class1>), typeof(IBaseGeneric<>))]
        [InlineData(typeof(ChildGenericA), typeof(BaseGenericA<,>))]
        [InlineData(typeof(ChildGenericA), typeof(IBaseGenericA<,>))]
        [InlineData(typeof(IChildGenericA), typeof(IBaseGenericA<,>))]
        [InlineData(typeof(ChildGenericA2<,>), typeof(BaseGenericA<,>))]
        [InlineData(typeof(ChildGenericA2<ClassA, ClassB>), typeof(BaseGenericA<,>))]
        [InlineData(typeof(ChildGenericA), typeof(BaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(ChildGenericA), typeof(IBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(IChildGenericA), typeof(IBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(ChildGenericA2<ClassA, ClassB>), typeof(BaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(IChildGenericA2<,>), typeof(IBaseGenericA<,>))]
        [InlineData(typeof(IChildGenericA2<ClassA, ClassB>), typeof(IBaseGenericA<,>))]
        [InlineData(typeof(IChildGenericA2<ClassA, ClassB>), typeof(IBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(BaseGenericA<ClassA, ClassB>), typeof(IBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(BaseGenericA<,>), typeof(IBaseGenericA<,>))]
        [InlineData(typeof(BaseGenericA<ClassA, ClassB>), typeof(IBaseGenericA<,>))]
        [InlineData(typeof(BaseGenericA2<ClassA, ClassB>), typeof(IBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(BaseGenericA2<,>), typeof(IBaseGenericA<,>))]
        [InlineData(typeof(BaseGenericA2<ClassA, ClassB>), typeof(IBaseGenericA<,>))]
        [InlineData(typeof(ChildGenericA3<ClassA, ClassB>), typeof(BaseGenericB<ClassA, ClassB, ClassC>))]
        [InlineData(typeof(ChildGenericA4<ClassA, ClassB>), typeof(IBaseGenericB<ClassA, ClassB, ClassC>))]
        [InlineData(typeof(ChildGenericA3<ClassA, ClassB2>), typeof(BaseGenericB<ClassA, ClassB, ClassC>))]
        [InlineData(typeof(ChildGenericA4<ClassA, ClassB2>), typeof(IBaseGenericB<ClassA, ClassB, ClassC>))]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsSubClassOfGeneric_ShouldBeTrue(Type currentType, Type typeToCheck)
        {
            // arrange

            // act 
            var result = currentType.IsSubClassOfGeneric(typeToCheck);

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(ChildGeneric), typeof(WrongBaseGeneric<>))]
        [InlineData(typeof(ChildGeneric), typeof(IWrongBaseGeneric<>))]
        [InlineData(typeof(IWrongBaseGeneric<>), typeof(ChildGeneric2<>))]
        [InlineData(typeof(ChildGeneric), typeof(WrongBaseGeneric<Class1>))]
        [InlineData(typeof(ChildGeneric), typeof(IWrongBaseGeneric<Class1>))]
        [InlineData(typeof(BaseGeneric<Class1>), typeof(ChildGeneric2<Class1>))]
        [InlineData(typeof(ChildGeneric), typeof(ChildGeneric))]
        [InlineData(typeof(IChildGeneric), typeof(IChildGeneric))]
        [InlineData(typeof(IBaseGeneric<>), typeof(IChildGeneric2<>))]
        [InlineData(typeof(IBaseGeneric<Class1>), typeof(IChildGeneric2<Class1>))]
        [InlineData(typeof(IBaseGeneric<Class1>), typeof(BaseGeneric<Class1>))]
        [InlineData(typeof(IBaseGeneric<>), typeof(BaseGeneric<>))]
        [InlineData(typeof(IBaseGeneric<Class1>), typeof(IBaseGeneric<Class1>))]
        [InlineData(typeof(IBaseGeneric<>), typeof(BaseGeneric2<>))]
        [InlineData(typeof(ChildGenericA), typeof(WrongBaseGenericA<,>))]
        [InlineData(typeof(ChildGenericA), typeof(IWrongBaseGenericA<,>))]
        [InlineData(typeof(IWrongBaseGenericA<,>), typeof(ChildGenericA2<,>))]
        [InlineData(typeof(ChildGenericA), typeof(WrongBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(ChildGenericA), typeof(IWrongBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(BaseGenericA<ClassA, ClassB>), typeof(ChildGenericA2<ClassA, ClassB>))]
        [InlineData(typeof(ChildGenericA), typeof(ChildGenericA))]
        [InlineData(typeof(IChildGenericA), typeof(IChildGenericA))]
        [InlineData(typeof(IBaseGenericA<,>), typeof(IChildGenericA2<,>))]
        [InlineData(typeof(IBaseGenericA<ClassA, ClassB>), typeof(IChildGenericA2<ClassA, ClassB>))]
        [InlineData(typeof(IBaseGenericA<ClassA, ClassB>), typeof(BaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(IBaseGenericA<,>), typeof(BaseGenericA<,>))]
        [InlineData(typeof(IBaseGenericA<ClassA, ClassB>), typeof(IBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(IBaseGenericA<,>), typeof(BaseGenericA2<,>))]
        [InlineData(typeof(BaseGenericA2<ClassB, ClassA>), typeof(IBaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(BaseGenericA<ClassB, ClassA>), typeof(ChildGenericA2<ClassA, ClassB>))]
        [InlineData(typeof(BaseGenericA2<ClassB, ClassA>), typeof(BaseGenericA<ClassA, ClassB>))]
        [InlineData(typeof(ChildGenericA3<ClassB, ClassA>), typeof(BaseGenericB<ClassA, ClassB, ClassC>))]
        [InlineData(typeof(ChildGenericA3<ClassB2, ClassA>), typeof(BaseGenericB<ClassA, ClassB, ClassC>))]
        [InlineData(typeof(ChildGenericA4<ClassB, ClassA>), typeof(IBaseGenericB<ClassA, ClassB, ClassC>))]
        [InlineData(typeof(ChildGenericA4<ClassB2, ClassA>), typeof(IBaseGenericB<ClassA, ClassB, ClassC>))]
        [InlineData(typeof(bool), typeof(IBaseGenericB<ClassA, ClassB, ClassC>))]
        [Trait("Category", Category)]
        [Trait("TestType", TestType)]
        public void IsSubClassOfGeneric_ShouldBeFalse(Type currentType, Type typeToCheck)
        {
            // arrange

            // act 
            var result = currentType.IsSubClassOfGeneric(typeToCheck);

            // assert
            result.Should().BeFalse();
        }
    }
}

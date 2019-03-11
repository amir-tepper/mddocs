﻿using System.Linq;
using Grynwald.MdDocs.ApiReference.Model;
using Grynwald.MdDocs.ApiReference.Test.TestData;
using Xunit;

namespace Grynwald.MdDocs.ApiReference.Test.Model
{
    public class MethodFormatterTest : TestBase
    {
        [Theory]
        [InlineData(nameof(TestClass_MethodFormatter.Method1), "Method1()")]
        [InlineData(nameof(TestClass_MethodFormatter.Method2), "Method2()")]
        [InlineData(nameof(TestClass_MethodFormatter.Method3), "Method3(string)")]
        [InlineData(nameof(TestClass_MethodFormatter.Method4), "Method4(IDisposable)")]
        [InlineData(nameof(TestClass_MethodFormatter.Method5), "Method5<T>(string)")]
        [InlineData(nameof(TestClass_MethodFormatter.Method6), "Method6<T>(T)")]
        [InlineData(nameof(TestClass_MethodFormatter.Method7), "Method7<T1, T2>(T1, T2)")]
        public void GetSignature_returns_the_expected_result(string methodName, string expectedSignature)
        {
            // ARRANGE
            var method = GetTypeDefinition(typeof(TestClass_MethodFormatter))
                   .Methods
                   .Single(x => x.Name == methodName);

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(method);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }

        [Theory]
        [InlineData(nameof(TestClass_MethodFormatter.Method1), "Method1()")]
        [InlineData(nameof(TestClass_MethodFormatter.Method2), "Method2()")]
        [InlineData(nameof(TestClass_MethodFormatter.Method3), "Method3(string)")]
        [InlineData(nameof(TestClass_MethodFormatter.Method4), "Method4(IDisposable)")]
        [InlineData(nameof(TestClass_MethodFormatter.Method5), "Method5<T>(string)")]
        [InlineData(nameof(TestClass_MethodFormatter.Method6), "Method6<T>(T)")]
        [InlineData(nameof(TestClass_MethodFormatter.Method7), "Method7<T1, T2>(T1, T2)")]
        public void GetSignature_returns_the_expected_result_for_method_ids(string methodName, string expectedSignature)
        {
            // ARRANGE
            var methodId = GetTypeDefinition(typeof(TestClass_MethodFormatter))
                   .Methods
                   .Single(x => x.Name == methodName)
                   .ToMethodId();

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(methodId);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }

        [Theory]
        [InlineData(0, "TestClass_MethodFormatter()")]
        [InlineData(1, "TestClass_MethodFormatter(string)")]
        [InlineData(2, "TestClass_MethodFormatter(string, IEnumerable<string>)")]
        [InlineData(3, "TestClass_MethodFormatter(string, IEnumerable<string>, IList<DirectoryInfo>)")]
        public void GetSignature_returns_the_expected_result_for_constructors(int parameterCount, string expectedSignature)
        {
            // ARRANGE
            var method = GetTypeDefinition(typeof(TestClass_MethodFormatter))
                   .Methods
                   .Single(x => x.IsConstructor && x.Parameters.Count == parameterCount);

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(method);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }

        [Theory]
        [InlineData(0, "TestClass_MethodFormatter()")]
        [InlineData(1, "TestClass_MethodFormatter(string)")]
        public void GetSignature_returns_the_expected_result_for_constructors_of_generic_types(int parameterCount, string expectedSignature)
        {
            // ARRANGE
            var method = GetTypeDefinition(typeof(TestClass_MethodFormatter<>))
                   .Methods
                   .Single(x => x.IsConstructor && x.Parameters.Count == parameterCount);

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(method);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }


        [Theory]
        [InlineData(0, "TestClass_MethodFormatter()")]
        [InlineData(1, "TestClass_MethodFormatter(string)")]
        [InlineData(2, "TestClass_MethodFormatter(string, IEnumerable<string>)")]
        [InlineData(3, "TestClass_MethodFormatter(string, IEnumerable<string>, IList<DirectoryInfo>)")]
        public void GetSignature_returns_the_expected_result_for_constructors_as_method_ids(int parameterCount, string expectedSignature)
        {
            // ARRANGE
            var methodId = GetTypeDefinition(typeof(TestClass_MethodFormatter))
                   .Methods
                   .Single(x => x.IsConstructor && x.Parameters.Count == parameterCount)
                   .ToMethodId();

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(methodId);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }


        [Theory]
        [InlineData(0, "TestClass_MethodFormatter()")]
        [InlineData(1, "TestClass_MethodFormatter(string)")]
        public void GetSignature_returns_the_expected_result_for_constructors_of_generic_types_as_method_ids(int parameterCount, string expectedSignature)
        {
            // ARRANGE
            var methodId = GetTypeDefinition(typeof(TestClass_MethodFormatter<>))
                   .Methods
                   .Single(x => x.IsConstructor && x.Parameters.Count == parameterCount)
                   .ToMethodId();

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(methodId);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }

        [Theory]
        [InlineData("op_Addition", "Addition(TestClass_MethodFormatter, TestClass_MethodFormatter)")]
        [InlineData("op_Implicit", "Implicit(TestClass_MethodFormatter to string)")]
        public void GetSignature_returns_the_expected_result_for_operators(string methodName, string expectedSignature)
        {
            // ARRANGE
            var method = GetTypeDefinition(typeof(TestClass_MethodFormatter))
                   .Methods
                   .Single(x => x.Name == methodName);

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(method);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }


        [Theory]
        [InlineData("op_Addition", "Addition(TestClass_MethodFormatter, TestClass_MethodFormatter)")]
        [InlineData("op_Implicit", "Implicit(TestClass_MethodFormatter to string)")]
        public void GetSignature_returns_the_expected_result_for_operators_as_method_ids(string methodName, string expectedSignature)
        {
            // ARRANGE
            var methodId = GetTypeDefinition(typeof(TestClass_MethodFormatter))
                   .Methods
                   .Single(x => x.Name == methodName)
                   .ToMethodId();

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(methodId);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }


        [Theory]
        [InlineData(1, "Item[int]")]
        [InlineData(2, "Item[int, int]")]
        public void GetSignature_returns_the_expected_result_for_indexers(int paramterCount, string expectedSignature)
        {
            // ARRANGE
            var method = GetTypeDefinition(typeof(TestClass_MethodFormatter))
                   .Properties
                   .Single(x => x.Parameters.Count == paramterCount);

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(method);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }


        [Theory]
        [InlineData(1, "Item[int]")]
        [InlineData(2, "Item[int, int]")]
        public void GetSignature_returns_the_expected_result_for_indexers_as_property_id(int paramterCount, string expectedSignature)
        {
            // ARRANGE
            var propertyId = GetTypeDefinition(typeof(TestClass_MethodFormatter))
                   .Properties
                   .Single(x => x.Parameters.Count == paramterCount)
                   .ToPropertyId();

            var sut = MethodFormatter.Instance;

            // ACT
            var actualSignature = sut.GetSignature(propertyId);

            // ASSERT
            Assert.Equal(expectedSignature, actualSignature);
        }
    }
}
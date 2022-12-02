using System;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using CSharpHelperExtensions.Enumerable;

namespace CSharpHelperExtensions.Test
{
    public class EnumerableExtensionTest
    {
        [Fact]
        public void IsNullOrEmpty_Test()
        {
            ((List<string>)null).IsNullOrEmpty().Should().BeTrue();
            var emptyList = new List<string>();
            emptyList.IsNullOrEmpty().Should().BeTrue();

            var nonEmptyList = new List<int> {1};
            nonEmptyList.IsNullOrEmpty().Should().BeFalse();
        }

        [Fact]
        public void CleanNullOrEmpty_Test()
        {
            var stringList =
                new List<string>() {"Magic", "Bean", "Stalk", "Giant"};
            IEnumerable<int?> numEnumerable = new List<int?>() {1, null, 2};

            var strListWithNullEmptyWs = new List<string>()
                {"Magic", null, "Bean", "Stalk", "", "Giant", " "};

            var expectedStrList = new List<string> {"Magic", "Bean", "Stalk", "Giant"};
            var expectedIntList = new List<int?> {1, 2};

            stringList.CleanNullOrEmptyItems().Should().Equal(expectedStrList);
            numEnumerable.CleanNullOrEmptyItems().Should().Equal(expectedIntList);
            strListWithNullEmptyWs
                .CleanNullOrEmptyItems()
                .Should()
                .Equal(expectedStrList);
        }

        [Fact]
        public void ContainsOnly_Test()
        {
            var stringList =
                new List<string>() {"Magic", "Bean", "Stalk", "Giant"};

            stringList.ContainsOnly("Magic").Should().BeFalse();
            stringList.ContainsOnly("Magic", "Bean", "Stalk", "Giant").Should().BeTrue();
            stringList.ContainsOnly("Magic", "Bean", "Stalk", "Jack").Should().BeFalse();

            var integerList = new List<int>() {123};
            integerList.ContainsOnly(123).Should().BeTrue();
            integerList.ContainsOnly(123, 111).Should().BeFalse();
        }

        [Fact]
        public void Verify_Enumerable_AreEqual()
        {
            var stringList =
                new List<string>() {"Magic", "Bean", "Stalk", "Giant"};

            var stringList2 = new List<string>() {"Magic", "Bean", "Stalk", "Giant"};

            stringList.AreEqual(stringList).Should().BeTrue();
            stringList.AreEqual(stringList2).Should().BeTrue();

            stringList2.AreEqual(stringList2, Comparison.InOrder).Should().BeTrue();
            stringList2.AreEqual(stringList2, Comparison.NoOrder).Should().BeTrue();

            stringList2 =
                new List<string>() {"Magic", "Bean", "Stalk"};
            stringList.AreEqual(stringList2).Should().BeFalse();

            stringList2 =
                new List<string>() {"Giant", "Magic", "Bean", "Stalk"};
            stringList.AreEqual(stringList2).Should().BeTrue();
            stringList.AreEqual(stringList2, Comparison.InOrder).Should().BeFalse();
        }

        [Fact]
        public void AreEqual_True_When_Source_NullOrEmpty()
        {
            List<string> stringList = null;
            List<string> stringList2 = null;
            stringList.AreEqual(stringList2).Should().BeTrue();

            stringList2 = new List<string>();
            stringList.AreEqual(stringList2).Should().BeTrue();

            stringList2 =
                new List<string>() {"Giant", "Magic", "Bean", "Stalk"};
            var result = stringList.AreEqual(stringList2);
            stringList.AreEqual(stringList2, Comparison.InOrder).Should().BeFalse();
        }

        [Fact]
        public void AreEqual_True_When_Other_Is_NullOrEmpty()
        {
            List<string> source = null;
            List<string> other = null;
            source.AreEqual(other).Should().BeTrue();

            source = new List<string>();
            source.AreEqual(other).Should().BeTrue();

            source = new List<string>() {"Giant", "Magic", "Bean", "Stalk"};
            source.AreEqual(other).Should().BeFalse();
            source.AreEqual(other, Comparison.InOrder).Should().BeFalse();
        }

        [Fact]
        public void ForEach_IterateAndAdd()
        {
            IEnumerable<int> source = new List<int>() {1, 2, 3, 4};
            int expected = 10;
            int actual = 0;
            source.ForEach(item =>
            {
                actual += item;
            });
            actual.Should().Be(expected);
        }

        [Fact]
        public void Reduce_ListOfNumbers_WithoutInitialValue_ReturnExpected()
        {
            IEnumerable<int> source = new List<int>() {1, 2, 3, 4};
            int expected = 10;
            var actual = source.Reduce<int, int>((item, temp) =>  temp + item);
            actual.Should().Be(expected);
        }

        [Fact]
        public void Reduce_Add4Numbers_WithInitialValue_ReturnExpected()
        {
            IEnumerable<int> source = new List<int>() {1, 2, 3, 4};
            Decimal expected = 11;
            var actual = source.Reduce<int, Decimal>((item, temp) => temp + item, 1);
            actual.Should().Be(expected);
            actual.Should().BeOfType(expected.GetType());
        }

        [Fact]
        public void Reduce_Add4Numbers_WithInitialValue_Decimal_ReturnExpected()
        {
            IEnumerable<int> source = new List<int>() {1, 2, 3, 4};
            Decimal expected = 11.5m;
            var actual = source.Reduce<int, Decimal>((item, temp) => temp + item, 1.5m);
            actual.Should().Be(expected);
            actual.Should().BeOfType(expected.GetType());
        }
    }
}
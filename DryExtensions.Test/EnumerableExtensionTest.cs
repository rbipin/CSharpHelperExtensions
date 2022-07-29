using System;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;

namespace DryExtensions.Test
{
    public class EnumerableExtensionTest
    {
        [Fact]
        public void Verify_List_Is_Null_Or_Empty()
        {
            ((List<string>) null).IsNullOrEmpty().Should().BeTrue();
            var emptyList = new List<string>();
            emptyList.IsNullOrEmpty().Should().BeTrue();

            var nonEmptyList = new List<int> { 1 };
            nonEmptyList.IsNullOrEmpty().Should().BeFalse();
        }

        [Fact]
        public void Verify_List_CleanNulls()
        {
            var stringList =
                new List<string>() { "Magic", "Bean", "Stalk", "Giant" };
            IEnumerable<int?> numEnumerable = new List<int?>() { 1, null, 2 };

            var strListWithNullEmptyWs = new List<string>()
                { "Magic", null, "Bean", "Stalk", "", "Giant", " " };

            var expectedStrList = new List<string> { "Magic", "Bean", "Stalk", "Giant" };
            var expectedIntList = new List<int?> { 1, 2 };

            stringList.CleanNullOrEmpty().Should().Equal(expectedStrList);
            numEnumerable.CleanNullOrEmpty().Should().Equal(expectedIntList);
            strListWithNullEmptyWs
                .CleanNullOrEmpty()
                .Should()
                .Equal(expectedStrList);
        }

        [Fact]
        public void Verify_Enumerable_Contains_Only()
        {
            var stringList =
                new List<string>() { "Magic", "Bean", "Stalk", "Giant" };

            stringList.ContainsOnly("Magic").Should().BeFalse();
            stringList.ContainsOnly("Magic", "Bean", "Stalk", "Giant").Should().BeTrue();
            stringList.ContainsOnly("Magic", "Bean", "Stalk", "Jack").Should().BeFalse();

            var integerList = new List<int>() { 123 };
            integerList.ContainsOnly(123).Should().BeTrue();
            integerList.ContainsOnly(123, 111).Should().BeFalse();
        }

        [Fact]
        public void Verify_Enumerable_AreEqual()
        {
            var stringList =
                new List<string>() { "Magic", "Bean", "Stalk", "Giant" };

            var stringList2 =
                new List<string>() { "Magic", "Bean", "Stalk", "Giant" };

            stringList.AreEqual(stringList).Should().BeTrue();
            stringList.AreEqual(stringList2).Should().BeTrue();
        
            stringList2.AreEqual(stringList2, Comparison.InOrder).Should().BeTrue();
            stringList2.AreEqual(stringList2, Comparison.NoOrder).Should().BeTrue();

            stringList2 =
                new List<string>() { "Magic", "Bean", "Stalk"};
            stringList.AreEqual(stringList2).Should().BeFalse();

            stringList2 =
                new List<string>() { "Giant", "Magic", "Bean", "Stalk"};
            stringList.AreEqual(stringList2).Should().BeTrue();
            stringList.AreEqual(stringList2, Comparison.InOrder).Should().BeFalse();
        }
    }
}

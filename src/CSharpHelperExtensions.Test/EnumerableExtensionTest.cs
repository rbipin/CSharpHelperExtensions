using System;
using System.Collections.Generic;
using System.Linq;
using CSharpHelperExtensions.Enumerable;
using Shouldly;
using Xunit;

namespace CSharpHelperExtensions.Test
{
    public class EnumerableExtensionTest
    {
        [Fact]
        public void IsNullOrEmpty_Test()
        {
            ((List<string>) null).IsNullOrEmpty().ShouldBeTrue();
            var emptyList = new List<string>();
            emptyList.IsNullOrEmpty().ShouldBeTrue();

            var nonEmptyList = new List<int> { 1 };
            nonEmptyList.IsNullOrEmpty().ShouldBeFalse();
        }

        [Fact]
        public void CleanNullOrEmpty_Test()
        {
            var stringList =
                new List<string>() { "Magic", "Bean", "Stalk", "Giant" };
            IEnumerable<int?> numEnumerable = new List<int?>() { 1, null, 2 };

            var strListWithNullEmptyWs =
                new List<string>()
                { "Magic", null, "Bean", "Stalk", "", "Giant", " " };

            var expectedStrList =
                new List<string> { "Magic", "Bean", "Stalk", "Giant" };
            var expectedIntList = new List<int?> { 1, 2 };

            stringList.CleanNullOrEmptyItems().ShouldBe(expectedStrList);
            numEnumerable
                .CleanNullOrEmptyItems()
                .ShouldBe(expectedIntList);
            strListWithNullEmptyWs
                .CleanNullOrEmptyItems()
                .ShouldBe(expectedStrList);
        }

        [Fact]
        public void ContainsOnly_Test()
        {
            var stringList =
                new List<string>() { "Magic", "Bean", "Stalk", "Giant" };

            stringList.ContainsOnly("Magic").ShouldBeFalse();
            stringList
                .ContainsOnly("Magic", "Bean", "Stalk", "Giant")
                .ShouldBeTrue();
            stringList
                .ContainsOnly("Magic", "Bean", "Stalk", "Jack")
                .ShouldBeFalse();

            var integerList = new List<int>() { 123 };
            integerList.ContainsOnly(123).ShouldBeTrue();
            integerList.ContainsOnly(123, 111).ShouldBeFalse();
        }

        [Fact]
        public void Verify_Enumerable_AreEqual()
        {
            var stringList =
                new List<string>() { "Magic", "Bean", "Stalk", "Giant" };

            var stringList2 =
                new List<string>() { "Magic", "Bean", "Stalk", "Giant" };

            stringList.AreEqual(stringList).ShouldBeTrue();
            stringList.AreEqual(stringList2).ShouldBeTrue();

            stringList2
                .AreEqual(stringList2, Compare.InOrder)
                .ShouldBeTrue();
            stringList2
                .AreEqual(stringList2, Compare.NoOrder)
                .ShouldBeTrue();

            stringList2 = new List<string>() { "Magic", "Bean", "Stalk" };
            stringList.AreEqual(stringList2).ShouldBeFalse();

            stringList2 =
                new List<string>() { "Giant", "Magic", "Bean", "Stalk" };
            stringList.AreEqual(stringList2).ShouldBeTrue();
            stringList
                .AreEqual(stringList2, Compare.InOrder)
                .ShouldBeFalse();
        }

        [Fact]
        public void AreEqual_True_When_Source_NullOrEmpty()
        {
            List<string> stringList = null;
            List<string> stringList2 = null;
            stringList.AreEqual(stringList2).ShouldBeTrue();

            stringList2 = new List<string>();
            stringList.AreEqual(stringList2).ShouldBeTrue();

            stringList2 =
                new List<string>() { "Giant", "Magic", "Bean", "Stalk" };
            var result = stringList.AreEqual(stringList2);
            stringList
                .AreEqual(stringList2, Compare.InOrder)
                .ShouldBeFalse();
        }

        [Fact]
        public void AreEqual_True_When_Other_Is_NullOrEmpty()
        {
            List<string> source = null;
            List<string> other = null;
            source.AreEqual(other).ShouldBeTrue();

            source = new List<string>();
            source.AreEqual(other).ShouldBeTrue();

            source = new List<string>() { "Giant", "Magic", "Bean", "Stalk" };
            source.AreEqual(other).ShouldBeFalse();
            source.AreEqual(other, Compare.InOrder).ShouldBeFalse();
        }

        [Fact]
        public void ForEach_IterateListOfInteger_ReturnSum()
        {
            IEnumerable<int> source = new List<int>() { 1, 2, 3, 4 };
            int expected = 10;
            int actual = 0;
            var returnValue =source
                                .ForEach(item =>
                                {
                                    actual += item;
                                });
            actual.ShouldBe(expected);
        }

        [Fact]
        public void Reduce_ListOfNumbers_WithoutInitialValue_ReturnExpected()
        {
            IEnumerable<int> source = new List<int>() { 1, 2, 3, 4 };
            int expected = 10;
            var actual = source.Reduce<int, int>((item, temp) => temp + item);
            actual.ShouldBe(expected);
        }

        [Fact]
        public void Reduce_Add4Numbers_WithInitialValue_ReturnExpected()
        {
            IEnumerable<int> source = new List<int>() { 1, 2, 3, 4 };
            Decimal expected = 11;
            var actual =
                source
                    .Reduce<int, Decimal>((item, currentTotal) =>
                        currentTotal + item,
                    1);
            actual.ShouldBe(expected);
            actual.GetType().ShouldBe(expected.GetType());
        }

        [Fact]
        public void Reduce_Add4Numbers_WithInitialValue_Decimal_ReturnExpected()
        {
            IEnumerable<int> source = new List<int>() { 1, 2, 3, 4 };
            Decimal expected = 11.5m;
            var actual =
                source
                    .Reduce<int, Decimal>((item, currentTotal) =>
                        currentTotal + item,
                    1.5m);
            actual.ShouldBe(expected);
            actual.GetType().ShouldBe(expected.GetType());
            }

        [Fact]
        public void HasAny_ReturnsTrue_WhenSequenceHasElements()
        {
            new[] { 1, 2, 3 }.HasAny().ShouldBeTrue();
            new[] { (string)null }.HasAny().ShouldBeTrue();
        }

        [Fact]
        public void HasAny_ReturnsFalse_WhenNullOrEmpty()
        {
            ((IEnumerable<int>)null).HasAny().ShouldBeFalse();
            System.Linq.Enumerable.Empty<string>().HasAny().ShouldBeFalse();
        }

        [Fact]
        public void OrEmpty_ReturnsOriginal_WhenNotNull()
        {
            new[] { 1, 2 }.OrEmpty().ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public void OrEmpty_ReturnsEmpty_WhenNull()
        {
            ((IEnumerable<int>)null).OrEmpty().ShouldBeEmpty();
        }

        [Fact]
        public void None_ReturnsTrue_WhenNullOrEmpty()
        {
            ((IEnumerable<int>)null).None().ShouldBeTrue();
            System.Linq.Enumerable.Empty<int>().None().ShouldBeTrue();
        }

        [Fact]
        public void None_ReturnsFalse_WhenSequenceHasElements()
        {
            new[] { 1, 2 }.None().ShouldBeFalse();
        }

        [Fact]
        public void WhereNotNull_FiltersNullsFromReferenceSequence()
        {
            var result = new[] { "a", null, "b", null, "c" }.WhereNotNull().ToList();
            result.ShouldBe(new[] { "a", "b", "c" });
        }

        [Fact]
        public void WhereNotNull_OnNullSource_ReturnsEmpty()
        {
            ((IEnumerable<string>)null).WhereNotNull().ShouldBeEmpty();
        }

        [Fact]
        public void AsReadOnlyList_MaterializesSequenceInOrder()
        {
            IReadOnlyList<int> result = new[] { 3, 1, 2 }.AsReadOnlyList();
            result.ShouldBe(new[] { 3, 1, 2 });
        }

        [Fact]
        public void AsReadOnlyList_OnNullSource_ReturnsEmpty()
        {
            IReadOnlyList<int> result = ((IEnumerable<int>)null).AsReadOnlyList();
            result.ShouldBeEmpty();
        }

        [Fact]
        public void ToHashSetSafe_DeduplicatesElements()
        {
            var result = new[] { 1, 2, 2, 3 }.ToHashSetSafe();
            result.ShouldBe(new HashSet<int> { 1, 2, 3 });
        }

        [Fact]
        public void ToHashSetSafe_OnNullSource_ReturnsEmpty()
        {
            ((IEnumerable<int>)null).ToHashSetSafe().ShouldBeEmpty();
        }

        [Fact]
        public void Yield_WrapsValueTypeAsSingleItemSequence()
        {
            42.Yield().ToList().ShouldBe(new[] { 42 });
        }

        [Fact]
        public void Yield_WrapsReferenceTypeAsSingleItemSequence()
        {
            "hello".Yield().Single().ShouldBe("hello");
        }

        [Fact]
        public void JoinAsString_JoinsWithSeparator()
        {
            new[] { "a", "b", "c" }.JoinAsString(", ").ShouldBe("a, b, c");
        }

        [Fact]
        public void JoinAsString_WorksForNonStringTypes()
        {
            new[] { 1, 2, 3 }.JoinAsString("-").ShouldBe("1-2-3");
        }

        [Fact]
        public void JoinAsString_OnNullSource_ReturnsEmptyString()
        {
            ((IEnumerable<string>)null).JoinAsString(",").ShouldBe(string.Empty);
        }

        [Fact]
        public void WithIndex_ProjectsZeroBasedIndexAndItem()
        {
            var result = new[] { "a", "b", "c" }.WithIndex().ToList();
            result[0].ShouldBe((0, "a"));
            result[1].ShouldBe((1, "b"));
            result[2].ShouldBe((2, "c"));
        }

        [Fact]
        public void WithIndex_OnNullSource_ReturnsEmpty()
        {
            ((IEnumerable<string>)null).WithIndex().ShouldBeEmpty();
        }
    }
}

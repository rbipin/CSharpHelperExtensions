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

        [Fact]
        public void ToDictionarySafe_CreatesDictionaryFromSequence()
        {
            var result = new[] { ("a", 1), ("b", 2) }
                .ToDictionarySafe(x => x.Item1, x => x.Item2);
            result["a"].ShouldBe(1);
            result["b"].ShouldBe(2);
        }

        [Fact]
        public void ToDictionarySafe_KeepsLastValue_OnDuplicateKey()
        {
            var result = new[] { ("a", 1), ("a", 99) }
                .ToDictionarySafe(x => x.Item1, x => x.Item2);
            result["a"].ShouldBe(99);
        }

        [Fact]
        public void ToDictionarySafe_OnNullSource_ReturnsEmptyDictionary()
        {
            var result = ((IEnumerable<(string, int)>)null)
                .ToDictionarySafe(x => x.Item1, x => x.Item2);
            result.ShouldBeEmpty();
        }

        [Fact]
        public void AddIf_AddsItem_WhenConditionIsTrue()
        {
            var list = new List<int> { 1, 2 };
            list.AddIf(true, 3);
            list.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void AddIf_DoesNotAdd_WhenConditionIsFalse()
        {
            var list = new List<int> { 1, 2 };
            list.AddIf(false, 3);
            list.ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public void AddIf_ReturnsSameListInstance()
        {
            var list = new List<int>();
            var returned = list.AddIf(true, 1);
            ReferenceEquals(list, returned).ShouldBeTrue();
        }

        [Fact]
        public void AddRangeIf_AddsItems_WhenConditionIsTrue()
        {
            var list = new List<int> { 1 };
            list.AddRangeIf(true, new[] { 2, 3 });
            list.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void AddRangeIf_DoesNotAdd_WhenConditionIsFalse()
        {
            var list = new List<int> { 1 };
            list.AddRangeIf(false, new[] { 2, 3 });
            list.ShouldBe(new[] { 1 });
        }

        [Fact]
        public void AddRangeIf_ReturnsSameListInstance()
        {
            var list = new List<int>();
            var returned = list.AddRangeIf(true, new[] { 1, 2 });
            ReferenceEquals(list, returned).ShouldBeTrue();
        }

        [Fact]
        public void ConcatIf_ConcatenatesOther_WhenConditionIsTrue()
        {
            new[] { 1, 2 }.ConcatIf(true, new[] { 3, 4 }).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public void ConcatIf_ReturnsSource_WhenConditionIsFalse()
        {
            new[] { 1, 2 }.ConcatIf(false, new[] { 3, 4 }).ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public void ConcatIf_OnNullSource_ReturnsOther_WhenConditionIsTrue()
        {
            ((IEnumerable<int>)null).ConcatIf(true, new[] { 1, 2 }).ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public void ConcatIf_OnNullSource_ReturnsEmpty_WhenConditionIsFalse()
        {
            ((IEnumerable<int>)null).ConcatIf(false, new[] { 1, 2 }).ShouldBeEmpty();
        }

        [Fact]
        public void None_WithPredicate_ReturnsTrue_WhenNoElementMatches()
        {
            new[] { 1, 2, 3 }.None(x => x > 10).ShouldBeTrue();
        }

        [Fact]
        public void None_WithPredicate_ReturnsFalse_WhenAnyElementMatches()
        {
            new[] { 1, 2, 3 }.None(x => x > 2).ShouldBeFalse();
        }

        [Fact]
        public void None_WithPredicate_ReturnsTrue_WhenSourceIsNull()
        {
            ((IEnumerable<int>)null).None(x => x > 0).ShouldBeTrue();
        }

        [Fact]
        public void IsSingle_ReturnsTrue_WhenExactlyOneElement()
        {
            new[] { 42 }.IsSingle().ShouldBeTrue();
        }

        [Fact]
        public void IsSingle_ReturnsFalse_WhenEmpty()
        {
            System.Linq.Enumerable.Empty<int>().IsSingle().ShouldBeFalse();
        }

        [Fact]
        public void IsSingle_ReturnsFalse_WhenMoreThanOneElement()
        {
            new[] { 1, 2 }.IsSingle().ShouldBeFalse();
        }

        [Fact]
        public void IsSingle_ReturnsFalse_WhenNull()
        {
            ((IEnumerable<int>)null).IsSingle().ShouldBeFalse();
        }

        [Fact]
        public void IsSingle_WithPredicate_ReturnsTrue_WhenExactlyOneMatches()
        {
            new[] { 1, 2, 3 }.IsSingle(x => x > 2).ShouldBeTrue();
        }

        [Fact]
        public void IsSingle_WithPredicate_ReturnsFalse_WhenZeroMatch()
        {
            new[] { 1, 2, 3 }.IsSingle(x => x > 10).ShouldBeFalse();
        }

        [Fact]
        public void IsSingle_WithPredicate_ReturnsFalse_WhenMoreThanOneMatch()
        {
            new[] { 1, 2, 3 }.IsSingle(x => x > 1).ShouldBeFalse();
        }

        [Fact]
        public void IndexOf_ReturnsFirstMatchingIndex()
        {
            new[] { "a", "b", "c" }.IndexOf(x => x == "b").ShouldBe(1);
        }

        [Fact]
        public void IndexOf_ReturnsZero_WhenFirstElementMatches()
        {
            new[] { "a", "b", "c" }.IndexOf(x => x == "a").ShouldBe(0);
        }

        [Fact]
        public void IndexOf_ReturnsMinusOne_WhenNoMatch()
        {
            new[] { "a", "b", "c" }.IndexOf(x => x == "z").ShouldBe(-1);
        }

        [Fact]
        public void IndexOf_ReturnsMinusOne_WhenSourceIsNull()
        {
            ((IEnumerable<string>)null).IndexOf(x => x == "a").ShouldBe(-1);
        }

        [Fact]
        public void Partition_SplitsSequenceIntoMatchedAndRest()
        {
            var (matched, rest) = new[] { 1, 2, 3, 4, 5 }.Partition(x => x % 2 == 0);
            matched.ShouldBe(new[] { 2, 4 });
            rest.ShouldBe(new[] { 1, 3, 5 });
        }

        [Fact]
        public void Partition_AllMatch_ReturnsEmptyRest()
        {
            var (matched, rest) = new[] { 2, 4, 6 }.Partition(x => x % 2 == 0);
            matched.ShouldBe(new[] { 2, 4, 6 });
            rest.ShouldBeEmpty();
        }

        [Fact]
        public void Partition_NoneMatch_ReturnsEmptyMatched()
        {
            var (matched, rest) = new[] { 1, 3, 5 }.Partition(x => x % 2 == 0);
            matched.ShouldBeEmpty();
            rest.ShouldBe(new[] { 1, 3, 5 });
        }

        [Fact]
        public void Partition_OnNullSource_ReturnsTwoEmptyLists()
        {
            var (matched, rest) = ((IEnumerable<int>)null).Partition(x => x > 0);
            matched.ShouldBeEmpty();
            rest.ShouldBeEmpty();
        }

        [Fact]
        public void Batch_SplitsSequenceIntoChunksOfGivenSize()
        {
            var result = new[] { 1, 2, 3, 4, 5 }.Batch(2).ToList();
            result.Count.ShouldBe(3);
            result[0].ShouldBe(new[] { 1, 2 });
            result[1].ShouldBe(new[] { 3, 4 });
            result[2].ShouldBe(new[] { 5 });
        }

        [Fact]
        public void Batch_OnNullSource_ReturnsEmpty()
        {
            ((IEnumerable<int>)null).Batch(3).ShouldBeEmpty();
        }

        [Fact]
        public void Batch_WhenSizeLargerThanSequence_ReturnsSingleChunk()
        {
            var result = new[] { 1, 2 }.Batch(10).ToList();
            result.Count.ShouldBe(1);
            result[0].ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public void MinByOrDefault_ReturnsElementWithSmallestKey()
        {
            new[] { 3, 1, 2 }.MinByOrDefault(x => x).ShouldBe(1);
        }

        [Fact]
        public void MinByOrDefault_ReturnsDefault_WhenSourceIsNull()
        {
            ((IEnumerable<int>)null).MinByOrDefault(x => x).ShouldBe(0);
        }

        [Fact]
        public void MinByOrDefault_ReturnsNull_WhenSourceIsEmpty_ReferenceType()
        {
            System.Linq.Enumerable.Empty<string>().MinByOrDefault(x => x).ShouldBeNull();
        }

        [Fact]
        public void MaxByOrDefault_ReturnsElementWithLargestKey()
        {
            new[] { 3, 1, 2 }.MaxByOrDefault(x => x).ShouldBe(3);
        }

        [Fact]
        public void MaxByOrDefault_ReturnsDefault_WhenSourceIsNull()
        {
            ((IEnumerable<int>)null).MaxByOrDefault(x => x).ShouldBe(0);
        }

        [Fact]
        public void MaxByOrDefault_ReturnsNull_WhenSourceIsEmpty_ReferenceType()
        {
            System.Linq.Enumerable.Empty<string>().MaxByOrDefault(x => x).ShouldBeNull();
        }
    }
}

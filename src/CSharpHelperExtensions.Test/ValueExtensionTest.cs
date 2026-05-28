using System.Collections.Generic;
using CSharpHelperExtensions.Strings;
using CSharpHelperExtensions.Values;
using Shouldly;
using Xunit;

namespace CSharpHelperExtensions.Test
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ValueExtensionsTest
    {
        [Fact]
        public void Verify_In_Exists()
        {
            var result = "Magic".In("Magic", "Bean", "Stalk");
            result.ShouldBeTrue();
        }

        [Fact]
        public void Verify_In_Integer_Exists()
        {
            var result = 1.In(1, 2, 3);
            result.ShouldBeTrue();
        }

        [Fact]
        public void Verify_In_NotExists()
        {
            var result = "Giant".In("Magic", "Bean", "Stalk");
            result.ShouldBeFalse();
        }

        [Fact]
        public void Verify_To_NullableType()
        {
            string testString = null;
            var nullableDecimal = testString.ToNullable<decimal>();
            nullableDecimal.ShouldBeNull();

            testString = "0";
            nullableDecimal = testString.ToNullable<decimal>();
            nullableDecimal.ShouldBe(0);

            testString = "1.5";
            nullableDecimal = testString.ToNullable<decimal>();
            nullableDecimal.ShouldBe(1.5m);

            testString = "";
            var decimalValue = testString.ToNullable<decimal>();
            decimalValue.ShouldBeNull();
        }

        [Fact]
        public void Verify_Is_InBetween_DefaultComparison()
        {
            decimal value = 3;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper);
            result.ShouldBeTrue();

            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.ShouldBeTrue();

            value = 2;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.ShouldBeTrue();

            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.ShouldBeFalse();

            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.ShouldBeTrue();
        }

        [Fact]
        public void Verify_Is_InBetween_ExcludeBothComparison()
        {
            decimal value = 2;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.ShouldBeTrue();

            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.ShouldBeFalse();

            value = 3;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.ShouldBeFalse();

            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.ShouldBeFalse();

            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.ShouldBeTrue();
        }

        [Fact]
        public void Verify_Is_InBetween_ExcludeLower()
        {
            decimal value = 2;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.ShouldBeTrue();

            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.ShouldBeFalse();

            value = 3;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.ShouldBeTrue();

            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.ShouldBeFalse();

            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.ShouldBeTrue();
        }

        [Fact]
        public void Verify_Is_InBetween_None()
        {
            decimal value = 1;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper);
            result.ShouldBeTrue();

            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.None);
            result.ShouldBeTrue();

            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.ShouldBeFalse();

            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.None);
            result.ShouldBeFalse();

            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.ShouldBeTrue();

            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.None);
            result.ShouldBeTrue();
        }

        [Fact]
        public void Verify_Is_InBetween_ExcludeUpper()
        {
            decimal value = 2;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.ShouldBeTrue();

            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.ShouldBeTrue();

            value = 3;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.ShouldBeFalse();

            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.ShouldBeFalse();

            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.ShouldBeTrue();
        }
    }
}

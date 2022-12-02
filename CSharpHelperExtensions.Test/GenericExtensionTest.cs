using System.Collections.Generic;
using CSharpHelperExtensions.Strings;
using FluentAssertions;
using Xunit;

namespace CSharpHelperExtensions.Test
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class GenericExtensionsTest
    {
        [Fact]
        public void Verify_In_Exists()
        {
            var result = "Magic".In("Magic", "Bean", "Stalk");
            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_In_Integer_Exists()
        {
            var result = 1.In(1, 2, 3);
            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_In_NotExists()
        {
            var result = "Giant".In("Magic", "Bean", "Stalk");
            result.Should().BeFalse();
        }

        [Fact]
        public void Verify_To_NullableType()
        {
            string testString = null;
            var nullableDecimal = testString.ToNullable<decimal>();
            nullableDecimal.Should().BeNull();
            
            testString = "0";
            nullableDecimal = testString.ToNullable<decimal>();
            nullableDecimal.Should().Be(0);
            
            testString = "1.5";
            nullableDecimal = testString.ToNullable<decimal>();
            nullableDecimal.Should().Be(1.5m);

            testString = "";
            var decimalValue = testString.ToNullable<decimal>();
            decimalValue.Should().BeNull();
        }

        [Fact]
        public void Verify_Is_InBetween_DefaultComparison()
        {
            decimal value = 3;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper);
            result.Should().BeTrue();
            
            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.Should().BeTrue();
            
            value = 2;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.Should().BeTrue();
            
            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.Should().BeFalse();
            
            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.Should().BeTrue();
            
        }
        
        [Fact]
        public void Verify_Is_InBetween_ExcludeBothComparison()
        {
            decimal value = 2;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.Should().BeTrue();
            
            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.Should().BeFalse();
            
            value = 3;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.Should().BeFalse();
            
            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.Should().BeFalse();
            
            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeBoth);
            result.Should().BeTrue();
        }
        
        [Fact]
        public void Verify_Is_InBetween_ExcludeLower()
        {
            decimal value = 2;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.Should().BeTrue();
            
            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.Should().BeFalse();
            
            value = 3;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.Should().BeTrue();
            
            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.Should().BeFalse();
            
            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeLower);
            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_Is_InBetween_None()
        {
            decimal value = 1;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper);
            result.Should().BeTrue();
            
            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.None);
            result.Should().BeTrue();
            
            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.Should().BeFalse();
            
            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.None);
            result.Should().BeFalse();
            
            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper);
            result.Should().BeTrue();

            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.None);
            result.Should().BeTrue();
        }
        
        [Fact]
        public void Verify_Is_InBetween_ExcludeUpper()
        {
            decimal value = 2;
            decimal lower = 1;
            decimal upper = 3;
            var result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.Should().BeTrue();
            
            value = 1;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.Should().BeTrue();
            
            value = 3;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.Should().BeFalse();
            
            value = 4;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.Should().BeFalse();
            
            value = 2.5m;
            lower = 1;
            upper = 3;
            result = value.IsBetween(lower, upper, BetweenComparison.ExcludeUpper);
            result.Should().BeTrue();
        }
        
    }
}
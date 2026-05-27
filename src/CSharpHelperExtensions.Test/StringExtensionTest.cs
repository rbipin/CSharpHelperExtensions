using System;
using CSharpHelperExtensions.Strings;
using FluentAssertions;
using Xunit;
namespace CSharpHelperExtensions.Test
{
    public class StringExtensionTest
    {
        [Fact]
        public void Verify_StringIsEmpty_CheckForWhitespace()
        {
            string strNull = null;
            strNull.IsNullOrEmpty().Should().BeTrue();
            "".IsNullOrEmpty().Should().BeTrue();
            "   ".IsNullOrEmpty().Should().BeTrue();
            " ".IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void Verify_HasValue_ReturnsTrue_WhenNotNullOrWhitespace()
        {
            "hello".HasValue().Should().BeTrue();
            "  x  ".HasValue().Should().BeTrue();
        }

        [Fact]
        public void Verify_HasValue_ReturnsFalse_WhenNullOrWhitespace()
        {
            ((string)null).HasValue().Should().BeFalse();
            "".HasValue().Should().BeFalse();
            "   ".HasValue().Should().BeFalse();
        }

        [Fact]
        public void Verify_OrEmpty_ReturnsEmpty_WhenNull()
        {
            ((string)null).OrEmpty().Should().Be(string.Empty);
            "hello".OrEmpty().Should().Be("hello");
            "   ".OrEmpty().Should().Be("   ");
        }

        [Fact]
        public void Verify_OrDefault_ReturnsFallback_WhenNullOrWhitespace()
        {
            ((string)null).OrDefault("fallback").Should().Be("fallback");
            "".OrDefault("fallback").Should().Be("fallback");
            "   ".OrDefault("fallback").Should().Be("fallback");
            "hello".OrDefault("fallback").Should().Be("hello");
        }

        [Fact]
        public void Verify_Truncate_CutsAtMaxLength()
        {
            "hello world".Truncate(5).Should().Be("hello");
            "hi".Truncate(10).Should().Be("hi");
            "".Truncate(5).Should().Be("");
            ((string)null).Truncate(5).Should().Be("");
        }

        [Fact]
        public void Verify_Truncate_ThrowsOnNegativeLength()
        {
            var act = () => "hello".Truncate(-1);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Verify_Reverse_ReturnsReversedString()
        {
            "hello".Reverse().Should().Be("olleh");
            "a".Reverse().Should().Be("a");
            "".Reverse().Should().Be("");
            ((string)null).Reverse().Should().Be("");
        }

        [Fact]
        public void Verify_TrimToLower_TrimsAndLowers()
        {
            "  HELLO  ".TrimToLower().Should().Be("hello");
            "WORLD".TrimToLower().Should().Be("world");
            ((string)null).TrimToLower().Should().Be("");
        }

        [Fact]
        public void Verify_TrimToUpper_TrimsAndUppers()
        {
            "  hello  ".TrimToUpper().Should().Be("HELLO");
            ((string)null).TrimToUpper().Should().Be("");
        }

        [Fact]
        public void Verify_EqualsIgnoreCase_ComparesIgnoringCase()
        {
            "Hello".EqualsIgnoreCase("hello").Should().BeTrue();
            "Hello".EqualsIgnoreCase("HELLO").Should().BeTrue();
            "Hello".EqualsIgnoreCase("world").Should().BeFalse();
            ((string)null).EqualsIgnoreCase(null).Should().BeTrue();
            ((string)null).EqualsIgnoreCase("x").Should().BeFalse();
        }

        [Fact]
        public void Verify_ContainsIgnoreCase_FindsSubstring()
        {
            "Hello World".ContainsIgnoreCase("world").Should().BeTrue();
            "Hello World".ContainsIgnoreCase("HELLO").Should().BeTrue();
            "Hello World".ContainsIgnoreCase("xyz").Should().BeFalse();
            ((string)null).ContainsIgnoreCase("x").Should().BeFalse();
        }

        [Fact]
        public void Verify_StartsWithIgnoreCase_ChecksPrefix()
        {
            "Hello World".StartsWithIgnoreCase("hello").Should().BeTrue();
            "Hello World".StartsWithIgnoreCase("world").Should().BeFalse();
            ((string)null).StartsWithIgnoreCase("x").Should().BeFalse();
        }

        [Fact]
        public void Verify_EndsWithIgnoreCase_ChecksSuffix()
        {
            "Hello World".EndsWithIgnoreCase("WORLD").Should().BeTrue();
            "Hello World".EndsWithIgnoreCase("hello").Should().BeFalse();
            ((string)null).EndsWithIgnoreCase("x").Should().BeFalse();
        }

        [Fact]
        public void Verify_MaskStart_MasksAllButLastN()
        {
            "123456".MaskStart(2).Should().Be("****56");
            "AB".MaskStart(2).Should().Be("AB");
            "hello".MaskStart(0).Should().Be("*****");
            ((string)null).MaskStart(2).Should().Be("");
        }

        [Fact]
        public void Verify_MaskStart_UsesCustomChar()
        {
            "123456".MaskStart(2, '#').Should().Be("####56");
        }

        [Fact]
        public void Verify_ToIntOrNull_ParsesValidInt()
        {
            "42".ToIntOrNull().Should().Be(42);
            "-10".ToIntOrNull().Should().Be(-10);
            "not-a-number".ToIntOrNull().Should().BeNull();
            ((string)null).ToIntOrNull().Should().BeNull();
            "".ToIntOrNull().Should().BeNull();
        }

        [Fact]
        public void Verify_ToDecimalOrNull_ParsesValidDecimal()
        {
            "3.14".ToDecimalOrNull().Should().Be(3.14m);
            "abc".ToDecimalOrNull().Should().BeNull();
            ((string)null).ToDecimalOrNull().Should().BeNull();
        }

        [Fact]
        public void Verify_ToDateTimeOrNull_ParsesValidDate()
        {
            "2024-01-15".ToDateTimeOrNull().Should().NotBeNull();
            "not-a-date".ToDateTimeOrNull().Should().BeNull();
            ((string)null).ToDateTimeOrNull().Should().BeNull();
        }

        [Fact]
        public void Verify_ToGuidOrNull_ParsesValidGuid()
        {
            "d3b07384-d9a0-4b6f-9e4d-5e3b7a9c2d1e".ToGuidOrNull().Should().NotBeNull();
            "not-a-guid".ToGuidOrNull().Should().BeNull();
            ((string)null).ToGuidOrNull().Should().BeNull();
        }

        [Fact]
        public void Verify_ToBoolOrNull_ParsesValidBool()
        {
            "true".ToBoolOrNull().Should().BeTrue();
            "false".ToBoolOrNull().Should().BeFalse();
            "True".ToBoolOrNull().Should().BeTrue();
            "yes".ToBoolOrNull().Should().BeNull();
            ((string)null).ToBoolOrNull().Should().BeNull();
        }
    }
}


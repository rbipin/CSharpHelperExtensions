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
    }
}


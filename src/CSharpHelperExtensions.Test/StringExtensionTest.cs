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
    }
}


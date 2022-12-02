using System;
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
        public void Verify_StringIsEmpty_Do_Not_Ignore_Whitespace()
        {
            string strNull = null;
            strNull.IsNullOrEmpty().Should().BeTrue();
            "".IsNullOrEmpty(false).Should().BeTrue();
            "   ".IsNullOrEmpty(false).Should().BeFalse();
            " ".IsNullOrEmpty(false).Should().BeFalse();
        }
    }
}


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
    }
}


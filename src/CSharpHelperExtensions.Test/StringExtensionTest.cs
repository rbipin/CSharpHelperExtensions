using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSharpHelperExtensions.Strings;
using Shouldly;
using Xunit;
namespace CSharpHelperExtensions.Test
{
    public class StringExtensionTest
    {
        [Fact]
        public void Verify_StringIsEmpty_CheckForWhitespace()
        {
            string strNull = null;
            strNull.IsNullOrEmpty().ShouldBeTrue();
            "".IsNullOrEmpty().ShouldBeTrue();
            "   ".IsNullOrEmpty().ShouldBeTrue();
            " ".IsNullOrEmpty().ShouldBeTrue();
        }

        [Fact]
        public void Verify_HasValue_ReturnsTrue_WhenNotNullOrWhitespace()
        {
            "hello".HasValue().ShouldBeTrue();
            "  x  ".HasValue().ShouldBeTrue();
        }

        [Fact]
        public void Verify_HasValue_ReturnsFalse_WhenNullOrWhitespace()
        {
            ((string)null).HasValue().ShouldBeFalse();
            "".HasValue().ShouldBeFalse();
            "   ".HasValue().ShouldBeFalse();
        }

        [Fact]
        public void Verify_OrEmpty_ReturnsEmpty_WhenNull()
        {
            ((string)null).OrEmpty().ShouldBe(string.Empty);
            "hello".OrEmpty().ShouldBe("hello");
            "   ".OrEmpty().ShouldBe("   ");
        }

        [Fact]
        public void Verify_OrDefault_ReturnsFallback_WhenNullOrWhitespace()
        {
            ((string)null).OrDefault("fallback").ShouldBe("fallback");
            "".OrDefault("fallback").ShouldBe("fallback");
            "   ".OrDefault("fallback").ShouldBe("fallback");
            "hello".OrDefault("fallback").ShouldBe("hello");
        }

        [Fact]
        public void Verify_Truncate_CutsAtMaxLength()
        {
            "hello world".Truncate(5).ShouldBe("hello");
            "hi".Truncate(10).ShouldBe("hi");
            "".Truncate(5).ShouldBe("");
            ((string)null).Truncate(5).ShouldBe("");
        }

        [Fact]
        public void Verify_Truncate_ThrowsOnNegativeLength()
        {
            Should.Throw<ArgumentOutOfRangeException>(() => "hello".Truncate(-1));
        }

        [Fact]
        public void Verify_Reverse_ReturnsReversedString()
        {
            "hello".Reverse().ShouldBe("olleh");
            "a".Reverse().ShouldBe("a");
            "".Reverse().ShouldBe("");
            ((string)null).Reverse().ShouldBe("");
        }

        [Fact]
        public void Verify_TrimToLower_TrimsAndLowers()
        {
            "  HELLO  ".TrimToLower().ShouldBe("hello");
            "WORLD".TrimToLower().ShouldBe("world");
            ((string)null).TrimToLower().ShouldBe("");
        }

        [Fact]
        public void Verify_TrimToUpper_TrimsAndUppers()
        {
            "  hello  ".TrimToUpper().ShouldBe("HELLO");
            ((string)null).TrimToUpper().ShouldBe("");
        }

        [Fact]
        public void Verify_EqualsIgnoreCase_ComparesIgnoringCase()
        {
            "Hello".EqualsIgnoreCase("hello").ShouldBeTrue();
            "Hello".EqualsIgnoreCase("HELLO").ShouldBeTrue();
            "Hello".EqualsIgnoreCase("world").ShouldBeFalse();
            ((string)null).EqualsIgnoreCase(null).ShouldBeTrue();
            ((string)null).EqualsIgnoreCase("x").ShouldBeFalse();
        }

        [Fact]
        public void Verify_ContainsIgnoreCase_FindsSubstring()
        {
            "Hello World".ContainsIgnoreCase("world").ShouldBeTrue();
            "Hello World".ContainsIgnoreCase("HELLO").ShouldBeTrue();
            "Hello World".ContainsIgnoreCase("xyz").ShouldBeFalse();
            ((string)null).ContainsIgnoreCase("x").ShouldBeFalse();
        }

        [Fact]
        public void Verify_StartsWithIgnoreCase_ChecksPrefix()
        {
            "Hello World".StartsWithIgnoreCase("hello").ShouldBeTrue();
            "Hello World".StartsWithIgnoreCase("world").ShouldBeFalse();
            ((string)null).StartsWithIgnoreCase("x").ShouldBeFalse();
        }

        [Fact]
        public void Verify_EndsWithIgnoreCase_ChecksSuffix()
        {
            "Hello World".EndsWithIgnoreCase("WORLD").ShouldBeTrue();
            "Hello World".EndsWithIgnoreCase("hello").ShouldBeFalse();
            ((string)null).EndsWithIgnoreCase("x").ShouldBeFalse();
        }

        [Fact]
        public void Verify_MaskStart_MasksAllButLastN()
        {
            "123456".MaskStart(2).ShouldBe("****56");
            "AB".MaskStart(2).ShouldBe("AB");
            "hello".MaskStart(0).ShouldBe("*****");
            ((string)null).MaskStart(2).ShouldBe("");
        }

        [Fact]
        public void Verify_MaskStart_UsesCustomChar()
        {
            "123456".MaskStart(2, '#').ShouldBe("####56");
        }

        [Fact]
        public void Verify_ToIntOrNull_ParsesValidInt()
        {
            "42".ToIntOrNull().ShouldBe(42);
            "-10".ToIntOrNull().ShouldBe(-10);
            "not-a-number".ToIntOrNull().ShouldBeNull();
            ((string)null).ToIntOrNull().ShouldBeNull();
            "".ToIntOrNull().ShouldBeNull();
        }

        [Fact]
        public void Verify_ToDecimalOrNull_ParsesValidDecimal()
        {
            "3.14".ToDecimalOrNull().ShouldBe(3.14m);
            "abc".ToDecimalOrNull().ShouldBeNull();
            ((string)null).ToDecimalOrNull().ShouldBeNull();
        }

        [Fact]
        public void Verify_ToDateTimeOrNull_ParsesValidDate()
        {
            "2024-01-15".ToDateTimeOrNull().ShouldNotBeNull();
            "not-a-date".ToDateTimeOrNull().ShouldBeNull();
            ((string)null).ToDateTimeOrNull().ShouldBeNull();
        }

        [Fact]
        public void Verify_ToGuidOrNull_ParsesValidGuid()
        {
            "d3b07384-d9a0-4b6f-9e4d-5e3b7a9c2d1e".ToGuidOrNull().ShouldNotBeNull();
            "not-a-guid".ToGuidOrNull().ShouldBeNull();
            ((string)null).ToGuidOrNull().ShouldBeNull();
        }

        [Fact]
        public void Verify_ToBoolOrNull_ParsesValidBool()
        {
            "true".ToBoolOrNull().ShouldBe(true);
            "false".ToBoolOrNull().ShouldBe(false);
            "True".ToBoolOrNull().ShouldBe(true);
            "yes".ToBoolOrNull().ShouldBeNull();
            ((string)null).ToBoolOrNull().ShouldBeNull();
        }

        [Fact]
        public void Verify_Base64Encode_EncodesString()
        {
            "hello".Base64Encode().ShouldBe("aGVsbG8=");
            ((string)null).Base64Encode().ShouldBeNull();
        }

        [Fact]
        public void Verify_Base64Decode_DecodesString()
        {
            "aGVsbG8=".Base64Decode().ShouldBe("hello");
            ((string)null).Base64Decode().ShouldBeNull();
        }

        [Fact]
        public void Verify_Base64_RoundTrip()
        {
            var original = "Hello, World! Émoji: 🌍";
            original.Base64Encode().Base64Decode().ShouldBe(original);
        }

        [Fact]
        public void Verify_ToBase64Url_ProducesUrlSafeString()
        {
            var encoded = "hello world".ToBase64Url();
            encoded.ShouldNotContain("+");
            encoded.ShouldNotContain("/");
            encoded.ShouldNotContain("=");
            ((string)null).ToBase64Url().ShouldBeNull();
        }

        [Fact]
        public void Verify_Base64Url_RoundTrip()
        {
            var original = "Hello, World! 123";
            original.ToBase64Url().FromBase64Url().ShouldBe(original);
            ((string)null).FromBase64Url().ShouldBeNull();
        }

        [Fact]
        public void Verify_ToUtf8Bytes_ReturnsBytes()
        {
            "hello".ToUtf8Bytes().ShouldBe(Encoding.UTF8.GetBytes("hello"));
            ((string)null).ToUtf8Bytes().ShouldBeEmpty();
        }

        [Fact]
        public void Verify_ToUtf8Stream_ReturnsReadableStream()
        {
            using var stream = "hello".ToUtf8Stream();
            stream.Length.ShouldBe(5);
            ((string)null).ToUtf8Stream().Length.ShouldBe(0);
        }

        [Fact]
        public void Verify_JoinWith_JoinsWithSeparator()
        {
            ", ".JoinWith(new[] { "a", "b", "c" }).ShouldBe("a, b, c");
            "-".JoinWith(new[] { "x" }).ShouldBe("x");
            ", ".JoinWith(Array.Empty<string>()).ShouldBe("");
        }

        [Fact]
        public void Verify_SplitNonEmpty_SplitsAndRemovesEmpty()
        {
            "a,b,,c".SplitNonEmpty(',').ShouldBe(new[] { "a", "b", "c" });
            ((string)null).SplitNonEmpty(',').ShouldBeEmpty();
            "   ".SplitNonEmpty(' ').ShouldBeEmpty();
        }

        [Fact]
        public void Verify_RemoveWhitespace_RemovesAllWhitespace()
        {
            "h e l l o".RemoveWhitespace().ShouldBe("hello");
            "  hello  ".RemoveWhitespace().ShouldBe("hello");
            "\thello\n".RemoveWhitespace().ShouldBe("hello");
            ((string)null).RemoveWhitespace().ShouldBe("");
        }

        [Fact]
        public void Verify_CollapseWhitespace_CollapsesRunsToSingleSpace()
        {
            "hello   world".CollapseWhitespace().ShouldBe("hello world");
            "  hello  world  ".CollapseWhitespace().ShouldBe("hello world");
            "hello\t\nworld".CollapseWhitespace().ShouldBe("hello world");
            ((string)null).CollapseWhitespace().ShouldBe("");
        }

        [Fact]
        public void Verify_ReplaceMany_ReplacesMultiplePairs()
        {
            "hello world"
                .ReplaceMany(new[] { ("hello", "hi"), ("world", "there") })
                .ShouldBe("hi there");
        }

        [Fact]
        public void Verify_ReplaceMany_AppliesInOrder()
        {
            "aaa"
                .ReplaceMany(new[] { ("aaa", "bbb"), ("bbb", "ccc") })
                .ShouldBe("ccc");
        }

        [Fact]
        public void Verify_ReplaceMany_ReturnsEmptyForNull()
        {
            ((string)null).ReplaceMany(new[] { ("a", "b") }).ShouldBe("");
        }

        [Fact]
        public void Verify_RemoveDiacritics_StripAccents()
        {
            "café".RemoveDiacritics().ShouldBe("cafe");
            "résumé".RemoveDiacritics().ShouldBe("resume");
            "naïve".RemoveDiacritics().ShouldBe("naive");
            "hello".RemoveDiacritics().ShouldBe("hello");
            ((string)null).RemoveDiacritics().ShouldBe("");
        }

        [Fact]
        public void Verify_ToSlug_CreatesSlug()
        {
            "Hello World".ToSlug().ShouldBe("hello-world");
            "  Café au Lait  ".ToSlug().ShouldBe("cafe-au-lait");
            "C# is great!".ToSlug().ShouldBe("c-is-great");
            "---".ToSlug().ShouldBe("");
            ((string)null).ToSlug().ShouldBe("");
        }

        [Fact]
        public void Verify_IsNumeric_DetectsDigitOnlyStrings()
        {
            "123".IsNumeric().ShouldBeTrue();
            "12.3".IsNumeric().ShouldBeFalse();
            "abc".IsNumeric().ShouldBeFalse();
            ((string)null).IsNumeric().ShouldBeFalse();
            "".IsNumeric().ShouldBeFalse();
        }

        [Fact]
        public void Verify_IsAlpha_DetectsLetterOnlyStrings()
        {
            "hello".IsAlpha().ShouldBeTrue();
            "hello1".IsAlpha().ShouldBeFalse();
            "héllo".IsAlpha().ShouldBeTrue();
            ((string)null).IsAlpha().ShouldBeFalse();
        }

        [Fact]
        public void Verify_IsAlphaNumeric_DetectsLetterOrDigitStrings()
        {
            "hello123".IsAlphaNumeric().ShouldBeTrue();
            "hello!".IsAlphaNumeric().ShouldBeFalse();
            ((string)null).IsAlphaNumeric().ShouldBeFalse();
        }

        [Fact]
        public void Verify_EnsurePrefix_AddsIfMissing()
        {
            "world".EnsurePrefix("hello ").ShouldBe("hello world");
            "hello world".EnsurePrefix("hello ").ShouldBe("hello world");
            ((string)null).EnsurePrefix("hello ").ShouldBe("hello ");
        }

        [Fact]
        public void Verify_EnsureSuffix_AddsIfMissing()
        {
            "hello".EnsureSuffix("!").ShouldBe("hello!");
            "hello!".EnsureSuffix("!").ShouldBe("hello!");
            ((string)null).EnsureSuffix("!").ShouldBe("!");
        }

        [Fact]
        public void Verify_TrimPrefix_RemovesIfPresent()
        {
            "hello world".TrimPrefix("hello ").ShouldBe("world");
            "world".TrimPrefix("hello ").ShouldBe("world");
            ((string)null).TrimPrefix("hello ").ShouldBe("");
        }

        [Fact]
        public void Verify_TrimSuffix_RemovesIfPresent()
        {
            "hello!".TrimSuffix("!").ShouldBe("hello");
            "hello".TrimSuffix("!").ShouldBe("hello");
            ((string)null).TrimSuffix("!").ShouldBe("");
        }

        [Fact]
        public void Verify_EnsurePrefix_ThrowsOnNullPrefix()
        {
            Should.Throw<ArgumentNullException>(() => "hello".EnsurePrefix(null));
        }

        [Fact]
        public void Verify_TrimPrefix_ThrowsOnNullPrefix()
        {
            Should.Throw<ArgumentNullException>(() => "hello".TrimPrefix(null));
        }

        [Fact]
        public void Verify_ToTitleCase_CapitalizesFirstLetterOfEachWord()
        {
            "hello world".ToTitleCase().ShouldBe("Hello World");
            "  hELLO   wORLD  ".ToTitleCase().ShouldBe("Hello World");
            "it's a test".ToTitleCase().ShouldBe("It's A Test");
            "SINGLE".ToTitleCase().ShouldBe("Single");
            ((string)null).ToTitleCase().ShouldBe("");
            "".ToTitleCase().ShouldBe("");
            "   ".ToTitleCase().ShouldBe("");
        }
    }
}

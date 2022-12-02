using System;
using System.ComponentModel;

namespace CSharpHelperExtensions.Strings
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert Nullable type
        /// </summary>
        /// <param name="input"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? ToNullable<T>(this string input) where T : struct
        {
            try
            {
                if (input.IsNullOrEmpty())
                {
                    return null;
                }

                var conv = TypeDescriptor.GetConverter(typeof(T));
                return (T?)conv.ConvertFrom(input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}


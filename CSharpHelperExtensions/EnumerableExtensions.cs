﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpHelperExtensions.Enumerable
{
    public enum Compare
    {
        InOrder,
        NoOrder
    }
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Check if the enumerable contains only a particular item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsOnly<T>(this IEnumerable<T> enumerable,params T[] value)
        {
            if (value.IsNullOrEmpty() || enumerable.IsNullOrEmpty())
            {
                return false;
            }
            if (enumerable.Count() != value.Count())
            {
                return false;
            }
            return value.All(item => enumerable.Contains(item));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="values"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool AreEqual<T>(this IEnumerable<T> enumerable, IEnumerable<T> values, 
        Compare comparison = Compare.NoOrder)
        {
            if (ReferenceEquals(enumerable, null) && ReferenceEquals(values, null))
            {
                return true;
            }
            values ??= new List<T>();
            enumerable ??= new List<T>();
            if (ReferenceEquals(enumerable, values))
            {
                return true;
            }
            if (values.Count() != enumerable.Count())
            {
                return false;
            }
            return comparison switch
            {
                Compare.InOrder => CompareItemsInOrder(enumerable, values),
                Compare.NoOrder => enumerable.All(item => values.Contains(item)),
                _ => false
            };
        }

        private static bool CompareItemsInOrder<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var firstList = list1.ToList();
            var secondList = list2.ToList();
            for(int index =0; index<= firstList.Count - 1; index ++)
            {
                var areEqual = firstList[index].Equals(secondList[index]);
                if (!areEqual)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Clean nulls and empty items from an IEnumerable,
        /// if this is IEnumerable of string this will clean the empty string and whitespace
        /// </summary>
        /// <param name="value">Enumerable to clean</param>
        /// <typeparam name="T">type</typeparam>
        /// <returns>Enumerable cleaned up</returns>
        public static IEnumerable<T> CleanNullOrEmptyItems<T>(this IEnumerable<T> value)
        {
            var list = value?.ToList();
            if (list is null || !list.Any())
            {
                return null;
            }

            return list.Where(item =>
            {
                if (item is string itemStr)
                {
                    return !string.IsNullOrWhiteSpace(itemStr);
                }

                return item is not null;
            }).ToList();
        }

        /// <summary>
        /// Check IEnumerable is null, empty or has items that are all null
        /// </summary>
        /// <param name="value">Enumerable to check</param>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>true or false</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> values)
        {
            var enumerable = values?.ToArray();
            return enumerable == null || !enumerable.Any() || enumerable.All(item => item is null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="execute"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> values, Action<T> execute)
        {
            var collection = values?.ToList() ?? new List<T>();
            foreach (var item in collection)
            {
                execute(item);
            }

            return values;
        }

        /// <summary>
        /// Reduce the result to a single value, runs each items through a reducer function
        /// </summary>
        /// <param name="values">Collection on which to perform reduce</param>
        /// <param name="execute">Callback function or the reducer function</param>
        /// <param name="initialValue">Initial value to start </param>
        /// <typeparam name="TIn">Type of the collection on which the reduce is called</typeparam>
        /// <typeparam name="TOut">Return value type</typeparam>
        /// <returns></returns>
        public static TOut Reduce<TIn, TOut>(this IEnumerable<TIn> values, Func<TIn, TOut, TOut> execute, TOut initialValue = default)
        {
            var collection = values?.ToList() ?? new List<TIn>();
            var result = default(TOut);
            var temp = initialValue;
            foreach (var item in collection)
            { 
               result = execute(item, temp);
               temp = result;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="execute"></param>
        /// <param name="initialValue"></param>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        public static TOut Reduce<TIn, TOut>(this IEnumerable<TIn> values, Func<TIn, TOut, int, TOut> execute, TOut initialValue = default)
        {
            var collection = values?.ToList() ?? new List<TIn>();
            var result = default(TOut);
            var temp = initialValue;
            for (int counter = 0; counter <= collection.Count -1; counter++)
            {
                var item = collection[counter];
                result = execute(item, temp, counter);
                temp = result;
            }

            return result;
        }
    }
}


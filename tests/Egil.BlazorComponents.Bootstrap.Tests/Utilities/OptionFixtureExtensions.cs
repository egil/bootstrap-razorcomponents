﻿using System.Collections.Generic;
using System.Linq;
using Shouldly;
using System;
using Egil.BlazorComponents.Bootstrap.Grid.Options;

namespace Egil.BlazorComponents.Bootstrap.Tests.Utilities
{

    public static class OptionFixtureExtensions
    {
        public static void ShouldBeCombinationOf(this string value, IOption option1, IOption option2)
        {
            value.ShouldBe($"{option1.Value}{Option.OptionSeparator}{option2.Value}");
        }

        public static void ShouldBeCombinationOf(this string value, IOption option1, int number)
        {
            value.ShouldBe($"{option1.Value}{Option.OptionSeparator}{number}");
        }

        /// <summary>
        /// Emulate the expresseion var x = first | second;
        /// </summary>
        /// <param name="first">first option in combination expression</param>
        /// <param name="second">second option in combination expression</param>
        /// <returns>An object that represents the result of the attempt of running the expression</returns>
        public static CombineAttemptResult CombinedWith(this object first, object second)
        {
            return CombineAttemptResult.TryCombine(first, second);
        }

        public static CombineAttemptResult ShouldResultInSetOf<TOption>(this CombineAttemptResult combineAttempt)
            where TOption : IOption
        {
            combineAttempt.CompilerError.ShouldBeEmpty();
            combineAttempt.ResultSet.ShouldBeAssignableTo<IOptionSet<TOption>>();
            return combineAttempt;
        }

        public static CombineAttemptResult ShouldNotResultInSetOf<TOption>(this CombineAttemptResult combineAttempt)
            where TOption : IOption
        {
            combineAttempt.ResultSet.ShouldNotBeAssignableTo<IOptionSet<TOption>>();
            return combineAttempt;
        }

        public static IEnumerable<(T first, T second)> AllPairs<T>(this IEnumerable<T> optionSource)
        {
            var options = optionSource.ToArray();
            for (int i = 0; i < options.Length; i++)
            {
                for (int j = i; j < options.Length; j++)
                {
                    var first = options[i];
                    var second = options[j];
                    yield return (first, second);
                }
            }
        }

        public static IEnumerable<(T1 first, T2 second)> AllPairsWith<T1, T2>(this IEnumerable<T1> firstOptionSource, IEnumerable<T2> secondOptionSource)
        {
            var secondSource = secondOptionSource.ToArray();
            foreach (var first in firstOptionSource)
                foreach (var second in secondSource)
                    yield return (first, second);
        }

        public static IEnumerable<(T2 second, T1 first)> ReversePairs<T1, T2>(this IEnumerable<(T1 first, T2 second)> source)
        {
            foreach (var (first, second) in source)
                yield return (second, first);
        }

        public static IEnumerable<object[]> ToFixtureData<T>(this IEnumerable<T> fixtureData, Func<T, object[]>? converter = null)
            where T : object
        {
            Func<T, object[]> defaultConverter = x => new[] { (object)x };
            converter = converter ?? defaultConverter;
            return fixtureData.Select(converter);
        }
    }
}
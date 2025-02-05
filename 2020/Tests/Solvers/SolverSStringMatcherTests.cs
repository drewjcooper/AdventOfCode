using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverS;

namespace AdventOfCode2020.Solvers
{
    public class SolverSStringMatcherTests
    {
        private readonly StringMatcher sut;

        public SolverSStringMatcherTests()
        {
            var matcherFactories = new Dictionary<string, Matcher>();
            matcherFactories["5"] = new AtomMatcher('b');
            matcherFactories["4"] = new AtomMatcher('a');
            matcherFactories["3"] = new EitherMatcher(
                new(),
                new SequenceMatcher(matcherFactories, "4", "5"),
                new SequenceMatcher(matcherFactories, "5", "4"));
            matcherFactories["2"] = new EitherMatcher(
                new(),
                new SequenceMatcher(matcherFactories, "4", "4"),
                new SequenceMatcher(matcherFactories, "5", "5"));
            matcherFactories["1"] = new EitherMatcher(
                new(),
                new SequenceMatcher(matcherFactories, "2", "3"),
                new SequenceMatcher(matcherFactories, "3", "2"));
            sut = new StringMatcher(new SequenceMatcher(matcherFactories, "4", "1", "5"), new());
        }

        [Theory]
        [InlineData("aaaabb")]
        [InlineData("aaabab")]
        [InlineData("abbabb")]
        [InlineData("abbbab")]
        [InlineData("aabaab")]
        [InlineData("aabbbb")]
        [InlineData("abaaab")]
        [InlineData("ababbb")]
        public void Match_MatchesWholeString_ReturnsTrue(string input)
        {
            var result = sut.Match(input);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("aaaabba")]
        [InlineData("aaababb")]
        [InlineData("abbabbab")]
        [InlineData("abbbabba")]
        public void Match_MatchesPartOfString_ReturnsFalse(string input)
        {
            var result = sut.Match(input);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("aaaab")]
        [InlineData("aaab")]
        [InlineData("abb")]
        [InlineData("")]
        public void Match_StringTooShort_ReturnsFalse(string input)
        {
            var result = sut.Match(input);

            result.Should().BeFalse();
        }
    }
}

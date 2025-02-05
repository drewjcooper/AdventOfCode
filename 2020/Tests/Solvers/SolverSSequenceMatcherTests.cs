using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static AdventOfCode2020.Solvers.SolverS;

namespace AdventOfCode2020.Solvers
{
    public class SolverSSequenceMatcherTests
    {
        private readonly ITestOutputHelper output;

        public SolverSSequenceMatcherTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("aab", "abaabba", 2)]
        [InlineData("abaa", "abaabba", 0)]
        [InlineData("ba", "abaabba", 1)]
        [InlineData("ba", "abaabba", 5)]
        public void Match_SequenceMatchesAtIndex_ReturnsTrue(string sequence, string input, int index)
        {
            var sut = CreateSequenceMatcher(sequence);

            var result = sut.Match(input, ref index);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("aab", "abaabba", 2)]
        [InlineData("abaa", "abaabba", 0)]
        [InlineData("ba", "abaabba", 1)]
        [InlineData("ba", "abaabba", 5)]
        public void Match_SequenceMatchesAtIndex_IncrementsIndex(string sequence, string input, int index)
        {
            var initial = index;
            var sut = CreateSequenceMatcher(sequence);

            sut.Match(input, ref index);

            index.Should().Be(initial + sequence.Length);
        }

        [Theory]
        [InlineData("aab", "abaabba", 1)]
        [InlineData("abaa", "abaabba", 3)]
        [InlineData("baab", "abaabba", 5)]
        [InlineData("ba", "abaabba", 2)]
        [InlineData("ba", "abaabba", 4)]
        public void Match_SequenceDoesntMatchAtIndex_ReturnsFalse(string sequence, string input, int index)
        {
            var sut = CreateSequenceMatcher(sequence);

            var result = sut.Match(input, ref index);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("aab", "abaabba", 1)]
        [InlineData("abaa", "abaabba", 3)]
        [InlineData("ba", "abaabba", 2)]
        [InlineData("ba", "abaabba", 4)]
        public void Match_SequenceDoesntMatchAtIndex_DoesNotChangeIndex(string sequence, string input, int index)
        {
            var initial = index;
            var sut = CreateSequenceMatcher(sequence);

            sut.Match(input, ref index);

            index.Should().Be(initial);
        }

        //[Theory]
        //[InlineData("2", "2 0", "abb")]
        //[InlineData("2", "2 0", "abbb")]
        //[InlineData("2", "2 0", "abbbb")]
        //[InlineData("2", "2 0", "abbbbb")]
        //[InlineData("2 1", "2 0 1", "aba")]
        //[InlineData("2 1", "2 0 1", "abbaa")]
        //[InlineData("2 1", "2 0 1", "abbbaaa")]
        //[InlineData("2 1", "2 0 1", "abbbbaaaa")]
        //[InlineData("2 1", "2 0 1", "ababbbaaa")]
        //[InlineData("2 1", "2 0 1", "abbaabbbaaa")]
        //public void Match_RecursiveRules_MatchesAsExpected(string sequence1, string sequence2, string input)
        //{
        //    var sut = CreateRecursiveSequenceMatcher(sequence1, sequence2);

        //    var result = sut.Match(input);

        //    result.Should().BeTrue();
        //}

        private static SequenceMatcher CreateSequenceMatcher(string sequence)
        {
            var matchers = sequence
                .Distinct()
                .ToDictionary(ch => ch.ToString(), ch => (Matcher)new AtomMatcher(ch));
            return new SequenceMatcher(matchers, sequence.Select(ch => ch.ToString()).ToArray());
        }

        private StringMatcher CreateRecursiveSequenceMatcher(string sequence1, string sequence2)
        {
            var matchers = new Dictionary<string, Matcher>
            {
                ["1"] = new AtomMatcher('a'),
                ["2"] = new AtomMatcher('b')
            };
            var backtracks = new Stack<(Matcher, int)>();
            matchers["3"] = Log(new EitherMatcher(
                backtracks,
                output.WriteLine,
                Log(new SequenceMatcher(matchers, sequence1.Split(' '))),
                Log(new SequenceMatcher(matchers, sequence2.Split(' ')))));
            matchers["0"] = Log(new SequenceMatcher(matchers, "1", "3"));
            return new StringMatcher(matchers["0"], backtracks);

            Matcher Log(Matcher matcher) => new LoggingMatcherDecorator(matcher, output.WriteLine);
        }
    }
}

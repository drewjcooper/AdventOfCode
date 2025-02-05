using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;
using FluentAssertions;
using Xunit;

namespace AdventOfCode2021.Solvers
{
    public class SolverNInsertionTests
    {
        private readonly SolverN.Rules rules = new()
        {
            "CH -> B",
            "HH -> N",
            "CB -> H",
            "NH -> C",
            "HB -> C",
            "HC -> B",
            "HN -> C",
            "NN -> C",
            "BH -> H",
            "NC -> B",
            "NB -> B",
            "BN -> B",
            "BB -> N",
            "BC -> B",
            "CC -> N",
            "CN -> C"
        };

        [Fact]
        public void DoPairInsertion_Once_ResultsInExpectedSequence()
        {
            var template = "NNCB".ToArray();
            var expected = "NCNBCHB".ToArray();

            var result = template.DoInsertions(rules);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DoPairInsertion_Twice_ResultsInExpectedSequence()
        {
            var template = "NNCB".ToArray();
            var expected = "NBCCNBBBCBHCB".ToArray();

            var result = template.DoInsertions(rules).DoInsertions(rules);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DoPairInsertion_TwiceSkip1_ResultsInExpectedSequence()
        {
            var template = "NNCB".ToArray();
            var expected = "BCCNBBBCBHCB".ToArray();

            var result = template.DoInsertions(rules).DoInsertions(rules).Skip(1);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData('N', 'N', 1, 0, 1, 0, 1)]
        [InlineData('H', 'N', 1, 0, 1, 0, 1)]
        [InlineData('N', 'N', 3, 3, 3, 0, 2)]
        [InlineData('N', 'N', 2, 1, 2, 0, 1)]
        [InlineData('N', 'C', 2, 3, 1, 0, 0)]
        [InlineData('C', 'B', 2, 2, 1, 1, 0)]
        public void Rules_CalculateFutureCounts(char A, char B, int rounds, params int[] counts)
        {
            rules.CalculateFutureCounts(rounds);

            foreach (var (c, x) in new[] { 'B', 'C', 'H', 'N' }.Zip(counts))
            {
                rules[A, B, c].Should().Be(x, $"[{A}, {B}, {c}]");
            }
        }

        [Theory]
        // [InlineData(1, 6, 4, 1, 2)]
        [InlineData(2, 23, 10, 5, 11)]
        public void Template_InsertionsAndFutureCounts(int rounds, params int[] expectedCounts)
        {
            rules.CalculateFutureCounts(rounds);
            var expected = rules.Alphabet.OrderBy(c => c).Zip(expectedCounts).ToDictionary(x => x.First, x => x.Second);

            var result = "NNCB".DoInsertions(rules, rounds).GetCounts(rules);

            result.Should().BeEquivalentTo(expected);
        }
    }
}

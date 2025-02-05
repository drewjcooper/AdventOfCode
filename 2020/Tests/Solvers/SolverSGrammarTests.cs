using System.Linq;
using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverS;

namespace AdventOfCode2020.Solvers
{
    public class SolverSGrammarTests
    {
        [Theory]
        [MemberData(nameof(RuleTreeTestCases))]
        public void BuildTree_BuildsExpectedTree(string[] rules, string[] expected)
        {
            var sut = new Grammar(rules.Select(r => Rule.Parse(r)));

            var result = sut.BuildTree();

            result.ToList<string>().Should().BeEquivalentTo(expected);
        }

        public static TheoryData<string[], string[]> RuleTreeTestCases()
            => new TheoryData<string[], string[]>
            {
                {
                    new[] 
                    {
                        "0: \"a\""
                    },
                    new[] { "a" }
                },
                {
                    new[] {
                        "0: 4 4 5",
                        "4: \"a\"",
                        "5: \"b\""
                    },
                    new[] { "aab" }
                },
                {
                    new[] 
                    {
                        "0: 4 4 | 5 5",
                        "4: \"a\"",
                        "5: \"b\""
                    },
                    new[] { "aa", "bb" }
                },
                {
                    new[] 
                    {
                        "0: 2 3 | 3 2",
                        "2: 4 4",
                        "3: 4 5",
                        "4: \"a\"",
                        "5: \"b\""
                    },
                    new[] { "aaab", "abaa" }
                },
                {
                    new[] 
                    {
                        "0: 2 3 | 3 2",
                        "2: 4 4 | 5 5",
                        "3: 4 5 | 5 4",
                        "4: \"a\"",
                        "5: \"b\""
                    },
                    new[] { "aaab", "aaba", "bbab", "bbba", "abaa", "abbb", "baaa", "babb" }
                },
                {
                    new[] 
                    {
                        "0: 4 1 5",
                        "1: 2 3 | 3 2",
                        "2: 4 4 | 5 5",
                        "3: 4 5 | 5 4",
                        "4: \"a\"",
                        "5: \"b\""
                    },
                    new[] { "aaaabb", "aaabab", "abbabb", "abbbab", "aabaab", "aabbbb", "abaaab", "ababbb" }
                },
                {
                    new[]
                    {
                        "0: 2 2 1",// | 2 0 1",
                        "1: \"a\"",
                        "2: \"b\""
                    },
                    new[] { "ba", "bbaa", "bbbaaa", "bbbbaaaa", "bbbbbaaaaa" }
                }
            };
    }
}

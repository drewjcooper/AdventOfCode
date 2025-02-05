using System.Linq;
using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverS;

namespace AdventOfCode2020.Solvers
{
    public class SolverSTreeTests
    {
        [Theory]
        [MemberData(nameof(RuleTreeTestCases))]
        public void BuildTree_BuildsExpectedTree(Tree sut, string[] expected)
        {
            var result = sut.ToList<string>();

            result.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<Tree, string[]> RuleTreeTestCases()
            => new TheoryData<Tree, string[]>
            {
                {
                    new Tree()
                    {
                        new Tree('a')
                        {
                            new Tree('a')
                            {
                                new Tree('a')
                                {
                                    new Tree('a') { new Tree('b') { new Tree('b') } },
                                    new Tree('b') { new Tree('a') { new Tree('b') } }
                                },
                                new Tree('b')
                                {
                                    new Tree('a') { new Tree('a') { new Tree('b') } },
                                    new Tree('b') { new Tree('b') { new Tree('b') } }
                                }
                            },
                            new Tree('b')
                            {
                                new Tree('a')
                                {
                                    new Tree('a') { new Tree('a') { new Tree('b') } },
                                    new Tree('b') { new Tree('b') { new Tree('b') } }
                                },
                                new Tree('b')
                                {
                                    new Tree('a') { new Tree('b') { new Tree('b') } },
                                    new Tree('b') { new Tree('a') { new Tree('b') } }
                                }
                            }
                        }
                    },
                    new[] { "aaaabb", "aaabab", "abbabb", "abbbab", "aabaab", "aabbbb", "abaaab", "ababbb" }
                }
            };
    }
}

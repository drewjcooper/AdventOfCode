using System.Collections.Generic;
using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverP;

namespace AdventOfCode2020.Solvers
{
    public class SolverPFieldMatcherTests
    {
        private static readonly Ticket[] tickets;
        private readonly FieldMatcher sut;

        static SolverPFieldMatcherTests()
        {
            tickets = new[] { new Ticket(3, 9, 18), new Ticket(15, 1, 5), new Ticket(5, 14, 9) };
        }

        public SolverPFieldMatcherTests()
        {
            sut = new FieldMatcher(
                new[] {
                    Rule.Parse("class: 0-1 or 4-19"),
                    Rule.Parse("row: 0-5 or 8-19"),
                    Rule.Parse("seat: 0-13 or 16-19")
                },
                new Ticket(11, 12, 13));
        }

        [Fact]
        public void Ctor_InitialisesProperties()
        {
            sut.Possibilities.Keys.Should().BeEquivalentTo(new[] { "class", "row", "seat" });
            sut.Possibilities.Values.Should().AllBeEquivalentTo(new[] { 0, 1, 2 });
            sut.FieldIndices.Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(ScanPossibilitiesTestCases))]
        public void Scan_UpdatesPossibilitiesAsExpected(int[] indices, Dictionary<string, int[]> expected)
        {
            foreach (var index in indices)
            {
                sut.Scan(tickets[index]);
            }

            sut.Possibilities.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(ScanFieldIndicesTestCases))]
        public void Scan_UpdatesFieldIndicesAsExpected(int[] indices, Dictionary<string, int> expected)
        {
            foreach (var index in indices)
            {
                sut.Scan(tickets[index]);
            }

            sut.FieldIndices.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<int[], Dictionary<string, int[]>> ScanPossibilitiesTestCases() =>
            new TheoryData<int[], Dictionary<string, int[]>>
            {
                {
                    new[] { 0 },
                    new Dictionary<string, int[]>
                    {
                        ["class"] = new[] { 1, 2 }, ["row"] = new[] { 0, 1, 2 }, ["seat"] = new[] { 0, 1, 2 }
                    }
                },
                {
                    new[] { 1 },
                    new Dictionary<string, int[]>
                    {
                        ["class"] = new[] { 0, 1, 2 }, ["row"] = new[] { 0, 1, 2 }, ["seat"] = new[] { 1, 2 }
                    }
                },
                {
                    new[] { 2 },
                    new Dictionary<string, int[]>
                    {
                        ["class"] = new[] { 0, 1, 2 }, ["row"] = new[] { 0, 1, 2 }, ["seat"] = new[] { 0, 2 }
                    }
                },
                {
                    new[] { 0, 1 },
                    new Dictionary<string, int[]>
                    {
                        ["class"] = new[] { 1, 2 }, ["seat"] = new[] { 1, 2 }
                    }
                },
                {
                    new[] { 1, 2 },
                    new Dictionary<string, int[]>
                    {
                        ["class"] = new[] { 0, 1 }, ["row"] = new[] { 0, 1 }
                    }
                },
                {
                    new[] { 2, 0 },
                    new Dictionary<string, int[]>
                    {
                        ["class"] = new[] { 1, 2 }, ["row"] = new[] { 0, 1, 2 }, ["seat"] = new[] { 0, 2 }
                    }
                },
                {
                    new[] { 0, 1, 2 },
                    new Dictionary<string, int[]>()
                }
            };

        public static TheoryData<int[], Dictionary<string, int>> ScanFieldIndicesTestCases() =>
            new TheoryData<int[], Dictionary<string, int>>()
            {
                {
                    new[] { 0 },
                    new Dictionary<string, int>()
                },
                {
                    new[] { 1 },
                    new Dictionary<string, int>()
                },
                {
                    new[] { 2 },
                    new Dictionary<string, int>()
                },
                {
                    new[] { 0, 1 },
                    new Dictionary<string, int> { ["row"] = 0 }
                },
                {
                    new[] { 1, 2 },
                    new Dictionary<string, int> { ["seat"] = 2 }
                },
                {
                    new[] { 2, 0 },
                    new Dictionary<string, int>()
                },
                {
                    new[] { 0, 1, 2 },
                    new Dictionary<string, int>() { ["class"] = 1, ["row"] = 0, ["seat"] = 2 }
                }
            };
    }
}

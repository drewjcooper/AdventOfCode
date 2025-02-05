using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverQ;

namespace AdventOfCode2020.Solvers
{
    public class SolverQ_3SpaceTests
    {
        [Theory]
        [MemberData(nameof(StepTestCases))]
        public void Step_ReturnsNewConfiguration(int z, string expected)
        {
            var initial = new[]
            {
                new[] { '.', '#', '.' },
                new[] { '.', '.', '#' },
                new[] { '#', '#', '#' }
            };
            var sut = new _3Space(initial);

            sut = sut.Step();

            sut.ToString(z).Should().Be(expected);
        }

        public static TheoryData<int, string> StepTestCases() =>
            new TheoryData<int, string>
            {
                {
-1,
@".....
.....
.#...
...#.
..#.."
                },
                {
0,
@".....
.....
.#.#.
..##.
..#.."
                },
                {
1,
@".....
.....
.#...
...#.
..#.."
                }
            };
    }
}

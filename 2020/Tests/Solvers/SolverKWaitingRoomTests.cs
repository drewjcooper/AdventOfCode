using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using static AdventOfCode2020.Solvers.SolverK;

namespace AdventOfCode2020.Solvers
{
    public class SolverKWaitingRoomTests
    {
        [Theory]
        [MemberData(nameof(NeighbourSeatsTestCases))]
        [MemberData(nameof(VisibleSeatsTestCases))]
        public void CalculatesSeatOccupancyCorrectly(int steps, string expected, IDecisionModel decisionModel)
        {
            var seats = SplitLines(
@"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL");
            var sut = new SolverK.WaitingRoom.Enumerator(seats, decisionModel);

            var result = Enumerable.Range(0, steps).Select(x => sut.MoveNext()).ToList();

            result.Should().AllBeEquivalentTo(true);
            sut.Current.Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 0, '#')]
        [InlineData(1, 2, 'L')]
        [InlineData(1, 3, 'L')]
        [InlineData(2, 3, '.')]
        [InlineData(1, 0, '.')]
        [InlineData(1, 1, 'L')]
        [InlineData(0, 0, '#')]
        [InlineData(0, 2, '#')]
        public void GetNext_CalculatesNextState(int row, int col, char expected)
        {
            var input =
@"
#.##.
.L##.
L.#.#";
            var sut = new SolverK.WaitingRoom.Enumerator(SplitLines(input), new NeighbouringSeatsDecisionModel());

            var result = sut.GetNext(row, col);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 0, 0)]
        [InlineData(1, 2, 4)]
        [InlineData(1, 3, 5)]
        [InlineData(2, 3, 4)]
        [InlineData(1, 0, 1)]
        [InlineData(1, 1, 4)]
        [InlineData(0, 0, 0)]
        [InlineData(0, 2, 3)]
        public void CountOccupiedNeighbours_CountsNeighbours(int row, int col, int expected)
        {
            var input =
@"
#.##.
.L##.
L.#.#";
            var sut = new SolverK.WaitingRoom.Enumerator(SplitLines(input), new NeighbouringSeatsDecisionModel());

            var result = sut.CountOccupiedNeighbours(row, col);

            result.Should().Be(expected);
        }

        [Fact]
        public void MoveNext_SetsCurrentCorrectly()
        {
            var input = SplitLines(
@"#.##.
.L##.
L.#.#");
            var sut = new SolverK.WaitingRoom.Enumerator(input, new NeighbouringSeatsDecisionModel());
            var expected =
@"#.##.
.LLL.
#.#.#";

            var result = sut.MoveNext();

            result.Should().BeTrue();
            sut.Current.Should().Be(expected);
        }

        private static string[] SplitLines(string input) =>
            input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        public static TheoryData<int, string, IDecisionModel> NeighbourSeatsTestCases()
            => SeatsTestCases(new[]
            {
@"#.##.##.##
#######.##
#.#.#..#..
####.##.##
#.##.##.##
#.#####.##
..#.#.....
##########
#.######.#
#.#####.##",

@"#.LL.L#.##
#LLLLLL.L#
L.L.L..L..
#LLL.LL.L#
#.LL.LL.LL
#.LLLL#.##
..L.L.....
#LLLLLLLL#
#.LLLLLL.L
#.#LLLL.##",

@"#.##.L#.##
#L###LL.L#
L.#.#..#..
#L##.##.L#
#.##.LL.LL
#.###L#.##
..#.#.....
#L######L#
#.LL###L.L
#.#L###.##",

@"#.#L.L#.##
#LLL#LL.L#
L.L.L..#..
#LLL.##.L#
#.LL.LL.LL
#.LL#L#.##
..L.L.....
#L#LLLL#L#
#.LLLLLL.L
#.#L#L#.##",

@"#.#L.L#.##
#LLL#LL.L#
L.#.L..#..
#L##.##.L#
#.#L.LL.LL
#.#L#L#.##
..L.L.....
#L#L##L#L#
#.LLLLLL.L
#.#L#L#.##"
            },
            new NeighbouringSeatsDecisionModel());

        public static TheoryData<int, string, IDecisionModel> VisibleSeatsTestCases()
            => SeatsTestCases(new[]
            {
@"#.##.##.##
#######.##
#.#.#..#..
####.##.##
#.##.##.##
#.#####.##
..#.#.....
##########
#.######.#
#.#####.##",

@"#.LL.LL.L#
#LLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLL#
#.LLLLLL.L
#.LLLLL.L#",

@"#.L#.##.L#
#L#####.LL
L.#.#..#..
##L#.##.##
#.##.#L.##
#.#####.#L
..#.#.....
LLL####LL#
#.L#####.L
#.L####.L#",

@"#.L#.L#.L#
#LLLLLL.LL
L.L.L..#..
##LL.LL.L#
L.LL.LL.L#
#.LLLLL.LL
..L.L.....
LLLLLLLLL#
#.LLLLL#.L
#.L#LL#.L#",

@"#.L#.L#.L#
#LLLLLL.LL
L.L.L..#..
##L#.#L.L#
L.L#.#L.L#
#.L####.LL
..#.#.....
LLL###LLL#
#.LLLLL#.L
#.L#LL#.L#",

@"#.L#.L#.L#
#LLLLLL.LL
L.L.L..#..
##L#.#L.L#
L.L#.LL.L#
#.LLLL#.LL
..#.L.....
LLL###LLL#
#.LLLLL#.L
#.L#LL#.L#"
            },
            new VisibleSeatsDecisionModel());

        private static TheoryData<int, string, IDecisionModel> SeatsTestCases(
            string[] stepExpectations,
            IDecisionModel decisionModel)
        {
            var theoryData = new TheoryData<int, string, IDecisionModel>();
            for (int i = 0; i < stepExpectations.Length; i++)
            {
                theoryData.Add(i + 1, stepExpectations[i], decisionModel);
            };
            return theoryData;
        }
    }
}

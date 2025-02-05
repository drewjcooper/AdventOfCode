using FluentAssertions;
using Xunit;

namespace AdventOfCode2020.Solvers
{
    public class SolverEBoardingPassTests
    {
        [Theory]
        [InlineData("FBFBBFFRLR", 357, 44, 5)]
        [InlineData("BFFFBBFRRR", 567, 70, 7)]
        [InlineData("FFFBBBFRRR", 119, 14, 7)]
        [InlineData("BBFFBBFRLL", 820, 102, 4)]
        public void CalculatesSeatIdRowAndColumnCorrectly(string pass, int id, int row, int column)
        {
            var expected = new { SeatId = id, Row = row, Column = column };

            var sut = new SolverE.BoardingPass(pass);

            sut.Should().BeEquivalentTo(expected);
        }
    }
}

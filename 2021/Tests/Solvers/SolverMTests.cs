using System.Linq;
using FluentAssertions;
using Xunit;

using static AdventOfCode2021.Solvers.SolverM;

namespace AdventOfCode2021.Solvers
{
    public class SolverMTests
    {
        [Theory]
        [InlineData("fold along x=2", 1, 4, "0,0", "0,1", "1,3", "1,4")]
        [InlineData("fold along y=2", 4, 1, "0,0", "1,0", "3,1", "4,1")]
        public void Command_Execute_ResultsInExpectedPage(
            string commandText,
            int xMax,
            int yMax,
            params string[] expectedDots)
        {
            var page = new Page(new[] { new Point(0, 0), new Point(1, 4), new Point(3, 3), new Point(4, 1)});
            var command = Command.Parse(commandText);
            var expected = new
            {
                Dots = expectedDots.Select(d => Point.Parse(d)).ToArray(),
                MaxX = xMax,
                MaxY = yMax
            };

            var newPage = command.Execute(page);

            newPage.Should().NotBe(page).And.BeEquivalentTo(expected);
        }
    }
}

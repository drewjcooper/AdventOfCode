using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using static AdventOfCode2021.Solvers.SolverI;

namespace AdventOfCode2021.Solvers
{
    public class PuzzleIPointTests
    {
        [Fact]
        public void Point_BehavesCorrectlyInHashSet()
        {
            var set = new HashSet<Point>();
            var sut = new Point(5, 4);

            set.Add(sut);
            set.Add(sut);

            set.Should().HaveCount(1);
        }
    }
}

using FluentAssertions;
using Xunit;
using static AdventOfCode2021.Solvers.SolverV;

namespace AdventOfCode2021.Solvers
{
    public class SolverVTests
    {
        [Theory]
        [MemberData(nameof(IntersectionCases))]
        public void Range3D_Intersect_ProducesExpectedResult(Range3D sut, Range3D other, Range3D? expected)
        {
            var result = sut.Intersect(other);

            result.Should().Be(expected);
        }

        public static TheoryData<Range3D, Range3D, Range3D?> IntersectionCases =>
            new TheoryData<Range3D, Range3D, Range3D?>
            {
                { new(10, 20, 50, 90, 35, 60), new(50, 65, 60, 95, 25, 50), null },                         // disjoint
                { new(10, 30, 50, 90, 30, 60), new(20, 50, 40, 60, 25, 45), new(20, 30, 50, 60, 30, 45) },  // 1 vertex
                { new(10, 70, 20, 90, 30, 80), new(20, 50, 40, 60, 35, 45), new(20, 50, 40, 60, 35, 45) },  // second
                { new(60, 70, 20, 30, 30, 50), new(20, 90, 10, 60, 15, 65), new(60, 70, 20, 30, 30, 50) },  // first
                { new(20, 70, 20, 30, 30, 80), new(40, 90, 10, 60, 15, 65), new(40, 70, 20, 30, 30, 65) },
                { new(20, 95, 20, 30, 30, 80), new(40, 60, 10, 60, 15, 65), new(40, 60, 20, 30, 30, 65) },
            };

        [Fact]
        public void GetRemainder()
        {
            var large = new Range3D(40, 75, 50, 105, 70, 165);
            var small = new Range3D(50, 60, 70, 90, 100, 130);
            var expected = new Range3D[]
            {
                new(40, 49, 50, 105, 70, 165),
                new(61, 75, 50, 105, 70, 165),
                new(50, 60, 50, 69, 70, 165),
                new(50, 60, 91, 105, 70, 165),
                new(50, 60, 70, 90, 70, 99),
                new(50, 60, 70, 90, 131, 165)
            };

            var result = large.GetRemainder(small);

            result.Should().BeEquivalentTo(expected);
        }
    }
}

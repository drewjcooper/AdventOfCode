using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace AdventOfCode2020.Solvers
{
    public class SolverPRangeTests
    {
        [Theory, AutoData]
        public void Parse_CreatesExpectedRange(int from, int delta)
        {
            var to = from + delta;
            var text = $"{from}-{to}";
            var expected = new { From = from, To = to };

            var result = SolverP.Range.Parse(text);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(23, 52, 12, false)]
        [InlineData(5, 9, 5, true)]
        [InlineData(110, 134, 123, true)]
        [InlineData(45, 77, 77, true)]
        [InlineData(76, 89, 98, false)]
        public void Contains_ChecksNumberInRangeWithInclusiveBounds(int from, int to, int number, bool expected)
        {
            var sut = new SolverP.Range(from, to);

            var result = sut.Contains(number);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 6, 8, 15, false)]   // Ranges disjoint
        [InlineData(6, 11, 1, 4, false)]
        [InlineData(2, 6, 7, 10, false)]   // Ranges disjoint, but adjacent
        [InlineData(5, 11, 1, 4, false)]
        [InlineData(2, 6, 6, 10, true)]   // Ranges overlap by 1
        [InlineData(9, 16, 6, 9, true)]
        [InlineData(3, 7, 5, 11, true)]   // Ranges overlap
        [InlineData(7, 26, 4, 16, true)]
        [InlineData(2, 16, 7, 12, true)]  // 1 range contains other
        [InlineData(7, 23, 4, 61, true)]
        public void Overlaps_GivesExpectedResultBasedOnInclusiveRanges(
            int from1,
            int to1,
            int from2,
            int to2,
            bool expected)
        {
            var sut = new SolverP.Range(from1, to1);
            var other = new SolverP.Range(from2, to2);

            var result = sut.Overlaps(other);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 6, 8, 15, false)]   // Ranges disjoint
        [InlineData(6, 11, 1, 4, false)]
        [InlineData(2, 6, 7, 10, true)]   // Ranges disjoint, but adjacent
        [InlineData(5, 11, 1, 4, true)]
        [InlineData(2, 6, 6, 10, false)]   // Ranges overlap by 1
        [InlineData(9, 16, 6, 9, false)]
        [InlineData(3, 6, 5, 11, false)]   // Ranges overlap
        [InlineData(7, 26, 4, 8, false)]
        public void IsAdjacentTo_GivesExpectedResultBasedOnInclusiveRanges(
            int from1,
            int to1,
            int from2,
            int to2,
            bool expected)
        {
            var sut = new SolverP.Range(from1, to1);
            var other = new SolverP.Range(from2, to2);

            var result = sut.IsAdjacentTo(other);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 6, 7, 10, 2, 10)]   // Ranges disjoint, but adjacent
        [InlineData(5, 11, 1, 4, 1, 11)]
        [InlineData(2, 6, 6, 10, 2, 10)]   // Ranges overlap by 1
        [InlineData(9, 16, 6, 9, 6, 16)]
        [InlineData(3, 7, 5, 11, 3, 11)]   // Ranges overlap
        [InlineData(7, 26, 4, 16, 4, 26)]
        [InlineData(2, 16, 7, 12, 2, 16)]  // 1 range contains other
        [InlineData(7, 23, 4, 61, 4, 61)]
        public void TryMerge_WhenRangesOverlapOrAdjacent_ProducesSingleRange(
            int from1,
            int to1,
            int from2,
            int to2,
            int expectedFrom,
            int expectedTo)
        {
            var sut = new SolverP.Range(from1, to1);
            var other = new SolverP.Range(from2, to2);
            var expected = new SolverP.Range(expectedFrom, expectedTo);

            var result = sut.TryMerge(other, out var merged);

            result.Should().BeTrue();
            merged.Should().Be(expected);
        }


        [Theory]
        [InlineData(2, 6, 8, 10)]   // Ranges disjoint
        [InlineData(7, 11, 1, 4)]
        public void TryMerge_WhenRangesDisjointAndNotAdjacent_ReturnsFalse(int from1, int to1, int from2, int to2)
        {
            var sut = new SolverP.Range(from1, to1);
            var other = new SolverP.Range(from2, to2);

            var result = sut.TryMerge(other, out _);

            result.Should().BeFalse();
        }
    }
}

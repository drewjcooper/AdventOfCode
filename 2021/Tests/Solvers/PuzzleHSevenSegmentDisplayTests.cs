using FluentAssertions;
using Xunit;
using static AdventOfCode2021.Solvers.SolverH;

namespace AdventOfCode2021.Solvers
{
    public class PuzzleHSevenSegmentDisplayTests
    {
        [Theory]
        [InlineData("abcdefg", 0b1111111, 7)]
        [InlineData("aceg", 0b1010101, 4)]
        [InlineData("bdf", 0b0101010, 3)]
        [InlineData("dacg", 0b1001101, 4)]
        public void Ctor_StringValue_PopulatesPropertiesCorrectly(string value, int flags, int count)
        {
            var expected = new { Flags = flags, Count = count };

            var sut = new SevenSegmentDisplay(value);

            sut.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(0b1111111, 7)]
        [InlineData(0b1010101, 4)]
        [InlineData(0b0101010, 3)]
        [InlineData(0b1001101, 4)]
        public void Ctor_Flags_PopulatesPropertiesCorrectly(int flags, int count)
        {
            var expected = new { Flags = flags, Count = count };

            var sut = new SevenSegmentDisplay(flags);

            sut.Should().BeEquivalentTo(expected);
        }
    }
}

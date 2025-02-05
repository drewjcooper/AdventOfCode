using FluentAssertions;
using Xunit;
using static AdventOfCode2021.Solvers.SolverR;

namespace AdventOfCode2021.Solvers
{
    public class SolverRTests
    {
        [Theory]
        [InlineData("[[1,2],[[3,4],5]]")]
        [InlineData("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")]
        [InlineData("[[[[1,1],[2,2]],[3,3]],[4,4]]")]
        [InlineData("[[[[3,0],[5,3]],[4,4]],[5,5]]")]
        [InlineData("[[[[5,0],[7,4]],[5,5]],[6,6]]")]
        [InlineData("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]")]
        public void Parse_ToString_RetrunsOriginalString(string value)
        {
            var sut = value.ParseNumber();

            var result = sut.ToString();

            result.Should().Be(value);
        }

        [Theory]
        [InlineData("[[1,2],[[3,4],5]]", 143)]
        [InlineData("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384)]
        [InlineData("[[[[1,1],[2,2]],[3,3]],[4,4]]", 445)]
        [InlineData("[[[[3,0],[5,3]],[4,4]],[5,5]]", 791)]
        [InlineData("[[[[5,0],[7,4]],[5,5]],[6,6]]", 1137)]
        [InlineData("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488)]
        public void Magnitude_CalculatesCorrectValue(string number, int expected)
        {
            var sut = number.ParseNumber();

            var result = sut.Magnitude;

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
        [InlineData("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
        [InlineData("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]")]
        [InlineData("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]")]
        [InlineData("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
        public void Exploder_Visit_CanExplode_ProducesExpectedResult(string start, string expected)
        {
            var number = start.ParseNumber();

            var result = Exploder.Explode(number);

            number.ToString().Should().Be(expected);
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("[[1,2],[[3,4],5]]")]
        [InlineData("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")]
        [InlineData("[[[[1,1],[2,2]],[3,3]],[4,4]]")]
        [InlineData("[[[[3,0],[5,3]],[4,4]],[5,5]]")]
        [InlineData("[[[[5,0],[7,4]],[5,5]],[6,6]]")]
        [InlineData("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]")]
        public void Exploder_Visit_CantExplode_ReturnsFalseWithoutModifyingNumber(string start)
        {
            var number = start.ParseNumber();

            var result = Exploder.Explode(number);

            number.ToString().Should().Be(start);
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("[[[[0,7],4],[[7,8],[0,13]]],[1,1]]", "[[[[0,7],4],[[7,8],[0,[6,7]]]],[1,1]]")]
        public void Splitter_CanSplit_ProducesExpectedResult(string start, string expected)
        {
            var number = start.ParseNumber();

            var result = Splitter.Split(number);

            number.ToString().Should().Be(expected);
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("[[[[0,7],4],[[7,8],[0,[6,7]]]],[1,1]]")]
        public void Splitter_CantSplit_ReturnsFalse(string start)
        {
            var number = start.ParseNumber();

            var result = Splitter.Split(number);

            number.ToString().Should().Be(start);
            result.Should().BeFalse();
        }

        [Fact]
        public void Addition_NullIsIdentity()
        {
            var right = "[[1,2],[[3,4],5]]".ParseNumber();

            var result = null + right;

            result.ToString().Should().Be(right.ToString());
        }

        [Theory]
        [InlineData("[1,2]", "[[3,4],5]", "[[1,2],[[3,4],5]]")]
        [InlineData("[[[[4,3],4],4],[7,[[8,4],9]]]", "[1,1]", "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")]
        public void Addition_ShouldGiveCorrectResult(string left, string right, string expected)
        {
            var result = left.ParseNumber() + right.ParseNumber();

            result.ToString().Should().Be(expected);
        }
    }
}

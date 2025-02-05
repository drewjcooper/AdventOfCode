using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverR;

namespace AdventOfCode2020.Solvers
{
    public class SolverRTokeniserTests
    {
        [Theory]
        [InlineData("1 + 2", "1", "+", "2")]
        [InlineData("3 * 4", "3", "*", "4")]
        [InlineData("1 + 2 * 3", "1", "+", "2", "*", "3")]
        [InlineData("1 + (23 * (4 + 56))", "1", "+", "(", "23", "*", "(", "4", "+", "56", ")", ")")]
        public void Tokens_ReturnsExpectedTokens(string input, params string[] expected)
        {
            var sut = new Tokeniser(input);

            var result = sut.Tokens;

            result.Should().BeEquivalentTo(expected);
        }
    }
}

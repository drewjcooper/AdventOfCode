using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverR;

namespace AdventOfCode2020.Solvers
{
    public class SolverRParserTests
    {
        [Theory]
        [InlineData("1 + 2", 3)]
        [InlineData("3 * 4", 12)]
        [InlineData("1 + 2 * 3 + 4 * 5 + 6", 71)]
        [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [InlineData("2 * 3 + (4 * 5)", 26)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)]
        public void Parse_ReturnsExpressionWithNoPrecedence(string expression, int expected)
        {
            var tokeniser = new Tokeniser(expression);
            var sut = new Parser();

            var result = sut.Parse(tokeniser.Tokens);

            result.Evaluate().Should().Be(expected);
        }
    }
}

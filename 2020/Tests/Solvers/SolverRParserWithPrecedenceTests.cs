using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverR;

namespace AdventOfCode2020.Solvers
{
    public class SolverRParserWithPrecedenceTests
    {
        [Theory]
        [InlineData("1 + 2", 3)]
        [InlineData("3 * 4", 12)]
        [InlineData("1 + 2 * 3 + 4 * 5 + 6", 231)]
        [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [InlineData("2 * 3 + (4 * 5)", 46)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 1445)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 669060)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340)]
        public void Parse_ReturnsExpressionWithAdditionPrecedence(string expression, int expected)
        {
            var tokeniser = new Tokeniser(expression);
            var sut = new ParserWithPrecedence();

            var result = sut.Parse(tokeniser.Tokens);

            result.Evaluate().Should().Be(expected);
        }
    }
}

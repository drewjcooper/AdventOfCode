using FluentAssertions;
using Xunit;

using static AdventOfCode2021.Solvers.SolverJ;
using static AdventOfCode2021.Solvers.SolverJ.Status;

namespace AdventOfCode2021.Solvers
{
    public class PuzzleJScannerTests
    {
        [Theory]
        [InlineData("{([(<{}[<>[]}>{[]{[(<()>", ']', '}')]
        [InlineData("[[<[([]))<([[{}[[()]]]", ']', ')')]
        [InlineData("[{[{({}]{}}([{[{{{}}([]", ')', ']')]
        [InlineData("[<(<(<(<{}))><([]([]()", '>', ')')]
        [InlineData("<{([([[(<>()){}]>(<<{{", ']', '>')]
        public void Scan_CorruptedLine_ReturnsFoundAndExpectedChar(string line, char expectedChar, char foundChar)
        {
            var sut = new Scanner();
            var expected = new { Status = Corrupted, Expected = expectedChar, Found = foundChar };

            var result = sut.Scan(line);

            result.Should().BeEquivalentTo(expected);
        }
    }
}

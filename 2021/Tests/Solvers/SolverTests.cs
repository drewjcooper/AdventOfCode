using AdventOfCode2021.Helpers;
using AdventOfCode2021.Input;
using FluentAssertions;
using Xunit;

namespace AdventOfCode2021.Solvers
{
    public partial class SolverTests
    {
        [Theory]
        [MemberData(nameof(TestCases))]
        public void Solve_GivesExpectedAnswer(string puzzleId, string input, string expected)
        {
            var id = PuzzleId.Parse(puzzleId);
            var sut = Solver.Get(id, PuzzleInput.From(input));

            var result = sut.Solve(id);

            result.Should().Be(expected);
        }
    }
}

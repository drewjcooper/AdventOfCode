using AdventOfCode.Helpers;
using AdventOfCode.Input;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Solvers;

public partial class SolverTests(ITestOutputHelper testOutputHelper)
{
    [Theory]
    [MemberData(nameof(TestCases))]
    public void Solve_GivesExpectedAnswer(string puzzleId, string input, string expected)
    {
        var id = PuzzleId.Parse(puzzleId);
        var sut = Solver.Get(id, PuzzleInput.From(input), testOutputHelper.WriteLine);

        var result = sut.Solve(id);

        result.Should().Be(expected);
    }
}

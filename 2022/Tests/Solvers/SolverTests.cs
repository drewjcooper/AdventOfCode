using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2022.Solvers;

public partial class SolverTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    public SolverTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Solve_GivesExpectedAnswer(string puzzleId, string input, string expected)
    {
        var id = PuzzleId.Parse(puzzleId);
        var sut = Solver.Get(id, PuzzleInput.From(input), _testOutputHelper.WriteLine);

        var result = sut.Solve(id);

        result.Should().Be(expected);
    }
}

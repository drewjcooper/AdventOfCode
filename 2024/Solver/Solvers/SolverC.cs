using System.Text.RegularExpressions;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal partial class SolverC(PuzzleInput input) : Solver
{
    private readonly string _input = input.RawText;

    protected override Answer SolvePart1()
        => InstructionRegex()
            .Matches(_input)
            .Select(m => int.Parse(m.Groups["x"].Value) * int.Parse(m.Groups["y"].Value))
            .Sum();

    protected override Answer SolvePart2()
        => InstructionRegex()
            .Matches(_input)
            .Aggregate((Enabled: true, Value: 0), HandleMatch)
            .Value;

    private static (bool, int) HandleMatch((bool Enabled, int Value) state, Match match)
        => (state.Enabled, match.Value) switch
        {
            (_, "do()") => (true, state.Value),
            (_, "don't()") => (false, state.Value),
            (true, _) => (state.Enabled, state.Value + int.Parse(match.Groups["x"].Value) * int.Parse(match.Groups["y"].Value)),
            _ => state
        };

    [GeneratedRegex(@"mul\((?'x'\d{1,3}),(?'y'\d{1,3})\)|(do\(\))|(don't\(\))")]
    private static partial Regex InstructionRegex();
}

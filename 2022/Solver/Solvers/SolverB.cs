using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverB : Solver
{
    private readonly Dictionary<(char, char), (int, int)> outcomes
        = new()
        {
            [('B', 'X')] = (1 + 0, 1 + 0),
            [('C', 'Y')] = (2 + 0, 3 + 3),
            [('A', 'Z')] = (3 + 0, 2 + 6),
            [('A', 'X')] = (1 + 3, 3 + 0),
            [('B', 'Y')] = (2 + 3, 2 + 3),
            [('C', 'Z')] = (3 + 3, 1 + 6),
            [('C', 'X')] = (1 + 6, 2 + 0),
            [('A', 'Y')] = (2 + 6, 1 + 3),
            [('B', 'Z')] = (3 + 6, 3 + 6)
        };
        private readonly IEnumerable<(char, char)> rounds;

    public SolverB(PuzzleInput input)
    {
        rounds = input.Lines.Select(l => (l[0], l[2])).ToList();
    }
    
    protected override string SolvePart1() => rounds.Sum(r => outcomes[r].Item1).ToString();

    protected override string SolvePart2() => rounds.Sum(r => outcomes[r].Item2).ToString();
}
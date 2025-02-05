using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverD : Solver
{
    private readonly IEnumerable<(Range, Range)> _pairs;

    public SolverD(PuzzleInput input)
    {
        _pairs = input.Lines.Select(l => l.Split(',')).Select(e => (Range.Parse(e[0]), Range.Parse(e[1])));
    }
    
    protected override string SolvePart1() => 
        _pairs.Count(p => p.Item1.Contains(p.Item2) || p.Item2.Contains(p.Item1)).ToString();

    protected override string SolvePart2() => _pairs.Count(p => p.Item1.Overlaps(p.Item2)).ToString();

    private record struct Range(int Min, int Max)
    {
        public static Range Parse(string candidate)
        {
            var parts = candidate.Split('-');
            return new Range(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public bool Contains(Range other) => Min <= other.Min && Max >= other.Max;

        public bool Contains(int item) => Min <= item && item <= Max;

        public bool Overlaps(Range other) =>
            Contains(other.Min) || Contains(other.Max) || other.Contains(Min) || other.Contains(Max);
    }
}
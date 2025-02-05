using System.Collections.Immutable;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public partial class SolverS(PuzzleInput input) : Solver
{
    private readonly TowelCatalog _towels = new(input.Lines[0].Split(',', StringSplitOptions.TrimEntries));
    private readonly IEnumerable<string> _designs = input.Lines.Skip(2);

    protected override Answer SolvePart1() => _designs.Count(_towels.IsPossible);

    protected override Answer SolvePart2() => _designs.Sum(_towels.CountArrangements);

    private class TowelCatalog(IEnumerable<string> towels)
    {
        private readonly IEnumerable<string> _towels = towels.ToImmutableArray();
        private readonly Dictionary<string, long> _cache = [];

        public bool IsPossible(string design)
        {
            var count = CountArrangements(design);
            Console.WriteLine($"{count}\t{design}");
            return count > 0;
        }

        public long CountArrangements(string design)
        {
            if (design == "") { return 1; }
            if (_cache.TryGetValue(design, out var count)) { return count; }
            return _cache[design] = _towels.Where(design.StartsWith).Sum(t => CountArrangements(design[t.Length..]));
        }
    }
}

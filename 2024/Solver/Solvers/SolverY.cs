using AdventOfCode.Helpers;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverY : Solver
{
    private readonly IEnumerable<Key> _keys;
    private readonly IEnumerable<Lock> _locks;

    public SolverY(PuzzleInput input)
    {
        List<Key> keys = [];
        List<Lock> locks = [];

        foreach (var schematic in input.Lines.Split("").Select(Schematic.Parse))
        {
            switch (schematic)
            {
                case Key key: keys.Add(key); break;
                case Lock @lock: locks.Add(@lock); break;
            }
        }

        _keys = keys;
        _locks = locks;
    }

    protected override Answer SolvePart1() => _keys.Sum(k => _locks.Count(l => k.Fits(l)));

    protected override Answer SolvePart2()
    {
        throw new NotImplementedException();
    }

    private class Schematic(IEnumerable<int> heights)
    {
        public IEnumerable<int> Heights { get; } = heights;

        public static Schematic Parse(IEnumerable<string> lines)
        {
            var firstLine = lines.First();

            return firstLine[0] == '#' ? new Lock(GetHeights()) : new Key(GetHeights());

            IEnumerable<int> GetHeights()
                => lines
                    .Aggregate(
                        Enumerable.Repeat(-1, firstLine.Length),
                        (h, l) => h.Zip(l, (h, ch) => h + (ch == '#' ? 1 : 0)).ToArray());
        }

        public override string ToString() => string.Join(",", Heights);
    }

    private class Key(IEnumerable<int> heights) : Schematic(heights)
    {
        public bool Fits(Lock l) => Heights.Zip(l.Heights, (k, l) => k + l).All(t => t < 6);
    }

    private class Lock(IEnumerable<int> heights) : Schematic(heights)
    {

    }
}
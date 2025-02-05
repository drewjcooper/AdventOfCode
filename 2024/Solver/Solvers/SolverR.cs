using System.Collections.Immutable;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverR(PuzzleInput input) : Solver
{
    private readonly Grid _grid = new(input.Lines.Length > 100 ? 70 : 6);
    private readonly int _byteCount = input.Lines.Length > 100 ? 1024 : 12;
    private readonly ImmutableArray<Location> _bytes = input.Lines.Select(Location.Parse).ToImmutableArray();

    protected override Answer SolvePart1() => _grid.Add(_bytes.Take(_byteCount)).FindPath();

    protected override Answer SolvePart2() => _grid.FindBlocker(_bytes, _byteCount, input.Lines.Length).ToString();

    private class Grid(int size)
    {
        private readonly int _max = size;
        private readonly HashSet<Location> _bytes = [];

        public Grid Add(IEnumerable<Location> locations)
        {
            foreach (var location in locations)
            {
                _bytes.Add(location);
            }

            return this;
        }

        public int FindPath()
        {
            var locations = new[] { new Location(0, 0) };
            var target = new Location(_max, _max);
            var visited = new HashSet<Location>();
            var steps = 0;
            var pathFound = false;

            while (!pathFound && locations.Length > 0)
            {
                locations = locations.SelectMany(l => l.GetNeighbours())
                    .Where(l => l.X >= 0 && l.X <= _max && l.Y >= 0 && l.Y <= _max)
                    .Where(l => !_bytes.Contains(l) && !visited.Contains(l))
                    .Distinct()
                    .ToArray();
                steps++;
                foreach (var location in locations)
                {
                    if (location == target) { pathFound = true; break; }
                    visited.Add(location);
                }
            }

            return pathFound ? steps : -1;
        }

        public Location FindBlocker(ImmutableArray<Location> bytes, int min, int max)
        {
            var byteCount = (min + max) / 2;
            _bytes.Clear();
            var pathLength = Add(bytes.Take(byteCount)).FindPath();
            return (pathLength, max - min) switch
            {
                (-1, <=1) => bytes[min - 1],
                (-1, >1) => FindBlocker(bytes, min, byteCount),
                (>0, <=1) => bytes[max - 1],
                (>0, >1) => FindBlocker(bytes, byteCount, max),
                _ => throw new InvalidOperationException()
            };
        }
    }

    private record struct Location(int X, int Y)
    {
        public static Location Parse(string line)
        {
            var parts = line.Split(",");
            return new(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public readonly IEnumerable<Location> GetNeighbours()
        {
            yield return new(X + 1, Y);
            yield return new(X - 1, Y);
            yield return new(X, Y + 1);
            yield return new(X, Y - 1);
        }

        public override readonly string ToString() => $"{X},{Y}";
    }
}

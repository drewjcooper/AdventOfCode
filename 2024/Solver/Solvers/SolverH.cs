using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal class SolverH(PuzzleInput input) : Solver
{
    private readonly string[] _lines = input.Lines;

    protected override Answer SolvePart1() => new Map(_lines).AntinodeCount;

    protected override Answer SolvePart2() => new ResonantMap(_lines).AntinodeCount;

    private record Location(int X, int Y);

    private class Map
    {
        private readonly int height;
        private readonly int width;
        private readonly Dictionary<char, List<Location>> _antennae = new();
        private readonly HashSet<Location> _antinodes = new();

        public Map(string[] lines)
        {
            height = lines.Length;
            width = lines[0].Length;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var ch = lines[y][x];
                    if (ch == '.') { continue; }

                    var location = new Location(x, y);

                    if (!_antennae.TryGetValue(ch, out var locations))
                    {
                        _antennae[ch] = locations = [];
                    }
                    else
                    {
                        AddAntinodes(location, locations);
                    }
                    locations.Add(location);
                }
            }
        }

        public int AntinodeCount => _antinodes.Count;

        private void AddAntinodes(Location location, IEnumerable<Location> antennae)
        {
            foreach (var antenna in antennae)
            {
                AddAntinodes(location, antenna);
            }
        }

        protected virtual void AddAntinodes(Location newAntenna, Location existingAntenna)
        {
            AddAntinode(newAntenna, existingAntenna);
            AddAntinode(existingAntenna, newAntenna);
        }

        private void AddAntinode(Location a, Location b)
        {
            AddAntinode(new(2 * b.X - a.X, 2 * b.Y - a.Y));
        }

        protected bool AddAntinode(Location antinode)
        {
            if (antinode.X >= 0 && antinode.X < width && antinode.Y >= 0 && antinode.Y < height)
            {
                _antinodes.Add(antinode);
                return true;
            }

            return false;
        }
    }

    private class ResonantMap(string[] lines) : Map(lines)
    {
        protected override void AddAntinodes(Location newAntenna, Location existingAntenna)
        {
            AddAntinode(newAntenna);
            AddAntinode(existingAntenna);

            foreach (var antinode in FindAntinodes(newAntenna, existingAntenna))
            {
                if (!AddAntinode(antinode)) { break; }
            }

            foreach (var antinode in FindAntinodes(existingAntenna, newAntenna))
            {
                if (!AddAntinode(antinode)) { break; }
            }
        }

        private static IEnumerable<Location> FindAntinodes(Location from, Location to)
        {
            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            var x = to.X;
            var y = to.Y;

            while (true)
            {
                x += dx;
                y += dy;
                yield return new(x, y);
            }
        }
    }
}

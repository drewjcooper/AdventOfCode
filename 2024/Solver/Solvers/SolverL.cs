using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverL(PuzzleInput input) : Solver
{
    private readonly Map _map = new(input.Lines);

    protected override Answer SolvePart1() => _map.GetRegions().Sum(r => r.Price);

    protected override Answer SolvePart2() => _map.GetRegions().Sum(r => r.DiscountPrice);

    private record struct Location(int X, int Y)
    {
        public IEnumerable<Location> Neighbours
        {
            get
            {
                yield return new(X - 1, Y);
                yield return new(X, Y - 1);
                yield return new(X + 1, Y);
                yield return new(X, Y + 1);
            }
        }
    }

    private record struct Edge(Location From, Location To);

    private class Map
    {
        private readonly Dictionary<Location, char> _unallocated = [];

        public Map(string[] lines)
        {
            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[0].Length; x++)
                {
                    _unallocated[new(x, y)] = lines[y][x];
                }
            }
        }

        public char this[Location location] => _unallocated[location];

        public IEnumerable<Region> GetRegions()
        {
            while (_unallocated.Count > 0)
            {
                if (Region.Find(this) is { } region)
                {
                    yield return region;
                }
            }
        }

        public bool TryGetUnallocated(out Location location)
        {
            location = default;
            if (_unallocated.Count == 0) { return false; }

            location = _unallocated.Keys.First();
            return true;
        }

        public bool TryAllocate(Location location, Region region)
        {
            if (IsInSameRegion(region, location))
            {
                _unallocated.Remove(location);
                return true;
            }

            return false;
        }

        private bool IsInSameRegion(Region region, Location plot)
            => region.Contains(plot) || region.Type == _unallocated.GetValueOrDefault(plot);

        internal void Remove(Location plot) => _unallocated.Remove(plot);
    }

    private class Region(Location plot, char type)
    {
        private readonly HashSet<Location> _plots = [ plot ];
        private int _edges;
        private readonly List<Side> _sides = [];

        public char Type { get; } = type;

        public bool Contains(Location location) => _plots.Contains(location);

        public long Price => _edges * _plots.Count;

        public long DiscountPrice => _sides.Count * _plots.Count;

        public static Region? Find(Map map)
        {
            if (!map.TryGetUnallocated(out var plot))
            {
                return null;
            }

            var region = new Region(plot, map[plot]);
            map.Remove(plot);


            region.Grow(map, plot);

            return region;
        }

        private void Grow(Map map, Location plot)
        {
            foreach (Location neighbour in plot.Neighbours)
            {
                if (map.TryAllocate(neighbour, this))
                {
                    if (_plots.Add(neighbour))
                    {
                        Grow(map, neighbour);
                    }
                }
                else
                {
                    _edges++;
                    AddSide(Side.Between(plot, neighbour));
                }
            }
        }

        public void AddSide(Side side)
        {
            var m = -1;
            for (int i = 0; i < _sides.Count; i++)
            {
                if (m < 0 && _sides[i].TryMerge(side, out var merged))
                {
                    m = i;
                    _sides[m] = merged;
                    continue;
                }

                if (m >= 0 && _sides[m].TryMerge(_sides[i], out merged))
                {
                    _sides[m] = merged;
                    _sides.RemoveAt(i);
                    i--;
                }
            }

            if (m == -1) { _sides.Add(side); }
        }
    }

    private record Side(Location Start, Location End)
    {
        private bool IsHorizontal => Start.Y == End.Y;

        public bool TryMerge(Side other, [NotNullWhen(true)] out Side? merged)
        {
            if (IsHorizontal != other.IsHorizontal)
            {
                merged = default;
                return false;
            }

            if (Start == other.End)
            {
                merged = new(other.Start, End);
                return true;
            }

            if (End == other.Start)
            {
                merged = new(Start, other.End);
                return true;
            }

            merged = default;
            return false;
        }

        public static Side Between(Location a, Location b) 
            => (a.X - b.X, a.Y - b.Y) switch
            {
                (1, 0) => new(a, new(a.X, a.Y + 1)),
                (0, 1) => new(a, new(a.X + 1, a.Y)),
                (-1, 0) => new(new(a.X + 1, a.Y + 1), new(a.X + 1, a.Y)),
                (0, -1) => new(new(a.X + 1, a.Y + 1), new(a.X, a.Y + 1)),
                _ => throw new ArgumentException("Invalid side")
            };
    }
}

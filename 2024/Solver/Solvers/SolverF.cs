using System.Collections.Immutable;
using AdventOfCode.Input;
using Location = (int X, int Y);

namespace AdventOfCode.Solvers;

internal partial class SolverF(PuzzleInput input) : Solver
{
    private readonly Map _map = Map.Parse(input.Lines);

    protected override Answer SolvePart1() =>_map.GetRoute().DistinctBy(g => g.Location).Count();

    protected override Answer SolvePart2()
        => _map.GetRoute()
            .Select(g => g.NextLocation)
            .Distinct()
            .Select(_map.AddObstacle)
            .Count(m => m?.HasLoop() ?? false);

    private class Map
    {
        private readonly ImmutableHashSet<Location> _obstacles;
        private readonly Guard _guard;
        private readonly Floor _floor;

        private Map(Location start, Floor floor, IEnumerable<Location> obstacles)
        {
            _obstacles = obstacles.ToImmutableHashSet();
            _guard = new(start);
            _floor = floor;
        }

        public static Map Parse(string[] lines)
        {
            var height = lines.Length;
            var width = lines[0].Length;
            var obstacles = new List<Location>();
            var startLocation = default(Location);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        obstacles.Add(new(x, y));
                    }
                    if (lines[y][x] == '^')
                    {
                        startLocation = new(x, y);
                    }
                }
            }

            return new(startLocation, new(height, width), obstacles);
        }

        public Map? AddObstacle(Location location)
            => _floor.Contains(location) && location != _guard.Location && !_obstacles.Contains(location)
                ? new(_guard.Location, _floor, _obstacles.Append(location))
                : null;

        public IEnumerable<Guard> GetRoute()
        {
            var guard = _guard;

            while (_floor.Contains(guard.Location))
            {
                yield return guard;
                guard = guard.IsBlocked(_obstacles) ? guard.TurnRight() : guard.Move();
            }
        }

        public bool HasLoop()
        {
            var positions = new HashSet<Guard>();
            foreach (var position in GetRoute())
            {
                if (!positions.Add(position))
                {
                    return true;
                }
            }
            return false;
        }

        private class Floor(int height, int width)
        {
            private readonly int _height = height;
            private readonly int _width = width;

            public bool Contains(Location location) 
                => location.X >= 0 && location.X < _width && location.Y >= 0 && location.Y < _height;
        }
    }

    private record Direction(int X, int Y)
    {
        public static readonly Direction Up = new(0, -1);

        public Direction TurnRight() => new(-Y, X);
    }

    private record Guard(Location Location, Direction Direction)
    {
        public Guard(Location location) : this(location, Direction.Up)
        {
        }

        public Location NextLocation => (Location.X + Direction.X, Location.Y + Direction.Y);

        public Guard TurnRight() => this with { Direction = Direction.TurnRight() };

        public Guard Move() => this with { Location = NextLocation };

        public bool IsBlocked(ImmutableHashSet<Location> obstacles) => obstacles.Contains(NextLocation);
    }
}

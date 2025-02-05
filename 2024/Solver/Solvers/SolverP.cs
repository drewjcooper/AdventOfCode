using System.Collections.Immutable;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverP(PuzzleInput input) : Solver
{
    private readonly Maze _maze = Maze.Parse(input.Lines);

    protected override Answer SolvePart1() => _maze.Navigate().Score;

    protected override Answer SolvePart2() => _maze.Navigate().GetVisited().Distinct().Count();

    private class Maze
    {
        private readonly ImmutableHashSet<Location> _walls;
        private readonly Location _end;
        private readonly Dictionary<Position, Reindeer> _visited;
        private IEnumerable<Reindeer> _reindeer;

        public Maze(Location start, Location end, IEnumerable<Location> walls)
        {
            _walls = walls.ToImmutableHashSet();
            _end = end;
            var reindeer = new Reindeer(new(start, Direction.East));
            _visited = new() { [new(start, Direction.East)] = reindeer };
            _reindeer = [reindeer];
        }

        public Reindeer Navigate()
        {
            while (_reindeer.Any())
            {
                _reindeer = _reindeer
                    .SelectMany(r => r.GetMoves(this))
                    .GroupBy(r => r.Position, (k, g) => g.Where(r => r.Score == g.Min(r => r.Score)))
                    .Select(g => g.Aggregate(Reindeer.None, (a, r) => r.Merge(a)))
                    .ToList();
                MarkVisited();
            }

            return _visited.Where(kv => kv.Key.Location == _end)
                .Select(kv => kv.Value)
                .MinBy(r => r.Score)!;
        }

        private void MarkVisited()
        {
            foreach (var reindeer in _reindeer)
            {
                _visited[reindeer.Position] = reindeer;
            }
        }

        public bool IsWall(Position position) => _walls.Contains(position.Location);

        public int GetCurrentScore(Position position) => _visited.GetValueOrDefault(position)?.Score ?? int.MaxValue;

        public void AddPrevious(Reindeer reindeer) => _visited[reindeer.Position].AddPrevious(reindeer);

        public static Maze Parse(string[] lines)
        {
            Location start = default, end = default;
            List<Location> walls = [];

            for (int x = 0; x < lines[0].Length; x++)
            {
                for (int y = 0; y < lines.Length; y++)
                {
                    if (lines[y][x] == '#') { walls.Add(new(x, y)); continue; }
                    if (lines[y][x] == 'S') { start = new(x, y); continue; }
                    if (lines[y][x] == 'E') { end = new(x, y); continue; }
                }
            }

            return new(start, end, walls);
        }
    }

    private record Reindeer(Position Position, int Score)
    {
        public static readonly Reindeer None = new(default, 0);

        private readonly List<Reindeer> _previous = [];

        public Reindeer(Position position) : this(position, Score: 0)
        {
        }

        public Reindeer(Position position, int score, Reindeer previous) : this(position, score)
            => _previous.Add(previous);

        public bool IsOnPossiblePath(Maze maze)
        {
            if (maze.IsWall(Position)) { return false; }
            var score = maze.GetCurrentScore(Position);

            if (score == Score) { maze.AddPrevious(this); return false; }
            return score > Score;
        }

        public Reindeer TurnLeft() => new(Position.Anticlockwise(), Score + 1000, this);
        public Reindeer Move() => new(Position.Forward(), Score + 1, this);
        public Reindeer TurnRight() => new(Position.Clockwise(), Score + 1000, this);

        public IEnumerable<Reindeer> GetMoves(Maze maze)
            => new[] { TurnLeft(), Move(), TurnRight() }
                .Where(r => r.IsOnPossiblePath(maze));

        public void AddPrevious(Reindeer reindeer) => _previous.Add(reindeer);

        public IEnumerable<Location> GetVisited() 
            => _previous.SelectMany(r => r.GetVisited()).Append(Position.Location);

        public Reindeer Merge(Reindeer other)
        {
            if (other == None) { return this; }

            if (Position != other.Position || Score != other.Score)
            {
                throw new InvalidOperationException("Can't merge unequal reindeer");
            }

            foreach (var previous in _previous)
            {
                other.AddPrevious(previous);
            }

            return other;
        }
    }

    private record struct Position(Location Location, Direction Direction)
    {
        public readonly Position Anticlockwise() => new(Location, Direction.Anticlockwise);
        public readonly Position Forward() => new(Location.Neighbour(Direction), Direction);
        public readonly Position Clockwise() => new(Location, Direction.Clockwise);
    }

    private record struct Location(int X, int Y)
    {
        public readonly Location Neighbour(Direction direction) => direction.NeighbourOf(this);
    }

    private readonly struct Direction(int dx, int dy)
    {
        public static readonly Direction North = new(0, -1);
        public static readonly Direction East = new(1, 0);
        public static readonly Direction South = new(0, 1);
        public static readonly Direction West = new(-1, 0);

        private readonly int _dx = dx;
        private readonly int _dy = dy;

        public readonly Direction Clockwise => new(-_dy, _dx);

        public readonly Direction Anticlockwise => new(_dy, -_dx);

        public readonly Location NeighbourOf(Location location) => new(location.X + _dx, location.Y + _dy);

        public override readonly string ToString()
            => (_dx, _dy) switch
            {
                (0, -1) => "North",
                (1, 0) => "East",
                (0, 1) => "South",
                (-1, 0) => "West",
                _ => throw new Exception("Invalid direction.")
            };
    }
}

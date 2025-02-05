using System.Collections;
using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverO : Solver
{
    private readonly Warehouse _warehouse;
    private readonly Robot _robot;
    private readonly IEnumerable<Direction> _moves;

    public SolverO(PuzzleInput input)
    {
        var lines = input.Break();

        _warehouse = Warehouse.Parse(lines, out _robot);

        _moves = lines.SelectMany(l => l).Where(ch => !char.IsWhiteSpace(ch)).Select(Direction.Parse);
    }

    protected override Answer SolvePart1() => _robot.Execute(_warehouse, _moves).Sum(b => b.GPS);

    protected override Answer SolvePart2() => _robot.Execute(_warehouse.Widen(), _moves).Sum(b => b.GPS);

    private class Warehouse(IEnumerable<Obstacle> obstacles) : IEnumerable<Box>
    {
        private readonly Dictionary<Location, Obstacle> _obstacles = obstacles.ToDictionary(x => x.Location, x => x);

        public bool TryGetObstacle(Location location, [NotNullWhen(true)] out Obstacle? obstacle)
            => _obstacles.TryGetValue(location, out obstacle);

        public bool TryMove(Obstacle start, Direction direction)
        {
            var destination = direction.GetDestination(start);

            if (_obstacles.TryGetValue(destination, out var obstacle) &&
                !obstacle.TryMove(this, direction))
            {
                return false;
            }

            _obstacles.Remove(start);
            var to = start with { Location = destination };
            _obstacles[to] = to;
            return true;
        }

        public static Warehouse Parse(IEnumerable<string> lines, out Robot robot)
        {
            Location robotLocation = new(0, 0);

            var warehouse = new Warehouse(
                lines.Select((l, y) => l.Select((c, x) => Parse(c, new(x, y))))
                    .SelectMany(o => o)
                    .Where(o => o != null)
                    .Select(o => o!));

            robot = new(robotLocation);

            return warehouse;

            Obstacle? Parse(char c, Location location)
            {
                if (Obstacle.TryParse(c, location, out var obstacle)) { return obstacle; }
                if (Robot.Is(c)) { robotLocation = location; }
                return null;
            }
        }

        public WideWarehouse Widen() => new(_obstacles.Values.Select(o => o.Widen()));

        public IEnumerator<Box> GetEnumerator() => _obstacles.Values.OfType<Box>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class WideWarehouse : IEnumerable<WideBox>
    {
        private readonly Dictionary<Location, WideObstacle> _obstacles = [];
        private readonly char[][] _display;

        public WideWarehouse(IEnumerable<WideObstacle> obstacles)
        {
            var maxX = 0;
            var maxY = 0;

            foreach (var obstacle in obstacles)
            {
                _obstacles[obstacle.Location1] = obstacle;
                _obstacles[obstacle.Location2] = obstacle;

                maxX = Math.Max(maxX, obstacle.Location2.X);
                maxY = Math.Max(maxY, obstacle.Location2.Y);
            }

            _display = Enumerable.Range(0, maxY + 1)
                .Select(_ => Enumerable.Range(0, maxX + 1).Select(_ => ' ').ToArray())
                .ToArray();
        }

        public bool TryGetObstacle(Location location, [NotNullWhen(true)] out WideObstacle? obstacle)
            => _obstacles.TryGetValue(location, out obstacle);

        public bool CanMove(WideObstacle start, Direction direction)
        {
            var move = direction.GetMove(start);

            return (!_obstacles.TryGetValue(move.First.To, out var obstacle) || obstacle.CanMove(this, direction))
                && (!_obstacles.TryGetValue(move.Second.To, out obstacle) ||
                    obstacle == start ||
                    obstacle.CanMove(this, direction));
        }

        public void Move(WideObstacle start, Direction direction)
        {
            if (!CanMove(start, direction))
            {
                throw new InvalidOperationException("Object cannot be moved");
            }

            var move = direction.GetMove(start);

            if (_obstacles.TryGetValue(move.First.To, out var obstacle))
            {
                obstacle.Move(this, direction);
            }
            
            if (_obstacles.TryGetValue(move.Second.To, out obstacle) && obstacle != start)
            {
                obstacle.Move(this, direction);
            }

            _obstacles.Remove(start.Location1);
            _obstacles.Remove(start.Location2);
            var to = start with { Location1 = direction.GetDestination(start.Location1) };
            _obstacles[to.Location1] = to;
            _obstacles[to.Location2] = to;
        }

        public IEnumerator<WideBox> GetEnumerator() => _obstacles.Values.OfType<WideBox>().Distinct().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public string ToString(Location location, char robot = '@')
        {
            for (var x = 0; x < _display[0].Length; x++)
            {
                for (var y = 0; y < _display.Length; y++)
                {
                    _display[y][x] = ' ';
                }
            }

            foreach (var obstacle in _obstacles.Values.Distinct())
            {
                _display[obstacle.Location1.Y][obstacle.Location1.X] = obstacle.Char1;
                _display[obstacle.Location2.Y][obstacle.Location2.X] = obstacle.Char2;
            }

            _display[location.Y][location.X] = robot;

            return string.Join(Environment.NewLine, _display.Select(r => new string(r)));
        }
    }

    private class Robot(Location location)
    {
        private Location _location = location;

        public static bool Is(char ch) => ch == '@';

        public Warehouse Execute(Warehouse warehouse, IEnumerable<Direction> moves)
        {
            foreach (var direction in moves)
            {
                _location = Execute(warehouse, direction);
            }

            return warehouse;
        }

        private Location Execute(Warehouse warehouse, Direction direction)
        {
            var destination = direction.GetDestination(_location);
            if (warehouse.TryGetObstacle(destination, out var obstacle))
            {
                return obstacle.TryMove(warehouse, direction) ? destination : _location;
            }

            return destination;
        }

        public WideWarehouse Execute(WideWarehouse warehouse, IEnumerable<Direction> moves)
        {
            _location = _location.Widen();

            foreach (var direction in moves)
            {
                _location = Execute(warehouse, direction);
            }

            return warehouse;
        }

        private Location Execute(WideWarehouse warehouse, Direction direction)
        {
            var destination = direction.GetDestination(_location);
            if (warehouse.TryGetObstacle(destination, out var obstacle))
            {
                if (obstacle.CanMove(warehouse, direction))
                {
                    obstacle.Move(warehouse, direction);
                    return destination;
                }
                
                return _location;
            }

            return destination;
        }
    }

    private abstract record Obstacle(Location Location)
    {
        public abstract bool TryMove(Warehouse warehouse, Direction direction);

        public abstract WideObstacle Widen();

        public static implicit operator Location(Obstacle obstacle) { return obstacle.Location; }

        public static bool TryParse(char c, Location location, out Obstacle? obstacle)
        {
            obstacle = c switch
            {
                '#' => new Wall(location),
                'O' => new Box(location),
                _ => null
            };
            return obstacle != null;
        }
    }

    private record Wall(Location Location) : Obstacle(Location)
    {
        public override bool TryMove(Warehouse warehouse, Direction direction) => false;

        public override WideObstacle Widen() => new WideWall(Location.Widen());
    }

    private record Box(Location Location) : Obstacle(Location)
    {
        public override bool TryMove(Warehouse warehouse, Direction direction) => warehouse.TryMove(this, direction);

        public int GPS => Location.X + 100 * Location.Y;

        public override WideObstacle Widen() => new WideBox(Location.Widen());
    }

    private abstract record WideObstacle(Location Location1)
    {
        public Location Location2 => Location1.Next;

        public abstract char Char1 { get; }
        public abstract char Char2 { get; }

        public abstract bool CanMove(WideWarehouse warehouse, Direction direction);
        public abstract void Move(WideWarehouse warehouse, Direction direction);
    }

    private record WideWall(Location Location1) : WideObstacle(Location1)
    {
        public override bool CanMove(WideWarehouse warehouse, Direction direction) => false;
        public override void Move(WideWarehouse warehouse, Direction direction) 
            => throw new InvalidOperationException("Can't move a wall");

        public override char Char1 { get; } = '#';
        public override char Char2 { get; } = '#';
    }

    private record WideBox(Location Location1) : WideObstacle(Location1)
    {
        public override bool CanMove(WideWarehouse warehouse, Direction direction)
            => warehouse.CanMove(this, direction);

        public override void Move(WideWarehouse warehouse, Direction direction)
            => warehouse.Move(this, direction);

        public override char Char1 { get; } = '[';
        public override char Char2 { get; } = ']';

        public int GPS => Location1.GPS;
    }

    private record Location(int X, int Y)
    {
        public int GPS => X + 100 * Y;
        public Location Next => new(X + 1, Y);
        public Location Widen() => new(X * 2, Y);
    }

    private record Move(Location From, Location To);

    private record WideMove(Move First, Move Second);

    private class Direction(int dx, int dy, char ch)
    {
        public static readonly Direction Up = new(0, -1, '^');
        public static readonly Direction Down = new(0, 1, 'v');
        public static readonly Direction Left = new(-1, 0, '<');
        public static readonly Direction Right = new(1, 0, '>');

        private readonly int _dx = dx;
        private readonly int _dy = dy;
        private readonly char _ch = ch;

        public Location GetDestination(Location from) => new(from.X + _dx, from.Y + _dy);

        public WideMove GetMove(WideObstacle obstacle)
        {
            var leftMove = GetMove(obstacle.Location1);
            var rightMove = GetMove(obstacle.Location2);

            return this == Right ? new(rightMove, leftMove) : new(leftMove, rightMove);
        }

        public Move GetMove(Location from) => new(from, GetDestination(from));

        public static Direction Parse(char ch) 
            => ch switch
            {
                '^' => Up,
                'v' => Down,
                '<' => Left,
                '>' => Right,
                _ => throw new ArgumentException($"Invalid direction: {ch}.")
            };

        public static implicit operator char(Direction direction) => direction._ch;
    }
}

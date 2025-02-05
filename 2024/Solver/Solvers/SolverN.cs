using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverN(PuzzleInput input) : Solver
{
    private readonly Robot[] _robots = input.Lines.Select(Robot.Parse).ToArray();

    protected override Answer SolvePart1() => new Floor(_robots).PassTime(100).CalculateSafetyFactor();

    protected override Answer SolvePart2() => new Floor(_robots).Animate(7400, 7424);

    private class Floor
    {
        private readonly Robot[] _robots;
        private readonly int _width;
        private readonly int _height;

        public Floor(IEnumerable<Robot> robots)
        {
            _robots = robots.ToArray();
            (_width, _height) = _robots.Length > 12 ? (101, 103) : (11, 7);
        }

        public Floor PassTime(int seconds = 1)
        {
            for (int i = 0; i < _robots.Length; i++)
            {
                _robots[i] = seconds == 1 ? _robots[i].Step(_width, _height) : _robots[i].Move(seconds, _width, _height);
            }
            return this;
        }

        public long CalculateSafetyFactor()
            => _robots
                .Select(r => r.Quadrant(_width / 2, _height / 2))
                .Where(q => q != 0)
                .GroupBy(q => q, (k, g) => g.Count())
                .Aggregate(1L, (a, x) => a * x);

        public int Animate(int from, int to)
        {
            if (from > 0) { PassTime(from); }

            while (from <= to)
            {
                Console.Clear();
                Console.WriteLine($"{from}:");
                Console.WriteLine(this);
                PassTime();
                from++;
                Thread.Sleep(100);
            }

            return 0;
        }

        public override string ToString()
        {
            var display = Enumerable.Range(0, _height)
                .Select(_ => Enumerable.Range(0, _width).Select(_ => ' ').ToArray())
                .ToArray();
            for (int i = 0; i < _robots.Length; i++)
            {
                display[_robots[i].Y][_robots[i].X] = '*';
            }
            return string.Join(Environment.NewLine, display.Select(l => new string(l)));
        }
    }

    private record Robot(int X, int Y, int DX, int DY)
    {
        public Robot Move(int seconds, int width, int height)
            => new(Move(X, DX, width, seconds), Move(Y, DY, height, seconds), DX, DY);

        public Robot Step(int width, int height) => new(Step(X, DX, width), Step(Y, DY, height), DX, DY);

        private static int Move(int v, int dv, int limit, int seconds) => ((v + dv * seconds) % limit + limit) % limit;

        private static int Step(int v, int dv, int limit) => ((v + dv) % limit + limit) % limit;

        public static Robot Parse(string input)
        {
            var parts = input.Split([' ', '=', ',']);
            return new Robot(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[4]), int.Parse(parts[5]));
        }

        public int Quadrant(int x, int y)
            => (X, Y) switch
            {
                _ when X < x && Y < y => 1,
                _ when X > x && Y < y => 2,
                _ when X < x && Y > y => 3,
                _ when X > x && Y > y => 4,
                _ => 0
            };
    }
}

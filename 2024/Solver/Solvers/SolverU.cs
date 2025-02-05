using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverU(PuzzleInput input) : Solver
{
    private readonly IEnumerable<string> _codes = input.Lines;

    protected override Answer SolvePart1() => CalculateComplexitySum(2);

    protected override Answer SolvePart2() => CalculateComplexitySum(25);

    internal static Robot CreateRobotChain(int robotCount)
        => new(
            Keypad.Numeric,
            Enumerable
                .Range(0, robotCount - 1)
                .Aggregate(
                    new Robot(Keypad.Directional), 
                    (r, _) => new Robot(Keypad.Directional, r)));

    private long CalculateComplexitySum(int robotCount)
    {
        var robots = CreateRobotChain(robotCount);
        return _codes.Sum(c => robots.GetButtonCount(c) * int.Parse(c[..^1]));
    }

    internal static long GetButtonCount(string code, Robot robots)
        => robots.GetButtonCount(code);

    internal class Robot(Keypad keypad)
    {
        public static readonly Robot Prime = new(new NumericKeypad());
        public static readonly Robot Intermediate = new(new DirectionalKeypad());

        private readonly Keypad _keypad = keypad;
        private readonly Dictionary<(char, char), long> _lengthCache = [];
        private readonly Robot? _robot;

        public Robot(Keypad keypad, Robot robot) : this(keypad) => _robot = robot;

        public long GetButtonCount(string code)
            => code.Prepend('A')
                .Zip(code)
                .Select(move =>
                    _lengthCache.TryGetValue(move, out var length)
                        ? length
                        : _lengthCache[move] = _robot == null
                            ? MoveTo(move.First, move.Second).Length
                            : _robot.GetButtonCount(MoveTo(move.First, move.Second)))
                .Sum();

        private readonly Dictionary<(char, char), string[]> _movesCache = new();

        public IEnumerable<string> GetButtonPresses(string code)
            => code.Prepend('A')
                .Zip(code)
                .Select(move =>
                    _movesCache.TryGetValue(move, out var buttons)
                        ? buttons
                        : _movesCache[move] = _robot == null
                            ? MovesTo(move.First, move.Second).ToArray()
                            : MovesTo(move.First, move.Second).SelectMany(c => _robot.GetButtonPresses(c)).ToArray())
                .Aggregate(new[] { "" }, (a, m) => a.SelectMany(a1 => m.Select(m1 => a1 + m1)).ToArray());

        public string MoveTo(char from, char to)
        {
            if (from == to) { return "A"; }

            var (deltaX, deltaY, xFirstSafe, yFirstSafe) = _keypad.GetDeltas(from, to);

            var xMoves = deltaX == 0 ? "" : new string(deltaX > 0 ? '>' : '<', Math.Abs(deltaX));
            var yMoves = deltaY == 0 ? "" : new string(deltaY > 0 ? '^' : 'v', Math.Abs(deltaY));

            if (xFirstSafe && deltaX < 0) { return $"{xMoves}{yMoves}A"; }
            if (yFirstSafe && deltaY < 0) { return $"{yMoves}{xMoves}A"; }

            if (yFirstSafe && deltaY != 0) { return $"{yMoves}{xMoves}A"; }
            if (xFirstSafe && deltaX != 0) { return $"{xMoves}{yMoves}A"; }

            throw new Exception("Should not get here!");
        }

        public IEnumerable<string> MovesTo(char from, char to)
        {
            if (from == to) { return ["A"]; }

            var (deltaX, deltaY, xFirstSafe, yFirstSafe) = _keypad.GetDeltas(from, to);

            var xMoves = deltaX == 0 ? "" : new string(deltaX > 0 ? '>' : '<', Math.Abs(deltaX));
            var yMoves = deltaY == 0 ? "" : new string(deltaY > 0 ? '^' : 'v', Math.Abs(deltaY));

            if (xFirstSafe && deltaX < 0) { return [$"{xMoves}{yMoves}A"]; }
            if (yFirstSafe && deltaY < 0) { return [$"{yMoves}{xMoves}A"]; }

            var moves = new List<string>();
            if (yFirstSafe && deltaY != 0) { moves.Add($"{yMoves}{xMoves}A"); }
            if (xFirstSafe && deltaX != 0) { moves.Add($"{xMoves}{yMoves}A"); }

            return moves.Take(1);
        }
    }

    internal abstract class Keypad
    {
        public static readonly Keypad Numeric = new NumericKeypad();
        public static readonly Keypad Directional = new DirectionalKeypad();

        protected abstract (int X, int Y) GetLocation(char button);

        public (int X, int Y, bool XFirstSafe, bool YFirstSafe) GetDeltas(char from, char to)
        {
            var (fromX, fromY) = GetLocation(from);
            var (toX, toY) = GetLocation(to);

            var deltaX = toX - fromX;
            var deltaY = toY - fromY;

            var xFirstSafe = fromY != 0 || fromX + deltaX != 0;
            var yFirstSafe = fromX != 0 || fromY + deltaY != 0;

            return (deltaX, deltaY, xFirstSafe, yFirstSafe);
        }
    }

    private class NumericKeypad : Keypad
    {
        protected override (int X, int Y) GetLocation(char button)
            => button switch
            {
                'A' => (2, 0),
                '0' => (1, 0),
                >= '1' and <= '9' => ((button - '1') % 3, (button - '1') / 3 + 1),
                _ => throw new ArgumentException($"Button {button} does not exist", nameof(button))
            };
    }

    private class DirectionalKeypad : Keypad
    {
        protected override (int X, int Y) GetLocation(char button)
            => button switch
            {
                'A' => (2, 0),
                '^' => (1, 0),
                '<' => (0, -1),
                'v' => (1, -1),
                '>' => (2, -1),
                _ => throw new ArgumentException($"Button {button} does not exist", nameof(button))
            };
    }
}

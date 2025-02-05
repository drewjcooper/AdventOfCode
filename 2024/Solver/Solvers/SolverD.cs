using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal partial class SolverD(PuzzleInput input) : Solver
{
    private readonly string[] _chars = input.Lines;

    protected override Answer SolvePart1() => new XmasSearch(_chars).CountMatches();

    protected override Answer SolvePart2() => new XMasSearch(_chars).CountMatches();

    private class XmasSearch(string[] chars)
    {
        public const string Target = "XMAS";
        private readonly string[] _chars = chars;

        private readonly IEnumerable<Scan> _scans =
        [
            new(0, -1, chars),
            new(1, -1, chars),
            new(1, 0, chars),
            new(1, 1, chars),
            new(0, 1, chars),
            new(-1, 1, chars),
            new(-1, 0, chars),
            new(-1, -1, chars)
        ];

        public int CountMatches()
        {
            var found = 0;
            for (int x = 0; x < _chars[0].Length; x++)
            {
                for (int y = 0; y < _chars.Length; y++)
                {
                    found += _chars[y][x] == Target[0] ? _scans.Count(scan => scan.IsSuccess(x, y)) : 0;
                }
            }

            return found;
        }

        private class Scan(int x, int y, string[] chars)
        {
            private readonly int _xDirection = x;
            private readonly int _yDirection = y;
            private readonly string[] _chars = chars;

            public bool IsSuccess(int x, int y)
            {
                if (x + _xDirection * 3 < 0 || x + _xDirection * 3 >= _chars[0].Length ||
                    y + _yDirection * 3 < 0 || y + _yDirection * 3 >= _chars.Length)
                {
                    return false;
                }

                for (var i = 1; i <= 3; i++)
                {
                    if (_chars[y + _yDirection * i][x + _xDirection * i] != XmasSearch.Target[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }

    private class XMasSearch(string[] chars)
    {
        public const string Target = "MAS";
        private readonly string[] _chars = chars;
        private readonly IEnumerable<Scan> _scans = [new(1, chars), new(-1, chars)];

        public int CountMatches()
        {
            var found = 0;
            for (int x = 1; x < _chars[0].Length - 1; x++)
            {
                for (int y = 1; y < _chars.Length - 1; y++)
                {
                    found += _chars[y][x] == Target[1] && _scans.All(scan => scan.IsMatch(x, y)) ? 1 : 0;
                }
            }

            return found;
        }

        private class Scan(int y, string[] chars)
        {
            private readonly int _xDirection = 1;
            private readonly int _yDirection = y;
            private readonly string[] _chars = chars;

            public bool IsMatch(int x, int y)
                => _chars[y + _yDirection][x + _xDirection] == Target[0] &&
                   _chars[y - _yDirection][x - _xDirection] == Target[2] ||
                   _chars[y + _yDirection][x + _xDirection] == Target[2] &&
                   _chars[y - _yDirection][x - _xDirection] == Target[0];
        }
    }
}

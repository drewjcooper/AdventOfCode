using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal class SolverB(PuzzleInput input) : Solver
{
    protected override Answer SolvePart1() => input.Lines.Select(Report.Parse).Count(r => r.IsSafe);

    protected override Answer SolvePart2() => input.Lines.Select(Report.Parse).Count(r => r.IsSafe || r.IsTolerablySafe());

    private class Report
    {
        private readonly int[] _values;
        private readonly int[] _deltas;
        private readonly int _initialSign;
        private readonly int _firstBadIndex;

        private Report(int[] values)
        {
            _values = values;

            var diffCount = values.Length - 1;
            _deltas = new int[diffCount];

            //var diff = Math.Abs(values[^1] - values[0]);
            //if (diff < diffCount || diff > 3 * diffCount)
            //{
            //    IsSafe = false;
            //    return;
            //}

            var initialDiff = values[1] - values[0];
            _initialSign = initialDiff == 0 ? 1 : initialDiff / Math.Abs(initialDiff);

            for (int i = 0; i < diffCount; i++)
            {
                _deltas[i] = (values[i + 1] - values[i]) * _initialSign;
                if (_deltas[i] is < 1 or > 3)
                {
                    IsSafe = false;
                    _firstBadIndex = i;
                    return;
                }    
            }

            IsSafe = true;
        }

        public bool IsSafe { get; }

        public static Report Parse(string line) => new([.. line.Split(' ').Select(int.Parse)]);

        public bool IsTolerablySafe()
        {
            for (var offset = Math.Max(-_firstBadIndex, -2); offset <= 1; offset++)
            {
                if (new Report([.. _values[..(_firstBadIndex + offset)], .. _values[(_firstBadIndex + offset + 1)..]]).IsSafe)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

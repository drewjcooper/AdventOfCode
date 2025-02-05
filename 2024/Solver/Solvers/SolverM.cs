using System.Text.RegularExpressions;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public partial class SolverM(PuzzleInput input) : Solver
{
    protected override Answer SolvePart1() => Machine.ParseAll(input.RawText).Sum(m => m.Cost);

    protected override Answer SolvePart2() => Machine.ParseAll(input.RawText, 10000000000000).Sum(m => m.Cost);

    private partial class Machine
    {
        public Machine(Match match, decimal targetOffset)
        {
            var xa = decimal.Parse(match.Groups["xa"].Value);
            var ya = decimal.Parse(match.Groups["ya"].Value);
            var xb = decimal.Parse(match.Groups["xb"].Value);
            var yb = decimal.Parse(match.Groups["yb"].Value);
            var xp = long.Parse(match.Groups["xp"].Value) + targetOffset;
            var yp = long.Parse(match.Groups["yp"].Value) + targetOffset;

            checked
            {
                var num = xb * yp - yb * xp;
                var den = xb * ya - yb * xa;

                var a = num / den;
                var b = (yp - a * ya) / yb;

                if (decimal.IsInteger(a) && decimal.IsPositive(a) && decimal.IsInteger(b) && decimal.IsPositive(b))
                {
                    A = a;
                    B = b;
                }
            }
        }

        public decimal A { get; }
        public decimal B { get; }
        public decimal Cost => A * 3 + B;

        public static IEnumerable<Machine> ParseAll(string input, long targetOffset = 0)
        {
            foreach (var match in MachineRegex().Matches(input).OfType<Match>())
            {
                yield return new Machine(match, targetOffset);
            }
        }

        [GeneratedRegex(
            @"Button A: X\+(?'xa'\d+), Y\+(?'ya'\d+)\s*" +
            @"Button B: X\+(?'xb'\d+), Y\+(?'yb'\d+)\s*" +
            @"Prize: X=(?'xp'\d+), Y=(?'yp'\d+)",
            RegexOptions.Multiline)]
        private static partial Regex MachineRegex();
    }
}

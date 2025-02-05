using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverQ : Solver
    {
        private readonly string input;

        public SolverQ(PuzzleInput input)
        {
            this.input = input.RawText;
        }

        protected override string SolvePart1()
        {
            var target = Target.Parse(input);

            var vX = FindVx(target);
            var vY = FindVy(target);
            return GetMaxHeight(vY).ToString();
        }

        protected override string SolvePart2()
        {
            var target = Target.Parse(input);
            return FindPossibleVxVy(target).Count().ToString();
        }

        private static int FindVx(Target target)
        {
            for (int rangeX = 1, vX = 1; target.CompareX(rangeX) <= 0; vX++, rangeX += vX)
            {
                if (target.CompareX(rangeX) == 0) { return vX; }
            }

            throw new Exception("Optimal vX not found");
        }

        private static int FindVy(Target target) => Math.Abs(target.Y2) - 1;

        private int GetMaxHeight(int vY) => vY * (vY + 1) / 2;

        private static IEnumerable<(int Vx, int Steps, bool ToInfinity)> FindPossibleVx(Target target)
        {
            var minVx = FindVx(target);
            var maxVx = target.X2;

            for (int vx0 = minVx; vx0 <= maxVx; vx0++)
            {
                for (int i = 0, vx = vx0, x = 0; x <= target.X2 && vx >= 0; i++, x += vx, vx--)
                {
                    if (target.CompareX(x) == 0)
                    {
                        yield return (vx0, i, vx == 0);
                    }
                }
            }
        }

        private static IEnumerable<(int Vy, int Steps)> FindPossibleVy(Target target)
        {
            for (int vy0 = -1; vy0 >= target.Y2; vy0--)
            {
                for (int i = 0, vy = vy0, y = 0; y >= target.Y2; i++, y += vy, vy--)
                {
                    if (target.CompareY(y) == 0)
                    {
                        yield return (vy0, i);
                        yield return (-vy0 - 1, (-vy0 - 1) * 2 + 1 + i);
                    }
                }
            }
        }

        internal static IEnumerable<(int Vx, int Vy)> FindPossibleVxVy(Target target)
        {
            var possibleVx = FindPossibleVx(target).ToList();

            var vXBySteps = possibleVx
                .GroupBy(x => x.Steps, x => x.Vx)
                .ToDictionary(g => g.Key, g => g.ToList());

            var indefiniteVx = possibleVx.Where(x => x.ToInfinity)
                .Select(x => (x.Vx, x.Steps));

            var vYBySteps = FindPossibleVy(target)
                .GroupBy(x => x.Steps, x => x.Vy)
                .ToDictionary(g => g.Key, g => g.ToList());

            return vXBySteps
                .Join(vYBySteps, x => x.Key, y => y.Key, (x, y) => x.Value.SelectMany(vx => y.Value.Select(vy => (vx, vy))))
                .SelectMany(x => x)
                .Concat(
                    indefiniteVx.SelectMany(x => vYBySteps.Keys.Where(k => k > x.Steps).SelectMany(k => vYBySteps[k].Select(vY => (x.Vx, vY))))
                )
                .Distinct()
                .ToList();
        }

        public record Target(int X1, int X2, int Y1, int Y2)
        {
            private static Regex pattern = new(@"^target area: x=(?<x1>\d+)\.\.(?<x2>\d+), y=(?<y2>-\d+)\.\.(?<y1>-\d+)$");

            public int CompareX(int x) => x < X1 ? -1 : x > X2 ? 1 : 0;
            public int CompareY(int y) => y > Y1 ? -1 : y < Y2 ? 1 : 0;

            public static Target Parse(string input)
            {
                var match = pattern.Match(input);
                return new(
                    int.Parse(match.Groups["x1"].Value),
                    int.Parse(match.Groups["x2"].Value),
                    int.Parse(match.Groups["y1"].Value),
                    int.Parse(match.Groups["y2"].Value));
            }
        }
    }
}

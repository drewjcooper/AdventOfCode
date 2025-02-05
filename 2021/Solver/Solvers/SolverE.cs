using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverE : Solver
    {
        private readonly IEnumerable<Line> input;

        public SolverE(PuzzleInput input)
        {
            this.input = input.Lines.Select(l => Line.Parse(l));
        }

        protected override string SolvePart1() =>
            GetHotSpotCount(input.Where(l => l.IsHorizontal || l.IsVertical)).ToString();

        protected override string SolvePart2() => GetHotSpotCount(input).ToString();

        private int GetHotSpotCount(IEnumerable<Line> lines)
        {
            var atLeastOne = new HashSet<Point>();
            var atLeastTwo = new HashSet<Point>();

            foreach (var point in lines.SelectMany(line => line.GetPoints()))
            {
                if (!atLeastOne.Add(point))
                {
                    atLeastTwo.Add(point);
                }
            }

            return atLeastTwo.Count;
        }

        private record Line(Point From, Point To)
        {
            private static readonly Regex _matcher = new Regex(@"^(?<x1>\d+),(?<y1>\d+) -> (?<x2>\d+),(?<y2>\d+)$");

            public bool IsHorizontal => From.Y == To.Y;
            public bool IsVertical => From.X == To.X;

            public IEnumerable<Point> GetPoints() => Interpolate().Prepend(From).Append(To);

            private IEnumerable<Point> Interpolate()
            {
                var range = Math.Max(Math.Abs(To.X - From.X), Math.Abs(To.Y - From.Y));
                var xStep = (To.X - From.X) / range;
                var yStep = (To.Y - From.Y) / range;

                for (
                    int x = From.X + xStep, y = From.Y + yStep;
                    !(x == To.X && y == To.Y);
                    x += xStep, y += yStep)
                {
                    yield return new(x, y);
                }
            }

            public static Line Parse(string candidate)
            {
                var match = _matcher.Match(candidate);
                return new(new(Int("x1"), Int("y1")), new(Int("x2"), Int("y2")));

                int Int(string name) => int.Parse(match.Groups[name].Value);
            }
        }

        private record Point(int X, int Y);
    }
}

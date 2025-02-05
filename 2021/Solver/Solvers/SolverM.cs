using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverM : Solver
    {
        private readonly IEnumerable<Point> dots;
        private readonly IEnumerable<Command> commands;

        public SolverM(PuzzleInput input)
        {
            var split = input.Split();
            dots = split.Select(l => Point.Parse(l)).ToList();
            commands = split.Select(l => Command.Parse(l)).ToList();
        }

        protected override string SolvePart1() => commands.First().Execute(new Page(dots)).DotCount.ToString();

        protected override string SolvePart2() =>
            commands.Aggregate(new Page(dots), (p, c) => c.Execute(p)).ToString();

        internal class Page
        {
            private readonly HashSet<Point> dots = new();

            public Page(IEnumerable<Point> dots)
            {
                foreach (var dot in dots.Where(d => this.dots.Add(d)))
                {
                    if (dot.X > MaxX) { MaxX = dot.X; }
                    if (dot.Y > MaxY) { MaxY = dot.Y; }
                }
            }

            public int DotCount => dots.Count;

            public IEnumerable<Point> Dots => dots;

            public int MaxX { get; }
            public int MaxY { get; }

            public override string ToString()
                => Environment.NewLine + string.Join(
                    Environment.NewLine,
                    Enumerable.Range(0, MaxY + 1).Select(y =>
                        string.Join(
                            "", Enumerable.Range(0, MaxX + 1)
                        .Select(x => dots.Contains(new Point(x, y)) ? '#' : ' '))));
        }

        internal record Point(int X, int Y)
        {
            public static Point Parse(string text)
            {
                var values = text.Split(',').Select(s => int.Parse(s)).ToArray();
                return new Point(values[0], values[1]);
            }
        }

        internal abstract class Command
        {
            private static Regex pattern = new(@"^fold along (?<axis>[xy])=(?<value>\d+)$");

            public static Command Parse(string text)
            {
                var match = pattern.Match(text);
                if (!match.Success) { throw new Exception($"Bad command format: {text}"); }

                var value = int.Parse(match.Groups["value"].Value);

                return match.Groups["axis"].Value switch
                {
                    "x" => new FoldOnX(value),
                    "y" => new FoldOnY(value),
                    _ => throw new InvalidOperationException("Unknown axis")
                };
            }

            public abstract Page Execute(Page start);
        }

        internal class FoldOnX : Command
        {
            private readonly int x;

            public FoldOnX(int x)
            {
                this.x = x;
            }

            public override Page Execute(Page page)
            {
                var offset = Math.Max(x, page.MaxX - x);
                return new Page(page.Dots.Select(d => d with { X = offset - Math.Abs(x - d.X) }));
            }
        }

        internal class FoldOnY : Command
        {
            private readonly int y;

            public FoldOnY(int y)
            {
                this.y = y;
            }

            public override Page Execute(Page page)
            {
                var offset = Math.Max(y, page.MaxY - y);
                return new Page(page.Dots.Select(d => d with { Y = offset - Math.Abs(y - d.Y) }));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverI : Solver
    {
        private readonly IEnumerable<string> input;

        public SolverI(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1()
        {
            var floor = new Floor(input);
            return floor.GetMinima().Select(i => floor[i] + 1).Sum().ToString();
        }

        protected override string SolvePart2()
        {
            var floor = new Floor(input);
            var minima = floor.GetMinima().ToHashSet();
            var basins = new List<Basin>();

            while (minima.Count > 0)
            {
                var basin = new Basin(minima.First());
                basin.FindExtent(floor);
                basins.Add(basin);

                var included = minima.Where(m => basin.Contains(m)).ToList();
                minima.ExceptWith(included);
            }

            return basins.Select(b => b.Size)
                .OrderByDescending(b => b)
                .Take(3)
                .Aggregate(1L, (a, b) => a * b)
                .ToString();
        }

        internal class Basin
        {
            private readonly Point seed;
            private readonly HashSet<Point> points = new();

            public Basin(Point seed)
            {
                this.seed = seed;
            }

            public int Size => points.Count;

            public bool Contains(Point p) => points.Contains(p);

            public void FindExtent(Floor floor)
            {
                FindPointsFrom(seed);

                void FindPointsFrom(Point start)
                {
                    if (floor[start] == 9 || !points.Add(start)) { return; }

                    FindPointsFrom(start.Forward);
                    FindPointsFrom(start.Back);
                    FindPointsFrom(start.Left);
                    FindPointsFrom(start.Right);
                }
            }
        }

        internal class Floor
        {
            private readonly int[][] heights;
            private readonly int width;
            private readonly int length;

            public Floor(IEnumerable<string> input)
            {
                heights = input.Select(l => l.Select(c => c - '0').ToArray()).ToArray();
                width = heights.Length;
                length = heights[0].Length;
            }

            public int this[Point p] => IsOutOfBounds(p) ? 9 : heights[p.W][p.L];

            private bool IsOutOfBounds(Point p) => p.W < 0 || p.W >= width || p.L < 0 || p.L >= length;

            public IEnumerable<Point> GetMinima()
            {
                for (int w = 0; w < width; w++)
                {
                    for (int l = 0; l < length; l++)
                    {
                        var p = new Point(w, l);
                        var height = this[p];
                        if (height < this[p.Left] &&
                            height < this[p.Right] &&
                            height < this[p.Forward] &&
                            height < this[p.Back])
                        {
                            yield return p;
                        }
                    }
                }
            }
        }

        public struct Point
        {
            public Point(int w, int l)
            {
                W = w;
                L = l;
            }

            public int W { get; }
            public int L { get; }

            public Point Forward => new(W, L - 1);
            public Point Back => new(W, L + 1);
            public Point Left => new(W - 1, L);
            public Point Right => new(W + 1, L);
        }
    }
}

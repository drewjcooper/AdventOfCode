using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverO : Solver
    {
        private readonly IEnumerable<string> input;

        public SolverO(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1()
        {
            var cavern = new Cavern(input);
            var ship = new Ship(cavern);
            ship.FindPath();
            return cavern.BottomRightTotalRisk.ToString();
        }

        protected override string SolvePart2()
        {
            var cavern = new Cavern(input, 5);
            var ship = new Ship(cavern);
            ship.FindPath();
            return cavern.BottomRightTotalRisk.ToString();
        }

        public class Cavern
        {
            private readonly int[][] tile;
            private readonly Location[][] locations;

            public Cavern(IEnumerable<string> lines, int scale = 1)
            {
                tile = lines.Select(l => l.Select(c => c - '0').ToArray()).ToArray();
                TileSizeX = tile.Length;
                TileSizeY = tile.First().Length;
                MaxX = TileSizeX * scale - 1;
                MaxY = TileSizeY * scale - 1;
                locations = Enumerable
                    .Range(0, MaxX + 1)
                    .Select(x =>
                        Enumerable.Range(0, MaxY + 1)
                            .Select(y => new Location(CalculateEntryRisk(x, y), short.MaxValue))
                            .ToArray())
                    .ToArray();
                locations[0][0] = locations[0][0] with { TotalRisk = 0 };
            }

            public int MaxX { get; }
            public int MaxY { get; }

            public int TileSizeX { get; }
            public int TileSizeY { get; }

            public int BottomRightTotalRisk => locations[MaxX][MaxY].TotalRisk;

            public Location this[int x, int y]
            {
                get => IsValidCoordinate(x, y) ? locations[x][y] : Location.MaxValue;
                set => locations[x][y] = value;
            }

            private int CalculateEntryRisk(int x, int y)
                => (tile[x % TileSizeX][y % TileSizeY] + x / TileSizeX + y / TileSizeY - 1) % 9 + 1;

            private bool IsValidCoordinate(int x, int y) => x >= 0 && x <= MaxX && y >= 0 && y <= MaxY;
        }

        internal record Location(int Risk, int TotalRisk)
        {
            public static Location MaxValue = new(short.MaxValue, short.MaxValue);
        }

        public class Ship
        {
            private readonly Cavern cavern;

            public Ship(Cavern cavern)
            {
                this.cavern = cavern;
            }

            public void FindPath()
            {
                var locationsUpdated = false;

                do
                {
                    locationsUpdated = false;

                    for (int diag = 1; diag <= cavern.MaxX + cavern.MaxY; diag++)
                    {
                        for (int x = Math.Min(diag, cavern.MaxX), y = diag - x; y <= Math.Min(diag, cavern.MaxY); x--, y++)
                        {
                            var minRisk = GetLeastPriorRisk(x, y) + cavern[x, y].Risk;
                            if (minRisk < cavern[x, y].TotalRisk)
                            {
                                cavern[x, y] = cavern[x, y] with { TotalRisk = minRisk };
                                locationsUpdated = true;
                            }
                        }
                    }
                } while (locationsUpdated);
            }

            private int GetLeastPriorRisk(int x, int y)
                => Math.Min(
                    cavern[x - 1, y].TotalRisk,
                    Math.Min(
                        cavern[x, y - 1].TotalRisk,
                        Math.Min(
                            cavern[x, y + 1].TotalRisk,
                            cavern[x + 1, y].TotalRisk)));
        }
    }
}

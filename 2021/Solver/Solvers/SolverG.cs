using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverG : Solver
    {
        private readonly int[] input;

        public SolverG(PuzzleInput input)
        {
            this.input = input.RawText.Split(',').Select(s => int.Parse(s)).ToArray();
        }

        protected override string SolvePart1()
        {
            var crabs = input.GroupBy(x => x)
                .Select(g => (Position: g.Key, Count: g.Count()))
                .OrderBy(x => x.Position)
                .ToList();
            var leftCount = 0;
            var rightCount = input.Length;
            for (int i = 0, position = 0, fuel = input.Sum(); i < crabs.Count; i++)
            {
                var deltaFuel = (leftCount - rightCount) * (crabs[i].Position - position);
                if (deltaFuel > 0)
                {
                    return fuel.ToString();
                }
                leftCount += crabs[i].Count;
                rightCount -= crabs[i].Count;
                position = crabs[i].Position;
                fuel += deltaFuel;
            }
            return "Not Found";
        }

        protected override string SolvePart2()
        {
            var crabs = input.GroupBy(x => x)
                .Select(g => new Crabs(g.Key, g.Count()))
                .OrderBy(c => c.Position)
                .ToList();
            var target = input.Sum() / input.Length;
            var max = input.Max();
            var fuelNeeded = crabs.Select(c => c.FuelNeeded(target)).Sum();
            var increment = 1;
            var isFalling = false;
            while (target >= 0 && target <= max)
            {
                target += increment;
                var nextFuelNeeded = crabs.Select(c => c.FuelNeeded(target)).Sum();
                if (nextFuelNeeded <= fuelNeeded)
                {
                    isFalling = true;
                    fuelNeeded = nextFuelNeeded;
                }
                else if (isFalling)
                {
                    return fuelNeeded.ToString();
                }
                else
                {
                    increment = -increment;
                }
            }

            return "Not found";
        }

        private record Crabs(int Position, int Count)
        {
            public int FuelNeeded(int to)
            {
                var distance = Math.Abs(to - Position);
                return Count * distance * (distance + 1) / 2;
            }
        }
    }
}

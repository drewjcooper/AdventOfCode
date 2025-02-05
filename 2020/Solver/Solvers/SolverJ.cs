using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverJ : Solver
    {
        private readonly (int Rating, bool Required)[] adapters;

        public SolverJ(PuzzleInput input)
        {
            adapters = input.Ints
                .OrderBy(x => x)
                .Select(x => (x, false))
                .Prepend((0, true))
                .ToArray();
        }

        protected override string SolvePart1()
        {
            var differenceCounts =
                adapters
                    .Zip(adapters.Skip(1), (a1, a2) => a2.Rating - a1.Rating)
                    .GroupBy(x => x)
                    .ToDictionary(g => g.Key, g => g.Count());

            return ((differenceCounts[3] + 1) * differenceCounts[1]).ToString();
        }

        protected override string SolvePart2()
        {
            adapters[adapters.Length - 1] = (adapters[adapters.Length - 1].Rating, true);
            for (int i = 1; i < adapters.Length - 1; i++)
            {
                if (adapters[i+1].Rating - adapters[i-1].Rating > 3)
                {
                    adapters[i] = (adapters[i].Rating, true);
                }
            }

            var arrangements = 1L;
            var index = 0;
            while (TryGetNextRangeVariants(ref index, out var variants))
            {
                arrangements *= variants;
            }

            return arrangements.ToString();
        }

        bool TryGetNextRangeVariants(ref int index, out int variants)
        {
            variants = 0;
            var from = -1;
            var to = -1;
            Console.WriteLine($"{index}: {adapters[index]}");
            while(++index < adapters.Length)
            {
            Console.WriteLine($"{index}: {adapters[index]}");
                if (from == -1 && !adapters[index].Required)
                {
                    from = index-1;
                    Console.WriteLine($"From = {from}");
                }
                if (from > -1 && adapters[index].Required)
                {
                    to = index;
                    Console.WriteLine($"To = {to}");
                    break;
                }
            }

            if (to == -1) { return false; }

            variants = (to - from) switch
            {
                2 => 2,
                3 => 4,
                4 => 7
            };

            Console.WriteLine($"From {from} to {to}: span = {to - from}, variants = {variants}");
            return true;
        }
    }
}

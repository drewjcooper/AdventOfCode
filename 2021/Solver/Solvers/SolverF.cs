using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverF : Solver
    {
        private readonly IEnumerable<int> input;

        public SolverF(PuzzleInput input)
        {
            this.input = input.RawText.Split(',').Select(x => int.Parse(x));
        }

        protected override string SolvePart1() => GetFinalCount(80).ToString();
        protected override string SolvePart2() => GetFinalCount(256).ToString();

        private long GetFinalCount(int days)
        {
            var school = new long[9];
            foreach (var daysLeft in input)
            {
                school[daysLeft]++;
            }

            for (int i = 0; i < days; i++)
            {
                school = GetNextGeneration(school);
            }

            return school.Sum(f => f);
        }

        private static long[] GetNextGeneration(long[] school)
        {
            var next = new long[9];
            for (int i = 0; i < 9; i++)
            {
                next[i] = i switch
                {
                    6 => school[0] + school[7],
                    8 => school[0],
                    _ => school[i+1],
                };
            }
            return next;
        }
    }
}

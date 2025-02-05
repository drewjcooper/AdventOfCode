using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverF : Solver
    {
        private readonly string[] lines;

        public SolverF(PuzzleInput input)
        {
            lines = input.Lines;
        }

        protected override string SolvePart1() =>
            lines
                .Split("")
                .Select(g => g.SelectMany(l => l).Distinct().Count())
                .Sum()
                .ToString();

        protected override string SolvePart2() =>
            lines
                .Split("")
                .Where(g => g.Any())
                .Select(CountCommonChars)
                .Sum()
                .ToString();

        private int CountCommonChars(IEnumerable<string> lines)
        {
            var answers = lines.ToList();
            var memberCount = answers.Count();
            return answers.SelectMany(l => l)
                .GroupBy(ch => ch, (_, g) => g.Count())
                .Count(x => x == memberCount);
        }
    }
}

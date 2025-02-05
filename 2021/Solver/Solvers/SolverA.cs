using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverA : Solver
    {
        private readonly IEnumerable<int> input;

        public SolverA(PuzzleInput input)
        {
            this.input = input.Ints;
        }

        protected override string SolvePart1()
            => input.Zip(input.Skip(1)).Count(x => x.First < x.Second).ToString();

        protected override string SolvePart2()
        {
            var sums = input.Zip(input.Skip(1), (x, y) => x + y).Zip(input.Skip(2), (s, z) => s + z).ToList();
            return sums.Zip(sums.Skip(1)).Count(x => x.First < x.Second).ToString();
        }
    }
}

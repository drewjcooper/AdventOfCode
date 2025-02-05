using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverA : Solver
    {
        private readonly IEnumerable<int> input;

        public SolverA(PuzzleInput input)
        {
            this.input = input.Ints;
        }

        protected override string SolvePart1()
        {
            var values = new HashSet<int>();

            foreach (var value in input)
            {
                var diff = 2020 - value;
                if (values.Contains(diff))
                {
                    return $"{diff * value}";
                }
                values.Add(value);
            }

            return "No solution found";
        }

        protected override string SolvePart2()
        {
            var values = new HashSet<int>();

            foreach (var value1 in input)
            {
                foreach (var value2 in input.Where(v2 => v2 != value1))
                {
                    var diff = 2020 - value1 - value2;
                    if (values.Contains(diff))
                    {
                        return $"{diff * value1 * value2}";
                    }
                    values.Add(value1);
                    values.Add(value2);
                }
            }

            return "No solution found";
        }
    }
}

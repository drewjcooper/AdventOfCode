using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverC : Solver
    {
        private readonly string[] lines;

        public SolverC(PuzzleInput input)
        {
            lines = input.Lines;
        }

        protected override string SolvePart1() => CountTreesOnSlope(3, 1).ToString();

        protected override string SolvePart2()
        {
            var trees =
                CountTreesOnSlope(1, 1) *
                CountTreesOnSlope(3, 1) *
                CountTreesOnSlope(5, 1) *
                CountTreesOnSlope(7, 1) *
                CountTreesOnSlope(1, 2);

            return trees.ToString();
        }

        private int CountTreesOnSlope(int right, int down)
        {
            var width = lines[0].Length;
            var trees = 0;
            for (int row = 0, column = 0; row < lines.Length; row += down, column += right, column %= width)
            {
                if (lines[row][column] == '#') { trees++; }
            }
            return trees;
        }
    }
}

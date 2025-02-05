using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverH : Solver
{
    private readonly Tree[][] trees;
    private readonly int rowCount;
    private readonly int colCount;

    public SolverH(PuzzleInput input)
    {
        trees = input.Lines.Select(l => l.Select(t => new Tree((sbyte)(t - '0'))).ToArray()).ToArray();
        rowCount = trees.Length;
        colCount = trees[0].Length;
    }
    
    protected override string SolvePart1() 
    {
        sbyte highestTree;

        for (int row = 0; row < rowCount; row++)
        {
            highestTree = -1;
            for (int col = 0; col < colCount && highestTree < 9; col++)
            {
                CheckVisibilityOf(trees[row][col]);
            }
            highestTree = -1;
            for (int col = colCount - 1; col >= 0 && highestTree < 9; col--)
            {
                CheckVisibilityOf(trees[row][col]);
            }
        }

        for (int col = 0; col < colCount; col++)
        {
            highestTree = -1;
            for (int row = 0; row < rowCount && highestTree < 9; row++)
            {
                CheckVisibilityOf(trees[row][col]);
            }
            highestTree = -1;
            for (int row = rowCount - 1; row >= 0 && highestTree < 9; row--)
            {
                CheckVisibilityOf(trees[row][col]);
            }
        }

        return trees.Select(r => r.Count(t => t.IsVisible)).Sum().ToString();

        void CheckVisibilityOf(Tree tree)
        {
            if (tree.Height > highestTree)
            {
                tree.IsVisible = true;
                highestTree = tree.Height;
            }
        }
    }

    protected override string SolvePart2()
    {
        var maxScenicScore = 0;
        for (var row = 0; row < rowCount; row++)
        {
            for (var col = 0; col < colCount; col++)
            {
                var scenicScore = GetDistance((row, col), (-1, 0)) * GetDistance((row, col), (1, 0)) * 
                    GetDistance((row, col), (0, 1)) * GetDistance((row, col), (0, -1));
                if (scenicScore > maxScenicScore) { maxScenicScore = scenicScore; }
            }
        }

        return maxScenicScore.ToString();

        int GetDistance ((int Row, int Col) from, (int Row, int Col) delta)
        {
            var sourceHeight = trees[from.Row][from.Col].Height;
            var distance = 0;

            while (true)
            {
                from = (from.Row + delta.Row, from.Col + delta.Col);
                if (from.Row < 0 || from.Row >= rowCount || from.Col < 0 || from.Col >= colCount) { return distance; }
                distance += 1;
                if (trees[from.Row][from.Col].Height >= sourceHeight) { return distance; }
            }
        }
    }

    private record Tree(sbyte Height)
    {
        public bool IsVisible { get; set; }
    }
}
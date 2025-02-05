using System;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode.Helpers;
using AdventOfCode.Input;
using AdventOfCode.Solvers;

namespace AdventOfCode;

class Program
{
    static async Task Main(string[] args)
    {
        var solutions = await Task.WhenAll(
            args.Select(a => PuzzleId.Parse(a))
                .Select(async id => (id, id.IsValid ? await SolvePuzzleAsync(id) : $"Not a valid puzzleId")));

        foreach (var (id, solution) in solutions)
        {
            Console.WriteLine($"Puzzle {id.Id}: {solution}");
        }
    }

    static async Task<string> SolvePuzzleAsync(PuzzleId puzzleId) =>
        Solver.Get(puzzleId, await PuzzleInput.LoadAsync(puzzleId), Console.WriteLine).Solve(puzzleId);
}

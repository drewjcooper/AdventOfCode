using System;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode2021.Helpers;
using AdventOfCode2021.Input;
using AdventOfCode2021.Solvers;

namespace AdventOfCode2021
{
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
            Solver.Get(puzzleId, await PuzzleInput.LoadAsync(puzzleId)).Solve(puzzleId);
    }
}

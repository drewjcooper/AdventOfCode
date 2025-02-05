using AdventOfCode.Helpers;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal class SolverA(PuzzleInput input) : Solver
{
    private readonly IEnumerable<string> _lines = input.Lines;

    protected override Answer SolvePart1() => GetDistances().Sum();

    protected override Answer SolvePart2() => CalculateSimilarity().Sum();

    private IEnumerable<int> GetDistances()
    {
        var list1 = new List<int>();
        var list2 = new List<int>();
        foreach (var (val1, val2) in _lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
        {
            list1.Add(int.Parse(val1));
            list2.Add(int.Parse(val2));
        }

        return list1.Order().Zip(list2.Order(), (a, b) => Math.Abs(a - b));
    }

    private IEnumerable<long> CalculateSimilarity()
    {
        var list1 = new List<int>();
        var list2 = new Dictionary<int, long>();
        foreach (var (val1, val2) in _lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
        {
            list1.Add(int.Parse(val1));
            var index = int.Parse(val2);
            list2[index] = list2.GetValueOrDefault(index) + 1;
        }

        return list1.Select(i => i * list2.GetValueOrDefault(i));
    }
}

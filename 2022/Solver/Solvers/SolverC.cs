using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverC : Solver
{
    private readonly IEnumerable<Rucksack> rucksacks;

    public SolverC(PuzzleInput input)
    {
        rucksacks = input.Lines.Select(Rucksack.Parse).ToList();
    }
    
    protected override string SolvePart1() => rucksacks.Sum(r => r.CommonItemValue).ToString();

    protected override string SolvePart2() => rucksacks
        .Select((r, i) => (Rucksack: r, Group: i / 3))
        .GroupBy(x => x.Group, x => x.Rucksack)
        .Select(g => g.Skip(1).Aggregate(g.First().Contents, (a, r) => a.Intersect(r.Contents)))
        .Sum(i => ValueOf(i.Single()))
        .ToString();

    private record struct Rucksack(IEnumerable<char> Contents, string Compartment1, string Compartment2)
    {
        public static Rucksack Parse(string contents)
            => new Rucksack(contents, contents[..(contents.Length / 2)], contents[(contents.Length / 2)..]);

        public int CommonItemValue => ValueOf(Compartment1.Intersect(Compartment2).Single());
    }

    private static int ValueOf(char item) => char.IsLower(item) ? 1 + item - 'a' : 27 + item - 'A';
}
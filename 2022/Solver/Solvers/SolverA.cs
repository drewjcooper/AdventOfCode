using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverA : Solver
{
    private readonly IEnumerable<Elf> elves;

    public SolverA(PuzzleInput input)
    {
        elves = input.Lines.Split("").Select(g => new Elf(g)).ToList();
    }
    
    protected override string SolvePart1() => elves.Max(e => e.Total).ToString();

    protected override string SolvePart2() => 
        elves.OrderByDescending(e => e.Total).Take(3).Sum(e => e.Total).ToString();

    private class Elf
    {
        private readonly IEnumerable<int> items;

        public Elf(IEnumerable<string> items) => this.items = items.Select(int.Parse).ToList();

        public int Total => items.Sum();
    }
}
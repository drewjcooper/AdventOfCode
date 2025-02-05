using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverE : Solver
{
    private readonly Stockpile stockpile;
    private readonly IEnumerable<Command> commands;

    public SolverE(PuzzleInput input)
    {
        var lines = input.Break();
        stockpile = new Stockpile(lines);
        commands = lines.Select(l => Command.Parse(l));
    }
    
    protected override string SolvePart1() => Solve(new CrateMover9000(stockpile));
    protected override string SolvePart2() => Solve(new CrateMover9001(stockpile));

    private string Solve(CrateMover crane)
    {
        crane.Execute(commands);
        return stockpile.TopCrates;
    }

    private class Stockpile
    {
        private readonly Stack<char>[] stacks;

        public Stockpile(IEnumerable<string> lines)
        {
            lines = lines.ToList();
            var stackCount = (lines.First().Length + 1) / 4;
            stacks = Enumerable.Range(0, stackCount).Select(_ => new Stack<char>()).ToArray();
            foreach (var line in lines)
            {
                for (int i = 0; i < stacks.Length; i++)
                {
                    if (char.IsDigit(line[1]))
                    {
                        stacks[i] = new Stack<char>(stacks[i].Where(c => char.IsLetter(c)));
                    }
                    else
                    {
                        stacks[i].Push(line[1 + 4 * i]);
                    }
                }
            }
        }

        public IReadOnlyList<Stack<char>> Stacks => stacks;
        public string TopCrates => string.Join("", stacks.Select(s => s.Peek()));
    }

    private abstract class CrateMover
    {
        protected readonly Stockpile stockpile;

        public CrateMover(Stockpile stockpile)
        {
            this.stockpile = stockpile;
        }

        public void Execute(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                Execute(command);
            }
        }

        protected abstract void Execute(Command command);
    }

    private class CrateMover9000 : CrateMover
    {
        public CrateMover9000(Stockpile stockpile)
            : base(stockpile)
        {
        }

        protected override void Execute(Command command)
        {
            for (int i = 0; i < command.Quantity; i++)
            {
                stockpile.Stacks[command.To].Push(stockpile.Stacks[command.From].Pop());
            }
        }
    }

    private class CrateMover9001 : CrateMover
    {
        private readonly Stack<char> temp = new();

        public CrateMover9001(Stockpile stockpile)
            : base(stockpile)
        {
        }

        protected override void Execute(Command command)
        {
            for (int i = 0; i < command.Quantity; i++)
            {
                temp.Push(stockpile.Stacks[command.From].Pop());
            }

            for (int i = 0; i < command.Quantity; i++)
            {
                stockpile.Stacks[command.To].Push(temp.Pop());
            }
        }
    }

    private record struct Command (int Quantity, int From, int To)
    {
        public static Command Parse(string line)
        {
            var parts = line.Split(' ');
            return new Command(int.Parse(parts[1]), int.Parse(parts[3]) - 1, int.Parse(parts[5]) - 1);
        }
    }
}
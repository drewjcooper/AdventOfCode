using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverJ : Solver
{
    private readonly IEnumerable<Command> commands;

    public SolverJ(PuzzleInput input)
    {
        commands = input.Lines.Select(l => Command.Parse(l));
    }
    
    protected override string SolvePart1() => DiagnoseSystem().ToString();

    protected override string SolvePart2() => DumpScreen();

    private int DiagnoseSystem()
    {
        var cpu = new CPU();
        using var sampler = new Sampler(cpu);
        cpu.RunProgram(commands);
        return sampler.TotalSignalStrength;
    }

    private string DumpScreen()
    {
        var cpu = new CPU();
        using var screen = new Screen(cpu);
        cpu.RunProgram(commands);
        return screen.ToString();
    }

    private abstract class Command
    {
        public static Command Parse(string line)
            => line switch
            {
                "noop" => new NoOp(),
                _ => new AddX { Value = int.Parse(line[5..]) }
            };
    }

    private class NoOp : Command
    {
    }

    private class AddX : Command
    {
        public int Value { get; init; }
    }

    private class CPU
    {
        private int clockCycle;

        public event EventHandler<int>? Tick;

        public int X { get; private set; } = 1;

        public void RunProgram(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                switch (command)
                {
                    case NoOp _: NoOp(); break;
                    case AddX { Value: var value }: AddX(value); break;
                }
            }
        }

        private void NoOp() => ClockTick();

        private void AddX(int value)
        {
            ClockTick();
            ClockTick();
            X += value;
        }

        private void ClockTick() 
        {
            clockCycle += 1;
            Tick?.Invoke(this, clockCycle);
        }
    }

    private class Sampler : IDisposable
    {
        private readonly CPU _cpu;
        private readonly List<int> _signalStrengths = new();

        public Sampler(CPU cpu)
        {
            _cpu = cpu;
            _cpu.Tick += Sample;
        }

        public int TotalSignalStrength => _signalStrengths.Sum();

        private void Sample(object? cpu, int clockCycle)
        {
            if (clockCycle <= 220 && clockCycle % 40 == 20)
            {
                _signalStrengths.Add(((CPU)cpu!).X * clockCycle);
            }
        }

        public void Dispose()
        {
            _cpu.Tick -= Sample;
        }
    }

    private class Screen: IDisposable
    {
        private readonly CPU _cpu;
        private readonly char[][] _pixels;

        public Screen(CPU cpu)
        {
            _cpu = cpu;
            _pixels = Enumerable.Range(0, 6).Select(_ => new char[40]).ToArray();
            _cpu.Tick += DrawPixel;
        }

        private void DrawPixel(object? cpu, int clockCycle)
        {
            if (clockCycle > 240) { return; }

            var col = (clockCycle - 1) % 40;
            var row = (clockCycle - 1) / 40;
            var sprite = ((CPU)cpu!).X;
            _pixels[row][col] = Math.Abs(col - sprite) <= 1 ? '#' : '.';
        }

        public override string ToString()
            => string.Join(Environment.NewLine, _pixels.Select(r => new string(r)).Prepend(""));

        public void Dispose()
        {
            _cpu.Tick -= DrawPixel;
        }
    }
}
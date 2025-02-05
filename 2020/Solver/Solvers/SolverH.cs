using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverH : Solver
    {
        private readonly Instruction[] instructions;

        public SolverH(PuzzleInput input)
        {
            instructions = input.Lines.Select((l, i) => Instruction.Parse(l, i)).ToArray();
        }

        protected override string SolvePart1()
        {
            var cpu = new CPU(0, 0);

            cpu.Reset();
            while (cpu.CanExecute)
            {
                cpu = cpu.Execute(instructions[cpu.InstructionPointer]);
            }

            return cpu.Accumulator.ToString();
        }

        protected override string SolvePart2()
        {
            var jumpsByTarget = instructions
                .Where(i => i.Operator != "acc")
                .ToLookup(i => i.Next, i => i.Index);

            var cpu = new CPU(0, 0);
            cpu.Reset();
            while (cpu.CanExecute)
            {
                cpu = cpu.Execute(instructions[cpu.InstructionPointer]);
            }

            var safeIndices = new HashSet<int>();
            var newSafeIndices = new Queue<int>();
            newSafeIndices.Enqueue(instructions.Length);

            while(newSafeIndices.TryDequeue(out var index))
            {
                do
                {
                    safeIndices.Add(index);
                    foreach (var jumpIndex in jumpsByTarget[index]) { newSafeIndices.Enqueue(jumpIndex); }
                }
                while(--index >= 0 && instructions[index].Operator != "jmp");

                if (index >= 0 && cpu.WasExecuted(index))
                {
                    instructions[index] = instructions[index].Flip();
                    break;
                }
            }

            cpu = cpu.Reset();
            while (cpu.CanExecute)
            {
                cpu = cpu.Execute(instructions[cpu.InstructionPointer]);
                if (cpu.InstructionPointer == instructions.Length)
                {
                    return cpu.Accumulator.ToString();
                }
            }

            return "Loop";
        }

        public record Instruction (int Index, string Operator, int Operand)
        {
            public static Instruction Parse(string instruction, int index)
            {
                var parts = instruction.Split(' ');
                return new Instruction(index, parts[0], int.Parse(parts[1]));
            }

            public int Next => Operand + Index;

            public Instruction Flip() => this with { Operator = Operator == "jmp" ? "nop" : "jmp" };
        }

        public record CPU (int InstructionPointer, int Accumulator)
        {
            private static readonly HashSet<int> executed = new();

            public bool CanExecute => executed.Add(InstructionPointer);

            public CPU Reset()
            {
                executed.Clear();
                return new CPU(0, 0);
            }

            public CPU Execute(Instruction instruction)
                => instruction.Operator switch
                {
                    "acc" => new CPU(InstructionPointer + 1, Accumulator + instruction.Operand),
                    "nop" => new CPU(InstructionPointer + 1, Accumulator),
                    "jmp" => new CPU(InstructionPointer + instruction.Operand, Accumulator)
                };

            public bool WasExecuted(int instructionIndex) => executed.Contains(instructionIndex);
        }
    }
}

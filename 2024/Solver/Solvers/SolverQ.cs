using System.Collections.Immutable;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public partial class SolverQ(PuzzleInput input) : Solver
{
    private readonly Processor _processor = new(input.Lines);
    private readonly Program _program = new(input.Lines[^1][9..]);

    protected override Answer SolvePart1() => string.Join(",", _processor.Execute(_program));

    protected override Answer SolvePart2()
    {
        IEnumerable<long> previous = [0];
        for (int x = 1; x <= _program.Length; x++)
        {
            previous = previous.SelectMany(p => FindSolutions(p << 3, x)).ToList();
        }

        return previous.Min();

        IEnumerable<long> FindSolutions(long seed, int length)
        {
            for (int x = 0; x < 8; x++)
            {
                var a = seed + x;
                if (_program.Matches(_processor.Execute(_program, a), length))
                {
                    yield return a;
                }
            }
        }
    }

    private class Processor(string[] state)
    {
        private Registers _registers = 
            new(ParseRegisterValue(state[0]), ParseRegisterValue(state[1]), ParseRegisterValue(state[2]));

        private static int ParseRegisterValue(string state) => int.Parse(state[state.LastIndexOf(' ')..]);

        public IEnumerable<int> Execute(Program program, long registerA)
        { 
            _registers = new(registerA, 0, 0, 0);
            return Execute(program);
        }

        public IEnumerable<int> Execute(Program program)
        {
            var registers = _registers;
            while (program[registers.ProgramCounter] is { } instruction)
            {
                registers = instruction.Execute(registers, out var output);
                if (instruction.HasOutput) { yield return output; }
                if (!instruction.Jumped) { registers = registers.AdvanceCounter(); }
            }
        }
    }

    private class Program(string sourceCode)
    {
        private readonly ImmutableArray<int> _sourceCode = sourceCode.Replace(",", "").Select(x => (int)(x - '0')).ToImmutableArray();
        private readonly ImmutableArray<Instruction> _instructions = ParseInstructions(sourceCode).ToImmutableArray();

        private static IEnumerable<Instruction> ParseInstructions(string program)
        {
            for (var i = 0; i < program.Length - 2; i += 2)
            {
                yield return Instruction.Parse(program[i], program[i+2] - '0');
            }
        }

        public int Length => _sourceCode.Length;

        public Instruction? this[int index] => index < _instructions.Length ? _instructions[index] : null;

        public bool Matches(IEnumerable<int> outputs)
        {
            using var enumerator = outputs.GetEnumerator();
            for (int i = 0; i < _sourceCode.Length; i++)
            {
                if (!enumerator.MoveNext()) { return false; }
                if (enumerator.Current != _sourceCode[i]) { return false; }
            }

            return !enumerator.MoveNext();
        }

        public bool Matches(IEnumerable<int> outputs, int length) => outputs.SequenceEqual(_sourceCode[^length..]);
    }

    private abstract class Instruction(Operand operand)
    {
        public virtual bool Jumped => false;

        public virtual bool HasOutput => false;

        protected Operand Operand { get; } = operand;

        public virtual Registers Execute(Registers registers, out int output)
        {
            output = -1;
            return Execute(registers);
        }

        protected abstract Registers Execute(Registers registers);

        public static Instruction Parse(char opcode, int operand)
            => opcode switch
            {
                '0' => new Adv(operand),
                '1' => new Bxl(operand),
                '2' => new Bst(operand),
                '3' => new Jnz(operand),
                '4' => new Bxc(operand),
                '5' => new Out(operand),
                '6' => new Bdv(operand),
                '7' => new Cdv(operand),
                _ => throw new ArgumentException($"Invalid opcode: {opcode}", nameof(opcode))
            };

        private class Adv(int operand) : Instruction(new ComboOperand(operand))
        {
            protected override Registers Execute(Registers registers)
                => registers with { A = registers.A / (1 << (int)Operand.GetValue(registers)) };
        }

        private class Bxl(int operand) : Instruction(new LiteralOperand(operand))
        {
            protected override Registers Execute(Registers registers)
                => registers with { B = registers.B ^ Operand.GetValue(registers) };
        }

        private class Bst(int operand) : Instruction(new ComboOperand(operand))
        {
            protected override Registers Execute(Registers registers)
                => registers with { B = Operand.GetValueMod8(registers) };
        }

        private class Jnz(int operand) : Instruction(new LiteralOperand(operand))
        {
            private bool _jumped;

            public override bool Jumped => _jumped;

            protected override Registers Execute(Registers registers)
            {
                _jumped = registers.A != 0;
                return _jumped ? registers.Jump((int)Operand.GetValue(registers)) : registers;
            }
        }

        private class Bxc(int operand) : Instruction(new LiteralOperand(operand))
        {
            protected override Registers Execute(Registers registers)
                => registers with { B = registers.B ^ registers.C };
        }

        private class Out(int operand) : Instruction(new ComboOperand(operand))
        {
            public override bool HasOutput => true;

            public override Registers Execute(Registers registers, out int output)
            {
                output = Operand.GetValueMod8(registers);
                return registers;
            }

            protected override Registers Execute(Registers registers) => throw new NotImplementedException();
        }

        private class Bdv(int operand) : Instruction(new ComboOperand(operand))
        {
            protected override Registers Execute(Registers registers)
                => registers with { B = registers.A / (1 << (int)Operand.GetValue(registers)) };
        }

        private class Cdv(int operand) : Instruction(new ComboOperand(operand))
        {
            protected override Registers Execute(Registers registers)
                => registers with { C = registers.A / (1 << (int)Operand.GetValue(registers)) };
        }
    }

    private abstract class Operand
    { 
        public abstract long GetValue(Registers registers);

        public int GetValueMod8(Registers registers) => (int)(GetValue(registers) % 8);
    }

    private class LiteralOperand(int value) : Operand
    {
        public override long GetValue(Registers registers) => value;
    }

    private class ComboOperand(int value) : Operand
    {
        private readonly int _value = value;

        public override long GetValue(Registers registers) 
            => _value switch
            {
                <= 3 => _value,
                4 => registers.A,
                5 => registers.B,
                6 => registers.C,
                _ => throw new ArgumentException($"Invalid operand: {_value}.")
            };
    }

    private record struct Registers(long A, long B, long C, int ProgramCounter)
    {
        public Registers(long a, long b, long c) : this(a, b, c, 0) 
        {
        }

        public Registers Jump(int location) => this with { ProgramCounter = location };

        public Registers AdvanceCounter() => this with { ProgramCounter = ProgramCounter + 2 };
    }
}

using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal partial class SolverG(PuzzleInput input) : Solver
{
    private readonly IEnumerable<Equation> equations = input.Lines.Select(Equation.Parse);

    protected override Answer SolvePart1() 
        => equations.Where(e => e.HasSolution(new(Operator.Add, Operator.Multiply))).Sum(e => e.Target);

    protected override Answer SolvePart2()
        => equations.Where(e => e.HasSolution(new(Operator.Add, Operator.Multiply, Operator.Append))).Sum(e => e.Target);

    private class Equation(long target, long[] operands)
    {
        private readonly long[] _operands = operands;

        public static Equation Parse(string line)
        {
            var parts = line.Split(':');
            var target = long.Parse(parts[0]);
            var operands = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            return new Equation(target, operands);
        }

        public long Target { get; } = target;

        public bool HasSolution(OperatorSet operatorSet)
        {
            var operators = Enumerable.Repeat(Operator.Add, _operands.Length).ToArray();

            do
            {
                if (Evaluate(operators) == Target) { return true; }

                for (int i = 1; i < operators.Length; i++)
                {
                    operators[i] = operatorSet.Rotate(operators[i]);
                    if (operators[i] != Operator.Add) { break; }
                }
            } while (operators.Any(op => op != Operator.Add));

            return false;
        }

        private long Evaluate(IEnumerable<Operator> operators)
            => operators
                .Zip(_operands, (op, operand) => new Term(op, operand))
                .Aggregate(0L, (a, t) => t.Apply(a));

        private record Term(Operator Operator, long Operand)
        {
            public long Apply(long accumulator) => Operator.Apply(accumulator, Operand);
        }
    }

    private class Operator(Func<long, long, long> operation)
    {
        public static readonly Operator Add = new((a, b) => a + b);
        public static readonly Operator Multiply = new((a, b) => a * b);
        public static readonly Operator Append = new((a, b) => long.Parse($"{a}{b}"));

        private readonly Func<long, long, long> _operation = operation;

        public long Apply(long a, long b) => _operation(a, b);
    }

    private class OperatorSet(params Operator[] operators)
    {
        private readonly Dictionary<Operator, Operator> _rotation =
            Enumerable.Range(0, operators.Length)
                .ToDictionary(i => operators[i], i => operators[(i + 1) % operators.Length]);

        public Operator Rotate(Operator op) => _rotation[op];
    }
}

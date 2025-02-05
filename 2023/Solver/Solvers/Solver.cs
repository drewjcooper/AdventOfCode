using AdventOfCode.Helpers;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public abstract class Solver
{
    private static readonly Solver _missingSolver = new MissingSolver();
    private static readonly Dictionary<char, Func<PuzzleInput, Action<string>, Solver>> _solverFactories;

    static Solver()
    {
        _solverFactories = typeof(Solver)
            .Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Solver)))
            .ToDictionary(t => t.Name.Last(), CreateSolverFactory);
    }

    public static Solver Get(PuzzleId puzzleId, PuzzleInput input, Action<string> log) =>
        _solverFactories.GetValueOrDefault(puzzleId.Code, (_, _) => _missingSolver)(input, log);

    public string Solve(PuzzleId puzzleId)
        => puzzleId.Part == 1 ? SolvePart1() : SolvePart2();

    protected abstract Answer SolvePart1();
    protected abstract Answer SolvePart2();

    private static Func<PuzzleInput, Action<string>, Solver> CreateSolverFactory(Type solverType)
    {
        var basicConstructor = solverType.GetConstructor([typeof(PuzzleInput)]);
        var loggingConstructor = solverType.GetConstructor([typeof(PuzzleInput), typeof(Action<string>)]);

        return (input, log) => 
            loggingConstructor?.Invoke([input, log]) as Solver 
            ?? basicConstructor?.Invoke([input]) as Solver 
            ?? throw new Exception($"Unable to create instance of {solverType}.");
    }

    private class MissingSolver : Solver
    {
        protected override Answer SolvePart1() => "No Solver available";
        protected override Answer SolvePart2() => "No Solver available";
    }

    protected readonly struct Answer
    {
        private readonly string _value;

        private Answer(string value) => _value = value;

        private Answer(long value) => _value = value.ToString();

        public static implicit operator string(Answer answer) => answer._value;

        public static implicit operator Answer(string value) => new(value);

        public static implicit operator Answer(int value) => new(value);

        public static implicit operator Answer(long value) => new(value);
    }
}

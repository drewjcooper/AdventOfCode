using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

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

    protected abstract string SolvePart1();
    protected abstract string SolvePart2();

    private static Func<PuzzleInput, Action<string>, Solver> CreateSolverFactory(Type solverType)
    {
        var basicConstructor = solverType.GetConstructor(new[] { typeof(PuzzleInput) });
        var loggingConstructor = solverType.GetConstructor(new[] { typeof(PuzzleInput), typeof(Action<string>) });

        return (input, log) => 
            loggingConstructor?.Invoke(new object[] { input, log }) as Solver 
            ?? basicConstructor?.Invoke(new object[] { input }) as Solver 
            ?? throw new Exception($"Unable to create instance of {solverType}.");
    }

    private class MissingSolver : Solver
    {
        protected override string SolvePart1() => "No Solver available";
        protected override string SolvePart2() => "No Solver available";
    }
}

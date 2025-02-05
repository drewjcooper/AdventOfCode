using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public abstract class Solver
    {
        private static Solver _missingSolver = new MissingSolver();
        private static Dictionary<char, Func<PuzzleInput, Solver>> _solverFactories;

        static Solver()
        {
            _solverFactories = typeof(Solver)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Solver)))
                .ToDictionary(t => t.Name.Last(), t => CreateSolverFactory(t));
        }

        public static Solver Get(PuzzleId puzzleId, PuzzleInput input) =>
            _solverFactories.GetValueOrDefault(puzzleId.Code, _ => _missingSolver)(input);

        public string Solve(PuzzleId puzzleId)
            => puzzleId.Part == 1 ? SolvePart1() : SolvePart2();

        protected abstract string SolvePart1();
        protected abstract string SolvePart2();

        private static Func<PuzzleInput, Solver> CreateSolverFactory(Type solverType) =>
            input => (Solver)Activator.CreateInstance(solverType, input);

        private class MissingSolver : Solver
        {
            protected override string SolvePart1() => "No Solver available";
            protected override string SolvePart2() => "No Solver available";
        }
    }
}

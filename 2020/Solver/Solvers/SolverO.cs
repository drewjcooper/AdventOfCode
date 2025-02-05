using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverO : Solver
    {
        private readonly int[] numbers;

        public SolverO(PuzzleInput input)
        {
            numbers = input.RawText.Split(",").Select(x => int.Parse(x)).ToArray();
        }

        protected override string SolvePart1() => PlayGame(2020);

        protected override string SolvePart2() => PlayGame(30_000_000);

        private string PlayGame(int toTurn)
        {
            var history = numbers.Select((n, i) => (Number: n, Turn: i + 1)).ToDictionary(x => x.Number, x => x.Turn);

            var numberSpoken = numbers[numbers.Length - 1];
            var turn = history.Count;

            do
            {
                var nextNumber = history.TryGetValue(numberSpoken, out var onTurn) ? turn - onTurn : 0;
                history[numberSpoken] = turn;
                numberSpoken = nextNumber;
            }
            while (++turn < toTurn);

            return numberSpoken.ToString();
        }
    }
}

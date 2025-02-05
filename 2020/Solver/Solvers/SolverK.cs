using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverK : Solver
    {
        private readonly string[] input;

        public SolverK(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1() =>
            new WaitingRoom(input, new NeighbouringSeatsDecisionModel())
                .Last()
                .Count(s => s == '#')
                .ToString();

        protected override string SolvePart2() =>
            new WaitingRoom(input, new VisibleSeatsDecisionModel())
                .Last()
                .Count(s => s == '#')
                .ToString();

        public interface IDecisionModel
        {
            int CountOccupiedSeats(char[][] seats, int row, int col);
            int SeatingThreshold { get; }
        }

        public class NeighbouringSeatsDecisionModel : IDecisionModel
        {
            public int SeatingThreshold => 4;

            public int CountOccupiedSeats(char[][] seats, int row, int col)
            {
                var rowCount = seats.Length;
                var colCount = seats[0].Length;

                return Enumerable
                    .Range(row-1, 3)
                    .Where(r => r >= 0 && r < rowCount)
                    .SelectMany(r =>
                        Enumerable
                            .Range(col-1, 3)
                            .Where(c => c >= 0 && c < colCount)
                            .Where(c => r != row || c != col)
                            .Select(c => seats[r][c]))
                    .Count(s => s == '#');
            }
        }

        public class VisibleSeatsDecisionModel : IDecisionModel
        {
            public int SeatingThreshold => 5;

            public int CountOccupiedSeats(char[][] seats, int row, int col)
            {
                var rowCount = seats.Length;
                var colCount = seats[0].Length;

                return Enumerable
                    .Range(-1, 3)
                    .SelectMany(r =>
                        Enumerable
                            .Range(-1, 3)
                            .Where(c => r != 0 || c != 0)
                            .Select(c => GetFirstSeat(r, c)))
                    .Count(s => s == '#');

                char GetFirstSeat(int rowDelta, int colDelta)
                {
                    for (int rowNdx = row + rowDelta, colNdx = col + colDelta;
                         rowNdx >= 0 && rowNdx < rowCount && colNdx >= 0 && colNdx < colCount;
                         rowNdx += rowDelta, colNdx += colDelta)
                    {
                        if (seats[rowNdx][colNdx] == '.') { continue; }

                        return seats[rowNdx][colNdx];
                    }

                    return '.';
                }
            }
        }

        public class WaitingRoom : IEnumerable<string>
        {
            private readonly string[] input;
            private readonly IDecisionModel decisionModel;

            public WaitingRoom(string[] input, IDecisionModel decisionModel)
            {
                this.input = input;
                this.decisionModel = decisionModel;
            }

            public IEnumerator<string> GetEnumerator() => new Enumerator(input, decisionModel);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public class Enumerator : IEnumerator<string>
            {
                private readonly string[] input;
                private readonly IDecisionModel decisionModel;
                private readonly int rowCount;
                private readonly int colCount;

                private char[][] seats;
                private bool changed;
                private bool isStable;

                public Enumerator(string[] input, IDecisionModel decisionModel)
                {
                    this.input = input;
                    this.decisionModel = decisionModel;
                    Reset();
                    rowCount = seats.Length;
                    colCount = seats[0].Length;
                }

                public string Current => string.Join(Environment.NewLine, seats.Select(r => new string(r)));

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (isStable) { return false; }
                    changed = false;
                    seats = Enumerable
                        .Range(0, rowCount)
                        .Select(ir => Enumerable
                            .Range(0, colCount)
                            .Select(ic => GetNext(ir, ic))
                            .ToArray())
                        .ToArray();
                    isStable = !changed;
                    return true;
                }

                public char GetNext(int row, int col)
                {
                    var state = seats[row][col];
                    if (state == '.') { return '.'; }
                    var occupiedSeatCount = decisionModel.CountOccupiedSeats(seats, row, col);
                    var nextState = occupiedSeatCount switch
                    {
                        _ when occupiedSeatCount == 0 => '#',
                        _ when occupiedSeatCount >= decisionModel.SeatingThreshold => 'L',
                        _ => state
                    };
                    changed |= state != nextState;
                    return nextState;
                }

                public int CountOccupiedNeighbours(int row, int col) =>
                    Enumerable
                        .Range(row-1, 3)
                        .Where(r => r >= 0 && r < rowCount)
                        .SelectMany(r =>
                            Enumerable
                                .Range(col-1, 3)
                                .Where(c => c >= 0 && c < colCount)
                                .Where(c => r != row || c != col)
                                .Select(c => seats[r][c]))
                        .Count(s => s == '#');

                public void Reset()
                {
                    changed = false;
                    isStable = false;
                    seats = input.Select(l => l.ToCharArray()).ToArray();
                }
            }
        }
    }
}

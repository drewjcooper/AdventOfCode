using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

using static System.StringSplitOptions;

namespace AdventOfCode2021.Solvers
{
    internal class SolverD : Solver
    {
        private readonly IEnumerable<int> numbers;
        private readonly List<Board> boards;

        public SolverD(PuzzleInput input)
        {
            var lines = input.Lines.AsEnumerable().GetEnumerator();
            lines.MoveNext();
            numbers = lines.Current.Split(',').Select(n => int.Parse(n)).ToArray();
            boards = ReadBoards(lines).ToList();
        }

        protected override string SolvePart1()
        {
            foreach (var number in numbers)
            {
                foreach (var board in boards)
                {
                    if (board.Mark(number))
                    {
                        return (number * board.Score).ToString();
                    }
                }
            }

            return null;
        }

        protected override string SolvePart2()
        {
            foreach (var number in numbers)
            {
                foreach (var board in boards.ToList())
                {
                    if (board.Mark(number))
                    {
                        if (boards.Count == 1)
                        {
                            return (number * boards[0].Score).ToString();
                        }
                        boards.Remove(board);
                    }
                }
            }

            return null;
        }


        private IEnumerable<Board> ReadBoards(IEnumerator<string> lines)
        {
            Board board = null;
            while (lines.MoveNext())
            {
                board = string.IsNullOrWhiteSpace(lines.Current)
                    ? new Board()
                    : board.Add(lines.Current);
                if (board.IsComplete) { yield return board; }
            }
        }

        private class Board
        {
            private readonly List<List<int>> rows = new();
            private readonly List<int>[] columns = Enumerable.Range(0,5).Select(_ => new List<int>()).ToArray();
            private readonly Dictionary<int, (int Row, int Column)> locations = new();

            public bool IsComplete => rows.Count == 5;
            public int Score => rows.SelectMany(r => r).Sum();

            public Board Add(string line)
            {
                var row = line.Split(' ', RemoveEmptyEntries).Select(s => int.Parse(s)).ToList();
                var rowIndex = rows.Count;

                for (var columnIndex = 0; columnIndex < 5; columnIndex++)
                {
                    var number = row[columnIndex];
                    locations[number] = (rowIndex, columnIndex);
                    columns[columnIndex].Add(number);
                }

                rows.Add(row);

                return this;
            }

            public bool Mark(int number)
            {
                return locations.TryGetValue(number, out var location) &&
                    (RemoveFrom(rows[location.Row]) || RemoveFrom(columns[location.Column]));

                bool RemoveFrom(List<int> group)
                {
                    group.Remove(number);
                    return group.Count == 0;
                }
            }
        }
    }
}

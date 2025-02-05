using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverE : Solver
    {
        private readonly IEnumerable<BoardingPass> boardingPasses;

        public SolverE(PuzzleInput input)
        {
            boardingPasses = input.Lines.Select(l => new BoardingPass(l)).ToList();
        }

        protected override string SolvePart1() => boardingPasses.Max(bp => bp.SeatId).ToString();

        protected override string SolvePart2()
        {
            var ids = new HashSet<int>(boardingPasses.Select(bp => bp.SeatId));

            for (int i = 0; i < 2 << 10; i++)
            {
                if (ids.Contains(i)) { continue; }
                if (ids.Contains(i - 1) && ids.Contains(i + 1)) { return i.ToString(); }
            }

            throw new Exception("Missing boarding pass not found.");
        }

        internal class BoardingPass : IComparable<BoardingPass>
        {
            private readonly string seatAssignment;
            private readonly Lazy<int> seatId;

            public BoardingPass(string seatAssignment)
            {
                this.seatAssignment = seatAssignment;
                seatId = new Lazy<int>(ParseAssignment);
            }

            public int SeatId => seatId.Value;
            public int Row => SeatId >> 3;
            public int Column => SeatId % 8;

            public int CompareTo(BoardingPass other)
            {
                for (int i = 0; i < seatAssignment.Length; i++)
                {
                    var characterComparison = (seatAssignment[i], other.seatAssignment[i]) switch
                    {
                        ('F', 'B') or ('L', 'R') => -1,
                        ('B', 'F') or ('R', 'L') => 1,
                        _ => 0
                    };

                    if (characterComparison != 0) { return characterComparison; }
                }

                return 0;
            }

            private int ParseAssignment()
            {
                var id = 0;
                for (int i = 0; i < seatAssignment.Length; i++)
                {
                    id = (id << 1) + seatAssignment[i] switch
                    {
                        'F' or 'L' => 0,
                        'B' or 'R' => 1,
                        _ => throw new ArgumentException($"Unrecognised seat assignment - {seatAssignment}")
                    };
                }
                return id;
            }
        }
    }
}

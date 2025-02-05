using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverI : Solver
{
    private readonly IEnumerable<Move> moves;

    public SolverI(PuzzleInput input)
    {
        moves = input.Lines.Select(l => Move.Parse(l));
    }
    
    protected override string SolvePart1() => new Rope(1).Move(moves).TailVisitCount.ToString();

    protected override string SolvePart2() => new Rope(9).Move(moves).TailVisitCount.ToString();

    private record struct Position(int X, int Y)
    {
        public static readonly Position Origin = new(0, 0);

        public static Position operator +(Position p, Delta d) => new Position(p.X + d.X, p.Y + d.Y);

        public static Delta operator -(Position a, Position b) => new Delta(a.X - b.X, a.Y - b.Y);
    }

    private record Knot(Position Position)
    {
        public Knot Move(Delta delta) => new Knot(Position + delta);

        public virtual Knot CatchUp(Knot previousKnot)
        {
            var distance = previousKnot.Position - Position;
            return Move(distance.Magnitude > 1 ? distance.UnitDirectionVector : Delta.Zero);
        }
    }

    private record struct Move(Delta Delta, int Distance)
    {
        private static readonly Dictionary<char, Delta> _namedDeltas = new()
        {
            ['U'] = new(0, 1),
            ['D'] = new(0, -1),
            ['L'] = new(-1, 0),
            ['R'] = new(1, 0)
        };

        public static Move Parse(string line) => new(_namedDeltas[line[0]], int.Parse(line[2..]));
    }

    private record struct Delta(int X, int Y)
    {
        public static readonly Delta Zero = new(0, 0);

        public int Magnitude => Math.Max(Math.Abs(X), Math.Abs(Y));
        public Delta UnitDirectionVector => new Delta(ScaleToUnit(X), ScaleToUnit(Y));

        private int ScaleToUnit(int value) => value == 0 ? 0 : (value / Math.Abs(value));
    }

    private class Rope
    {
        private readonly List<Knot> _knots;
        private readonly HashSet<Position> _tailPositions = new() { Position.Origin };

        public Rope(int length)
        {
            _knots = Enumerable
                .Range(0, length + 1)
                .Select(_ => new Knot(Position.Origin))
                .ToList();
        }

        public int TailVisitCount => _tailPositions.Count;

        public Rope Move(IEnumerable<Move> moves)
        {
            foreach (var move in moves)
            {
                Move(move);
            }
            return this;
        }

        private void Move(Move move)
        {
            for (var i = 0; i < move.Distance; i++)
            {
                _knots[0] = _knots[0].Move(move.Delta);

                for (var k = 1; k < _knots.Count; k++)
                {
                    _knots[k] = _knots[k].CatchUp(_knots[k-1]);
                }

                _tailPositions.Add(_knots[^1].Position);
            }
        }
    }
}
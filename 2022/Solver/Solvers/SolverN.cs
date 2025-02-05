using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverN : Solver
{
    private readonly Cave _cave;

    public SolverN(PuzzleInput input)
    {
        _cave = Cave.Build(input.Lines.Select(l => Wall.Parse(l)));
    }
    
    protected override string SolvePart1() => _cave.SimulateSand().ToString();

    protected override string SolvePart2() => _cave.WithFloor().SimulateSand().ToString();

    private class Cave
    {
        private readonly Dictionary<int, Column> _columns = new();
        private int? _floorHeight;

        private Cave()
        {
        }

        private void AddWall(Position at)
        {
            if (!_columns.TryGetValue(at.X, out var column))
            {
                _columns.Add(at.X, column = new Column());
            }

            column.AddWall(at);
        }

        public Cave WithFloor()
        {
            var lowestWall = _columns.Values.Aggregate(new Position(0, 0), (a, c) => c.Lowest < a ? c.Lowest : a);
            _floorHeight = lowestWall.Y + 2;

            foreach (var (x, column) in _columns)
            {
                column.AddWall(new Position(x, _floorHeight.Value));
            }

            return this;
        }

        public int SimulateSand()
        {
            var count = 0;
            var start = new Position(500, 0);
            while (AddSand(start))
            {
                count++;
            }

            if (_floorHeight.HasValue)
            {
                for (var i = 0; _columns[500].HasSpaceAt(new(500, i)); i++)
                {
                    count += _floorHeight.Value - i;
                }
            }
            return count;
        }

        private bool AddSand(Position at)
        {
            var column = _columns[at.X];
            Position? current = at;

            if (!column.HasSpaceAt(at)) { return false; }

            while ((current = column.WillFallTo(current.Value)).HasValue)
            {
                if (!TryFallSideways(current.Value, out var newColumn, out var newPosition))
                {
                    column.AddWall(current.Value);
                    return true;
                }

                column = newColumn;
                current = newPosition;
            }

            return false;
        }

        private bool TryFallSideways(Position from, out Column newColumn, out Position newPosition)
        {
            return Try(Delta.DownLeft, out newColumn, out newPosition)
                || Try(Delta.DownRight, out newColumn, out newPosition);

            bool Try(Delta step, out Column newColumn, out Position newPosition)
            {
                newPosition = from + step;
                if (!_columns.TryGetValue(newPosition.X, out newColumn!))
                {
                    newColumn = _floorHeight.HasValue 
                        ? new Column().WithFloor(new Position(newPosition.X, _floorHeight.Value)) 
                        : Column.Empty;
                    _columns[newPosition.X] = newColumn;
                }
                return newColumn.HasSpaceAt(newPosition);
            }
        }

        public static Cave Build(IEnumerable<Wall> walls)
        {
            var cave = new Cave();
            foreach (var position in walls.SelectMany(w => w))
            {
                cave.AddWall(position);
            }
            return cave;
        }
    }

    private class Wall : IEnumerable<Position>
    {
        private readonly HashSet<Position> _positions = new();

        public static Wall Parse(string line)
        {
            var parts = line.Split(" ->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var corners = parts.Select(p => Position.Parse(p)).ToList();
            var segments = corners.Zip(corners.Skip(1), (a, b) => (From: a, To: b));
            
            var wall = new Wall();
            foreach (var (to, from) in segments)
            {
                var step = (to - from).ToStep();
                for (var position = from; position != to + step; position += step)
                {
                    wall._positions.Add(position);
                }
            }

            return wall;
        }

        public IEnumerator<Position> GetEnumerator() => _positions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _positions.GetEnumerator();
    }

    private class Column
    {
        public static readonly Column Empty = new();
        private readonly HashSet<Position> _occupied = new();
        private Position _lowest = new();

        public Position Lowest => _lowest;

        public Column WithFloor(Position at)
        {
            AddWall(at);
            return this;
        }

        public void AddWall(Position at)
        {
            _occupied.Add(at);
            if (at < _lowest) { _lowest = at; }
        }

        public Position? WillFallTo(Position from)
        {
            if (from < _lowest) { return null; }
            for (var next = from + Delta.Down; !_occupied.Contains(next); next += Delta.Down)
            {
                from += Delta.Down;
            }
            return from;
        }

        public bool HasSpaceAt(Position position) => !_occupied.Contains(position);
    }

    private record struct Position(int X, int Y)
    {
        public static Delta operator -(Position left, Position right) => new(left.X - right.X, left.Y - right.Y);
        public static Position operator +(Position from, Delta step) => new(from.X + step.X, from.Y + step.Y);
        public static bool operator <(Position left, Position right) => left.Y > right.Y;
        public static bool operator >(Position left, Position right) => left.Y < right.Y;

        public static Position Parse(string candidate)
        {
            var parts = candidate.Split(',');
            return new(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }

    private record struct Delta(int X, int Y)
    {
        public static Delta Down = new(0, 1);
        public static Delta DownLeft = new(-1, 1);
        public static Delta DownRight = new(1, 1);

        public Delta ToStep() => new(X == 0 ? 0 : X / Math.Abs(X), Y == 0 ? 0 : Y / Math.Abs(Y));
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverL : Solver
{
    private readonly Grid _grid;

    public SolverL(PuzzleInput input)
    {
        _grid = new(input.Lines);
        Console.WriteLine($"{_grid.Start} -> {_grid.End}");
    }
    
    protected override string SolvePart1() => FindEnd().ToString();

    protected override string SolvePart2() => FindOptimalStart().ToString();

    public int FindEnd()
    {
        var from = new HashSet<Position> { _grid.Start };
        var step = 0;

        while (true)
        {
            step++;
            var to = new HashSet<Position>();
            foreach (var fromPosition in from)
            {
                var fromSquare = _grid[fromPosition];
                fromSquare.Visited = true;
                foreach (var toPosition in _grid.GetNeighbours(fromPosition))
                {
                    var toSquare = _grid[toPosition];
                    if (toSquare.Visited) { continue; }
                    if (toSquare.Elevation <= fromSquare.Elevation + 1)
                    {
                        if (toSquare.IsEnd) { return step; }
                        to.Add(toPosition);
                    }
                }
            }

            from = to;
        }
    }

    public int FindOptimalStart()
    {
        var from = new HashSet<Position> { _grid.End };
        var step = 0;

        while (true)
        {
            step++;
            var to = new HashSet<Position>();
            foreach (var fromPosition in from)
            {
                var fromSquare = _grid[fromPosition];
                fromSquare.Visited = true;
                foreach (var toPosition in _grid.GetNeighbours(fromPosition))
                {
                    var toSquare = _grid[toPosition];
                    if (toSquare.Visited) { continue; }
                    if (toSquare.Elevation >= fromSquare.Elevation - 1)
                    {
                        if (toSquare.Elevation == 1) { return step; }
                        to.Add(toPosition);
                    }
                }
            }

            from = to;
        }
    }

    private class Grid
    {
        private readonly Square[][] _squares;
        private readonly int _lastRow;
        private readonly int _lastCol;

        public Grid(IEnumerable<string> lines)
        {
            _squares = lines.Select(l => l.Select(c => new Square(c)).ToArray()).ToArray();
            _lastRow = _squares.Length - 1;
            _lastCol = _squares[0].Length - 1;
            for (int r = 0; r <= _lastRow; r++)
            for (int c = 0; c <= _lastCol; c++)
            {
                if (_squares[r][c].IsStart) { Start = new Position(r, c); }
                if (_squares[r][c].IsEnd) { End = new Position(r, c); }
            }
        }

        public Square this[Position position] => _squares[position.Row][position.Col];
        public Position Start { get; }
        public Position End { get; }

        public IEnumerable<Position> GetNeighbours(Position position)
        {
            if (position.Row > 0) { yield return position.GetNorth(); }
            if (position.Row < _lastRow) { yield return position.GetSouth(); }
            if (position.Col < _lastCol) { yield return position.GetEast(); }
            if (position.Col > 0) { yield return position.GetWest(); }
        }
    }

    private class Square
    {
        public Square(char code)
        {
            IsStart = code == 'S';
            IsEnd = code == 'E';
            Elevation = code switch
            {
                'S' => 1,
                'E' => 26,
                _ => (int)(code - 'a') + 1
            };
        }

        public bool IsStart { get; }
        public bool IsEnd { get; }
        public int Elevation { get; }
        public bool Visited { get; set; }
    }

    private record struct Position(int Row, int Col)
    {
        public Position GetNorth() => new(Row - 1, Col);
        public Position GetSouth() => new(Row + 1, Col);
        public Position GetEast() => new(Row, Col + 1);
        public Position GetWest() => new(Row, Col - 1);
    }
}
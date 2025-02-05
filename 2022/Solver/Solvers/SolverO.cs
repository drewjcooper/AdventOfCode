using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal partial class SolverO : Solver
{
    private readonly IEnumerable<Sensor> _sensors;
    private readonly bool _isTestCase;

    public SolverO(PuzzleInput input)
    {
        _isTestCase = input.RawText.Contains("x=8,");
        _sensors = input.Lines.Select(l => Sensor.Parse(l));
    }

    protected override string SolvePart1()
        => _sensors.SelectMany(s => s.GetExcludedLocations(_isTestCase)).Distinct().Count().ToString();

    protected override string SolvePart2() => FindDistressBeacon().TuningFrequency.ToString();

    private Position FindDistressBeacon()
    {
        var max = _isTestCase ? 20 : 4_000_000;
        for (var r = 0; r <= max; r++)
        {
            var row = new Row(max);
            foreach (Range range in _sensors.Select(s => s.GetRange(r)).Where(r => r != null))
            {
                row.Remove(range);
            }

            if (row.Remainder.HasValue) { return new(row.Remainder.Value, r); }
        }

        return default;
    }

    private partial class Sensor
    {
        private readonly Position _location;
        private readonly Position _nearestBeacon;
        private readonly int _exclusionRange;

        private Sensor(Position location, Position nearestBeacon)
        {
            _location = location;
            _nearestBeacon = nearestBeacon;
            _exclusionRange = location - nearestBeacon;
        }

        public IEnumerable<Position> GetExcludedLocations(bool isTestCase)
            => GetExcludedLocations(isTestCase ? 10 : 2_000_000).Where(p => p != _nearestBeacon);

        private IEnumerable<Position> GetExcludedLocations(int row)
        {
            if (GetRange(row) is Range range)
            {
                for (var x = range.From; x <= range.To; x++)
                {
                    yield return new(x, row);
                }
            }
        }

        public Range? GetRange(int row)
        {
            var distanceInsideExclusionZone = _exclusionRange - Math.Abs(_location.Y - row);
            return distanceInsideExclusionZone >= 0
                ? new(_location.X - distanceInsideExclusionZone, _location.X + distanceInsideExclusionZone)
                : null;
        }

        public static Sensor Parse(string line)
        {
            var parts = line.Split(':');
            return new(Position.Parse(parts[0]), Position.Parse(parts[1]));
        }
    }

    private class Row
    {
        private readonly List<Range> _spans = new();

        public Row(int max) => _spans.Add(new(0, max));

        public int? Remainder => _spans.Count == 1 ? _spans[0].From : null;

        public void Remove(Range range)
        {
            for (var i = 0; i < _spans.Count; i++)
            {
                if (range.To < _spans[i].From) { break; }
                if (_spans[i].TryRemove(range, out var left, out var right))
                {
                    _spans.RemoveAt(i--);
                    if (left.HasValue) { _spans.Insert(++i, left.Value); }
                    if (right.HasValue) { _spans.Insert(++i, right.Value); }
                }
            }
        }
    }

    private record struct Range(int From, int To)
    {
        public bool Contains(int x) => From <= x && x <= To;
        public bool Contains(Range other) => Contains(other.From) && Contains(other.To);

        public bool TryRemove(Range range, out Range? left, out Range? right)
        {
            left = right = null;
            var result = false;
            if (Contains(range.From) && From < range.From)
            {
                left = this with { To = range.From - 1 };
                result |= true;
            }

            if (Contains(range.To) && To > range.To)
            {
                right = this with { From = range.To + 1 };
                result |= true;
            }

            return result || range.Contains(this);
        }
    }

    private partial record struct Position(int X, int Y)
    {
        public long TuningFrequency => X * 4_000_000L + Y;

        public static int operator -(Position a, Position b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        public static Position Parse(string line)
        {
            var match = PositionRegex().Match(line);
            return new(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value));
        }

        [GeneratedRegex(@".+x=(?'x'[\-\d]+).+y=(?'y'[\-\d]+)")]
        public static partial Regex PositionRegex();
    }
}
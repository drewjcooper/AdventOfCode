using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    public class SolverV : Solver
    {
        private readonly IEnumerable<Command> commands;

        public SolverV(PuzzleInput input)
        {
            commands = input.Lines.Select(l => Command.Parse(l));
        }

        protected override string SolvePart1()
        {
            var core = new HashSet<(int, int, int)>();
            var within = new Range1D(-50, 50);
            foreach (var command in commands)
            {
                command.Execute(core, within);
            }
            return core.Count.ToString();
        }

        protected override string SolvePart2()
        {
            var onRegions = new HashSet<Range3D>();
            foreach (var command in commands)
            {
                command.Execute(onRegions);
            }
            return onRegions.Select(r => r.GetCubeCount()).Sum().ToString();
        }

        private abstract class Command
        {
            protected Command(Range3D range)
            {
                Range = range;
            }

            protected Range3D Range { get; }

            public void Execute(HashSet<Range3D> onRegions)
            {
                var toAdd = new List<Range3D>();
                var toRemove = new List<Range3D>();

                foreach (var region in onRegions)
                {
                    var intersection = Range.Intersect(region);
                    if (intersection is not null)
                    {
                        toRemove.Add(region);
                        if (intersection.Value != region) { toAdd.AddRange(region.GetRemainder(intersection.Value)); }
                    }
                }

                onRegions.ExceptWith(toRemove);
                onRegions.UnionWith(toAdd);

                if (AddRangeToList) { onRegions.Add(Range); }
            }

            public abstract bool AddRangeToList { get; }

            public void Execute(HashSet<(int, int, int)> core, Range1D within)
            {
                foreach (var cube in Range.GetCubes(within))
                {
                    Execute(core, cube);
                }
            }

            public abstract void Execute(HashSet<(int, int, int)> core, (int, int, int) cube);

            public static Command Parse(string text)
            {
                var parts = text.Split(' ');
                var range = Range3D.Parse(parts[1]);
                return parts[0] switch
                {
                    "on" => new TurnOn(range),
                    "off" => new TurnOff(range)
                };
            }
        }

        private class TurnOn : Command
        {
            public TurnOn(Range3D range)
                : base(range)
            {
            }

            public override void Execute(HashSet<(int, int, int)> core, (int, int, int) cube)
            {
                core.Add(cube);
            }

            public override bool AddRangeToList => true;
        }

        private class TurnOff : Command
        {
            public TurnOff(Range3D range)
                : base(range)
            {
            }

            public override void Execute(HashSet<(int, int, int)> core, (int, int, int) cube)
            {
                core.Remove(cube);
            }

            public override bool AddRangeToList => false;
        }

        public record struct Range3D(Range1D X, Range1D Y, Range1D Z)
        {
            public Range3D(int x1, int x2, int y1, int y2, int z1, int z2)
                : this(new(x1, x2), new(y1, y2), new(z1, z2))
            {
            }

            public IEnumerable<(int, int, int)> GetCubes(Range1D within)
            {
                var range = this;
                return
                    from x in range.X.GetCoordinates(within)
                    from y in range.Y.GetCoordinates(within)
                    from z in range.Z.GetCoordinates(within)
                    select (x, y, z);
            }

            public long GetCubeCount() => X.GetCubeCount() * Y.GetCubeCount() * Z.GetCubeCount();

            public static Range3D Parse(string text)
            {
                var ranges = text.Split(new[] { ',' }).Select(s => Range1D.Parse(s)).ToArray();
                return new(ranges[0], ranges[1], ranges[2]);
            }

            public Range3D? Intersect(Range3D other)
            {
                var xRange = X.Intersect(other.X);
                var yRange = Y.Intersect(other.Y);
                var zRange = Z.Intersect(other.Z);

                return xRange.HasValue && yRange.HasValue && zRange.HasValue
                    ? new(xRange.Value, yRange.Value, zRange.Value)
                    : null;
            }

            public IEnumerable<Range3D> GetRemainder(Range3D removal)
            {
                var remaining = this;
                if (remaining.X.From < removal.X.From)
                {
                    yield return SliceX(new(remaining.X.From, removal.X.From - 1), new(removal.X.From, remaining.X.To));
                }
                if (remaining.X.To > removal.X.To)
                {
                    yield return SliceX(new(removal.X.To + 1, remaining.X.To), new(remaining.X.From, removal.X.To));
                }
                if (remaining.Y.From < removal.Y.From)
                {
                    yield return SliceY(new(remaining.Y.From, removal.Y.From - 1), new(removal.Y.From, remaining.Y.To));
                }
                if (remaining.Y.To > removal.Y.To)
                {
                    yield return SliceY(new(removal.Y.To + 1, remaining.Y.To), new(remaining.Y.From, removal.Y.To));
                }
                if (remaining.Z.From < removal.Z.From)
                {
                    yield return SliceZ(new(remaining.Z.From, removal.Z.From - 1), new(removal.Z.From, remaining.Z.To));
                }
                if (remaining.Z.To > removal.Z.To)
                {
                    yield return SliceZ(new(removal.Z.To + 1, remaining.Z.To), new(remaining.Z.From, removal.Z.To));
                }

                Range3D SliceX(Range1D slice, Range1D remainder)
                {
                    var result = remaining with { X = slice };
                    remaining = remaining with { X = remainder };
                    return result;
                }

                Range3D SliceY(Range1D slice, Range1D remainder)
                {
                    var result = remaining with { Y = slice };
                    remaining = remaining with { Y = remainder };
                    return result;
                }

                Range3D SliceZ(Range1D slice, Range1D remainder)
                {
                    var result = remaining with { Z = slice };
                    remaining = remaining with { Z = remainder };
                    return result;
                }
            }
        }

        public record struct Range1D(int From, int To)
        {
            public IEnumerable<int> GetCoordinates(Range1D within)
            {
                if (From > within.To || To < within.From) { yield break; }
                var from = Math.Max(From, within.From);
                var to = Math.Min(To, within.To);
                for (int c = from; c <= to; c++) { yield return c; }
            }

            public static Range1D Parse(string text)
            {
                var limits = text.Split(new[] { "=", ".." }, default).Skip(1).Select(s => int.Parse(s)).ToArray();
                if (limits[0] > limits[1]) { throw new Exception($"Limits out of order: {text}"); }
                return new(limits[0], limits[1]);
            }

            public long GetCubeCount() => To - From + 1;

            public Range1D? Intersect(Range1D other) =>
                other switch
                {
                    _ when From >= other.From && To <= other.To => this,
                    _ when From <= other.From && To >= other.To => other,
                    _ when From >= other.From && From <= other.To => new(From, other.To),
                    _ when From <= other.From && To >= other.From => new(other.From, To),
                    _ => null
                };

        }
    }
}

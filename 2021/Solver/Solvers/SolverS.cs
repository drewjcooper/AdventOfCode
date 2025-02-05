using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;
using static AdventOfCode2021.Solvers.SolverS;

namespace AdventOfCode2021.Solvers
{
    internal class SolverS : Solver
    {
        private readonly List<Scanner> input;

        public SolverS(PuzzleInput input)
        {
            this.input = input.Lines.ParseScanners().ToList();
        }

        protected override string SolvePart1()
        {
            var map = new Map(input[0]);
            map.LocateScanners(input.Skip(1));
            return map.BeaconCount.ToString();
        }

        protected override string SolvePart2()
        {
            var map = new Map(input[0]);
            map.LocateScanners(input.Skip(1));
            return map.GetMaxDistance().ToString();
        }

        internal class Map
        {
            private readonly List<Scanner> locatedScanners = new();
            private readonly HashSet<Vector> beacons;

            public Map(Scanner root)
            {
                locatedScanners.Add(root);
                beacons = root.Beacons.ToHashSet();
            }

            public IEnumerable<Vector> Beacons => beacons;
            public int BeaconCount => beacons.Count;

            public void LocateScanners(IEnumerable<Scanner> others)
            {
                var otherScanners = others.ToList();

                for (var l = 0; l < locatedScanners.Count; l++)
                {
                    var knownScanner = locatedScanners[l];

                    for (var o = 0; o < otherScanners.Count;)
                    {
                        var scanner = otherScanners[o];
                        if (scanner.TryAlignTo(knownScanner))
                        {
                            foreach (var beacon in scanner.Beacons)
                            {
                                beacons.Add(beacon);
                            }
                            locatedScanners.Add(scanner);
                            otherScanners.Remove(scanner);
                            continue;
                        }
                        o++;
                    }
                }
            }

            public int GetMaxDistance() =>
                (from a in locatedScanners
                from b in locatedScanners
                select a.DistanceTo(b)).Max();
        }

        internal class Scanner
        {
            private readonly List<Vector> beacons = new();
            private Dictionary<Vector, Dictionary<Vector, Vector>> beaconVectors = new();

            public Vector Location { get; private set; }

            public IEnumerable<Vector> Beacons => beacons.Select(b => b + Location);

            public void Add(Vector beacon)
            {
                beaconVectors[beacon] = new();
                foreach (var previous in beacons)
                {
                    var hash = (previous - beacon).Abs();
                    beaconVectors[beacon][hash] = previous;
                    beaconVectors[previous][hash] = beacon;
                }
                beacons.Add(beacon);
            }

            public bool TryAlignTo(Scanner other)
            {
                if (TryMatch(other, out var match))
                {
                    AlignTo(other, match);
                    return true;
                }
                return false;
            }

            private bool TryMatch(Scanner other, out ScannerMatch match)
            {
                foreach (var localBeacon in beacons)
                {
                    foreach (var otherBeacon in other.beacons)
                    {
                        var commonVectors =
                            beaconVectors[localBeacon].Keys.Intersect(other.beaconVectors[otherBeacon].Keys);
                        if (commonVectors.Count() >= 11)
                        {
                            match = new(localBeacon, otherBeacon, commonVectors.First(v => v.CanAlign()));
                            return true;
                        }
                    }
                }

                match = default;
                return false;
            }

            public void AlignTo(Scanner other, ScannerMatch match)
            {
                var localBeacon = match.LocalBeacon;
                var otherBeacon = match.OtherBeacon;

                var localVector = beaconVectors[localBeacon][match.AlignmentVector] - localBeacon;
                var otherVector = other.beaconVectors[otherBeacon][match.AlignmentVector] - otherBeacon;

                if (localVector != otherVector)
                {
                    var rotation = new Rotation(localVector, otherVector);
                    for (var i = 0; i < beacons.Count; i++)
                    {
                        beacons[i] = rotation.Apply(beacons[i]);
                    }
                    beaconVectors = beaconVectors.ToDictionary(
                        v => rotation.Apply(v.Key),
                        v => v.Value.ToDictionary(x => x.Key, x => rotation.Apply(x.Value)));
                    localBeacon = rotation.Apply(localBeacon);
                }

                Location = otherBeacon - localBeacon + other.Location;
            }

            public int DistanceTo(Scanner other) =>
                Math.Abs(Location.X - other.Location.X) +
                Math.Abs(Location.Y - other.Location.Y) +
                Math.Abs(Location.Z - other.Location.Z);
        }

        internal record struct ScannerMatch(Vector LocalBeacon, Vector OtherBeacon, Vector AlignmentVector);

        internal class Rotation
        {
            private readonly int[][] matrix;

            public Rotation(Vector from, Vector to)
            {
                matrix = new[]
                {
                    new[]
                    {
                        from.X == to.X ? 1 : from.X == -to.X ? -1 : 0,
                        from.Y == to.X ? 1 : from.Y == -to.X ? -1 : 0,
                        from.Z == to.X ? 1 : from.Z == -to.X ? -1 : 0
                    },
                    new[]
                    {
                        from.X == to.Y ? 1 : from.X == -to.Y ? -1 : 0,
                        from.Y == to.Y ? 1 : from.Y == -to.Y ? -1 : 0,
                        from.Z == to.Y ? 1 : from.Z == -to.Y ? -1 : 0
                    },
                    new[]
                    {
                        from.X == to.Z ? 1 : from.X == -to.Z ? -1 : 0,
                        from.Y == to.Z ? 1 : from.Y == -to.Z ? -1 : 0,
                        from.Z == to.Z ? 1 : from.Z == -to.Z ? -1 : 0
                    }
                };
            }

            public Vector Apply(Vector from) =>
                new(
                    from.X * matrix[0][0] + from.Y * matrix[0][1] + from.Z * matrix[0][2],
                    from.X * matrix[1][0] + from.Y * matrix[1][1] + from.Z * matrix[1][2],
                    from.X * matrix[2][0] + from.Y * matrix[2][1] + from.Z * matrix[2][2]);
        }

        internal record struct Vector(int X, int Y, int Z)
        {
            public Vector Abs()
            {
                var x = Math.Abs(X);
                var y = Math.Abs(Y);
                var z = Math.Abs(Z);

                if (x <= y)
                {
                    if (y <= z) { return new(x, y, z); }
                    if (z <= x) { return new(z, x, y); }
                    return new(x, z, y);
                }
                else
                {
                    if (x <= z) { return new(y, x, z); }
                    if (z <= y) { return new(z, y, x); }
                    return new(y, z, x);
                }
            }

            public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            public static Vector operator -(Vector a, Vector b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

            public static Vector Parse(string line)
            {
                var values = line.Split(',').Select(s => int.Parse(s)).ToArray();
                return new(values[0], values[1], values[2]);
            }

            public bool CanAlign() => X != 0 && X != Y && Y != Z;
        }
    }

    internal static class SolverSExtensions
    {
        public static IEnumerable<Scanner> ParseScanners(this IEnumerable<string> input)
            => new Queue<string>(input).ParseScanners();

        public static IEnumerable<Scanner> ParseScanners(this Queue<string> input)
        {
            Scanner scanner = null;

            while (input.TryDequeue(out var line))
            {
                if (line.StartsWith("---")) { scanner = new Scanner(); }
                else if (line == string.Empty) { yield return scanner; }
                else { scanner.Add(Vector.Parse(line)); }
            }

            yield return scanner;
        }
    }
}

using System.Collections.Immutable;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverT(PuzzleInput input) : Solver
{
    private readonly int _threshhold1 = input.Lines.Length > 20 ? 100 : 10;
    private readonly int _threshhold2 = input.Lines.Length > 20 ? 100 : 50;
    private readonly Racetrack _racetrack = Racetrack.Parse(input.Lines);

    protected override Answer SolvePart1() => _racetrack.FindCheatCount(_threshhold1);

    protected override Answer SolvePart2() => _racetrack.FindCheatCount(_threshhold2, 20);

    private class Racetrack(Location start, Location end, IEnumerable<string> map)
    {
        private readonly Location _start = start;
        private readonly Location _end = end;
        private readonly ImmutableArray<string> _map = map.ToImmutableArray();
        private readonly Dictionary<Location, int> _timing = [];
        private readonly List<Location> _path = [];

        public int FindCheatCount(int threshhold, int maxLength = 2)
        {
            var location = _start;
            var time = 0;
            var cheatCount = 0;

            while (location != _end)
            {
                _path.Add(location);
                _timing[location] = time++;
                var nextLocation = location.GetNeighbours().Single(IsNext);

                if (time > threshhold)
                {
                    for (int i = 0; i < time - threshhold; i++)
                    {
                        var potentialCheatStart = _path[i];
                        var potentialCheatDuration = potentialCheatStart.GetCheatDuration(nextLocation);
                        if (potentialCheatDuration <= maxLength)
                        {
                            var cheatSaving = time - i - potentialCheatDuration;
                            if (cheatSaving >= threshhold)
                            {
                                cheatCount++;
                            }
                        }
                    }
                }
                location = nextLocation;
            }

            return cheatCount;
        }

        private bool IsNext(Location location) => _map[location.Y][location.X] != '#' && !_timing.ContainsKey(location);

        public static Racetrack Parse(string[] lines)
        {
            Location start = default, end = default;

            for (byte x = 0; x < lines[0].Length; x++)
            {
                for (byte y = 0; y < lines.Length; y++)
                {
                    if (lines[y][x] == 'S') { start = new(x, y); }
                    if (lines[y][x] == 'E') { end = new(x, y); }
                }
            }

            return new(start, end, lines);
        }
    }

    private record struct Location(byte X, byte Y)
    {
        public Location(int x, int y) : this((byte)x, (byte)y) { }

        public readonly IEnumerable<Location> GetNeighbours()
        {
            yield return new(X, Y - 1);
            yield return new(X + 1, Y);
            yield return new(X, Y + 1);
            yield return new(X - 1, Y);
        }

        public readonly IEnumerable<Location> GetPotentialCheats()
        {
            yield return new(X, Y - 2);
            yield return new(X + 2, Y);
            yield return new(X, Y + 2);
            yield return new(X - 2, Y);
        }

        public readonly int GetCheatDuration(Location destination) 
            => Math.Abs(destination.X - X) + Math.Abs(destination.Y - Y);
    }

    private record struct Cheat(Location Start, Location End, int Saving);
}

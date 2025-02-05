using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverJ(PuzzleInput input) : Solver
{
    private readonly Map _map = new(input.Lines);

    protected override Answer SolvePart1() => _map.ScoreTrailheads().Sum();

    protected override Answer SolvePart2() => _map.RateTrailheads().Sum();

    private class Map(string[] lines)
    {
        private readonly byte[][] _heights = lines.Select(l => l.Select(ch => (byte)(ch - '0')).ToArray()).ToArray();
        private readonly Dictionary<Location, int> _peakReachability = [];
        private readonly Dictionary<Location, int> _locationRating = [];
        private readonly List<Location> _trailheads = [];

        private int Width => _heights.Length;
        private int Height => _heights[0].Length;

        private int GetHeight(Location location) => _heights[location.X][location.Y];

        private IEnumerable<Location> GetNeighbours(Location location)
        {
            if (location.X > 0) { yield return new(location.X - 1, location.Y); }
            if (location.Y > 0) { yield return new(location.X, location.Y - 1); }
            if (location.X < Width - 1) { yield return new(location.X + 1, location.Y); }
            if (location.Y < Height - 1) { yield return new(location.X, location.Y + 1); }  
        }

        public IEnumerable<int> ScoreTrailheads()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var location = new Location(x, y);

                    if (GetHeight(location) == 0)
                    {
                        _trailheads.Add(location);
                        continue;
                    }

                    if (GetHeight(location) == 9)
                    {
                        foreach (var priorStep in GetDistinctPriorSteps(location).Distinct())
                        {
                            _peakReachability[priorStep] = _peakReachability.GetValueOrDefault(priorStep) + 1;
                        }
                        continue;
                    }
                }
            }

            foreach (var trailhead in _trailheads)
            {
                yield return _peakReachability[trailhead];
            }
        }

        public IEnumerable<int> RateTrailheads()
        {

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var location = new Location(x, y);

                    if (GetHeight(location) == 0)
                    {
                        _trailheads.Add(location);
                        continue;
                    }

                    if (GetHeight(location) == 9)
                    {
                        _locationRating[location] = 1;
                        foreach (var priorStep in GetAllPriorSteps(location))
                        {
                            _locationRating[priorStep] = _locationRating.GetValueOrDefault(priorStep) + 1;
                        }
                        continue;
                    }
                }
            }

            foreach (var trailhead in _trailheads)
            {
                yield return _locationRating[trailhead];
            }
        }

        private IEnumerable<Location> GetDistinctPriorSteps(Location location)
        {
            if (GetHeight(location) == 0) { return []; }
            var priorSteps = GetPriorSteps(location);
            return priorSteps.Concat(priorSteps.SelectMany(GetDistinctPriorSteps).Distinct());
        }

        private IEnumerable<Location> GetAllPriorSteps(Location location)
        {
            if (GetHeight(location) == 0) { return []; }
            var priorSteps = GetPriorSteps(location);
            return priorSteps.Concat(priorSteps.SelectMany(GetAllPriorSteps));
        }

        private IEnumerable<Location> GetPriorSteps(Location location)
            => GetNeighbours(location).Where(l => GetHeight(l) == GetHeight(location) - 1);

        private record struct Location(int X, int Y);
    }
}

using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverK(PuzzleInput input) : Solver
{
    private readonly Stones _stones = new(input.RawText.Split(' '));

    protected override Answer SolvePart1() => _stones.Blink(25);
    protected override Answer SolvePart2() => _stones.Blink(75);

    private class Stones(IEnumerable<string> stones)
    {
        private readonly IEnumerable<string> _stones = stones;
        private readonly Dictionary<(string, int), long> _cache = new();
        private long _lookups = 0;
        private long _cacheHits = 0;

        public long Blink(int times)
        {
            var result = _stones.Select(s => Blink(s, times)).Sum();
            Console.WriteLine($"{_cacheHits}/{_lookups} = {100.0 * _cacheHits / _lookups}");
            return result;
        }

        private long Blink(string stone, int times)
        {
            if (times == 0) { return 1; }
            _lookups++;
            if (_cache.TryGetValue((stone, times), out var cached)) 
            {
                _cacheHits++;
                return cached; 
            }

            var stones = Blink(stone);
            var count = Blink(stones.Original, times - 1) + (stones.New is null ? 0 : Blink(stones.New, times - 1));
            _cache[(stone, times)] = count;
            return count;
        }

        private static (string Original, string? New) Blink(string stone)
        {
            return stone == "0" ? ("1", null) : Split() ?? ($"{long.Parse(stone) * 2024}", null);

            (string, string?)? Split()
            {
                if (stone.Length % 2 != 0) { return null; }

                var left = stone[..(stone.Length / 2)];
                var right = stone[(stone.Length / 2)..].TrimStart('0');

                return (left, right == "" ? "0" : right);
            }
        }
    }
}

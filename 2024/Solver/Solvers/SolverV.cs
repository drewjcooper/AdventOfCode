using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverV(PuzzleInput input) : Solver
{
    private readonly IEnumerable<Buyer> _buyers = input.Lines.Select((l, i) => new Buyer(i, int.Parse(l))).ToList();

    protected override Answer SolvePart1() => _buyers.Sum(b => b.GetSecretNumber(2000));

    protected override Answer SolvePart2()
    {
        _buyers.Select(b => b.GetSecretNumber(2000)).ToList();
        return Sequences.GetMostBananas();
    }

    private class Buyer(int id, int secret)
    {
        private readonly int _id = id;
        private Secret _secret = new(secret);
        private int _previousPrice = secret % 10;
        private (int, int, int, int) _diffs;

        public long GetSecretNumber(int number)
        {
            for (int i = 0; i < number; i++)
            {
                var price = (int)(GetNextSecret() % 10);
                _diffs = (_diffs.Item2, _diffs.Item3, _diffs.Item4, price - _previousPrice);

                if (i >= 3) { Sequences.Add(_diffs, _id, price); }

                _previousPrice = price;
            }

            return _secret.Value;
        }

        public long GetNextSecret() => (_secret = _secret * 64 / 32 * 2048).Value;
    }

    private class Sequences
    {
        private static readonly Dictionary<(int, int, int, int), Dictionary<int, int>> _sequences = [];

        public static void Add((int, int, int, int) diffs, int id, int price)
        {
            if (!_sequences.TryGetValue(diffs, out var buyerPrice))
            {
                _sequences[diffs] = buyerPrice = [];
            }

            buyerPrice.TryAdd(id, price);
        }

        public static int GetMostBananas() => _sequences.Values.Max(s => s.Values.Sum());
    }

    private record struct Secret(long Value)
    {
        public static Secret operator *(Secret secret, long multiplier)
            => new(Prune(secret.Mix(secret.Value * multiplier)));

        public static Secret operator /(Secret secret, long multiplier)
            => new(Prune(secret.Mix(secret.Value / multiplier)));

        private readonly long Mix(long value) => value ^ Value;

        private static long Prune(long value) => value % 16777216;
    }
}

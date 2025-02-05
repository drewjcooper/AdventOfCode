using System.Collections.Immutable;
using AdventOfCode.Input;
using AdventOfCode.Solvers;

internal class SolverD(PuzzleInput input) : Solver
{
    private readonly IEnumerable<Card> _cards = input.Lines.Select(Card.Parse);

    protected override Answer SolvePart1() => _cards.Sum(c => c.Points);

    protected override Answer SolvePart2()
    {
        var cardCounts = new Dictionary<int, int>();
        foreach (var card in _cards)
        {
            var count = IncrementCount(card.Id);
            for (int i = 1; i <= card.WinCount; i++)
            {
                IncrementCount(card.Id + i, count);
            }
        }

        return cardCounts.Values.Sum();

        int IncrementCount(int cardId, int by = 1) => cardCounts[cardId] = cardCounts.GetValueOrDefault(cardId) + by;
    }

    private class Card
    {
        private Card(int id, IEnumerable<int> winningNumbers, IEnumerable<int> givenNumbers)
        {
            WinCount = winningNumbers.ToImmutableHashSet().Intersect(givenNumbers).Count;
            Id = id;
        }

        public int Id { get; }

        public int WinCount { get; }

        public int Points => WinCount == 0 ? 0 : 1 << (WinCount - 1);
    
        public static Card Parse(string line)
        {
            var parts = line.Split(new[] { ':', '|' }, StringSplitOptions.TrimEntries);

            return new(
                int.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]),
                ParseNumbers(parts[1]),
                ParseNumbers(parts[2]));
            
            static IEnumerable<int> ParseNumbers(string line) 
                => line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        }
    }
}
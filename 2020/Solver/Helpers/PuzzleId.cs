using System.Text.RegularExpressions;

namespace AdventOfCode2020.Helpers
{
    public record PuzzleId
    {
        private static readonly Regex puzzleIdMatcher = new Regex(@"^[a-z][12]$", RegexOptions.IgnoreCase);

        private PuzzleId(string id, bool isValid)
        {
            IsValid = isValid;
            Id = id.ToUpper();

            if (isValid)
            {
                Code = Id[0];
                Part = int.Parse(Id.Substring(1));
                Day = Code - 'A' + 1;
            }
        }

        public bool IsValid { get; }
        public int Day { get; }
        public char Code { get; }
        public int Part { get; }
        public string Id { get; }

        public static PuzzleId Parse(string candidate) =>
            new PuzzleId(candidate, puzzleIdMatcher.IsMatch(candidate));
    }
}

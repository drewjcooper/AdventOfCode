using AdventOfCode.Helpers;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal partial class SolverE : Solver
{
    private readonly RuleGraph _rules;
    private readonly IEnumerable<Update> _updates;

    public SolverE(PuzzleInput input)
    {
        var (rules, updates) = input.Lines.Split("").ToArray();
        _rules = new RuleGraph(rules);
        _updates = updates.Select(Update.Parse);
    }

    protected override Answer SolvePart1() => _updates.Where(u => u.IsValid(_rules)).Sum(u => u.MiddlePage);

    protected override Answer SolvePart2() 
        => _updates.Where(u => !u.IsValid(_rules)).Select(u => u.Fix(_rules)).Sum(u => u.MiddlePage);

    private class RuleGraph
    {
        private readonly Dictionary<int, HashSet<int>> _pageFollowers = [];

        public RuleGraph(IEnumerable<string> rules)
        {
            foreach (var (x, y) in rules.Select(Rule.Parse))
            {
                if (!_pageFollowers.TryGetValue(x, out var followers))
                {
                    _pageFollowers[x] = followers = [];
                }

                followers.Add(y);
            }
        }

        public bool IsValidOrder(int first, int second)
            => _pageFollowers.TryGetValue(first, out var followers) && followers.Contains(second);

        private static class Rule
        {
            public static (int, int) Parse(string line)
            {
                var values = line.Split('|');
                return (int.Parse(values[0]), int.Parse(values[1]));
            }
        }
    }

    private class Update(int[] pages)
    {
        private readonly int[] _pages = pages;

        public static Update Parse(string line) => new(line.Split(',').Select(int.Parse).ToArray());

        public bool IsValid(RuleGraph rules) => _pages.Zip(_pages.Skip(1), rules.IsValidOrder).All(b => b);

        public Update Fix(RuleGraph rules)
        {
            Array.Sort(_pages, new PageComparer(rules));
            return this;
        }

        public int MiddlePage => _pages[_pages.Length / 2];

        private class PageComparer(RuleGraph rules) : IComparer<int>
        {
            private readonly RuleGraph _rules = rules;

            public int Compare(int x, int y) => _rules.IsValidOrder(x, y) ? -1 : 1;
        }
    }
}

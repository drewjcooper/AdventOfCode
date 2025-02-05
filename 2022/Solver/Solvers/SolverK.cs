using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal partial class SolverK : Solver
{
    private readonly List<Monkey> _monkeys = new();

    public SolverK(PuzzleInput input)
    {
        _monkeys = Monkey.Parse(input.RawText).ToList();
    }
    
    protected override string SolvePart1() => GetMonkeyBusiness(20, 3).ToString();

    protected override string SolvePart2() => GetMonkeyBusiness(10000, 1).ToString();

    public long GetMonkeyBusiness(int rounds, int worryFactor)
    {
        var modulus = _monkeys.Select(m => m.Divisor).Aggregate(1L, (a, i) => a * i);

        for (int r = 0; r < rounds; r++)
        {
            foreach(var monkey in _monkeys)
            {
                foreach (var action in monkey.GetActions(worryFactor, modulus))
                {
                    action.Invoke(_monkeys);
                }
            }
        }

        return _monkeys.Select(m => m.ItemsInspected).OrderDescending().Take(2).Aggregate(1L, (a, i) => a * i);
    }

    public partial class Monkey
    {
        private readonly Queue<long> _items = new();
        private readonly Func<long, long> _operation;
        private readonly Dictionary<bool, int> _next;

        public Monkey(IEnumerable<long> items, Func<long, long> operation, int divisor, int nextIfTrue, int nextIfFalse)
        {
            _items = new Queue<long>(items);
            _operation = operation;
            Divisor = divisor;
            _next = new() { [true] = nextIfTrue, [false] = nextIfFalse };
        }

        public int Divisor { get; }
        public int ItemsInspected { get; private set; }

        public IEnumerable<Action<List<Monkey>>> GetActions(int worryFactor, long modulus)
        {
            while(_items.TryDequeue(out var item))
            {
                ItemsInspected += 1;
                item = _operation.Invoke(item) / worryFactor;
                yield return monkeys => monkeys[_next[item % Divisor == 0]].Catch(item % modulus);
            }
        }

        public void Catch(long item) => _items.Enqueue(item);

        [GeneratedRegex("""
            Monkey\s\d:
              .+?items:\s(?'items'[\d\s,]+)
              .+?old\s(?'operation'.)\s(?'operand'\w+)
              .+?by\s(?'divisor'\d+)
              .+?true.+?(?'ifTrue'\d)
              .+?false.+?(?'ifFalse'\d)
            """, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace)]
        public static partial Regex MonkeyRegex();

        public static IEnumerable<Monkey> Parse(string input)
        {
            foreach (Match match in MonkeyRegex().Matches(input))
            {
                yield return new Monkey(
                    match.Groups["items"].Value.Split(',').Select(i => long.Parse(i.Trim())),
                    ParseOperation(match.Groups["operation"].Value, match.Groups["operand"].Value),
                    int.Parse(match.Groups["divisor"].Value),
                    int.Parse(match.Groups["ifTrue"].Value),
                    int.Parse(match.Groups["ifFalse"].Value));
            }

            Func<long, long> ParseOperation(string operation, string operand)
                => (operation, long.TryParse(operand, out var value)) switch
                {
                    ("+", _) => i => i + value,
                    ("*", true) => i => i * value,
                    _ => i => i * i
                };
        }
    }
}
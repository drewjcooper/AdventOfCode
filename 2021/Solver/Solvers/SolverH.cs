using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverH : Solver
    {
        private readonly IEnumerable<string> input;

        public SolverH(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1()
        {
            var targetCounts = new HashSet<int>(new[] { 2, 3, 4, 7 });
            return input.SelectMany(l => l.Split('|')[1].Trim().Split(' '))
                .Count(d => targetCounts.Contains(d.Length))
                .ToString();
        }

        protected override string SolvePart2()
            => input.Select(l => l.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Select(d => new SevenSegmentDisplay(d)).ToArray())
                .Select(d => Evaluate(d[..10], d[10..]))
                .Select(o => o.GetNumber())
                .Sum()
                .ToString();

        private Output Evaluate(SevenSegmentDisplay[] patterns, SevenSegmentDisplay[] digits)
        {
            var output = new Output(digits);
            var patternsBySegmentCount = patterns.ToLookup(p => p.Count);

            output.Add(patternsBySegmentCount[2].First(), 1);
            output.Add(patternsBySegmentCount[3].First(), 7);
            output.Add(patternsBySegmentCount[4].First(), 4);
            output.Add(patternsBySegmentCount[7].First(), 8);

            var fiveSegmentPatterns = patternsBySegmentCount[5].ToHashSet();
            var sixSegmentPatterns = patternsBySegmentCount[6].ToHashSet();

            output.Add(sixSegmentPatterns.First(p => !p.Contains(output[1])), 6);
            sixSegmentPatterns.Remove(output[6]);

            output.Add(fiveSegmentPatterns.First(p => p.Contains(output[1])), 3);
            fiveSegmentPatterns.Remove(output[3]);

            output.Add(fiveSegmentPatterns.First(p => output[6].Contains(p)), 5);
            fiveSegmentPatterns.Remove(output[5]);

            output.Add(fiveSegmentPatterns.Single(), 2);

            output.Add(sixSegmentPatterns.First(p => p.Contains(output[5])), 9);
            sixSegmentPatterns.Remove(output[9]);

            output.Add(sixSegmentPatterns.Single(), 0);

            return output;
        }

        internal record Output(SevenSegmentDisplay[] Digits)
        {
            private readonly Dictionary<SevenSegmentDisplay, int> values = new();
            private readonly Dictionary<int, SevenSegmentDisplay> patterns = new();

            public SevenSegmentDisplay this[int i] => patterns[i];

            public void Add(SevenSegmentDisplay display, int value)
            {
                values[display] = value;
                patterns[value] = display;
            }

            public int GetNumber() => Digits.Aggregate(0, (a, d) => a * 10 + values[d]);
        }

        internal record SevenSegmentDisplay
        {
            private static Dictionary<char, int> segments = new()
            {
                ['a'] = 1, ['b'] = 2, ['c'] = 4, ['d'] = 8, ['e'] = 16, ['f'] = 32, ['g'] = 64
            };

            public SevenSegmentDisplay(string value)
            {
                Count = value.Length;
                Flags = value.Select(c => segments[c]).Sum();
            }
            public SevenSegmentDisplay(int flags)
            {
                Flags = flags;

                for (; flags > 0; flags >>= 1)
                {
                    Count += flags % 2;
                }
            }

            public int Flags { get; }
            public int Count { get; }

            public bool Contains(SevenSegmentDisplay other) => (Flags & other.Flags) == other.Flags;

            public static SevenSegmentDisplay operator -(SevenSegmentDisplay left, SevenSegmentDisplay right)
                => new SevenSegmentDisplay(left.Flags & ~right.Flags);
        }
    }
}

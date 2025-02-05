using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

using static AdventOfCode2021.Solvers.SolverJ.Status;

namespace AdventOfCode2021.Solvers
{
    internal class SolverJ : Solver
    {
        private readonly IEnumerable<string> input;
        private static readonly Dictionary<char, int> scoresCorrupted = new()
        {
            [')'] = 3,
            [']'] = 57,
            ['}'] = 1197,
            ['>'] = 25137
        };

        private static readonly Dictionary<char, int> scoresIncomplete = new()
        {
            [')'] = 1,
            [']'] = 2,
            ['}'] = 3,
            ['>'] = 4
        };

        public SolverJ(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1()
        {
            var scanner = new Scanner();
            return input.Select(l => scanner.Scan(l))
                .Where(r => r.Status == Corrupted)
                .Select(r => scoresCorrupted[r.Found])
                .Sum()
                .ToString();
        }

        protected override string SolvePart2()
        {
            var scanner = new Scanner();
            var scores = input.Select(l => scanner.Scan(l))
                .Where(r => r.Status == Incomplete)
                .Select(r => r.Completion.Aggregate(0L, (a, c) => a * 5 + scoresIncomplete[c]))
                .OrderBy(s => s)
                .ToArray();
            return scores[scores.Length/2].ToString();
        }


        internal class Scanner
        {
            private static readonly Dictionary<char, char> pairs = new()
            {
                ['('] = ')',
                ['['] = ']',
                ['{'] = '}',
                ['<'] = '>'
            };

            public Result Scan(string line)
            {
                var pendingClosures = new Stack<char>();

                foreach(var ch in line)
                {
                    if (pairs.TryGetValue(ch, out var closure))
                    {
                        pendingClosures.Push(closure);
                    }
                    else if (pendingClosures.TryPeek(out var expected))
                    {
                        if (ch == expected)
                        {
                            pendingClosures.Pop();
                        }
                        else
                        {
                            return new Result(Corrupted, expected, ch, default);
                        }
                    }
                    else
                    {
                        throw new Exception("Couldn't peek the stack");
                    }
                }

                return pendingClosures.Any()
                    ? new Result(Incomplete, default, default, pendingClosures)
                    : throw new Exception("No problem with line " + line);
            }
        }

        internal record Result(Status Status, char Expected, char Found, IEnumerable<char> Completion);

        internal enum Status
        {
            Ok,
            Corrupted,
            Incomplete,
            Unexpected
        }
    }
}

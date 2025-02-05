using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverG : Solver
    {
        private readonly string[] lines;

        public SolverG(PuzzleInput input)
        {
            lines = input.Lines;
        }

        protected override string SolvePart1()
        {
            var containersByBag = lines
                .Select(l => new BagRule(l))
                .SelectMany(br => br.Contains.Select(c => (Content: c, Container: br.Colour)))
                .ToLookup(x => x.Content.Colour, x => x.Container);

            var containersSeen = new HashSet<string>();
            var pendingBags = new Queue<string>(new[] { "shiny gold" });

            while (pendingBags.TryDequeue(out string bag))
            {
                foreach (var container in containersByBag[bag])
                {
                    if (containersSeen.Add(container))
                    {
                        pendingBags.Enqueue(container);
                    }
                }
            }

            return containersSeen.Count.ToString();
        }

        protected override string SolvePart2()
        {
            var bagRules = lines
                .Select(l => new BagRule(l))
                .ToDictionary(br => br.Colour, br => br.Contains);

            return CountBagsContained("shiny gold").ToString();

            int CountBagsContained(string colour)
            {
                int total = 0;
                foreach (var contained in bagRules[colour])
                {
                    total += (CountBagsContained(contained.Colour) + 1) * contained.Quantity;
                }
                return total;
            }
        }

        private class BagRule
        {
            private static Regex _rulePattern = new Regex(
                @"^
                (?<colour>[a-z]+\s[a-z]+)
                \sbags\scontain\s
                (
                  (
                    (?<quantity>\d+)
                    \s
                    (?<content>[a-z]+\s[a-z]+)
                    \sbags?(\.|,\s)
                  )*
                  |
                  no\sother\sbags.
                )$",
                RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

            public BagRule(string rule)
            {
                var match = _rulePattern.Match(rule);

                Colour = match.Groups["colour"].Value;
                Contains = match.Groups["quantity"].Captures.Select(c => int.Parse(c.Value))
                    .Zip(match.Groups["content"].Captures.Select(c => c.Value))
                    .ToList();
            }

            public string Colour { get; }
            public IEnumerable<(int Quantity, string Colour)> Contains { get; }
        }
    }
}

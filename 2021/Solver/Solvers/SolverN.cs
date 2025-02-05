using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverN : Solver
    {
        private readonly string template;
        private readonly Rules rules;

        public SolverN(PuzzleInput input)
        {
            var parts = input.Split();
            template = parts.Single();
            rules = parts.Aggregate(new Rules(), (r, l) => r.Add(l));
        }

        protected override string SolvePart1() => CalculateAnswer(10).ToString();

        protected override string SolvePart2() => CalculateAnswer(40).ToString();

        private long CalculateAnswer(int rounds)
        {
            rules.CalculateFutureCounts(rounds / 2);
            return template.DoInsertions(rules, rounds / 2).GetCounts(rules).GetRange();
        }

        internal class Rules : IEnumerable
        {
            private static readonly Regex pattern = new Regex(@"^([A-Z]){2} -> ([A-Z])$");
            private readonly Dictionary<char, Dictionary<char, char>> rules = new();
            private readonly Dictionary<char, Dictionary<char, Dictionary<char, long>>> futureCounts = new();

            private readonly HashSet<char> alphabet = new();

            public char this[char a, char b] => rules[a][b];
            public long this[char a, char b, char x] => futureCounts[a][b].GetValueOrDefault(x);
            public IEnumerable<char> Alphabet => alphabet;

            public Rules Add(string candidate)
            {
                var match = pattern.Match(candidate);
                if (!match.Success) { throw new ArgumentException($"Unexpected input: {candidate}"); }

                var a = match.Groups[1].Captures[0].Value[0];
                var b = match.Groups[1].Captures[1].Value[0];
                var insert = match.Groups[2].Captures[0].Value[0];

                if (!rules.ContainsKey(a))
                {
                    rules[a] = new();
                    futureCounts[a] = new();
                }

                rules[a][b] = insert;
                alphabet.Add(a);
                alphabet.Add(b);
                alphabet.Add(insert);

                return this;
            }

            public Rules CalculateFutureCounts(int rounds)
            {
                foreach (var a in rules.Keys)
                foreach (var b in rules[a].Keys)
                {
                    futureCounts[a][b] = $"{a}{b}".DoInsertions(this, rounds).Skip(1).GetCounts(alphabet);
                }
                return this;
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }

    internal static class InsertionExtensions
    {

        public static IEnumerable<char> DoInsertions(this string template, SolverN.Rules rules, int rounds)
        {
            var polymer = template.AsEnumerable();
            for (int i = 0; i < rounds; i++)
            {
                polymer = polymer.DoInsertions(rules);
            }
            return polymer;
        }

        public static IEnumerable<char> DoInsertions(this IEnumerable<char> source, SolverN.Rules rules)
        {
            var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext()) { yield break; }

            yield return enumerator.Current;
            var previous = enumerator.Current;

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return rules[previous, current];
                yield return current;
                previous = current;
            }
        }

        public static Dictionary<char, long> GetCounts(this IEnumerable<char> polymer, IEnumerable<char> alphabet)
        {
            var counts = alphabet.ToDictionary(c => c, _ => 0L);
            var enumerator = polymer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                counts[enumerator.Current] += 1;
            }

            return counts;
        }

        public static Dictionary<char, long> GetCounts(this IEnumerable<char> polymer, SolverN.Rules rules)
        {
            var counts = rules.Alphabet.ToDictionary(c => c, _ => 0L);
            var enumerator = polymer.GetEnumerator();
            enumerator.MoveNext();

            var a = enumerator.Current;
            counts[a] = 1;

            while (enumerator.MoveNext())
            {
                var b = enumerator.Current;
                foreach (var x in rules.Alphabet)
                {
                    counts[x] += rules[a, b, x];
                }
                a = b;
            }

            return counts;
        }

        public static long GetRange(this Dictionary<char, long> counts)
        {
            var orderedCounts = counts.Values.OrderBy(x => x).ToArray();
            return orderedCounts[^1] - orderedCounts[0];
        }
    }
}

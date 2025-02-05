using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverP : Solver
    {
        private readonly Rule[] rules;
        private readonly Ticket myTicket;
        private readonly Ticket[] otherTickets;
        private readonly TicketValidator validator;

        public SolverP(PuzzleInput input)
        {
            var inputSections = input.Lines.Split("").ToArray();

            rules = inputSections[0].Select(l => Rule.Parse(l)).ToArray();
            myTicket = Ticket.Parse(inputSections[1].Skip(1).First());
            otherTickets = inputSections[2].Skip(1).Select(l => Ticket.Parse(l)).ToArray();
            validator = new TicketValidator(rules);
        }

        protected override string SolvePart1() =>
            otherTickets.SelectMany(t => validator.GetInvalidValues(t)).Sum().ToString();

        protected override string SolvePart2()
        {
            var validTickets = otherTickets.Where(t => validator.IsValid(t)).ToArray();

            var matcher = new FieldMatcher(rules, myTicket);

            foreach (var ticket in validTickets)
            {
                matcher.Scan(ticket);
            }

            return matcher
                .GetFieldIndices("departure")
                .Select(f => myTicket[f])
                .Aggregate(1L, (a, v) => a * v)
                .ToString();
        }

        public class FieldMatcher
        {
            public readonly Dictionary<string, Rule> unmatchedRules;
            public readonly HashSet<int> unknownFields;
            public readonly Dictionary<int, HashSet<string>> possibleFields;

            public FieldMatcher(IEnumerable<Rule> rules, Ticket ticket)
            {
                unmatchedRules = rules.ToDictionary(r => r.Name, r => r);
                unknownFields = Enumerable.Range(0, ticket.Length).ToHashSet();
                possibleFields = unknownFields.ToDictionary(f => f, f => rules.Select(r => r.Name).ToHashSet());

                Possibilities = rules.ToDictionary(r => r.Name, r => unknownFields.ToHashSet());
                FieldIndices = new();
            }

            public Dictionary<string, HashSet<int>> Possibilities { get; }
            public Dictionary<string, int> FieldIndices { get; }
            public IEnumerable<int> GetFieldIndices(string prefix) =>
                FieldIndices.Where(x => x.Key.StartsWith(prefix)).Select(x => x.Value);

            public void Scan(Ticket ticket)
            {
                EliminatePossibilities(ticket);
                while (ProcessSinglePossibilities() || ProcessSinglePossibleFields());
            }

            private void EliminatePossibilities(Ticket ticket)
            {
                foreach (var (fieldName, rule) in unmatchedRules)
                {
                    foreach (var fieldIndex in unknownFields)
                    {
                        if (!rule.IsValid(ticket[fieldIndex]))
                        {
                            Possibilities[fieldName].Remove(fieldIndex);
                            possibleFields[fieldIndex].Remove(fieldName);
                        }
                    }
                }
            }

            private bool ProcessSinglePossibilities()
            {
                var singleFound = false;

                foreach (var fieldName in Possibilities.Keys.ToList())
                {
                    if (Possibilities[fieldName].Count != 1) { continue; }

                    singleFound = true;
                    var index = Possibilities[fieldName].Single();

                    FieldIndices[fieldName] = index;

                    RemoveKnownItems(index, fieldName);
                    foreach(var possibilities in Possibilities.Values)
                    {
                        possibilities.Remove(index);
                    }
                }

                return singleFound;
            }

            private bool ProcessSinglePossibleFields()
            {
                var singleFound = false;

                foreach (var index in possibleFields.Keys.ToList())
                {
                    if (possibleFields[index].Count != 1) { continue; }

                    singleFound = true;
                    var fieldName = possibleFields[index].Single();

                    FieldIndices[fieldName] = index;

                    RemoveKnownItems(index, fieldName);
                    foreach(var possibilities in possibleFields.Values)
                    {
                        possibilities.Remove(fieldName);
                    }
                }

                return singleFound;
            }

            private void RemoveKnownItems(int index, string fieldName)
            {
                possibleFields.Remove(index);
                unknownFields.Remove(index);
                Possibilities.Remove(fieldName);
                unmatchedRules.Remove(fieldName);
            }
        }

        public class TicketValidator
        {
            private readonly Rule matchesAtLeastOne;

            public TicketValidator(params Rule[] rules)
            {
                matchesAtLeastOne = rules.Aggregate((x, y) => x | y);
            }

            internal IEnumerable<int> GetInvalidValues(Ticket ticket) =>
                ticket.Values.Where(v => !matchesAtLeastOne.IsValid(v));

            internal bool IsValid(Ticket ticket) => ticket.Values.All(v => matchesAtLeastOne.IsValid(v));
        }

        public class Ticket
        {
            private readonly int[] values;

            public Ticket(params int[] values)
            {
                this.values = values;
            }

            public int this[int index] => values[index];

            public int Length => values.Length;

            public int[] Values => values;

            public static Ticket Parse(string text) => new Ticket(text.Split(',').Select(x => int.Parse(x)).ToArray());
        }

        public class Rule
        {
            public Rule(string name, params Range[] ranges)
            {
                Name = name;
                Ranges = ranges;
            }

            public string Name { get; }
            public Range[] Ranges { get; }

            private static readonly Regex pattern =
                new Regex(@"^(?<name>[^:]+):\s(?<range>\d+-\d+) or (?<range>\d+-\d+)");

            internal static Rule Parse(string text)
            {
                var match = pattern.Match(text);
                var ranges = match.Groups["range"].Captures.Select(c => Range.Parse(c.Value)).ToArray();
                return new Rule(match.Groups["name"].Value, ranges);
            }

            public static Rule operator |(Rule x, Rule y)
            {
                var ranges = x.Ranges.Concat(y.Ranges).OrderBy(r => r.From).ToArray();

                return new Rule("composite", Merge(ranges));
            }

            private static Range[] Merge(Range[] ranges)
            {
                var merged = new List<Range>();

                for (int i = 1; i < ranges.Length; i++)
                {
                    var range = ranges[i-1];
                    int j = i;
                    while (range.TryMerge(ranges[j], out var mergedRange))
                    {
                        range = mergedRange;
                        if (++j == ranges.Length) { break; }
                    }
                    merged.Add(range);
                    if (i + 1 == ranges.Length && range == ranges[i-1])
                    {
                        merged.Add(ranges[i]);
                    }
                    i = j;
                }
                return merged.ToArray();
            }

            internal bool IsValid(int value) => Ranges.Any(r => r.Contains(value));
        }

        public record Range(int From, int To)
        {
            public static Range Parse(string text)
            {
                var parts = text.Split('-');
                return new Range(int.Parse(parts[0]), int.Parse(parts[1]));
            }

            public bool Contains(int number) => From <= number && number <= To;

            public bool TryMerge(Range other, out Range merged)
            {
                if (Overlaps(other) || IsAdjacentTo(other))
                {
                    merged = new Range(Math.Min(From, other.From), Math.Max(To, other.To));
                    return true;
                }

                merged = default;
                return false;
            }

            internal bool Overlaps(Range other) =>
                Contains(other.From) || Contains(other.To) || other.Contains(From) || other.Contains(To);

            internal bool IsAdjacentTo(Range other) =>
                From < other.From && To + 1 == other.From ||
                From - 1 == other.To && To > other.To;

            public override string ToString() => $"{From}-{To}";
        }
    }
}

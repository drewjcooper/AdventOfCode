using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverS : Solver
    {
        //private readonly StringMatcherBuilder matcherBuilder;
        private readonly Grammar grammar;
        private readonly IEnumerable<string> data;

        public SolverS(PuzzleInput input)
        {
            var blocks = input.Lines.Split("").ToArray();
            grammar = new Grammar(blocks[0].Select(l => Rule.Parse(l)));
            data = blocks[1];
        }

        protected override string SolvePart1() => CountMatches(grammar.BuildTree());

        protected override string SolvePart2() =>
            "";// CountMatches(matcherBuilder.WithOverride("8: 42 | 42 8").WithOverride("11: 42 31 | 42 11 31"));

        private string CountMatches(StringMatcherBuilder matcherBuilder) =>
            data.Count(d => matcherBuilder.Build().Match(d)).ToString();

        private string CountMatches(Tree tree) =>
            data.Count(d => tree.Contains(d)).ToString();

        public class Rule
        {

            public Rule(string index)
            {
                Index = index;
            }

            public static Rule Parse(string rule)
            {
                var (index, pattern) = rule.Split(':', StringSplitOptions.TrimEntries);

                return pattern switch
                {
                    _ when pattern.Contains("\"") => ParseAtomRule(),
                    _ when pattern.Contains("|") => ParseOptionRule(),
                    _ => ParseSequenceRule()
                };

                Rule ParseAtomRule() => new AtomRule(index, pattern.Trim('\"')[0]);

                Rule ParseOptionRule() =>
                    new OptionRule(
                        index,
                        pattern.Split('|', StringSplitOptions.TrimEntries)
                            .Select(p => new SequenceRule(index, p.Split(' '))));

                Rule ParseSequenceRule() => new SequenceRule(index, pattern.Split(' '));
            }

            public string Index { get; }
            public IEnumerable<IEnumerable<string>> Sequences { get; }
        }

        public class OptionRule : Rule
        {
            public OptionRule(string index, IEnumerable<SequenceRule> rules)
                : base(index)
            {
                Rules = rules.ToArray();
            }

            public IEnumerable<SequenceRule> Rules { get; }
        }

        public class SequenceRule : Rule
        {
            public SequenceRule(string index, IEnumerable<string> rules)
                : base(index)
            {
                Rules = rules.ToArray();
            }

            public IEnumerable<string> Rules { get; }
        }

        public class AtomRule : Rule
        {
            public AtomRule(string index, char value)
                : base(index)
            {
                Value = value;
            }

            public char Value { get; }
        }

        public class Grammar
        {
            private readonly Dictionary<string, Rule> rules;
            private readonly int maxDepth;

            public Grammar(IEnumerable<Rule> rules, int maxDepth = 10)
            {
                this.rules = rules.ToDictionary(r => r.Index, r => r);
            }

            public Grammar WithOverride(Rule rule)
            {
                rules[rule.Index] = rule;
                return this;
            }

            public Tree BuildTree()
            {
                var tree = new Tree();
                var rule = rules["0"];
                BuildTree(tree, rule).ToList();

                return tree;
            }

            private IEnumerable<Tree> BuildTree(Tree current, Rule rule)
            {
                return (rule switch
                {
                    AtomRule atom => BuildTree(current, atom),
                    SequenceRule sequence => BuildTree(current, sequence),
                    OptionRule option => BuildTree(current, option)
                }).ToList();
            }

            private IEnumerable<Tree> BuildTree(Tree current, AtomRule rule)
            {
                if (current.Depth <= maxDepth)
                {
                    yield return current.Add(rule.Value);
                }
            }

            private IEnumerable<Tree> BuildTree(Tree tree, SequenceRule rule)
            {
                var tails = Enumerable.Repeat(tree, 1);

                foreach (var ruleIndex in rule.Rules)
                {
                    tails = tails.SelectMany(t => BuildTree(t, rules[ruleIndex]));
                }

                return tails;
            }

            private IEnumerable<Tree> BuildTree(Tree tree, OptionRule rule)
            {
                return rule.Rules.SelectMany(r => BuildTree(tree, r));
            }
        }

        public class Tree : IEnumerable<Tree>, IEnumerable<string>
        {
            private readonly Dictionary<string, Tree> branches = new();

            public Tree(int depth = 0)
            {
                Depth = depth;
            }

            public Tree(char value)
                : this(value.ToString())
            {
            }

            public Tree(string value)
            {
                Value = value;
            }

            public int Depth { get; }

            public Tree Match(string ch) => branches.GetValueOrDefault(ch);

            public bool Contains(string value)
            {
                return Contains(value, 0);
            }

            public bool Contains(string value, int index)
            {
                if (index == value.Length) { return IsTerminal; }

                return branches.TryGetValue(value[index].ToString(), out var child)
                    ? child.Contains(value, index + 1)
                    : false;
            }

            public Tree Add(char ch)
            {
                var index = ch.ToString();
                branches.TryAdd(index, new Tree(Depth + 1));
                return branches[index];
            }

            public Tree Add(Tree tree)
            {
                branches.TryAdd(tree.Value, tree);
                return tree;
            }

            IEnumerator<Tree> IEnumerable<Tree>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public IEnumerator<string> GetEnumerator() => new TreeEnumerator(this);

            public string Value { get; }
            public bool IsTerminal => branches.Count == 0;

            //public string Matches => new string(branches.Keys.OrderBy(c => c).ToArray());

            private class TreeEnumerator : IEnumerator<string>
            {
                private readonly Tree tree;
                private readonly IEnumerator<KeyValuePair<string, Tree>> branchEnumerator;
                private IEnumerator<string> childEnumerator;
                private string currentChar;

                public TreeEnumerator(Tree tree)
                {
                    this.tree = tree;
                    branchEnumerator = tree.branches.GetEnumerator();
                }

                public string Current => currentChar + childEnumerator?.Current;

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                    childEnumerator?.Dispose();
                    branchEnumerator.Dispose();
                }

                public bool MoveNext()
                {
                    if (childEnumerator?.MoveNext() == true)
                    {
                        return true;
                    }

                    childEnumerator?.Dispose();

                    if (branchEnumerator.MoveNext())
                    {
                        currentChar = branchEnumerator.Current.Key;
                        childEnumerator = branchEnumerator.Current.Value.GetEnumerator();
                        childEnumerator.MoveNext();
                        return true;
                    }

                    return false;
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class StringMatcherBuilder
        {
            private readonly Dictionary<string, Matcher> matchers = new();
            private readonly Stack<(Matcher, int)> backtracks = new();

            public StringMatcherBuilder(IEnumerable<string> rules)
            {
                foreach (var rule in rules)
                {
                    AddMatcher(rule);
                }
            }

            public StringMatcherBuilder WithOverride(string rule)
            {
                AddMatcher(rule);
                return this;
            }

            public StringMatcher Build() => new StringMatcher(matchers["0"], backtracks);

            public void AddMatcher(string rule)
            {
                var ruleParts = rule.Split(new[] { ':', '|' }, StringSplitOptions.TrimEntries);
                matchers[ruleParts[0]] = ParseMatcher(ruleParts);
            }

            public Matcher ParseMatcher(string[] ruleParts)
            {
                return ruleParts.Length switch
                {
                    2 when ruleParts[1].Contains("\"") => ParseAtomMatcher(ruleParts[1]),
                    2 => ParseSequenceMatcher(ruleParts[1]),
                    _ => ParseEitherMatcher(backtracks, ruleParts.Skip(1))
                };
            }

            public Matcher ParseAtomMatcher(string atom) => new AtomMatcher(atom.ToCharArray()[1]);

            public Matcher ParseSequenceMatcher(string sequence) =>
                new SequenceMatcher(matchers, sequence.Split(' '));

            public Matcher ParseEitherMatcher(Stack<(Matcher, int)> backtracks, IEnumerable<string> ruleParts) =>
                new EitherMatcher(backtracks, ruleParts.Select(rp => ParseSequenceMatcher(rp)));
        }

        public abstract class Matcher
        {
            public abstract bool Match(string input, ref int index);
        }

        public class LoggingMatcherDecorator : Matcher
        {
            private readonly Matcher decorated;
            private readonly Action<string> log;

            public LoggingMatcherDecorator(Matcher decorated, Action<string> logAction)
            {
                this.decorated = decorated;
                this.log = logAction;
            }

            public override bool Match(string input, ref int index)
            {
                log($"Executing {decorated} at input index {index}.");
                var result = decorated.Match(input, ref index);
                log($"Executed {decorated} with result '{result}' advancing index to {index}.");
                return true;
            }

            public override string ToString() => decorated.ToString();
        }

        public class SequenceMatcher : Matcher
        {
            private readonly Dictionary<string, Matcher> matchers;
            private readonly IEnumerable<string> matcherKeys;
            private readonly string name;

            public SequenceMatcher(Dictionary<string, Matcher> matchers, params string[] matcherKeys)
            {
                this.matchers = matchers;
                this.matcherKeys = matcherKeys.ToList();
                name = "[" + string.Join(" ", this.matcherKeys) + "]";
            }

            public override bool Match(string input, ref int index)
            {
                var initial = index;
                foreach (var key in matcherKeys)
                {
                    if (!matchers[key].Match(input, ref index))
                    {
                        index = initial;
                        return false;
                    }
                }

                return true;
            }

            public override string ToString() => name;
        }

        public class AtomMatcher : Matcher
        {
            private readonly char ch;

            public AtomMatcher(char ch)
            {
                this.ch = ch;
            }

            public override bool Match(string input, ref int index)
            {
                if (index < input.Length && input[index] == ch)
                {
                    index += 1;
                    return true;
                }

                return false;
            }
        }

        public class EitherMatcher : Matcher
        {
            private readonly List<Matcher> matchers;
            private readonly Stack<(Matcher, int)> backtracks;
            private readonly Action<string> log;

            public EitherMatcher(Stack<(Matcher, int)> backtracks, Action<string> log, params Matcher[] matchers)
                : this(backtracks, matchers.AsEnumerable())
            {
                this.log = log;
            }

            public EitherMatcher(Stack<(Matcher, int)> backtracks, params Matcher[] matchers)
                : this(backtracks, matchers.AsEnumerable())
            {
            }

            public EitherMatcher(Stack<(Matcher, int)> backtracks, IEnumerable<Matcher> matchers)
            {
                this.backtracks = backtracks;
                this.matchers = matchers.ToList();
                log = _ => {};
            }

            public override bool Match(string input, ref int index)
            {
                var initial = index;
                if (matchers[0].Match(input, ref index))
                {
                    log($"Pushing {matchers[1]} at index {initial}");
                    backtracks.Push((matchers[1], initial));
                    backtracks.Push((matchers[1], index));
                    return true;
                }

                index = initial;
                if (matchers[1].Match(input, ref index))
                {
                    return true;
                }

                index = initial;
                return false;
            }

            public override string ToString() => string.Join(" | ", matchers);
        }

        public class StringMatcher
        {
            private readonly Matcher matcher;
            private readonly Stack<(Matcher, int)> backtracks;

            public StringMatcher(Matcher matcher, Stack<(Matcher, int)> backtracks)
            {
                this.matcher = matcher;
                this.backtracks = backtracks;
            }

            public bool Match(string input)
            {
                var index = 0;
                var currentMatcher = matcher;

                while (true)
                {
                    if (currentMatcher.Match(input, ref index) && index == input.Length)
                    {
                        return true;
                    }

                    if (!backtracks.TryPop(out var backtrack))
                    {
                        break;
                    }
                    (currentMatcher, index) = backtrack;
                }

                return false;
            }
        }
    }
}

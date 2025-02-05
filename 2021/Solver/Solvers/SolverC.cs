using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverC : Solver
    {
        private readonly IEnumerable<string> input;

        public SolverC(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1()
        {
            var (wordCount, ones) = GetOnesCounts();
            var (gamma, epsilon) = CalculateRates(wordCount, ones);
            return (gamma * epsilon).ToString();
        }

        protected override string SolvePart2()
        {
            var tree = Tree.Build(0, input);
            var o2 = tree.Find(t => t.HasMost);
            var co2 = tree.Find(t => 1 - t.HasMost);
            return (o2 * co2).ToString();
        }

        private (int, int[]) GetOnesCounts()
        {
            var wordSize = input.First().Length;
            return input.Aggregate((0, new int[wordSize]), IncrementOnes);
        }

        private static (int, int[]) IncrementOnes((int WordCount, int[] Ones) accumulator, string word)
        {
            accumulator.WordCount++;
            for (int i = 0; i < accumulator.Ones.Length; i++)
            {
                accumulator.Ones[i] += word[i] - '0';
            }
            return accumulator;
        }

        private static (int, int) CalculateRates(int wordCount, int[] ones)
        {
            var midCount = wordCount / 2;
            var gamma = ones.Aggregate(0, (a, o) => (a << 1) + (o > midCount ? 1 : 0));
            var epsilon = (1 << ones.Length) - 1 - gamma;
            return (gamma, epsilon);
        }

        private class Tree
        {
            private static int[] keys = new[] { 0, 1 };
            private readonly Dictionary<int, Tree> children;

            public Tree(int hasMost, Dictionary<int, Tree> children)
            {
                HasMost = hasMost;
                this.children = children;
            }

            public Tree(string value)
            {
                Value = value;
            }

            public string Value { get; }
            public int HasMost { get; }
            public Tree this[char index] => children[index];

            public int Find(Func<Tree, int> branchSelector)
            {
                if (Value != null) { return Convert.ToInt32(Value, 2); }

                return children[branchSelector.Invoke(this)].Find(branchSelector);
            }

            public static Tree Build(int level, IEnumerable<string> values) =>
                values.Count() switch
                {
                    0 => null,
                    1 => new Tree(values.First()),
                    _ => Build(level+1, values.ToLookup(v => v[level] - '0'))
                };

            public static Tree Build(int level, ILookup<int, string> groups) =>
                new Tree(
                    groups[0].Count() > groups[1].Count() ? 0 : 1,
                    keys.ToDictionary(k => k, k => Build(level, groups[k])));
        }
    }
}

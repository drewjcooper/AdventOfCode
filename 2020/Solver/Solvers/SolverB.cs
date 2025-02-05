using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverB : Solver
    {
        private readonly string[] input;

        public SolverB(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        private static Regex passwordPattern = new Regex(@"^
            (?<first>\d+)
            -
            (?<second>\d+)
            \s
            (?<char>[a-z])
            :\s
            (?<password>[a-z]+)
            $", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        protected override string SolvePart1() => CountPasswordsWhich(HaveValidCount);

        protected override string SolvePart2() => CountPasswordsWhich(HaveEitherOr);

        private string CountPasswordsWhich(Func<int, int, char, string, bool> rule) =>
            input.Count(l => IsValid(l, rule)).ToString();

        private bool IsValid(string passwordRecord, Func<int, int, char, string, bool> rule)
        {
            var match = passwordPattern.Match(passwordRecord);

            var first = int.Parse(match.Groups["first"].Value);
            var second = int.Parse(match.Groups["second"].Value);
            var character = char.Parse(match.Groups["char"].Value);
            var password = match.Groups["password"].Value;

            return rule(first, second, character, password);
        }

        private bool HaveValidCount(int min, int max, char character, string password)
        {
            var count = password.Count(c => c == character);

            return count >= min && count <= max;
        }

        private bool HaveEitherOr(int first, int second, char character, string password) =>
            password[first-1] == character ^ password[second-1] == character;
    }
}

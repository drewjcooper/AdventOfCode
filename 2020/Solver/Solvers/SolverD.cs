using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverD : Solver
    {
        private readonly string[] lines;

        public SolverD(PuzzleInput input)
        {
            lines = input.Lines;
        }

        protected override string SolvePart1() => CountValidPassports(validateFields: false).ToString();

        protected override string SolvePart2() => CountValidPassports(validateFields: true).ToString();

        private int CountValidPassports(bool validateFields) =>
            lines.Split("").Select(lines => new PassportRecord(lines, validateFields)).Count(pr => pr.IsValid);

        private class PassportRecord
        {
            private static readonly HashSet<string> requiredFields =
                new() { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            private static readonly Regex hclPattern = new Regex("^#[0-9a-f]{6}$");
            private static readonly Regex hgtPattern = new Regex(@"^(?<value>\d+)(?<unit>(?:cm|in))$");
            private static readonly HashSet<string> eclValues =
                new() { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
            private readonly HashSet<string> fields;

            public PassportRecord(IEnumerable<string> lines, bool validateFields)
            {
                fields = lines
                    .SelectMany(l => l.Split(' '))
                    .Select(f => f.Split(':'))
                    .Where(s => !validateFields || IsValidField(s[0], s[1]))
                    .Select(s => s[0])
                    .ToHashSet();
            }

            public bool IsValid => !requiredFields.Except(fields).Any();

            private bool IsValidField(string key, string value) =>
                key switch
                {
                    "byr" => IsInRange(value, 1920, 2002),
                    "iyr" => IsInRange(value, 2010, 2020),
                    "eyr" => IsInRange(value, 2020, 2030),
                    "hgt" => IsValidHGT(value),
                    "hcl" => hclPattern.IsMatch(value),
                    "ecl" => eclValues.Contains(value),
                    "pid" => value.Length == 9 && int.TryParse(value, out _),
                    _     => false
                };

            private bool IsInRange(string text, int min, int max) =>
                int.TryParse(text, out var value) && value >= min && value <= max;

            private bool IsValidHGT(string text)
            {
                var match = hgtPattern.Match(text);

                if (!match.Success) { return false; }

                var value = match.Groups["value"].Value;

                return match.Groups["unit"].Value switch
                {
                    "cm" => IsInRange(value, 150, 193),
                    "in" => IsInRange(value, 59, 76),
                    _ => false
                };
            }
        }
    }
}

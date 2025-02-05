using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverY : Solver
{
    private readonly IEnumerable<Snafu> _fuelRequirements;

    public SolverY(PuzzleInput input)
    {
        _fuelRequirements = input.Lines.Select(l => Snafu.Parse(l));
    }
    
    protected override string SolvePart1() => _fuelRequirements.Sum().ToString();

    protected override string SolvePart2() => "";

    internal struct Snafu : IAdditionOperators<Snafu, Snafu, Snafu>
    {
        private readonly long _value;

        private Snafu(long value) => _value = value;

        public static Snafu Parse(string candidate)
        {
            var result = 0L;
            for (var i = 0; i < candidate.Length; i++)
            {
                result = result * 5 + candidate[i] switch
                {
                    '=' => -2,
                    '-' => -1,
                    char ch => ch - '0'
                };
            }
            return result;
        }

        public override string ToString()
        {
            const string numerals = "=-012";
            var digits = new char[(int)Math.Ceiling(Math.Log(_value) / Math.Log(5)) + 1];
            var value = _value;
            for (int i = digits.Length - 1; i >= 0 && value != 0; i--)
            {
                var digitValue = (int)((value + 2) % 5);
                digits[i] = numerals[digitValue];
                value = (value - digitValue + 2) / 5;
            }
            return new string(digits).TrimStart('\0');
        }

        public static Snafu operator +(Snafu left, Snafu right) => new(left._value + right._value);

        public static implicit operator Snafu(long value) => new(value);
        public static implicit operator long(Snafu snafu) => snafu._value;
    }
}
internal static class IEnumerableExtensions
{
    public static SolverY.Snafu Sum(this IEnumerable<SolverY.Snafu> source)
    {
        SolverY.Snafu result = default;
        foreach (var value in source)
        {
            result += value;
        }
        return result;
    }
}

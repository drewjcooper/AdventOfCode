using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Input;
using static AdventOfCode2021.Solvers.SolverR;

namespace AdventOfCode2021.Solvers
{
    internal class SolverR : Solver
    {
        private readonly IEnumerable<string> input;

        public SolverR(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1() => input.Select(l => l.ParseNumber()).Aggregate((a, n) => a + n).Magnitude.ToString();

        protected override string SolvePart2() =>
            input.SelectMany(a =>
                input.Where(b => a != b)
                    .Select(b => a.ParseNumber() + b.ParseNumber())
                    .Select(s => s.Magnitude))
                .Max()
                .ToString();

        public abstract class Number
        {
            public abstract long Magnitude { get; }

            public override string ToString()
            {
                var builder = new StringBuilder();
                AppendTo(builder);
                return builder.ToString();
            }

            protected abstract void AppendTo(StringBuilder builder);

            public static Number operator +(Number left, Number right) =>
                left is null ? right : new NumberPair(left, right).Reduce();

            public Number Reduce()
            {
                while (Exploder.Explode(this) || Splitter.Split(this));

                return this;
            }
        }

        public class RegularNumber : Number
        {
            public RegularNumber(int value) => Value = value;

            public int Value { get; private set; }

            public override long Magnitude => Value;

            protected override void AppendTo(StringBuilder builder) => builder.Append(Value);

            public void Merge(RegularNumber number)
            {
                if (number is not null) { Value += number.Value; }
            }

            public NumberPair Split()
            {
                var halfRoundedDown = Value / 2;
                return new NumberPair(new RegularNumber(halfRoundedDown), new RegularNumber(Value - halfRoundedDown));
            }

            public static RegularNumber Zero => new RegularNumber(0);
        }

        public class NumberPair : Number
        {
            public NumberPair(Number left, Number right)
            {
                Left = left;
                Right = right;
            }

            public Number Left { get; set; }
            public Number Right { get; set; }

            public override long Magnitude => Left.Magnitude * 3 + Right.Magnitude * 2;

            protected override void AppendTo(StringBuilder builder) =>
                builder.Append('[').Append(Left).Append(',').Append(Right).Append(']');
        }

        public class Exploder
        {
            private RegularNumber previous;
            private RegularNumber explodedLeft;
            private RegularNumber explodedRight;
            private bool exploding;
            private bool complete;

            public bool Visit(Number node, int level = 0)
            {
                if (exploding)
                {
                    switch (node)
                    {
                        case RegularNumber regular:
                            regular.Merge(explodedRight);
                            explodedRight = null;
                            complete = true;
                            return true;

                        case NumberPair pair:
                            return Visit(pair.Left) || Visit(pair.Right) || true;
                    }
                }

                switch (node)
                {
                    case RegularNumber regular:
                        previous = regular;
                        return false;

                    case NumberPair pair:
                        if (level == 4)
                        {
                            explodedLeft = pair.Left as RegularNumber;
                            explodedRight = pair.Right as RegularNumber;
                            return true;
                        }
                        if (Visit(pair.Left, level + 1))
                        {
                            if (complete) { return true; }
                            if (!exploding)
                            {
                                exploding = true;
                                pair.Left = RegularNumber.Zero;
                            }
                            previous?.Merge(explodedLeft);
                            explodedLeft = null;
                        }
                        if (Visit(pair.Right, level + 1))
                        {
                            if (complete) { return true; }
                            if (!exploding)
                            {
                                exploding = true;
                                pair.Right = RegularNumber.Zero;
                            }
                            previous?.Merge(explodedLeft);
                            explodedLeft = null;
                        }
                        return exploding;

                    default:
                        return false;
                }
            }

            public static bool Explode(Number number) => new Exploder().Visit(number);
        }

        public class Splitter
        {
            private Number replacement;

            public static bool Split(Number number) => new Splitter().Visit(number);

            private bool Visit(Number node)
            {
                switch (node)
                {
                    case RegularNumber regular:
                        if (regular.Value < 10) { return false; }
                        replacement = regular.Split();
                        return true;

                    case NumberPair pair:
                        if (Visit(pair.Left))
                        {
                            if (replacement is not null)
                            {
                                pair.Left = replacement;
                                replacement = null;
                            }
                            return true;
                        }
                        if (Visit(pair.Right))
                        {
                            if (replacement is not null)
                            {
                                pair.Right = replacement;
                                replacement = null;
                            }
                            return true;
                        }
                        return false;

                    default:
                        return false;
                }
            }
        }
    }

    internal static class SolverRExtensions
    {
        public static Number ParseNumber(this IEnumerable<char> chars) => new Queue<char>(chars).ParseNumber();

        public static Number ParseNumber(this Queue<char> chars)
        {
            var ch = chars.Dequeue();
            return ch switch
            {
                '[' => chars.ParsePair(),
                _ when char.IsDigit(ch) => chars.ParseRegular(ch),
                _ => throw new Exception($"Unexpected char: {ch}")
            };
        }

        public static RegularNumber ParseRegular(this Queue<char> chars, char digit)
        {
            var value = 0;
            while(true)
            {
                value = value * 10 + (digit - '0');
                if (!char.IsDigit(chars.Peek())) { return new RegularNumber(value); }
                digit = chars.Dequeue();
            }
        }

        public static NumberPair ParsePair(this Queue<char> chars)
        {
            var left = chars.ParseNumber();
            var ch = chars.Dequeue();
            if (left is NumberPair && ch != ',') { throw new Exception($"Expected ',', but found '{ch}"); }

            var right = chars.ParseNumber();
            ch = chars.Dequeue();
            if (right is NumberPair && ch != ']') { throw new Exception($"Expected ']', but found '{ch}"); }

            return new NumberPair(left, right);
        }
    }
}

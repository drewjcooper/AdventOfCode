using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverM : Solver
{
    private readonly IEnumerable<IEnumerable<string>> _linePairs;

    public SolverM(PuzzleInput input)
    {
        _linePairs = input.Lines.Split("");
    }
    
    protected override string SolvePart1() => _linePairs
        .Select(PacketPair.Parse)
        .Select((p, i) => (Index: i + 1, p.IsOrdered))
        .Sum(x => x.IsOrdered ? x.Index : 0)
        .ToString();

    protected override string SolvePart2() => _linePairs
        .SelectMany(x => x.Select(l => Value.Parse(l)))
        .Append(new DividerValue(2))
        .Append(new DividerValue(6))
        .Order()
        .Select((p, i) => (Index: i + 1, IsMarker: p is DividerValue))
        .Where(x => x.IsMarker)
        .Aggregate(1, (a, v) => a * v.Index)
        .ToString();

    private record PacketPair(Value Left, Value Right)
    {
        public bool IsOrdered => Left.CompareTo(Right) <= 0;

        public static PacketPair Parse(IEnumerable<string> lines)
            => lines.ToArray() switch
            {
                [var left, var right] => new(Value.Parse(left), Value.Parse(right)),
                _ => throw new ArgumentException("Unexpected line count in pair", nameof(lines))
            };
    }

    private abstract class Value : IComparable<Value>
    {
        public abstract int CompareTo(Value other);
        public abstract ListValue AsList();

        public static Value Parse(string chars) => Parse(new Queue<char>(chars));

        public static Value Parse(Queue<char> chars)
            => chars.Peek() switch
            {
                '[' => ListValue.Parse(chars),
                ',' => ParseNext(chars),
                var ch when char.IsDigit(ch) => IntegerValue.Parse(chars),
                var ch => throw new InvalidOperationException($"Expected `[` or digit. Found '{ch}'")
            };

        private static Value ParseNext(Queue<char> chars)
        {
            chars.Dequeue();
            return Parse(chars);
        }
    }

    private class IntegerValue : Value
    {
        private readonly int _value;

        public IntegerValue(int value) => _value = value;

        public override ListValue AsList() => new(this);

        public override int CompareTo(Value other)
            => other is IntegerValue otherInteger 
                ? _value.CompareTo(otherInteger._value) 
                : AsList().CompareTo(other);

        public new static IntegerValue Parse(Queue<char> chars)
        {
            var value = 0;
            while (char.IsDigit(chars.Peek()))
            {
                value = value * 10 + chars.Dequeue() - '0';
            }
            return new IntegerValue(value);
        }
    }

    private class ListValue : Value
    {
        private readonly List<Value> _values;

        public ListValue(params Value[] values) => _values = values.ToList();

        public override ListValue AsList() => this;

        public void Add(Value value) => _values.Add(value);

        public override int CompareTo(Value other) => CompareTo(other.AsList());

        public int CompareTo(ListValue other)
        {
            var comparison = _values.Zip(other._values)
                            .Select(v => v.First.CompareTo(v.Second))
                            .SkipWhile(x => x == 0)
                            .FirstOrDefault();
            return comparison != 0 ? comparison : _values.Count.CompareTo(other._values.Count);
        }

        public new static ListValue Parse(Queue<char> chars)
        {
            chars.Dequeue();
            var list = new ListValue();
            while (chars.Peek() != ']')
            {
                list.Add(Value.Parse(chars));
            }
            chars.Dequeue();
            return list;
        }
    }

    private class DividerValue : ListValue
    {
        public DividerValue(int value)
            : base(new ListValue(new IntegerValue(value)))
        {
        }
    }
}
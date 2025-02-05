using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using AdventOfCode.Helpers;
using AdventOfCode.Input;
using Microsoft.VisualBasic;

namespace AdventOfCode.Solvers;

internal class SolverE(PuzzleInput input) : Solver
{
    private readonly IEnumerable<long> _seeds = input.Lines[0].Split(' ').Skip(1).Select(long.Parse).ToList();
    private readonly SeedToLocationMap _maps = new(input.Lines.Split("").Skip(1));

    protected override Answer SolvePart1() => _seeds.Select(_maps.Map).Min();

    protected override Answer SolvePart2() => Interval.CreateMany(_seeds).SelectMany(_maps.Map).Min().From;

    private class SeedToLocationMap(IEnumerable<IEnumerable<string>> lineBlocks)
    {
        private readonly IEnumerable<CategoryMap> _categoryMaps = lineBlocks.Select(b => new CategoryMap(b.Skip(1)));

        public long Map(long from) => _categoryMaps.Aggregate(from, (v, m) => m.Map(v));

        public IEnumerable<Interval> Map(Interval seeds) 
            => _categoryMaps.Aggregate(Enumerable.Repeat(seeds, 1), (v, m) => v.SelectMany(i => m.Map(i)));

        public IEnumerable<Interval> Map(IEnumerable<Interval> seeds) 
            => _categoryMaps.Aggregate(seeds, (v, m) => m.Map(v).Where(x => !x.IsEmpty).ToList());
    }

    internal class CategoryMap(IEnumerable<string> lines)
    {
        private readonly ImmutableArray<IntervalMap> _maps = [.. lines.Select(l => new IntervalMap(l)).Order()];

        public string Name { get; } = lines.First().Trim(':');

        public long Map(long from) => Map(from, _maps.AsSpan());

        private static long Map(long from, ReadOnlySpan<IntervalMap> maps)
        {
            if (maps.IsEmpty) { return from; }
            if (maps.Length == 1) { return maps[0].TryMap(from, out var result) ? result : from; }
            var index = maps.Length / 2;
            if (maps[index].TryMap(from, out var to, out var nextMap)) { return to; }

            return nextMap switch
            {
                NextMap.Higher => Map(from, maps[(index + 1)..]),
                NextMap.Lower => Map(from, maps[..index]),
                _ => from
            };
        }

        public IEnumerable<Interval> Map(IEnumerable<Interval> intervals)
        {
            var mappedIntervals = new SortedIntervalCollection();

            var maps = _maps.GetEnumerator();
            maps.MoveNext();

            var map = maps.Current;
            var done = false;
            foreach (var interval in intervals.Where(i => !i.IsEmpty))
            {
                if (done) 
                {
                    mappedIntervals.Add(interval);
                    continue;
                }
                done |= AddMapped(interval);
            }

            return mappedIntervals.Keys;

            bool AddMapped(Interval interval)
            {
                while (!interval.IsEmpty)
                {
                    var (before, mapped, after) = maps.Current.Map(interval);
                    mappedIntervals.Add(before);
                    mappedIntervals.Add(mapped);
                    if (after.IsEmpty) { return false; }
                    if (!maps.MoveNext()) 
                    { 
                        mappedIntervals.Add(after);
                        return true; 
                    }
                    interval = after;
                }
                return false;
            }
        }

        public IEnumerable<Interval> Map(Interval interval)
        {
            var mappedIntervals = new SortedIntervalCollection();
            foreach (var map in _maps)
            {
                var (before, mapped, after) = map.Map(interval);
                mappedIntervals.Add(before);
                mappedIntervals.Add(mapped);
                if (after.IsEmpty) { break; }
                interval = after;
            }
            if (!mappedIntervals.ContainsKey(interval)) { mappedIntervals.Add(interval); }

            return mappedIntervals.Keys;
        }
    }

    internal class SortedIntervalCollection : SortedList<Interval, object?>
    {
        public void Add(Interval interval)
        {
            if (interval.IsEmpty) { return; }

            Add(interval, null);
            var insertedIndex = IndexOfKey(interval);
            
            var merged = interval;
            while (TryMergeAndRemove(insertedIndex + 1));
            while (TryMergeAndRemove(insertedIndex - 1))
            {
                insertedIndex -= 1;
            }

            if (merged != interval)
            {
                RemoveAt(insertedIndex);
                Add(merged, null);
            }

            bool TryMergeAndRemove(int index)
            {
                if (index >= 0 && index < Count && merged.TryMerge(Keys[index], out merged))
                {
                    RemoveAt(index);
                    return true;
                }

                return false;
            }
        }
    }

    internal record struct Interval(long From, long To) : IComparable<Interval>
    {
        public static readonly Interval Empty = new(-1, -1);

        public readonly bool IsEmpty => this == Empty;

        public static Interval Create(long from, long count) => new(from, from + count - 1);

        public static IEnumerable<Interval> CreateMany(IEnumerable<long> values)
            => values.Chunk(2).Select(x => Create(x[0], x[1]));

        public readonly int CompareTo(Interval other) => From.CompareTo(other.From);

        public readonly (Interval, Interval, Interval) Split(Interval interval)
            => (SubInterval(From, interval.From - 1), SubInterval(interval), SubInterval(interval.To + 1, To));

        private readonly Interval SubInterval(Interval interval) => SubInterval(interval.From, interval.To);

        private readonly Interval SubInterval(long from, long to)
            => to < From || from > To
                ? Empty
                : this with { From = long.Max(from, From), To = long.Min(to, To) };

        public static Interval operator +(Interval interval, long delta) 
            => interval.IsEmpty ? interval : interval with { From = interval.From + delta, To = interval.To + delta };

        public readonly bool TryMerge(Interval interval, [NotNullWhen(true)] out Interval merged)
        {
            if (To < interval.From - 1 || From > interval.To + 1)
            {
                merged = this;
                return false;
            } 

            merged = new(Math.Min(From, interval.From), Math.Max(To, interval.To));
            return true;
        }
    }

    internal readonly struct IntervalMap : IComparable<IntervalMap>
    {
        public IntervalMap(string line)
        {
            var parts = line.Split(' ');
            var from = long.Parse(parts[1]);
            var to = from + long.Parse(parts[2]) - 1;
            Interval = new(from, to);
            Delta = long.Parse(parts[0]) - from;
        }

        public Interval Interval { get; }
        public long Start { get; private init; }
        public long End { get; private init; }
        public long Delta { get; }

        public readonly int CompareTo(IntervalMap other) => Interval.CompareTo(other.Interval);

        public readonly (Interval, Interval, Interval) Map(Interval interval)
        {
            var (left, middle, right) = interval.Split(Interval);

            return (left, middle + Delta, right);
        }

        public bool TryMap(long from, out long to) => TryMap(from, out to, out _);

        public bool TryMap(long from, out long to, out NextMap nextMap)
        {
            to = default;
            nextMap = default;
            if (from < Interval.From) { nextMap = NextMap.Lower; return false; }
            if (from > Interval.To) { nextMap = NextMap.Higher; return false; }

            to = from + Delta;
            return true;
        }

        public override string ToString() => $"{Start} - {End} ({Delta:+0;-0;0})";
    }

    internal enum NextMap
    {
        None,
        Higher,
        Lower
    }
}
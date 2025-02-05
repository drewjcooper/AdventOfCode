using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    internal class SolverI : Solver
    {
        private readonly long[] data;

        public SolverI(PuzzleInput input)
        {
            data = input.Longs;
        }

        protected override string SolvePart1() => FindInvalidData().ToString();

        protected override string SolvePart2()
        {
            var target = FindInvalidData().Value;

            var first = data[0] < 0 ? 1 : 0;
            var last = first;
            var sum = data[first];

            while (true)
            {
                while (sum < target)
                {
                    if (++last == data.Length) { return "Not found"; }
                    sum += data[last];
                }
                while (sum > target)
                {
                    sum -= data[first++];
                }
                if (sum == target) { break; }
            }

            var current = first;
            var min = long.MaxValue;
            var max = long.MinValue;
            for (; current <= last; current++)
            {
                min = Math.Min(min, data[current]);
                max = Math.Max(max, data[current]);
            }

            return (min + max).ToString();
        }

        private long? FindInvalidData()
        {
            var window = new Window(data);
            while (true)
            {
                if (!window.Slide(out var invalid))
                {
                    return invalid;
                }
            }
        }

        public class Window
        {
            private readonly long[] data;
            private readonly HashSet<long> contents = new();
            private int startOfWindow;
            private int startOfData;

            public Window(long[] data)
            {
                this.data = data;

                var windowSize = (data[0] < 0) ? -data[0] : 25;
                if (data[0] < 0)
                {
                    startOfWindow = startOfData = 1;
                }

                for (; startOfData < startOfWindow + windowSize; startOfData++)
                {
                    contents.Add(data[startOfData]);
                }
            }

            public bool Slide(out long? invalidData)
            {
                invalidData = null;

                if (startOfData == data.Length) { return false; }

                var nextNumber = data[startOfData++];

                foreach (var number in contents)
                {
                    if (contents.Contains(nextNumber - number) && number << 1 != nextNumber)
                    {
                        contents.Remove(data[startOfWindow++]);
                        contents.Add(nextNumber);
                        return true;
                    }
                }

                invalidData = nextNumber;
                return false;
            }
        }
    }
}

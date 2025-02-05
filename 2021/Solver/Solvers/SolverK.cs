using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverK : Solver
    {
        private readonly IEnumerable<string> input;

        public SolverK(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1()
        {
            var octopi = new Octopi(input);
            for (int i = 0; i < 100; i++)
            {
                octopi.Step();
            }
            return octopi.Flashes.ToString();
        }

        protected override string SolvePart2()
        {
            var octopi = new Octopi(input);
            var step = 0;
            do
            {
                step++;
                octopi.ResetCount();
                octopi.Step();
            } while (octopi.Flashes != octopi.Count);

            return step.ToString();
        }

        internal class Octopi
        {
            private readonly Dictionary<Point, Octopus> octopi;

            public Octopi(IEnumerable<string> input)
            {
                octopi = input
                    .SelectMany((l, i) => l.Select((c, j) => new Octopus(new Point(i, j), c - '0', this)))
                    .ToDictionary(o => o.Location, o => o);
            }

            public int Flashes { get; private set; }
            public int Count => octopi.Count;

            public void Step()
            {
                foreach (var octopus in octopi.Values) { octopus.Charge(); }
                foreach (var octopus in octopi.Values) { octopus.Reset(); }
            }

            public void Flash(Point location)
            {
                Flashes++;
                foreach (var neighbour in location.GetNeighbours().Where(n => octopi.ContainsKey(n)))
                {
                    octopi[neighbour].Charge();
                }
            }

            public void ResetCount() => Flashes = 0;
        }

        public class Octopus
        {
            private readonly Octopi octopi;
            private int energy;

            public Octopus(Point location, int energy, Octopi octopi)
            {
                Location = location;
                this.energy = energy;
                this.octopi = octopi;
            }

            public Point Location { get; }

            public void Charge()
            {
                energy++;
                if (energy == 10) { octopi.Flash(Location); }
            }

            public void Reset()
            {
                if (energy > 9) { energy = 0; }
            }
        }

        public struct Point
        {
            public Point(int w, int l)
            {
                W = w;
                L = l;
            }

            public int W { get; }
            public int L { get; }

            public IEnumerable<Point> GetNeighbours()
            {
                yield return new(W - 1, L - 1);
                yield return new(W, L - 1);
                yield return new(W + 1, L - 1);
                yield return new(W - 1, L);
                yield return new(W + 1, L);
                yield return new(W - 1, L + 1);
                yield return new(W, L + 1);
                yield return new(W + 1, L + 1);
            }
        }
    }
}

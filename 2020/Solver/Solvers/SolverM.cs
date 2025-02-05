using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverM : Solver
    {
        private readonly string[] input;

        public SolverM(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1()
        {
            var timestamp = int.Parse(input[0]);
            var firstBus = input[1]
                .Split(',')
                .Where(x => x != "x")
                .Select(x => int.Parse(x))
                .Select(x => new { Id = x, DueIn = (x - timestamp % x) % x })
                .OrderBy(x => x.DueIn)
                .First();
            return (firstBus.Id * firstBus.DueIn).ToString();
        }

        protected override string SolvePart2()
        {
            var buses = input[1]
                .Split(',')
                .Select((x, i) => (Value: x, Index: i))
                .Where(x => x.Value != "x")
                .Select(x => (Id: long.Parse(x.Value), Index: x.Index))
                .OrderByDescending(b => b.Id)
                .ToList();

            var (currentPeriod, firstBusIndex) = buses.First();
            var originTimestamp = 0L - firstBusIndex;

            foreach (var bus in buses.Skip(1))
            {
                foreach (var periodMultiple in GetMultiplesOf(currentPeriod))
                {
                    var candidateTimestamp = originTimestamp + periodMultiple;
                    if ((candidateTimestamp + bus.Index) % bus.Id == 0)
                    {
                        originTimestamp = candidateTimestamp;
                        currentPeriod *= bus.Id;
                        break;
                    }
                }
            }

            return originTimestamp.ToString();
        }

        private IEnumerable<long> GetMultiplesOf(long number)
        {
            var multiple = 0L;
            while (true)
            {
                checked { yield return multiple += number; }
            }
        }
    }
}

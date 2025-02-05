using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverB : Solver
    {
        private readonly IEnumerable<string> input;

        public SolverB(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1()
        {
            var position = input.Select(l => Command.Parse(l))
                .Aggregate(new Position(0, 0, 0), (p, c) => c.ExecuteV1(p));
            return $"{position.Distance * position.Depth}";
        }

        protected override string SolvePart2()
        {
            var position = input.Select(l => Command.Parse(l))
                .Aggregate(new Position(0, 0, 0), (p, c) => c.ExecuteV2(p));
            return $"{position.Distance * position.Depth}";
        }

        private record Command (Direction Direction, int Amount)
        {
            public Position ExecuteV1(Position start)
                => Direction switch
                {
                    Direction.Forward => start with { Distance = start.Distance + Amount },
                    Direction.Up => start with { Depth = start.Depth - Amount },
                    Direction.Down => start with { Depth = start.Depth + Amount },
                    _ => throw new InvalidOperationException()
                };

            public Position ExecuteV2(Position start)
                => Direction switch
                {
                    Direction.Forward => start with
                    {
                        Distance = start.Distance + Amount,
                        Depth = start.Depth + start.Aim * Amount
                    },
                    Direction.Up => start with { Aim = start.Aim - Amount },
                    Direction.Down => start with { Aim = start.Aim + Amount },
                    _ => throw new InvalidOperationException()
                };

            public static Command Parse(string input)
            {
                var parts = input.Split(' ');
                return new Command(Enum.Parse<Direction>(parts[0], true), int.Parse(parts[1]));
            }
        }

        private enum Direction { Forward, Up, Down }

        private record Position (int Distance, int Depth, int Aim);
    }
}

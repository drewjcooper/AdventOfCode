using System;
using System.Linq;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverL : Solver
    {
        private readonly string[] input;

        public SolverL(PuzzleInput input)
        {
            this.input = input.Lines;
        }

        protected override string SolvePart1() =>
            input
                .Select(l => new Command(l))
                .Aggregate(new Ship(new Position(0, 0), 0), (s, c) => c.Execute(s))
                .DistanceFromStart
                .ToString();

        protected override string SolvePart2() =>
            input
                .Select(l => new Command(l))
                .Aggregate(
                    (Ship: new Ship(new Position(0, 0), 0), Waypoint: new Position(10, 1)),
                    (s, c) => c.Execute(s.Ship, s.Waypoint))
                .Ship
                .DistanceFromStart
                .ToString();

        public class Command
        {
            public Command(string command)
            {
                Operator = command[0];
                Operand = int.Parse(command.Substring(1));
            }

            public char Operator { get; }
            public int Operand { get; }

            public Ship Execute(Ship ship)
            {
                return Operator switch
                {
                    'N' => ship.Move(0, Operand),
                    'S' => ship.Move(0, -Operand),
                    'E' => ship.Move(Operand, 0),
                    'W' => ship.Move(-Operand, 0),
                    'R' => ship.Rotate(-Operand),
                    'L' => ship.Rotate(Operand),
                    'F' => ship.Forward(Operand)
                };
            }

            public (Ship, Position) Execute(Ship ship, Position waypoint)
            {
                return Operator switch
                {
                    'N' => (ship, waypoint.Move(0, Operand)),
                    'S' => (ship, waypoint.Move(0, -Operand)),
                    'E' => (ship, waypoint.Move(Operand, 0)),
                    'W' => (ship, waypoint.Move(-Operand, 0)),
                    'R' => (ship, waypoint.Rotate(-Operand)),
                    'L' => (ship, waypoint.Rotate(Operand)),
                    'F' => (ship.Move(waypoint.East * Operand, waypoint.North * Operand), waypoint)
                };
            }
        }

        public record Ship (Position Position, int Bearing)
        {
            public Ship Move(int east, int north)
                => this with { Position = Position.Move(east, north) };

            public Ship Forward(int distance) =>
                Bearing switch
                {
                    0 => Move(distance, 0),
                    90 => Move(0, distance),
                    180 => Move(-distance, 0),
                    270 => Move(0, -distance)
                };

            public Ship Rotate(int degrees)
                => this with { Bearing = (Bearing + degrees + 360) % 360 };

            public int DistanceFromStart => Position.DistanceFromOrigin;
        }

        public record Position (int East, int North)
        {
            public Position Move(int east, int north)
                => this with { North = North + north, East = East + east };

            public Position Rotate(int degrees)
                => ((degrees + 360) % 360) switch
                {
                    0 => this,
                    90 => new Position(-North, East),
                    180 => new Position(-East, -North),
                    270 => new Position(North, -East)
                };

            public int DistanceFromOrigin => Math.Abs(North) + Math.Abs(East);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverU : Solver
    {
        private readonly Player[] players;

        public SolverU(PuzzleInput input)
        {
            players = input.Lines.Select(l => new Player(int.Parse(l.Split(':')[1].Trim()), 0)).ToArray();
        }

        protected override string SolvePart1()
        {
            var game = new Game(players, new DerministicD100());
            game.Play();
            return (game.LoserScore * game.DieRollCount).ToString();
        }

        protected override string SolvePart2()
        {
            var game = new DiracGame(players);
            game.Play();
            return Math.Max(game.Player1Wins, game.Player2Wins).ToString();
        }

        public class DiracGame
        {
            private readonly GameState initial;
            private readonly DiracDie die = new();

            private readonly Dictionary<GameState, Tally> knownStates = new();

            public DiracGame(Player[] players)
            {
                initial = new(players[0], players[1], 0);
            }

            public long Player1Wins { get; private set; }
            public long Player2Wins { get; private set; }

            public void Play()
            {
                var tally = Play(initial);
                Player1Wins = tally.Player1;
                Player2Wins = tally.Player2;
            }

            private Tally Play(GameState state)
            {
                if (state.HasWinner) { return state.Tally; }

                if (knownStates.TryGetValue(state, out var tally)) { return tally; }

                tally = new Tally();

                foreach (var (dieRoll, universes) in die.Rolls())
                {
                    tally += Play(state.Next(dieRoll)) * universes;
                }

                return knownStates[state] = tally;
            }
        }

        public class DiracDie
        {
            private static readonly IEnumerable<(int, int)> outcomeFrequency = new[]
            {
                (3, 1), (4, 3), (5, 6), (6, 7), (7, 6), (8, 3), (9, 1)
            };

            public IEnumerable<(int, int)> Rolls() => outcomeFrequency;
        }

        public record struct GameState(Player Player1, Player Player2, int PlayerIndex)
        {
            public bool HasWinner => Player1.Score >= 21 || Player2.Score >= 21;

            public Tally Tally => new(Player1.Score >= 21 ? 1 : 0, Player2.Score >= 21 ? 1 : 0);

            public GameState Next(int dieRoll) =>
                PlayerIndex switch
                {
                    0 => new(Player1.Move(dieRoll), Player2, 1 - PlayerIndex),
                    1 => new(Player1, Player2.Move(dieRoll), 1 - PlayerIndex)
                };
        }

        public record struct Tally(long Player1, long Player2)
        {
            public static Tally operator+(Tally left, Tally right) =>
                new(left.Player1 + right.Player1, left.Player2 + right.Player2);

            public static Tally operator*(Tally tally, int count) =>
                new(tally.Player1 * count, tally.Player2 * count);
        }

        public class Game
        {
            private readonly Player[] players;
            private readonly Die die;
            private int current;

            public Game(IEnumerable<Player> players, Die die)
            {
                this.players = players.ToArray();
                this.die = die;
            }

            public int WinnerScore { get; private set; }
            public int LoserScore { get; private set; }
            public int DieRollCount => die.RollCount;

            public void Play()
            {
                while (true)
                {
                    var roll = die.Roll(3);
                    players[current] = players[current].Move(roll);

                    if (players[current].Score >= 1000) { break; }

                    current = 1 - current;
                }

                WinnerScore = players[current].Score;
                LoserScore = players[1 - current].Score;
            }
        }

        public record struct Player(Position Position, int Score)
        {
            public Player Move(int spaces)
            {
                var newPosition = Position + spaces;
                return new(newPosition, Score + newPosition);
            }
        }

        public record struct Position(int Value)
        {
            public static Position operator +(Position position, int spaces) =>
                new((position.Value - 1 + spaces) % 10 + 1);

            public static int operator +(int score, Position position) => score + position.Value;

            public static implicit operator Position(int value) => new(value);
        }

        public abstract class Die
        {
            public int RollCount { get; private set; }

            public int Roll(int times = 1)
            {
                var value = Enumerable.Range(0, times).Sum(_ => Roll());
                RollCount += times;
                return value;
            }

            protected abstract int Roll();
        }

        public class DerministicD100 : Die
        {
            private int nextValue = 1;

            protected override int Roll()
            {
                var value = nextValue;
                nextValue = nextValue % 100 + 1;
                return value;
            }
        }
    }
}

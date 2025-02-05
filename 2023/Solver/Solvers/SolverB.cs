using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal class SolverB(PuzzleInput input) : Solver
{
    private readonly IEnumerable<Game> _games = input.Lines.Select(Game.Parse);

    protected override Answer SolvePart1() => _games.Where(g => g.IsPossible(new(12, 13, 14))).Select(g => g.Id).Sum();

    protected override Answer SolvePart2() => _games.Select(g => g.GetMinimumBag().Power).Sum();

    private class Game
    {
        private readonly IEnumerable<Draw> _draws;

        private Game(int id, IEnumerable<Draw> draws)
        {
            Id = id;
            _draws = draws;
        }

        public int Id { get; }

        public bool IsPossible(Bag bag) => _draws.All(d => d.IsPossible(bag));

        public Bag GetMinimumBag() => _draws.Aggregate(new Bag(), (b, d) => b.StretchToAllow(d));

        public static Game Parse(string candidate)
        {
            var parts = candidate.Split(':', StringSplitOptions.TrimEntries);
            
            return new Game(
                int.Parse(parts[0].Split(' ')[1]), 
                parts[1].Split(';', StringSplitOptions.TrimEntries).Select(Draw.Parse).ToList());
        }
    }

    private class Draw
    {
        private Draw(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public int Red { get; }
        public int Green { get; }
        public int Blue { get; }

        public static Draw Parse(string candidate)
        {
            var cubes = candidate
                .Split(',', StringSplitOptions.TrimEntries)
                .Select(c => c.Split(' '))
                .ToDictionary(c => c[1], c => int.Parse(c[0]));

            return new Draw(
                cubes.GetValueOrDefault("red"), 
                cubes.GetValueOrDefault("green"), 
                cubes.GetValueOrDefault("blue"));
        }

        public bool IsPossible(Bag bag) => bag.ContainsAtLeast(Red, Green, Blue);
    }

    private class Bag(int _red, int _green, int _blue)
    {
        public Bag() : this(0, 0, 0) { }

        public int Power => _red * _green * _blue;

        internal bool ContainsAtLeast(int red, int green, int blue) => _red >= red && _green >= green && _blue >= blue;

        internal Bag StretchToAllow(Draw draw)
            => new(int.Max(draw.Red, _red), int.Max(draw.Green, _green), int.Max(draw.Blue, _blue));
    }
}

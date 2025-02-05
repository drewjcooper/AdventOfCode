using AdventOfCode.Input;
using Microsoft.VisualBasic;

namespace AdventOfCode.Solvers;

internal class SolverC(PuzzleInput input) : Solver
{
    private readonly Scanner _scanner = new(input.Lines);

    protected override Answer SolvePart1() => _scanner.FindPartNumbers().Sum();

    protected override Answer SolvePart2() => _scanner.FindGears().Sum(g => g.Ratio);

    private class Scanner(string[] lines)
    {
        private readonly string _firstLine = lines[0];
        private readonly IEnumerable<string?> _lines = lines.Skip(1).Append(null);

        public IEnumerable<int> FindPartNumbers()
        {
            var buffer = new TriBuffer(_firstLine);
            foreach (var line in _lines)
            {
                buffer = buffer.Next(line);
                foreach (var partNumber in FindPartNumbers(buffer))
                {
                    yield return partNumber;
                }
            }
        }

        public IEnumerable<Gear> FindGears()
        {
            var buffer = new TriBuffer(_firstLine);
            foreach (var line in _lines)
            {
                buffer = buffer.Next(line);
                foreach (var gear in FindGears(buffer))
                {
                    yield return gear;
                }
            }
        }

        private IEnumerable<int> FindPartNumbers(TriBuffer buffer)
        {
            var number = 0;
            var isPartNumber = false;
            for (int i = 0; i < buffer.Length; i++)
            {
                var ch = buffer[i];
                if (char.IsDigit(ch))
                {
                    isPartNumber |= buffer.HasSymbolAt(i) || number == 0 && buffer.HasSymbolAt(i-1);
                    number = number * 10 + ch - '0';
                }
                else
                {
                    if (number > 0 && (isPartNumber || buffer.HasSymbolAt(i))) { yield return number; }
                    number = 0;
                    isPartNumber = false;
                }
            }

            if (number > 0 && isPartNumber) { yield return number; }
        }

        private static IEnumerable<Gear> FindGears(TriBuffer buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer.TryParseGear(i, out var gear))
                {
                    yield return gear;
                }
            }
        }
    }

    private readonly struct TriBuffer 
    {
        private readonly string _blankLine;
        private readonly string[] _lines;

        public TriBuffer(string line) : this(new string('.', line.Length), null, null, line) { }

        private TriBuffer(string blankLine, string? line1, string? line2, string? line3)
        {
            _blankLine = blankLine;
            _lines = [line1 ?? blankLine, line2 ?? blankLine, line3 ?? blankLine];
        }

        public char this[int index] => _lines[1][index];

        public int Length => _blankLine.Length;

        public bool HasSymbolAt(int index) 
            => index >= 0 && index < Length && _lines.Select(l => l[index]).Any(c => c != '.' && (c < '0' || c > '9'));

        public bool TryParseGear(int index, out Gear gear) => Gear.TryParse(index, _lines, out gear);

        public TriBuffer Next(string? line = null) => new(_blankLine, _lines[1], _lines[2], line);
    }

    public readonly struct Gear(int part1, int part2)
    {
        public int Ratio => part1 * part2;

        public static bool TryParse(int index, string[] lines, out Gear gear)
        {
            gear = default;
            if (lines[1][index] != '*') { return false; }

            var partNumbers = new List<int>();
            AddNumbers(lines[0]);
            AddNumbers(lines[1]);
            AddNumbers(lines[2]);

            if (partNumbers.Count != 2) { return false; }
            gear = new Gear(partNumbers[0], partNumbers[1]);
            return true;
            
            void AddNumbers(string line)
            {
                if (TryParseNumber(index, line, out var number))
                {
                    partNumbers.Add(number);
                    return;
                }
                if (TryParseNumber(index - 1, line, out number)) { partNumbers.Add(number); }
                if (TryParseNumber(index + 1, line, out number)) { partNumbers.Add(number); }
            }
        }

        private static bool TryParseNumber(int index, string line, out int number)
        {
            number = 0;
            if (index < 0 || index >= line.Length) { return false; }
            if (!char.IsDigit(line[index])) { return false; }

            while(index > 0 && char.IsDigit(line[index - 1])) { index -= 1; }

            for (; index < line.Length && char.IsDigit(line[index]); index++) 
            { 
                number = number * 10 + line[index] - '0'; 
            }

            return true;
        }
    }
}
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

internal class SolverA(PuzzleInput input) : Solver
{
    private readonly IEnumerable<string> _lines = input.Lines;

    protected override Answer SolvePart1() => _lines.Select(GetDigitSumValue).Sum();

    protected override Answer SolvePart2() => _lines.Select(GetProperValue).Sum();

    private int GetDigitSumValue(string line)
    {
        for (int f = 0, l = line.Length - 1; f < l;)
        {
            if (!char.IsDigit(line[f])) { f += 1; }
            if (!char.IsDigit(line[l])) { l -= 1; }
            if (char.IsDigit(line[f]) && char.IsDigit(line[l])) 
            { 
                return (line[f] - '0') * 10 + (line[l] - '0');
            }
        }

        return -1;
    }

    private int GetProperValue(string line)
    {
        var numbers = new Dictionary<string, int?> 
        { 
            ["zero"] = 0,
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3,
            ["four"] = 4,
            ["five"] = 5,
            ["six"] = 6,
            ["seven"] = 7,
            ["eight"] = 8,
            ["nine"] = 9
        };
        char[] digits = [ '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' ];

        var firstDigitIndex = line.IndexOfAny(digits);
        var lastDigitIndex = line.LastIndexOfAny(digits);

        var beforeFirstDigit = firstDigitIndex < 0 ? line : line[..firstDigitIndex];
        var afterLastDigit = lastDigitIndex < 0 ? line : line[lastDigitIndex..];

        var firstNumber = numbers.Keys.Select(n => new { Number = n, Index = beforeFirstDigit.IndexOf(n) }).Where(x => x.Index >= 0).OrderBy(x => x.Index).Select(x => numbers[x.Number]).FirstOrDefault();
        var lastNumber = numbers.Keys.Select(n => new { Number = n, Index = afterLastDigit.LastIndexOf(n) }).Where(x => x.Index >= 0).OrderBy(x => x.Index).Select(x => numbers[x.Number]).LastOrDefault();

        var firstValue = firstNumber ?? line[firstDigitIndex] - '0';
        var lastValue = lastNumber ?? line[lastDigitIndex] - '0';

        return firstValue * 10 + lastValue;
    }
}
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverF : Solver
{
    private readonly IEnumerable<char> data;

    public SolverF(PuzzleInput input)
    {
        data = input.RawText.ToCharArray();
    }
    
    protected override string SolvePart1() => FindFirstMarker(4).ToString();

    protected override string SolvePart2() => FindFirstMarker(14).ToString();

    public int FindFirstMarker(int markerLength) 
    {
        var buffer = new Queue<char>(markerLength);
        var charsRead = 0;
        foreach (var ch in data)
        {
            charsRead++;
            if (buffer.Count == markerLength) { buffer.Dequeue(); }
            buffer.Enqueue(ch);
            if (buffer.Distinct().Count() == markerLength)
            {
                return charsRead;
            }
        }

        return -1;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AdventOfCode2020.Helpers;

namespace AdventOfCode2020.Input
{
    public class PuzzleInput
    {
        private readonly Lazy<string[]> _lines;

        private PuzzleInput(string inputText)
        {
            RawText = inputText;
            _lines = new Lazy<string[]>(
                () => inputText.Trim().Split(new[] { '\n' }).Select(l => l.Trim('\r')).ToArray());
        }

        public string RawText { get; }

        public string[] Lines => _lines.Value;

        public int[] Ints => Lines.Select(l => l.Trim()).Select(l => Int32.Parse(l)).ToArray();
        public long[] Longs => Lines.Select(l => l.Trim()).Select(l => Int64.Parse(l)).ToArray();

        public static PuzzleInput From(string inputText) => new PuzzleInput(inputText);

        public static async Task<PuzzleInput> LoadAsync(PuzzleId puzzleId)
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(typeof(PuzzleInput), $"Puzzle{puzzleId.Code}");
            using var reader = new StreamReader(stream);
            return new PuzzleInput(await reader.ReadToEndAsync());
        }
    }
}

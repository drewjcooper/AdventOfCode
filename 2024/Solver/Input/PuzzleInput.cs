using System.Collections;
using System.Reflection;
using AdventOfCode.Helpers;

namespace AdventOfCode.Input;

public class PuzzleInput
{
    private readonly Lazy<string[]> lines;

    private PuzzleInput(string inputText)
    {
        RawText = inputText;
        lines = new Lazy<string[]>(
            () => inputText.Split('\n').Select(l => l.Trim('\r')).ToArray());
    }

    public string RawText { get; }

    public string[] Lines => lines.Value;

    public int[] Ints => Lines.Select(l => l.Trim()).Select(l => Int32.Parse(l)).ToArray();
    public long[] Longs => Lines.Select(l => l.Trim()).Select(l => Int64.Parse(l)).ToArray();

    public IEnumerable<string> Break(Predicate<string>? predicate = null) =>
        new BrokenSequence<string>(Lines, predicate ?? string.IsNullOrEmpty);

    public static PuzzleInput From(string inputText) => new PuzzleInput(inputText);

    public static async Task<PuzzleInput> LoadAsync(PuzzleId puzzleId)
    {
        var puzzleName = $"Puzzle{puzzleId.Code}";
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream(typeof(PuzzleInput), puzzleName)
            ?? throw new Exception($"Unable to load resource stream '{puzzleName}'.");
        using var reader = new StreamReader(stream);
        return new PuzzleInput(await reader.ReadToEndAsync());
    }

    private class BrokenSequence<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> elements;
        private readonly Predicate<T> splitOn;

        public BrokenSequence(IEnumerable<T> elements, Predicate<T> splitOn)
        {
            this.elements = elements.GetEnumerator();
            this.splitOn = splitOn;
        }

        public IEnumerator<T> GetEnumerator() => new BrokenEnumerator(elements, splitOn);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class BrokenEnumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> parent;
            private readonly Predicate<T> breakOn;
            private bool atEndOfSection;
            private bool atEndOfSequence;

            public BrokenEnumerator(IEnumerator<T> parent, Predicate<T> breakOn)
            {
                this.parent = parent;
                this.breakOn = breakOn;
            }

            public T Current => parent.Current;

            object? IEnumerator.Current => Current;

            public void Dispose()
            {
                atEndOfSection = false;
                if (atEndOfSequence) { parent.Dispose(); }
            }

            public bool MoveNext()
            {
                if (atEndOfSection || atEndOfSequence) { return false; }
                atEndOfSection = atEndOfSequence = !parent.MoveNext();
                if (atEndOfSequence) { return false; }
                if (breakOn.Invoke(Current)) { atEndOfSection = true; }
                return !atEndOfSection;
            }

            public void Reset()
            {
                parent.Reset();
                atEndOfSection = atEndOfSequence = false;
            }
        }
    }
}

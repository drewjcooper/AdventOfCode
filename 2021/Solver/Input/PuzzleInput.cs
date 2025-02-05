using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AdventOfCode2021.Helpers;

namespace AdventOfCode2021.Input
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

        public IEnumerable<string> Split(Predicate<string> predicate = null) =>
            new SplitSequence<string>(Lines, predicate ?? string.IsNullOrEmpty);

        public static PuzzleInput From(string inputText) => new PuzzleInput(inputText);

        public static async Task<PuzzleInput> LoadAsync(PuzzleId puzzleId)
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(typeof(PuzzleInput), $"Puzzle{puzzleId.Code}");
            using var reader = new StreamReader(stream);
            return new PuzzleInput(await reader.ReadToEndAsync());
        }

        private class SplitSequence<T> : IEnumerable<T>
        {
            private readonly IEnumerator<T> elements;
            private readonly Predicate<T> splitOn;

            public SplitSequence(IEnumerable<T> elements, Predicate<T> splitOn)
            {
                this.elements = elements.GetEnumerator();
                this.splitOn = splitOn;
            }

            public IEnumerator<T> GetEnumerator() => new SplitEnumerator(elements, splitOn);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private class SplitEnumerator : IEnumerator<T>
            {
                private readonly IEnumerator<T> parent;
                private readonly Predicate<T> splitOn;
                private bool atEndOfSection;
                private bool atEndOfSequence;

                public SplitEnumerator(IEnumerator<T> parent, Predicate<T> splitOn)
                {
                    this.parent = parent;
                    this.splitOn = splitOn;
                }

                public T Current => parent.Current;

                object IEnumerator.Current => Current;

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
                    if (splitOn.Invoke(Current)) { atEndOfSection = true; }
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
}

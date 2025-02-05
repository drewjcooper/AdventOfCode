using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverI(PuzzleInput input) : Solver
{
    protected override Answer SolvePart1() => new FileSystem(input.RawText).CompactBlocks();

    protected override Answer SolvePart2() => new FileSystem(input.RawText).CompactFiles();

    private class FileSystem(string map)
    {
        private readonly string _map = map;

        public long CompactBlocks()
        {
            var checksum = 0L;
            var targetBlockIndex = 0;

            File sourceFile = default;

            for (int dstRegion = 0, srcRegion = _map.Length + 1; dstRegion < srcRegion; dstRegion++)
            {
                if (ContainsFile(dstRegion))
                {
                    var file = GetFile(dstRegion);
                    checksum += file.GetChecksum(targetBlockIndex);
                    targetBlockIndex += file.Size;
                    continue;
                }

                var space = new Space(targetBlockIndex, _map[dstRegion]);
                while (!space.IsFull)
                {
                    if (sourceFile.IsEmpty)
                    {
                        srcRegion -= 2;
                        sourceFile = GetFile(srcRegion);
                    }
                    space.FillFrom(ref sourceFile);
                }
                checksum += space.Checksum;
                targetBlockIndex += space.Size;
            }

            return checksum + sourceFile.GetChecksum(targetBlockIndex);
        }

        public long CompactFiles()
        {
            var files = new List<File>();
            var spaces = new List<Space>();

            for (int i = 1, blockIndex = new File(0, _map[0]).Size; i < _map.Length; i++)
            {
                var space = new Space(blockIndex, _map[i++]);
                blockIndex += space.Size;
                spaces.Add(space);

                var file = new File(i / 2, blockIndex, _map[i]);
                blockIndex += file.Size;
                files.Add(file);
            }

            for (int i = files.Count - 1; i > 0; i--)
            {
                var file = files[i];

                for (int j = 0; j <= i; j++)
                {
                    var space = spaces[j];

                    if (space.CanFit(file))
                    {
                        space.FillFrom(ref file);
                        spaces[j] = space;
                        files.RemoveAt(i);
                        break;
                    }
                }
            }

            return files.Sum(f => f.Checksum) + spaces.Sum(s => s.Checksum);
        }

        private static bool ContainsFile(int regionIndex) => regionIndex % 2 == 0;

        private File GetFile(int region) => new(region / 2, _map[region]);

        private struct Space(int startBlock, int unused)
        {
            private int _nextEmptyBlock = startBlock;
            private int _unused = unused;
            private readonly int _size = unused;
            private long _checksum;

            public Space(int startBlock, char unused) : this(startBlock, (int)(unused - '0')) { }

            public readonly bool IsFull => _unused == 0;
            public readonly int Size => _size;
            public readonly long Checksum => _checksum;

            public bool CanFit(File file) => _unused >= file.Size;

            public void FillFrom(ref File file)
            {
                for (; !IsFull && !file.IsEmpty; _nextEmptyBlock++)
                {
                    _checksum += CopyFrom(ref file, _nextEmptyBlock);
                }
            }

            private long CopyFrom(ref File file, int blockIndex)
            {
                _unused--;
                file.RemoveBlock();
                return file.Id * blockIndex;
            }
        }

        private struct File(int id, int blocks)
        {
            private int _blocks = blocks;

            public File(int id, char blocks) : this(id, (int)(blocks - '0')) { }
            public File(int id, int startBlock, char blocks) : this(id, blocks) 
            {
                Checksum = GetChecksum(startBlock);
            }

            public int Id { get; } = id;
            public readonly bool IsEmpty => _blocks == 0;
            public readonly int Size => _blocks;
            public long Checksum { get; }

            public long GetChecksum(int startblock)
            {
                long id = Id;
                return Enumerable.Range(startblock, _blocks).Sum(b => b * id);
            }

            public void RemoveBlock() => _blocks--;
        }
    }
}

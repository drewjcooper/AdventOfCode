using System.Linq;
using System.Text;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverQ : Solver
    {
        private readonly char[][] initial;

        public SolverQ(PuzzleInput input)
        {
            initial = input.Lines.Select(l => l.ToCharArray()).ToArray();
        }

        protected override string SolvePart1() =>
            new _3Space(initial)
                .Step()
                .Step()
                .Step()
                .Step()
                .Step()
                .Step()
                .GetActiveCubeCount()
                .ToString();

        protected override string SolvePart2() =>
            new _4Space(initial)
                .Step()
                .Step()
                .Step()
                .Step()
                .Step()
                .Step()
                .GetActiveCubeCount()
                .ToString();

        public class _3Space
        {
            private readonly int[,,] cubes;
            private readonly int xLength;
            private readonly int yLength;
            private readonly int zLength;

            public _3Space(char[][] initial)
            {
                xLength = initial.Length;
                yLength = initial[0].Length;
                zLength = 1;
                cubes = new int[xLength, yLength, zLength];

                for (var x = 0; x < xLength; x++)
                for (var y = 0; y < yLength; y++)
                {
                    cubes[x,y,0] = initial[x][y] == '#' ? 1 : 0;
                }
            }

            private _3Space(int[,,] cubes)
            {
                this.cubes = cubes;
                xLength = cubes.GetLength(0);
                yLength = cubes.GetLength(1);
                zLength = cubes.GetLength(2);
            }

            public _3Space Step()
            {
                var next = new int[xLength+2, yLength+2, zLength+2];

                for (int x = 0; x <= xLength + 1; x++)
                for (int y = 0; y <= yLength + 1; y++)
                for (int z = 0; z <= zLength + 1; z++)
                {
                    next[x,y,z] = GetNext(x-1, y-1, z-1);
                }

                return new _3Space(next);
            }

            public int GetActiveCubeCount()
            {
                var count = 0;

                for (int x = 0; x < xLength; x++)
                for (int y = 0; y < yLength; y++)
                for (int z = 0; z < zLength; z++)
                {
                    count += cubes[x,y,z];
                }

                return count;
            }

            public string ToString(int z)
            {
                var sb = new StringBuilder();
                z = z + zLength / 2;

                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        sb.Append(cubes[x,y,z] == 1 ? '#' : '.');
                    }

                    sb.AppendLine();
                }

                return sb.ToString().TrimEnd();
            }

            private int GetNext(int x, int y, int z) =>
                CountActiveNeighbours(x, y, z) switch
                {
                    3 => 1,
                    2 => IsOutOfBounds(x,y,z) ? 0 : cubes[x,y,z],
                    _ => 0
                };

            private int CountActiveNeighbours(int x, int y, int z)
            {
                var count = 0;

                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx != 0 || dy != 0 || dz != 0)
                    {
                        count += GetState(x+dx, y+dy, z+dz);
                    }
                }

                return count;
            }

            private int GetState(int x, int y, int z) => IsOutOfBounds(x, y, z) ? 0 : cubes[x,y,z];

            private bool IsOutOfBounds(int x, int y, int z)
                => x < 0 || x >= xLength ||
                    y < 0 || y >= yLength ||
                    z < 0 || z >= zLength;
        }

        public class _4Space
        {
            private readonly int[,,,] cubes;
            private readonly int xLength;
            private readonly int yLength;
            private readonly int zLength;
            private readonly int wLength;

            public _4Space(char[][] initial)
            {
                xLength = initial.Length;
                yLength = initial[0].Length;
                zLength = wLength = 1;
                cubes = new int[xLength, yLength, zLength, wLength];

                for (var x = 0; x < xLength; x++)
                for (var y = 0; y < yLength; y++)
                {
                    cubes[x,y,0,0] = initial[x][y] == '#' ? 1 : 0;
                }
            }

            private _4Space(int[,,,] cubes)
            {
                this.cubes = cubes;
                xLength = cubes.GetLength(0);
                yLength = cubes.GetLength(1);
                zLength = cubes.GetLength(2);
                wLength = cubes.GetLength(3);
            }

            public _4Space Step()
            {
                var next = new int[xLength+2, yLength+2, zLength+2, wLength+2];

                for (int x = 0; x <= xLength + 1; x++)
                for (int y = 0; y <= yLength + 1; y++)
                for (int z = 0; z <= zLength + 1; z++)
                for (int w = 0; w <= wLength + 1; w++)
                {
                    next[x,y,z,w] = GetNext(x-1, y-1, z-1, w-1);
                }

                return new _4Space(next);
            }

            public int GetActiveCubeCount()
            {
                var count = 0;

                for (int x = 0; x < xLength; x++)
                for (int y = 0; y < yLength; y++)
                for (int z = 0; z < zLength; z++)
                for (int w = 0; w < wLength; w++)
                {
                    count += cubes[x,y,z,w];
                }

                return count;
            }

            private int GetNext(int x, int y, int z, int w) =>
                CountActiveNeighbours(x, y, z, w) switch
                {
                    3 => 1,
                    2 => IsOutOfBounds(x,y,z,w) ? 0 : cubes[x,y,z,w],
                    _ => 0
                };

            private int CountActiveNeighbours(int x, int y, int z, int w)
            {
                var count = 0;

                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++)
                for (int dw = -1; dw <= 1; dw++)
                {
                    if (dx != 0 || dy != 0 || dz != 0 || dw != 0)
                    {
                        count += GetState(x+dx, y+dy, z+dz, w+dw);
                    }
                }

                return count;
            }

            private int GetState(int x, int y, int z, int w) => IsOutOfBounds(x, y, z, w) ? 0 : cubes[x,y,z,w];

            private bool IsOutOfBounds(int x, int y, int z, int w)
                => x < 0 || x >= xLength ||
                    y < 0 || y >= yLength ||
                    z < 0 || z >= zLength ||
                    w < 0 || w >= wLength;
        }
    }
}

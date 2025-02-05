using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Input;

namespace AdventOfCode2021.Solvers
{
    internal class SolverL : Solver
    {
        private readonly Caves caves;

        public SolverL(PuzzleInput input) => caves = new Caves(input.Lines);

        protected override string SolvePart1() => caves.GetPathCount().Count().ToString();

        protected override string SolvePart2() => caves.GetPathCount(true).Count().ToString();

        internal class Caves
        {
            private readonly Dictionary<string, Cave> caves = new();

            public Caves(IEnumerable<string> input)
            {
                foreach (var passage in input)
                {
                    var ends = passage.Split('-');
                    var from = GetOrCreate(ends[0]);
                    var to = GetOrCreate(ends[1]);
                    from.Add(to);
                    to.Add(from);
                }
            }

            private Cave GetOrCreate(string name)
            {
                if (caves.TryGetValue(name, out var cave)) { return cave; }
                return caves[name] = new Cave(name);
            }

            public List<string> GetPathCount(bool allowDoubleVisit = false)
            {
                var visited = new HashSet<string>();
                var secondVisit = new HashSet<string>();
                var pathCount = 0;
                var paths = new List<Path>();
                var path = new Path("");

                FindPathsFrom(caves["start"], path);

                return paths.Select(p => p.Caves).ToList();

                void FindPathsFrom(Cave cave, Path path)
                {
                    path = path with { Caves = path.Caves + cave.Name + "," };

                    if (!visited.Add(cave.Name) && cave.IsSmall)
                    {
                        secondVisit.Add(cave.Name);
                    }

                    foreach (var neighbour in cave.Neighbours)
                    {
                        if (neighbour.Name == "end")
                        {
                            paths.Add(path with { Caves = path.Caves + "end" });
                            pathCount++;
                        }
                        else if (neighbour.Name != "start" && CanEnter(neighbour))
                        {
                            FindPathsFrom(neighbour, path);
                        }
                    }

                    if (!secondVisit.Remove(cave.Name))
                    {
                        visited.Remove(cave.Name);
                    }
                }

                bool CanEnter(Cave cave) =>
                    cave.IsLarge ||
                    cave.IsSmall && (!visited.Contains(cave.Name) || secondVisit.Count == 0 && allowDoubleVisit);
            }
        }

        internal record Path(string Caves);

        internal class Cave
        {
            private readonly List<Cave> neighbours = new();

            public Cave(string name)
            {
                Name = name;
                IsLarge = char.IsUpper(name[0]);
            }

            public string Name { get; }
            public bool IsLarge { get; }
            public bool IsSmall => !IsLarge;
            public IEnumerable<Cave> Neighbours => neighbours;

            public void Add(Cave neighbour) => neighbours.Add(neighbour);
        }
    }
}

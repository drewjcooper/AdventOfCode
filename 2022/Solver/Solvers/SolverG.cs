using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverG : Solver
{
    private readonly FileSystem fileSystem = new();

    public SolverG(PuzzleInput input)
    {
        foreach (var line in input.Lines.Select(l => Line.Parse(l)))
        {
            line.Evaluate(fileSystem);
        }
    }
    
    protected override string SolvePart1() => 
        fileSystem.FindDirectories(d => d.Size <= 100_000).Sum(d => d.Size).ToString();

    protected override string SolvePart2()
    {
        const int freeSpaceNeeded = 30_000_000;
        const int capacity = 70_000_000;
        var freeSpace = capacity - fileSystem.Size;
        var spaceNeeded = freeSpaceNeeded - freeSpace;

        var toBeDeleted = fileSystem.FindDirectories(d => d.Size >= spaceNeeded).OrderBy(d => d.Size).First();
        return toBeDeleted.Size.ToString();
    }

    private class FileSystem
    {
        private readonly Directory root = new("/");
        private readonly Stack<Directory> currentPath = new();

        private Directory Current => currentPath.Peek();
        public int Size => root.Size;

        public void AddDirectory(string name) => Current.AddDirectory(name);

        public void AddFile(string name, int size) => Current.AddFile(name, size);

        public void ChangeDirectory(string name) 
        {
            switch (name)
            {
                case "/":
                    currentPath.Clear();
                    currentPath.Push(root);
                    break;

                case "..":
                    currentPath.Pop();
                    break;

                default:
                    currentPath.Push(Current.FindDirectory(name));
                    break;
            }
        } 

        public IEnumerable<Directory> FindDirectories(Func<Directory, bool> predicate) => 
            root.FindDirectories(predicate);

        public record Directory(string Name)
        {
            private readonly List<File> files = new();
            private readonly List<Directory> directories = new();

            public int Size => directories.Sum(d => d.Size) + files.Sum(f => f.Size);

            public void AddFile(string name, int size) => files.Add(new(name, size));

            public void AddDirectory(string name) => directories.Add(new(name));

            public Directory FindDirectory(string name) => directories.First(d => d.Name == name);

            public IEnumerable<Directory> FindDirectories(Func<Directory, bool> predicate) => 
                directories.SelectMany(d => d.FindDirectories(predicate)).Concat(directories.Where(predicate));
        }

        private record File (string Name, int Size);
    }

    private abstract class Line
    {
        public abstract void Evaluate(FileSystem fileSystem);

        public static Line Parse(string line)
            => line[0] switch
            {
                '$' => Command.Parse(line),
                'd' => Directory.Parse(line),
                _ => File.Parse(line)
            };

        private abstract class Command : Line
        {
            public new static Command Parse(string line)
                => line.Split(' ')[1..] switch
                {
                    ["cd", string argument] => new ChangeDirectory(argument),
                    ["ls"] => new List(),
                    _ => throw new ArgumentException($"Unknown command: {line}", nameof(line))
                };

            private class ChangeDirectory : Command
            {
                private readonly string name;

                public ChangeDirectory(string name) => this.name = name;

                public override void Evaluate(FileSystem fileSystem) => fileSystem.ChangeDirectory(name);
            }

            private class List : Command
            {
                public override void Evaluate(FileSystem fileSystem)
                {
                }
            }
        }

        private class Directory : Line
        {
            private readonly string name;

            public Directory(string name) => this.name = name;

            public new static Directory Parse(string line) => new(line.Split(' ')[1]);

            public override void Evaluate(FileSystem fileSystem) => fileSystem.AddDirectory(name);
        }

        private class File : Line
        {
            private readonly string name;
            private readonly int size;

            public File(string name, int size)
            {
                this.name = name;
                this.size = size;
            }

            public new static File Parse(string line) 
                => line.Split(' ') is [string size, string name] 
                    ? new File(name, int.Parse(size))
                    : throw new ArgumentException("Unexpected format", nameof(line));

            public override void Evaluate(FileSystem fileSystem) => fileSystem.AddFile(name, size);
        }
    }
}
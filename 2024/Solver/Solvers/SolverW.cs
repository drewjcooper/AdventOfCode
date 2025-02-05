using System.Collections.Immutable;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public class SolverW(PuzzleInput input) : Solver
{
    private readonly Lan _lan = Lan.Parse(input.Lines);

    protected override Answer SolvePart1() => _lan.FindGroups();

    protected override Answer SolvePart2() => _lan.FindLargestGroupPassword();

    public class Lan
    {
        private readonly HashSet<Connection> _connections = [];
        private readonly Dictionary<string, Computer> _computers = [];

        public void AddConnection(string connection)
        {
            var computerNames = connection.Split('-');

            var computer1 = AddOrGet(computerNames[0]);
            var computer2 = AddOrGet(computerNames[1]);
            _connections.Add(new(computer1, computer2));
            computer1.ConnectTo(computer2);
            computer2.ConnectTo(computer1);
        }

        private Computer AddOrGet(string name)
        {
            if (!_computers.TryGetValue(name, out var computer))
            {
                _computers[name] = computer = new(name);
            }

            return computer;
        }

        public static Lan Parse(string[] input)
        {
            var lan = new Lan();
            foreach (var line in input) 
            {
                lan.AddConnection(line);
            }
            return lan;
        }

        public int FindGroups() => 
            _computers.Values.Where(c1 => c1.Name[0] == 't')
                .SelectMany(c1 => c1.Connected
                    .SelectMany(c2 => c2.Connected
                        .Where(c3 => c3.ConnectsTo(c1))
                        .Select(c3 => new Group(c1.Name, c2.Name, c3.Name))))
                .Distinct()
                .Count();

        public string FindLargestGroupPassword()
            => string.Join(",", BronKerbosch([], [.. _computers.Values], [])
                .MaxBy(c => c.Count)!
                .Select(c => c.Name)
                .Order());

        private IEnumerable<ImmutableHashSet<Computer>> BronKerbosch(
            ImmutableHashSet<Computer> r,
            ImmutableHashSet<Computer> p,
            ImmutableHashSet<Computer> x)
        {
            if (p.Count == 0 && x.Count == 0) 
            { 
                yield return r;
                yield break;
            }

            var u = p.Concat(x).First();
            foreach (var v in p.Except(u.Connected))
            {
                foreach (var c in BronKerbosch(r.Add(v), p.Intersect(v.Connected), x.Intersect(v.Connected)))
                {
                    yield return c;
                }
                p.Remove(v);
                x.Add(v);
            }
        }
    }

    private class Group(params string[] computers) : IEquatable<Group>
    {
        private readonly ImmutableArray<string> _computers = [.. computers.Order()];

        public bool Equals(Group? other) => other != null && _computers.SequenceEqual(other._computers);

        public override bool Equals(object? obj) => Equals(obj as Group);

        public override int GetHashCode() => HashCode.Combine(_computers[0], _computers[1], _computers[2]);
    }

    private class Connection : IEquatable<Connection>
    {
        private readonly Computer _computer1;
        private readonly Computer _computer2;

        public Connection(Computer computer1, Computer computer2)
        {
            (_computer1, _computer2) = computer1 < computer2 ? (computer1, computer2) : (computer2, computer1);
        }

        public bool Equals(Connection? other) 
            => other != null && other._computer1 == _computer1 && other._computer2 == _computer2;

        public override bool Equals(object? obj) => Equals(obj as Connection);

        public override int GetHashCode() => HashCode.Combine(_computer1, _computer2);
    }

    private class Computer(string name) : IEquatable<Computer>
    {
        private readonly HashSet<Computer> _connected = [];

        public string Name { get; } = name;

        public IEnumerable<Computer> Connected => _connected;

        public bool ConnectsTo(Computer computer) => _connected.Contains(computer);

        public void ConnectTo(Computer computer) => _connected.Add(computer);

        public bool Equals(Computer? other) => other != null && other.Name == Name;

        public override bool Equals(object? obj) => Equals(obj as Computer);

        public override int GetHashCode() => HashCode.Combine(nameof(Computer), Name);

        public override string ToString() => Name;

        public static bool operator <(Computer x, Computer y) => x.Name.CompareTo(y.Name) < 0;
        public static bool operator >(Computer x, Computer y) => x.Name.CompareTo(y.Name) > 0;
    }
}

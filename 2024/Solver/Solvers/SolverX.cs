using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Helpers;
using AdventOfCode.Input;

namespace AdventOfCode.Solvers;

public partial class SolverX(PuzzleInput input) : Solver
{
    private readonly Circuit _circuit = Circuit.Parse(input.Lines);

    protected override Answer SolvePart1() => _circuit.GetOutput();

    protected override Answer SolvePart2() => string.Join(",", _circuit.GetWireSwaps().Select(w => w?.Name).Order());

    private partial class Circuit
    {
        private readonly Dictionary<string, Wire> _wires = [];
        private readonly List<Input> _inputs = [];

        public Wire this[string name]
        {
            get
            {
                if (!_wires.TryGetValue(name, out var wire))
                {
                    _wires[name] = wire = new Wire(name);
                }
                return wire;
            }
        }

        private void AddInput(string wireName, int value) => _inputs.Add(new(this[wireName], value));

        public long GetOutput()
        {
            foreach (var input in _inputs) { input.Set(); }

            return GetValue('z');
        }

        private void SetInput(char index, long value)
        {
            foreach (var wire in GetWires(index).Reverse())
            {
                wire.SetValue((int)(value % 2));
                value >>= 1;
            }
        }

        public IEnumerable<Wire?> GetWireSwaps()
        {
            SetInput('x', 0);

            var yWidth = GetWires('y').Count();
            for (int i = 0; i < yWidth; i++)
            {
                var y = 1L << i;
                SetInput('y', y);

                if (GetValue('z') != y)
                {
                    foreach (var wire in GetWireSwaps(i))
                    {
                        yield return wire;
                    }
                }
            }
        }

        private IEnumerable<Wire?> GetWireSwaps(int inputIndex)
        {
            Console.WriteLine($"Error in input channel {inputIndex}");

            var xInput = _wires[$"x{inputIndex:00}"];
            var xor1Out = xInput.GetGate<Xor>()!.Output;
            var xor2Out = xor1Out.GetGate<Xor>()?.Output;

            if (xor2Out == null)
            {
                var and1Out = xInput.GetGate<And>()!.Output;
                yield return xor1Out;
                yield return and1Out;

            }
            else
            {
                yield return _wires[$"z{inputIndex:00}"];
                yield return xor2Out;
            }
        }

        public string DumpAnalysis()
        {
            var sb = new StringBuilder();

            SetInput('x', 0);
            sb.AppendLine().Append("x: ").AppendJoin("", GetBits('x')).AppendLine();

            var yWidth = GetWires('y').Count();
            for (int i = 0; i < yWidth; i++)
            {
                SetInput('y', 1L << i);
                sb.Append($"    y{i:00}: ").AppendJoin("", GetBits('y'));
                sb.Append("\tout: ").AppendJoin("", GetBits('z')).AppendLine();
            }

            SetInput('y', 0);
            sb.AppendLine().Append("y: ").AppendJoin("", GetBits('y')).AppendLine();

            var xWidth = GetWires('x').Count();
            for (int i = 0; i < xWidth; i++)
            {
                SetInput('x', 1L << i);
                sb.Append($"    x{i:00}: ").AppendJoin("", GetBits('x'));
                sb.Append("\tout: ").AppendJoin("", GetBits('z')).AppendLine();
            }

            return sb.ToString();
        }

        private long GetValue(char index) => GetWires(index).Aggregate(0L, (a, w) => (a << 1) + w.Value!.Value);
        private IEnumerable<char> GetBits(char index) => GetWires(index).Select(w => (char)(w.Value!.Value + '0'));

        private IEnumerable<Wire> GetWires(char index)
            => _wires.Where(kv => kv.Key.StartsWith(index))
                .OrderByDescending(kv => kv.Key)
                .Select(kv => kv.Value);

        public static Circuit Parse(IEnumerable<string> lines)
        {
            var circuit = new Circuit();

            var (inputs, gates) = lines.Split("").ToArray();

            foreach (var line in inputs)
            {
                var (wireName, value) = line.Split(':', StringSplitOptions.TrimEntries);
                circuit.AddInput(wireName, int.Parse(value));
            }

            foreach (var line in gates)
            {
                Gate.Parse(line, circuit);
            }

            return circuit;
        }

        private class Input(Wire wire, int value)
        {
            private readonly Wire _wire = wire;
            private readonly int _value = value;

            public void Set() => _wire.SetValue(_value);
        }
    }


    private class Wire(string name)
    {
        private int? _value;

        private readonly List<Gate> _gates = [];

        public string Name { get; } = name;

        public int? Value => _value;

        public bool IsSet => _value.HasValue;

        public bool TryGetValue(out int value)
        {
            value = _value ?? 0;
            return _value.HasValue;
        }

        public void SetValue(int value)
        {
            _value = value;
            foreach (var gate in _gates)
            {
                gate.DriveOutput();
            }
        }

        public void ConnectTo(Gate gate) => _gates.Add(gate);

        public T? GetGate<T>() where T : Gate
        {
            var gate = _gates.OfType<T>().FirstOrDefault();
            Console.WriteLine($"{typeof(T).Name} gate from {this}: {gate}");
            return gate;
        }

        public IEnumerable<Gate> GetGates()
        {
            Console.WriteLine($"""
                Gates from {this}:
                    {string.Join($"{Environment.NewLine}    ", _gates)}
                """);
            return _gates;
        }


        public Gate? GetGate() => _gates.FirstOrDefault();

        public override string ToString() => Name;
    }

    private abstract partial class Gate
    {
        private readonly Wire _input1;
        private readonly Wire _input2;
        private readonly Wire _output;

        public Gate(Wire input1, Wire input2, Wire output)
        {
            _input1 = input1;
            _input2 = input2;
            _output = output;

            input1.ConnectTo(this);
            input2.ConnectTo(this);
        }

        public Wire Output => _output;

        protected abstract int GetOutput(int value1, int value2);

        public void DriveOutput()
        {
            if (_input1.TryGetValue(out var value1) && _input2.TryGetValue(out var value2))
            {
                _output.SetValue(GetOutput(value1, value2));
            }
        }

        public static Gate Parse(string input, Circuit circuit)
        {
            var match = GateSpecification().Match(input);

            var input1 = GetWire("input1");
            var input2 = GetWire("input2");
            var output = GetWire("output");

            return match.Groups["operation"].Value switch
            {
                "AND" => new And(input1, input2, output),
                "OR" => new Or(input1, input2, output),
                "XOR" => new Xor(input1, input2, output),
                var unknown => throw new ArgumentException($"Unkown operation {unknown}", nameof(input))
            };

            Wire GetWire(string name) => circuit[match.Groups[name].Value];
        }

        public override string ToString() => $"{_input1} {GetType().Name} {_input2} => {_output}";

        [GeneratedRegex(@"^(?'input1'\w+) (?'operation'(AND|OR|XOR)) (?'input2'\w+) -> (?'output'\w+)$")]
        private static partial Regex GateSpecification();
    }

    private class And(Wire input1, Wire input2, Wire output) : Gate(input1, input2, output)
    {
        protected override int GetOutput(int value1, int value2) => value1 & value2;
    }

    private class Or(Wire input1, Wire input2, Wire output) : Gate(input1, input2, output)
    {
        protected override int GetOutput(int value1, int value2) => value1 | value2;
    }

    private class Xor(Wire input1, Wire input2, Wire output) : Gate(input1, input2, output)
    {
        protected override int GetOutput(int value1, int value2) => value1 ^ value2;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Input;

using static AdventOfCode2021.Solvers.SolverP;

namespace AdventOfCode2021.Solvers
{
    internal class SolverP : Solver
    {
        private static readonly Dictionary<char, string> hexToBinary = new()
        {
            ['0'] = "0000",
            ['1'] = "0001",
            ['2'] = "0010",
            ['3'] = "0011",
            ['4'] = "0100",
            ['5'] = "0101",
            ['6'] = "0110",
            ['7'] = "0111",
            ['8'] = "1000",
            ['9'] = "1001",
            ['A'] = "1010",
            ['B'] = "1011",
            ['C'] = "1100",
            ['D'] = "1101",
            ['E'] = "1110",
            ['F'] = "1111"
        };

        private readonly IEnumerable<int> input;

        public SolverP(PuzzleInput input)
        {
            this.input = input.RawText.SelectMany(h => hexToBinary[h]).Select(b => b - '0');
        }

        protected override string SolvePart1() => input.ReadPacket().VersionSum.ToString();

        protected override string SolvePart2() => input.ReadPacket().Value.ToString();

        internal abstract class Packet
        {
            protected Packet(int version)
            {
                Version = version;
            }

            public int Version { get; }

            public abstract long Value { get; }
            public abstract int VersionSum { get; }
        }

        internal class ValuePacket : Packet
        {
            public ValuePacket(int version, IEnumerator<int> bits)
                : base(version)
            {
                while (true)
                {
                    var nibble = bits.ReadNibble();
                    Value = (Value << 4) + nibble.Value;
                    if (nibble.IsLast) { break; }
                }
            }

            public override long Value { get; }

            public override int VersionSum => Version;
        }

        internal abstract class OperatorPacket : Packet
        {
            public OperatorPacket(int version, IEnumerator<int> bits)
                : base(version)
            {
                var lengthType = bits.ReadNext();
                var lengthBitCount = lengthType == 0 ? 15 : 11;
                var length = 0;
                for (int i = 0; i < lengthBitCount; i++)
                {
                    length = (length << 1) + bits.ReadNext();
                }

                Packets = lengthType == 0 ? bits.ReadPacketsByLength(length) : bits.ReadPacketsByCount(length);
            }

            protected IEnumerable<Packet> Packets { get; }

            public override int VersionSum => Packets.Sum(p => p.VersionSum) + Version;
        }

        internal class SumPacket : OperatorPacket
        {
            public SumPacket(int version, IEnumerator<int> bits)
                : base(version, bits)
            {
            }

            public override long Value => Packets.Sum(p => p.Value);
        }

        internal class ProductPacket : OperatorPacket
        {
            public ProductPacket(int version, IEnumerator<int> bits)
                : base(version, bits)
            {
            }

            public override long Value => Packets.Aggregate(1L, (a, p) => a * p.Value);
        }

        internal class MinimumPacket : OperatorPacket
        {
            public MinimumPacket(int version, IEnumerator<int> bits)
                : base(version, bits)
            {
            }

            public override long Value => Packets.Min(p => p.Value);
        }

        internal class MaximumPacket : OperatorPacket
        {
            public MaximumPacket(int version, IEnumerator<int> bits)
                : base(version, bits)
            {
            }

            public override long Value => Packets.Max(p => p.Value);
        }

        internal abstract class BinaryOperatorPacket : OperatorPacket
        {
            public BinaryOperatorPacket(int version, IEnumerator<int> bits)
                : base(version, bits)
            {
                Operands = Packets.ToArray();
            }

            protected Packet[] Operands;
        }

        internal class GreaterThanPacket : BinaryOperatorPacket
        {
            public GreaterThanPacket(int version, IEnumerator<int> bits)
                : base(version, bits)
            {
            }

            public override long Value => Operands[0].Value > Operands[1].Value ? 1 : 0;
        }

        internal class LessThanPacket : BinaryOperatorPacket
        {
            public LessThanPacket(int version, IEnumerator<int> bits)
                : base(version, bits)
            {
            }

            public override long Value => Operands[0].Value < Operands[1].Value ? 1 : 0;
        }

        internal class EqualToPacket : BinaryOperatorPacket
        {
            public EqualToPacket(int version, IEnumerator<int> bits)
                : base(version, bits)
            {
            }

            public override long Value => Operands[0].Value == Operands[1].Value ? 1 : 0;
        }

        internal record Nibble(int Value, bool IsLast)
        {
        }
    }

    public static class SolverPExtensions
    {
        internal static int ReadTriad(this IEnumerator<int> bits)
        {
            var value = 0;

            for (int i = 0; i < 3; i++)
            {
                value = value * 2 + bits.ReadNext();
            }

            return value;
        }

        internal static Nibble ReadNibble(this IEnumerator<int> bits)
        {
            var isLast = bits.ReadNext() == 0;
            var value = 0;

            for (int i = 0; i < 4; i++)
            {
                value = value * 2 + bits.ReadNext();
            }

            return new(value, isLast);
        }

        internal static int ReadNext(this IEnumerator<int> bits)
        {
            bits.MoveNext();
            return bits.Current;
        }

        internal static Packet ReadPacket(this IEnumerable<int> bits) => bits.GetEnumerator().ReadPacket();

        internal static Packet ReadPacket(this IEnumerator<int> bits)
        {
            var version = bits.ReadTriad();
            var type = bits.ReadTriad();

            return type switch
            {
                0 => new SumPacket(version, bits),
                1 => new ProductPacket(version, bits),
                2 => new MinimumPacket(version, bits),
                3 => new MaximumPacket(version, bits),
                4 => new ValuePacket(version, bits),
                5 => new GreaterThanPacket(version, bits),
                6 => new LessThanPacket(version, bits),
                7 => new EqualToPacket(version, bits),
                _ => throw new InvalidOperationException()
            };
        }

        internal static IEnumerable<Packet> ReadPacketsByCount(this IEnumerator<int> bits, int packetCount) =>
            Enumerable.Range(0, packetCount).Select(_ => bits.ReadPacket()).ToList();

        internal static IEnumerable<Packet> ReadPacketsByLength(this IEnumerator<int> bits, int packetsLength) =>
            bits.SubSequence(packetsLength).ReadPackets();

        internal static IEnumerable<Packet> ReadPackets(this IEnumerable<int> bits) => bits.GetEnumerator().ReadPackets();

        internal static IEnumerable<Packet> ReadPackets(this IEnumerator<int> bits)
        {
            var packets = new List<Packet>();

            while (true)
            {
                if (!bits.TryReadPacket(out var packet)) { break; }
                packets.Add(packet);
            }

            return packets;
        }

        internal static bool TryReadPacket(this IEnumerator<int> bits, out Packet packet)
        {
            try
            {
                packet = bits.ReadPacket();
                return true;
            }
            catch
            {
                packet = default;
                return false;
            }
        }

        internal static IEnumerable<int> SubSequence(this IEnumerator<int> bits, int count)
        {
            var buffer = new int[count];

            for (int i = 0; i < count; i++)
            {
                buffer[i] = bits.ReadNext();
            }

            return buffer;
        }
    }
}

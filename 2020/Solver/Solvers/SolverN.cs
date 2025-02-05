using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverN : Solver
    {
        private readonly string[] lines;

        public SolverN(PuzzleInput input)
        {
            this.lines = input.Lines;
        }

        protected override string SolvePart1() => ExecuteCommands<MaskV1>(new DecoderV1());

        private string ExecuteCommands<TMask>(Decoder decoder)
            where TMask : Mask
        {
            foreach (var command in lines.Select(l => Command.Parse<TMask>(l)))
            {
                decoder.Execute(command);
            }
            return decoder.MemorySum.ToString();
        }

        protected override string SolvePart2() => ExecuteCommands<MaskV2>(new DecoderV2());

        public abstract class Decoder
        {
            protected readonly Dictionary<long, long> memory = new();
            protected Mask mask;

            public Mask Mask { set => mask = value; }

            public long MemorySum => memory.Values.Sum();

            public abstract void WriteMemory(long address, long rawValue);

            public void Execute(Command command) => command.Execute(this);
        }

        public class DecoderV1 : Decoder
        {
            public override void WriteMemory(long address, long rawValue) => memory[address] = mask.MaskValue(rawValue);
        }

        public class DecoderV2 : Decoder
        {
            public override void WriteMemory(long rawAddress, long value)
            {
                foreach (var address in mask.GetAddresses(rawAddress))
                {
                    memory[address] = value;
                }
            }
        }

        public abstract class Command
        {
            public abstract void Execute(Decoder computer);

            public static Command Parse<TMask>(string command)
                where TMask : Mask
            {
                var commandParts = command.Split('=', StringSplitOptions.TrimEntries);
                if (commandParts[0] == "mask")
                {
                    var mask = (Mask)Activator.CreateInstance(typeof(TMask), new object[] { commandParts[1] });
                    return new SetMaskCommand(mask);
                }

                var address = long.Parse(commandParts[0].Trim("mem[]".ToCharArray()));
                var rawValue = long.Parse(commandParts[1]);

                return new WriteMemoryCommand(address, rawValue);
            }
        }

        public class SetMaskCommand : Command
        {
            private readonly Mask mask;

            public SetMaskCommand(Mask mask) => this.mask = mask;

            public override void Execute(Decoder decoder) => decoder.Mask = mask;
        }

        public class WriteMemoryCommand : Command
        {
            private readonly long address;
            private readonly long value;

            public WriteMemoryCommand(long address, long value)
            {
                this.address = address;
                this.value = value;
            }

            public override void Execute(Decoder computer) => computer.WriteMemory(address, value);
        }

        public abstract class Mask
        {
            public abstract long MaskValue(long value);
            public abstract IEnumerable<long> GetAddresses(long address);
        }

        public class MaskV1 : Mask
        {
            private readonly long onesMask;
            private readonly long zerosMask;

            public MaskV1() { }

            public MaskV1(string mask)
            {
                foreach (var ch in mask)
                {
                    onesMask = (onesMask << 1) + (ch == '1' ? 1 : 0);
                    zerosMask = (zerosMask << 1) + (ch == 'X' ? 1 : 0);
                }
            }

            public override long MaskValue(long value) => value & zerosMask | onesMask;

            public override IEnumerable<long> GetAddresses(long address) => Enumerable.Repeat(address, 1);
        }

        public class MaskV2 : Mask
        {
            private readonly List<long> onesMasks = new();
            private readonly List<long> zerosMasks = new();

            public MaskV2() { }

            public MaskV2(string mask)
            {
                onesMasks.Add(0);
                zerosMasks.Add(0);

                foreach (var ch in mask)
                {
                    switch (ch)
                    {
                        case '1': Handle1(); break;
                        case '0': Handle0(); break;
                        case 'X': HandleX(); break;
                    }
                }
            }

            private void Handle0()
            {
                Extend(onesMasks, 0);
                Extend(zerosMasks, 1);
            }

            private void Handle1()
            {
                Extend(onesMasks, 1);
                Extend(zerosMasks, 1);
            }

            private void HandleX()
            {
                DuplicateAndExtend(onesMasks);
                DuplicateAndExtend(zerosMasks);
            }

            private void DuplicateAndExtend(List<long> masks)
            {
                var newMasks = masks.ToList();
                Extend(masks, 0);
                Extend(newMasks, 1);
                masks.AddRange(newMasks);
            }

            private void Extend(List<long> masks, int digit)
            {
                for (int i = 0; i < masks.Count; i++)
                {
                    masks[i] = Extend(masks[i], digit);
                }
            }

            private long Extend(long mask, int digit) => (mask << 1) + digit;

            public override long MaskValue(long value) => value;

            public override IEnumerable<long> GetAddresses(long address)
            {
                for (int i = 0; i < onesMasks.Count; i++)
                {
                    yield return address & zerosMasks[i] | onesMasks[i];
                }
            }
        }
    }
}

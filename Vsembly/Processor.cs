using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Vsembly.Test.Unit")]

namespace Vsembly
{
    ///<summary>
    /// REGISTERS BYTE-CODES
    ///
    /// 0 - r0
    /// 1 - r1
    /// 2 - r2
    /// 3 - r3
    ///</summary>
    public sealed class Processor
    {
        internal static readonly Action<Processor>[] instructions = new Action<Processor>[]
        {
            // 00 - NOP
            (processor) => { },
            // 01 register value               MOV register, value
            (processor) => {
                processor.registers[getInstructionParameter(processor, 1)] = getInstructionParameter(processor, 2);
                processor.ip += 2;
            },
            // 02 register register            MOV register, register
            (processor) =>
            {
                processor.registers[getInstructionParameter(processor, 1)] = processor.registers[getInstructionParameter(processor, 2)];
                processor.ip += 2;
            },
            // 03 register [address]           MOV register, [address]
            (processor) =>
            {
                processor.registers[getInstructionParameter(processor, 1)] = processor.memory[getInstructionParameter(processor, 2)];
                processor.ip += 2;
            },
            // 04 register [register]          MOV register, [register]
            (processor) =>
            {
                processor.registers[getInstructionParameter(processor, 1)] = processor.memory[processor.registers[getInstructionParameter(processor, 2)]];
                processor.ip += 2;
            },
            // 05 [address], reg               MOV [address], reg
            (processor) =>
            {
                processor.memory[getInstructionParameter(processor, 1)] = processor.registers[getInstructionParameter(processor,2)];
                processor.ip += 2;
            },
            // 06 [register], reg              MOV [register], reg
            (processor) =>
            {
                processor.memory[processor.registers[getInstructionParameter(processor, 1)]] = processor.registers[getInstructionParameter(processor, 2)];
                processor.ip += 2;
            },
        };

        private static int getInstructionParameter(Processor processor, int offset)
        {
            return processor.memory[processor.ip + offset];
        }

        public int[] registers { get; set; } = new int[4];

        public int ip { get; private set; } = 0;

        public int[] memory { get; set; } = new int[1024 * 1024];

        public void Step()
        {
            instructions[memory[ip]].Invoke(this);
            ip++;
        }

        internal void SetRegisterValue(int register, int value)
        {
            registers[register] = value;
        }

        internal int GetRegisterValue(int register)
        {
            return registers[register];
        }
    }
}

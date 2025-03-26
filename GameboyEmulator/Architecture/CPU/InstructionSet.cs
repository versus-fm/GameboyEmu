using GameboyEmulator.Architecture.Interface;
using GameboyEmulator.Architecture.Rendering;
using GameboyEmulator.Exceptions;

namespace GameboyEmulator.Architecture;

public class InstructionSet
{
    /* Registers parameter is the same as hardware.Registers, but passed as a parameter to make multiple accesses within Registers slightly faster */
    private delegate void Instruction(IProcessor processor, ref Registers registers);

    private readonly Instruction[] standardInstructions;
    private readonly Instruction[] extendedInstructions;
    
    public InstructionSet()
    {
        this.standardInstructions = new Instruction[256];
        this.extendedInstructions = new Instruction[256];
        this.BuildStandardInstructions();
        this.BuildExtendedInstructions();
    }
    
    public void ExecuteInstruction(IProcessor processor, byte opcode)
    {
        if (opcode == 0xCB)
        {
            var extendedOpcode = processor.ReadByte(processor.Registers.ProgramCounter);
            processor.Registers.IncProgramCounter();
            this.extendedInstructions[extendedOpcode](processor, ref processor.Registers);
        }
        else
        { 
            //Console.WriteLine($"{(processor.Registers.ProgramCounter - 1):X4}: {(OpCode)opcode} ({opcode:X2})");
            if (this.standardInstructions[opcode] == null)
            {
                Console.WriteLine($"{(processor.Registers.ProgramCounter - 1):X4}: {(OpCode)opcode} ({opcode:X2})");
            }
            this.standardInstructions[opcode](processor, ref processor.Registers);
        }
    }

    private void BuildStandardInstructions()
    {
        foreach (var opcode in Enum.GetValues<OpCode>())
        {
            this.standardInstructions[(byte)opcode] = GetInstruction(opcode);
        }
    }
    // Flags are listed in ZNHC order
    private Instruction GetInstruction(OpCode opCode)
    {
        return opCode switch
        {
            OpCode.NOP => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.Cycle();
            },
            OpCode.LD_BC_d16 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadWord(registers.ProgramCounter);
                registers.BC = value;
                registers.IncProgramCounter(2);
                hardware.Cycle(3);
            },
            OpCode.LD_BC_A => (IProcessor hardware, ref Registers registers) => 
            {
                hardware.WriteByte(registers.BC, registers.Accumulator);
                hardware.Cycle(2);
            },
            OpCode.INC_BC => (IProcessor hardware, ref Registers registers) =>
            {
                registers.BC++;
                hardware.Cycle(2);
            },
            OpCode.INC_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetAddHalfCarry(registers.B, 1);
                registers.B++;
                registers.Zero = registers.B == 0;
                registers.Subtract = false;
                hardware.Cycle(1);
            },
            OpCode.DEC_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetSubHalfCarry(registers.B, 1);
                registers.B--;
                registers.Zero = registers.B == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.LD_B_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.B = value;
                hardware.Cycle(2);
            },
            OpCode.RLCA => (IProcessor hardware, ref Registers registers) =>
            {
                var a = registers.Accumulator;
                var msb = (a & 0x80) != 0;
                registers.SetZNHC(
                    false,  // Z is always reset
                    false,  // N is always reset
                    false,  // H is always reset
                    msb    // C is set to the MSB before rotation
                );
                registers.Accumulator = (byte)((a << 1) | (msb ? 1 : 0));
                hardware.Cycle();
            },
            OpCode.LD_a16_SP => (IProcessor hardware, ref Registers registers) =>
            {
                var address = hardware.ReadWord(registers.ProgramCounter);
                registers.IncProgramCounter(2);
                hardware.WriteWord(address, registers.StackPointer);
                hardware.Cycle(5);
            },
            OpCode.ADD_HL_BC => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.SetAddHalfCarry(registers.HL, registers.BC);
                registers.SetAddCarry(registers.HL, registers.BC);
                registers.HL += registers.BC;
                hardware.Cycle(2);
            },
            OpCode.LD_A_BC => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = hardware.ReadByte(registers.BC);
                hardware.Cycle(2);
            },
            OpCode.DEC_BC => (IProcessor hardware, ref Registers registers) =>
            {
                registers.BC--;
                hardware.Cycle(2);
            },
            OpCode.INC_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.SetAddHalfCarry(registers.C, 1);
                registers.C++;
                registers.Zero = registers.C == 0;
                hardware.Cycle(1);
            },
            OpCode.DEC_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.C, 1);
                registers.C--;
                registers.Zero = registers.C == 0;
                hardware.Cycle(1);
            },
            OpCode.LD_C_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.C = value;
                hardware.Cycle(2);
            },
            OpCode.RRCA => (IProcessor hardware, ref Registers registers) =>
            {
                var a = registers.Accumulator;
                var carry = (byte)(a & 1);
                registers.SetZNHC(
                    false,
                    false,
                    false,
                    (a & 0x1) != 0
                );
                registers.Accumulator = (byte)((a >> 1) | (carry << 7));
                hardware.Cycle();
            },
            OpCode.STOP => (IProcessor hardware, ref Registers registers) =>
            {
                var speedFlag = hardware.ReadSpeed();
                if (speedFlag.HasFlag(SpeedFlags.PrepareSpeedSwitch))
                {
                    speedFlag |= SpeedFlags.CurrentSpeed;
                    speedFlag &= ~SpeedFlags.PrepareSpeedSwitch;
                    hardware.WriteSpeed(speedFlag);
                }
                hardware.WriteByte(0xFF04, 0);
                hardware.Registers.IncProgramCounter();
            },
            OpCode.LD_DE_d16 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadWord(registers.ProgramCounter);
                registers.IncProgramCounter(2);
                registers.DE = value;
                hardware.Cycle(3);
            },
            OpCode.LD_DE_A => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.DE, registers.Accumulator);
                hardware.Cycle(2);
            },
            OpCode.INC_DE => (IProcessor hardware, ref Registers registers) =>
            {
                registers.DE++;
                hardware.Cycle(2);
            },
            OpCode.INC_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetAddHalfCarry(registers.D, 1);
                registers.D++;
                registers.Zero = registers.D == 0;
                registers.Subtract = false;
                hardware.Cycle(1);
            },
            OpCode.DEC_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetSubHalfCarry(registers.D, 1);
                registers.D--;
                registers.Zero = registers.D == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.LD_D_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.D = value;
                hardware.Cycle(2);
            },
            OpCode.RLA => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.SetZNHC(
                    false,
                    false,
                    false,
                    (registers.Accumulator & 0x80) != 0
                );
                registers.Accumulator <<= 1;
                registers.Accumulator |= (byte)(carry ? 1 : 0);
            },
            OpCode.JR_s8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = (sbyte)hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.ProgramCounter = (ushort)(registers.ProgramCounter + value);
                hardware.Cycle(3);
            },
            OpCode.ADD_HL_DE => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.DE;
                registers.Subtract = false;
                registers.SetAddHalfCarry(registers.HL, value);
                registers.SetAddCarry(registers.HL, value);
                registers.HL += value;
                hardware.Cycle(2);
            },
            OpCode.LD_A_DE => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = hardware.ReadByte(registers.DE);
                hardware.Cycle(2);
            },
            OpCode.DEC_DE => (IProcessor hardware, ref Registers registers) =>
            {
                registers.DE--;
                hardware.Cycle(2);
            },
            OpCode.INC_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetAddHalfCarry(registers.E, 1);
                registers.E++;
                registers.Zero = registers.E == 0;
                registers.Subtract = false;
                hardware.Cycle(1);
            },
            OpCode.DEC_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetSubHalfCarry(registers.E, 1);
                registers.E--;
                registers.Zero = registers.E == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.LD_E_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.E = value;
                hardware.Cycle(2);
            },
            OpCode.RRA => (IProcessor hardware, ref Registers registers) =>
            {
                var a = registers.Accumulator;
                registers.Accumulator = (byte)((byte)(a >> 1) | (byte)(registers.Carry ? 0x80 : 0));
                registers.SetZNHC(
                    false,
                    false,
                    false,
                    (a & 0b1) != 0
                );
            },
            OpCode.JR_NZ_s8 => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Zero)
                {
                    var value = (sbyte)hardware.ReadByte(registers.ProgramCounter);
                    registers.IncProgramCounter();
                    registers.ProgramCounter = (ushort)(registers.ProgramCounter + value);
                    hardware.Cycle(3);
                }
                else
                {
                    registers.ProgramCounter++;
                    hardware.Cycle(2);
                }
            },
            OpCode.LD_HL_d16 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadWord(registers.ProgramCounter);
                registers.IncProgramCounter(2);
                registers.HL = value;
                hardware.Cycle(3);
            },
            OpCode.LD_HLI_A => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.Accumulator);
                registers.HL++;
                hardware.Cycle(2);
            },
            OpCode.INC_HL => (IProcessor hardware, ref Registers registers) =>
            {
                registers.HL++;
                hardware.Cycle(2);
            },
            OpCode.INC_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetAddHalfCarry(registers.H, 1);
                registers.H++;
                registers.Zero = registers.H == 0;
                registers.Subtract = false;
                hardware.Cycle(1);
            },
            OpCode.DEC_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetSubHalfCarry(registers.H, 1);
                registers.H--;
                registers.Zero = registers.H == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.LD_H_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.H = value;
                hardware.Cycle(2);
            },
            OpCode.DAA => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Subtract)
                {
                    if (registers.Carry || registers.Accumulator > 0x99)
                    {
                        registers.Accumulator += 0x60;
                        registers.Carry = true;
                    }
                    if (registers.HalfCarry || (registers.Accumulator & 0x0F) > 0x09)
                    {
                        registers.Accumulator += 0x06;
                    }
                }
                else
                {
                    if (registers.Carry)
                    {
                        registers.Accumulator -= 0x60;
                    }

                    if (registers.HalfCarry)
                    {
                        registers.Accumulator -= 0x6;
                    }
                }

                registers.Zero = registers.Accumulator == 0;
                registers.HalfCarry = false;
            },
            OpCode.JR_Z_s8 => (IProcessor hardware, ref Registers registers) =>
            {
                if (registers.Zero)
                {
                    var value = (sbyte)hardware.ReadByte(registers.ProgramCounter);
                    registers.IncProgramCounter();
                    registers.ProgramCounter = (ushort)(registers.ProgramCounter + value);
                    hardware.Cycle(3);
                }
                else
                {
                    registers.ProgramCounter++;
                    hardware.Cycle(2);
                }
            },
            OpCode.ADD_HL_HL => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.HL;
                registers.Subtract = false;
                registers.SetAddHalfCarry(registers.HL, value);
                registers.SetAddCarry(registers.HL, value);
                registers.HL += value;
                hardware.Cycle(2);
            },
            OpCode.LD_A_HLI => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = hardware.ReadByte(registers.HL);
                registers.HL++;
                hardware.Cycle(2);
            },
            OpCode.DEC_HL => (IProcessor hardware, ref Registers registers) =>
            {
                registers.HL--;
                hardware.Cycle(2);
            },
            OpCode.INC_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetAddHalfCarry(registers.L, 1);
                registers.L++;
                registers.Zero = registers.L == 0;
                registers.Subtract = false;
                hardware.Cycle(1);
            },
            OpCode.DEC_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetSubHalfCarry(registers.L, 1);
                registers.L--;
                registers.Zero = registers.L == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.LD_L_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.L = value;
                hardware.Cycle(2);
            },
            OpCode.CPL => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = true;
                registers.HalfCarry = true;
                registers.Accumulator = (byte)(registers.Accumulator ^ 0xFF);
                hardware.Cycle(1);
            },
            OpCode.JR_NC_s8 => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Carry)
                {
                    var value = (sbyte)hardware.ReadByte(registers.ProgramCounter);
                    registers.IncProgramCounter();
                    registers.ProgramCounter = (ushort)(registers.ProgramCounter + value);
                    hardware.Cycle(3);
                }
                else
                {
                    registers.ProgramCounter++;
                    hardware.Cycle(2);
                }
            },
            OpCode.LD_SP_d16 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadWord(registers.ProgramCounter);
                registers.IncProgramCounter(2);
                registers.StackPointer = value;
                hardware.Cycle(3);
            },
            OpCode.LD_HLD_A => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.Accumulator);
                registers.HL--;
                hardware.Cycle(2);
            },
            OpCode.INC_SP => (IProcessor hardware, ref Registers registers) =>
            {
                registers.StackPointer++;
                hardware.Cycle(2);
            },
            OpCode.INC_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.SetAddHalfCarry(value, 1);
                hardware.WriteByte(registers.HL, (byte)(value + 1));
                registers.Zero = (byte)(value + 1) == 0;
                registers.Subtract = false;
                hardware.Cycle(3);
            },
            OpCode.DEC_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.SetSubHalfCarry(value, 1);
                hardware.WriteByte(registers.HL, (byte)(value - 1));
                registers.Zero = (byte)(value - 1) == 0;
                registers.Subtract = true;
                hardware.Cycle(3);
            },
            OpCode.LD_HLm_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, hardware.ReadByte(registers.ProgramCounter));
                registers.IncProgramCounter();
                hardware.Cycle(3);
            },
            OpCode.SCF => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = true;
                hardware.Cycle(1);
            },
            OpCode.JR_C_s8 => (IProcessor hardware, ref Registers registers) =>
            {
                if (registers.Carry)
                {
                    var value = (sbyte)hardware.ReadByte(registers.ProgramCounter);
                    registers.IncProgramCounter();
                    registers.ProgramCounter = (ushort)(registers.ProgramCounter + value);
                    hardware.Cycle(3);
                }
                else
                {
                    registers.ProgramCounter++;
                    hardware.Cycle(2);
                }
            },
            OpCode.ADD_HL_SP => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.StackPointer;
                registers.Subtract = false;
                registers.SetAddHalfCarry(registers.HL, value);
                registers.SetAddCarry(registers.HL, value);
                registers.HL += value;
                hardware.Cycle(2);
            },
            OpCode.LD_A_HLD => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = hardware.ReadByte(registers.HL);
                registers.HL--;
                hardware.Cycle(2);
            },
            OpCode.DEC_SP => (IProcessor hardware, ref Registers registers) =>
            {
                registers.StackPointer--;
                hardware.Cycle(2);
            },
            OpCode.INC_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetAddHalfCarry(registers.Accumulator, 1);
                registers.Accumulator++;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                hardware.Cycle(1);
            },
            OpCode.DEC_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.SetSubHalfCarry(registers.Accumulator, 1);
                registers.Accumulator--;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.LD_A_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.Accumulator = value;
                hardware.Cycle(2);
            },
            OpCode.CCF => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = !registers.Carry;
                hardware.Cycle(1);
            },
            OpCode.LD_B_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = registers.B;
                hardware.Cycle(1);
            },
            OpCode.LD_B_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = registers.C;
                hardware.Cycle(1);
            },
            OpCode.LD_B_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = registers.D;
                hardware.Cycle(1);
            },
            OpCode.LD_B_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = registers.E;
                hardware.Cycle(1);
            },
            OpCode.LD_B_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = registers.H;
                hardware.Cycle(1);
            },
            OpCode.LD_B_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = registers.L;
                hardware.Cycle(1);
            },
            OpCode.LD_B_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = hardware.ReadByte(registers.HL);
                hardware.Cycle(2);
            },
            OpCode.LD_B_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = registers.Accumulator;
                hardware.Cycle(1);
            },
            OpCode.LD_C_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = registers.B;
                hardware.Cycle(1);
            },
            OpCode.LD_C_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = registers.C;
                hardware.Cycle(1);
            },
            OpCode.LD_C_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = registers.D;
                hardware.Cycle(1);
            },
            OpCode.LD_C_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = registers.E;
                hardware.Cycle(1);
            },
            OpCode.LD_C_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = registers.H;
                hardware.Cycle(1);
            },
            OpCode.LD_C_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = registers.L;
                hardware.Cycle(1);
            },
            OpCode.LD_C_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = hardware.ReadByte(registers.HL);
                hardware.Cycle(2);
            },
            OpCode.LD_C_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = registers.Accumulator;
                hardware.Cycle(1);
            },
            OpCode.LD_D_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = registers.B;
                hardware.Cycle(1);
            },
            OpCode.LD_D_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = registers.C;
                hardware.Cycle(1);
            },
            OpCode.LD_D_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = registers.D;
                hardware.Cycle(1);
            },
            OpCode.LD_D_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = registers.E;
                hardware.Cycle(1);
            },
            OpCode.LD_D_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = registers.H;
                hardware.Cycle(1);
            },
            OpCode.LD_D_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = registers.L;
                hardware.Cycle(1);
            },
            OpCode.LD_D_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = hardware.ReadByte(registers.HL);
                hardware.Cycle(2);
            },
            OpCode.LD_D_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = registers.Accumulator;
                hardware.Cycle(1);
            },
            OpCode.LD_E_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = registers.B;
                hardware.Cycle(1);
            },
            OpCode.LD_E_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = registers.C;
                hardware.Cycle(1);
            },
            OpCode.LD_E_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = registers.D;
                hardware.Cycle(1);
            },
            OpCode.LD_E_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = registers.E;
                hardware.Cycle(1);
            },
            OpCode.LD_E_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = registers.H;
                hardware.Cycle(1);
            },
            OpCode.LD_E_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = registers.L;
                hardware.Cycle(1);
            },
            OpCode.LD_E_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = hardware.ReadByte(registers.HL);
                hardware.Cycle(2);
            },
            OpCode.LD_E_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = registers.Accumulator;
                hardware.Cycle(1);
            },
            OpCode.LD_H_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = registers.B;
                hardware.Cycle(1);
            },
            OpCode.LD_H_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = registers.C;
                hardware.Cycle(1);
            },
            OpCode.LD_H_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = registers.D;
                hardware.Cycle(1);
            },
            OpCode.LD_H_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = registers.E;
                hardware.Cycle(1);
            },
            OpCode.LD_H_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = registers.H;
                hardware.Cycle(1);
            },
            OpCode.LD_H_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = registers.L;
                hardware.Cycle(1);
            },
            OpCode.LD_H_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = hardware.ReadByte(registers.HL);
                hardware.Cycle(2);
            },
            OpCode.LD_H_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = registers.Accumulator;
                hardware.Cycle(1);
            },
            OpCode.LD_L_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = registers.B;
                hardware.Cycle(1);
            },
            OpCode.LD_L_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = registers.C;
                hardware.Cycle(1);
            },
            OpCode.LD_L_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = registers.D;
                hardware.Cycle(1);
            },
            OpCode.LD_L_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = registers.E;
                hardware.Cycle(1);
            },
            OpCode.LD_L_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = registers.H;
                hardware.Cycle(1);
            },
            OpCode.LD_L_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = registers.L;
                hardware.Cycle(1);
            },
            OpCode.LD_L_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = hardware.ReadByte(registers.HL);
                hardware.Cycle(2);
            },
            OpCode.LD_L_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = registers.Accumulator;
                hardware.Cycle(1);
            },
            OpCode.LD_HLm_B => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.B);
                hardware.Cycle(2);
            },
            OpCode.LD_HLm_C => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.C);
                hardware.Cycle(2);
            },
            OpCode.LD_HLm_D => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.D);
                hardware.Cycle(2);
            },
            OpCode.LD_HLm_E => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.E);
                hardware.Cycle(2);
            },
            OpCode.LD_HLm_H => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.H);
                hardware.Cycle(2);
            },
            OpCode.LD_HLm_L => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.L);
                hardware.Cycle(2);
            },
            OpCode.HALT => (IProcessor hardware, ref Registers registers) =>
            {
                if (!hardware.InterruptsEnabled && (hardware.ReadByte((ushort)Addresses.IF) & 0x1F) != 0)
                {
                    hardware.Cycle(1);
                }
                else
                {
                    registers.IncProgramCounter();
                    hardware.Halt();
                    hardware.Cycle(1);
                }
            },
            OpCode.LD_HLm_A => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, registers.Accumulator);
                hardware.Cycle(2);
            },
            OpCode.LD_A_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = registers.B;
                hardware.Cycle(1);
            },
            OpCode.LD_A_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = registers.C;
                hardware.Cycle(1);
            },
            OpCode.LD_A_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = registers.D;
                hardware.Cycle(1);
            },
            OpCode.LD_A_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = registers.E;
                hardware.Cycle(1);
            },
            OpCode.LD_A_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = registers.H;
                hardware.Cycle(1);
            },
            OpCode.LD_A_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = registers.L;
                hardware.Cycle(1);
            },
            OpCode.LD_A_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = hardware.ReadByte(registers.HL);
                hardware.Cycle(2);
            },
            OpCode.LD_A_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = registers.Accumulator;
                hardware.Cycle(1);
            },
            OpCode.ADD_A_B => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.B;
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADD_A_C => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.C;
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADD_A_D => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.D;
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADD_A_E => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.E;
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADD_A_H => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.H;
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADD_A_L => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.L;
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADD_A_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            OpCode.ADD_A_A => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.Accumulator;
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADC_A_B => (IProcessor hardware, ref Registers registers) =>
            {
                var value = (byte)(registers.B);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADC_A_C => (IProcessor hardware, ref Registers registers) =>
            {
                var value = (byte)(registers.C);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADC_A_D => (IProcessor hardware, ref Registers registers) =>
            {
                var value = (byte)(registers.D);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADC_A_E => (IProcessor hardware, ref Registers registers) =>
            {
                var value = (byte)(registers.E);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADC_A_H => (IProcessor hardware, ref Registers registers) =>
            {
                var value = (byte)(registers.H);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADC_A_L => (IProcessor hardware, ref Registers registers) =>
            {
                var value = (byte)(registers.L);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.ADC_A_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            OpCode.ADC_A_A => (IProcessor hardware, ref Registers registers) =>
            {
                var value = (byte)(registers.Accumulator);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.SUB_B => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.B;
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.SUB_C => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.C;
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.SUB_D => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.D;
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.SUB_E => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.E;
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.SUB_H => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.H;
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.SUB_L => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.L;
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.SUB_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(2);
            },
            OpCode.SUB_A => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.Accumulator;
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = true;
                hardware.Cycle(1);
            },
            OpCode.SBC_A_B => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = registers.B;
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.SBC_A_C => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = registers.C;
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.SBC_A_D => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = registers.D;
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.SBC_A_E => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = registers.E;
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.SBC_A_H => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = registers.H;
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.SBC_A_L => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = registers.L;
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.SBC_A_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = hardware.ReadByte(registers.HL);
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.SBC_A_A => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = registers.Accumulator;
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.AND_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator &= registers.B;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = true;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.AND_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator &= registers.C;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = true;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.AND_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator &= registers.D;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = true;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.AND_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator &= registers.E;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = true;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.AND_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator &= registers.H;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = true;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.AND_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator &= registers.L;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = true;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.AND_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator &= hardware.ReadByte(registers.HL);
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = true;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            OpCode.AND_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator &= registers.Accumulator;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = true;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.XOR_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator ^= registers.B;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.XOR_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator ^= registers.C;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.XOR_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator ^= registers.D;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.XOR_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator ^= registers.E;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.XOR_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator ^= registers.H;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.XOR_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator ^= registers.L;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.XOR_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator ^= hardware.ReadByte(registers.HL);
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            OpCode.XOR_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator ^= registers.Accumulator;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.OR_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator |= registers.B;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.OR_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator |= registers.C;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.OR_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator |= registers.D;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.OR_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator |= registers.E;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.OR_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator |= registers.H;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.OR_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator |= registers.L;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.OR_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator |= hardware.ReadByte(registers.HL);
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            OpCode.OR_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator |= registers.Accumulator;
                registers.Zero = registers.Accumulator == 0;
                registers.Subtract = false;
                registers.HalfCarry = false;
                registers.Carry = false;
                hardware.Cycle(1);
            },
            OpCode.CP_B => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.B;
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(1);
            },
            OpCode.CP_C => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.C;
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(1);
            },
            OpCode.CP_D => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.D;
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(1);
            },
            OpCode.CP_E => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.E;
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(1);
            },
            OpCode.CP_H => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.H;
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(1);
            },
            OpCode.CP_L => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.L;
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(1);
            },
            OpCode.CP_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(2);
            },
            OpCode.CP_A => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.Accumulator;
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(1);
            },
            OpCode.RET_NZ => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Zero)
                {
                    var address = hardware.PopWord();
                    registers.ProgramCounter = address;
                    hardware.Cycle(5);
                }
                else
                {
                    hardware.Cycle(2);
                }
            },
            OpCode.POP_BC => (IProcessor hardware, ref Registers registers) =>
            {
                registers.BC = hardware.PopWord();
                hardware.Cycle(3);
            },
            OpCode.JP_NZ_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Zero)
                {
                    var address = hardware.ReadWord(registers.ProgramCounter);
                    registers.ProgramCounter = address;
                    hardware.Cycle(4);
                }
                else
                {
                    registers.ProgramCounter += 2;
                    hardware.Cycle(3);
                }

            },
            OpCode.JP_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                var address = hardware.ReadWord(registers.ProgramCounter);
                registers.ProgramCounter = address;
                hardware.Cycle(4);
            },
            OpCode.CALL_NZ_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Zero)
                {
                    var address = hardware.ReadWord(registers.ProgramCounter);
                    hardware.PushWord((ushort)(registers.ProgramCounter + 2));
                    registers.ProgramCounter = address;
                    hardware.Cycle(6);
                }
                else
                {
                    registers.IncProgramCounter(2);
                    hardware.Cycle(3);
                }
            },
            OpCode.PUSH_BC => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.BC);
                hardware.Cycle(4);
            },
            OpCode.ADD_A_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.Subtract = false;
                registers.SetAddCarry(registers.Accumulator, value);
                registers.SetAddHalfCarry(registers.Accumulator, value);
                registers.Accumulator += value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(1);
            },
            OpCode.RST_0 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.ProgramCounter);
                registers.ProgramCounter = 0x0000;
                hardware.Cycle(4);
            },
            OpCode.RET_Z => (IProcessor hardware, ref Registers registers) =>
            {
                if (registers.Zero)
                {
                    var address = hardware.PopWord();
                    registers.ProgramCounter = address;
                    hardware.Cycle(5);
                }
                else
                {
                    hardware.Cycle(2);
                }
            },
            OpCode.RET => (IProcessor hardware, ref Registers registers) =>
            {
                var address = hardware.PopWord();
                registers.ProgramCounter = address;
                hardware.Cycle(4);
            },
            OpCode.JP_Z_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                if (registers.Zero)
                {
                    var address = hardware.ReadWord(registers.ProgramCounter);
                    registers.ProgramCounter = address;
                    hardware.Cycle(4);
                }
                else
                {
                    registers.ProgramCounter += 2;
                    hardware.Cycle(3);
                }
            },
            OpCode.PREFIX_CB => (IProcessor hardware, ref Registers registers) => throw new PanicException(),
            OpCode.CALL_Z_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                if (registers.Zero)
                {
                    var address = hardware.ReadWord(registers.ProgramCounter);
                    hardware.PushWord((ushort)(registers.ProgramCounter + 2));
                    registers.ProgramCounter = address;
                    hardware.Cycle(6);
                }
                else
                {
                    registers.ProgramCounter += 2;
                    hardware.Cycle(3);
                }
            },
            OpCode.CALL_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                var address = hardware.ReadWord(registers.ProgramCounter);
                hardware.PushWord((ushort)(registers.ProgramCounter + 2));
                registers.ProgramCounter = address;
                hardware.Cycle(6);
            },
            OpCode.ADC_A_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                var carry = (byte)(registers.Carry ? 1 : 0);
                registers.Subtract = false;
                registers.Carry = registers.Accumulator + value + carry > 0xff;
                registers.SetAddHalfCarry(registers.Accumulator, value, carry);
                registers.Accumulator += (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                registers.IncProgramCounter();
                hardware.Cycle(2);
            },
            OpCode.RST_1 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.ProgramCounter);
                registers.ProgramCounter = 0x0008;
                hardware.Cycle(4);
            },
            OpCode.RET_NC => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Carry)
                {
                    var address = hardware.PopWord();
                    registers.ProgramCounter = address;
                    hardware.Cycle(5);
                }
                else
                {
                    hardware.Cycle(2);
                }
            },
            OpCode.POP_DE => (IProcessor hardware, ref Registers registers) =>
            {
                registers.DE = hardware.PopWord();
                hardware.Cycle(3);
            },
            OpCode.JP_NC_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Carry)
                {
                    var address = hardware.ReadWord(registers.ProgramCounter);
                    registers.ProgramCounter = address;
                    hardware.Cycle(4);
                }
                else
                {
                    registers.ProgramCounter += 2;
                    hardware.Cycle(3);
                }
            },
            OpCode.CALL_NC_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                if (!registers.Carry)
                {
                    var address = hardware.ReadWord(registers.ProgramCounter);
                    hardware.PushWord((ushort)(registers.ProgramCounter + 2));
                    registers.ProgramCounter = address;
                    hardware.Cycle(6);
                }
                else
                {
                    registers.ProgramCounter += 2;
                    hardware.Cycle(3);
                }
            },
            OpCode.PUSH_DE => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.DE);
                hardware.Cycle(4);
            },
            OpCode.SUB_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value);
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.Accumulator -= value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            OpCode.RST_2 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.ProgramCounter);
                registers.ProgramCounter = 0x0010;
                hardware.Cycle(4);
            },
            OpCode.RET_C => (IProcessor hardware, ref Registers registers) =>
            {
                if (registers.Carry)
                {
                    var address = hardware.PopWord();
                    registers.ProgramCounter = address;
                    hardware.Cycle(5);
                }
                else
                {
                    hardware.Cycle(2);
                }
            },
            OpCode.RETI => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.InterruptsEnabled = true;
                var address = hardware.PopWord();
                registers.ProgramCounter = address;
                hardware.Cycle(4);
            },
            OpCode.JP_C_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                if (registers.Carry)
                {
                    var address = hardware.ReadWord(registers.ProgramCounter);
                    registers.ProgramCounter = address;
                    hardware.Cycle(4);
                }
                else
                {
                    registers.ProgramCounter += 2;
                    hardware.Cycle(3);
                }
            },
            OpCode.CALL_C_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                if (registers.Carry)
                {
                    var address = hardware.ReadWord(registers.ProgramCounter);
                    hardware.PushWord((ushort)(registers.ProgramCounter + 2));
                    registers.ProgramCounter = address;
                    hardware.Cycle(6);
                }
                else
                {
                    registers.ProgramCounter += 2;
                    hardware.Cycle(3);
                }
            },
            OpCode.SBC_A_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = (byte)(registers.Carry ? 1 : 0);
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.Subtract = true;
                registers.SetSubCarry(registers.Accumulator, value, carry);
                registers.SetSubHalfCarry(registers.Accumulator, (byte)(value), carry);
                registers.Accumulator -= (byte)(value + carry);
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            OpCode.RST_3 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.ProgramCounter);
                registers.ProgramCounter = 0x0018;
                hardware.Cycle(4);
            },
            OpCode.LDH_a8_A => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.ProgramCounter++;
                hardware.WriteByte((ushort)(0xFF00 + value), registers.Accumulator);
                hardware.Cycle(3);
            },
            OpCode.POP_HL => (IProcessor hardware, ref Registers registers) =>
            {
                registers.HL = hardware.PopWord();
                hardware.Cycle(3);
            },
            OpCode.LD_Cm_A => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte((ushort)(0xFF00 + registers.C), registers.Accumulator);
                hardware.Cycle(2);
            },
            OpCode.PUSH_HL => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.HL);
                hardware.Cycle(4);
            },
            OpCode.AND_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.Subtract = false;
                registers.Carry = false;
                registers.HalfCarry = true;
                registers.Accumulator &= value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            OpCode.RST_4 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.ProgramCounter);
                registers.ProgramCounter = 0x0020;
                hardware.Cycle(4);
            },
            OpCode.ADD_SP_s8 => (IProcessor hardware, ref Registers registers) =>
            {
                var sp = registers.StackPointer;
                var value = (sbyte)hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.StackPointer = (ushort)(registers.StackPointer + value);
                registers.Subtract = false;
                registers.Carry = ((sp ^ value ^ (registers.StackPointer & 0xFFFF)) & 0x100) == 0x100;
                registers.HalfCarry = ((sp ^ value ^ (registers.StackPointer & 0xFFFF)) & 0x10) == 0x10;
                registers.Zero = false;
                hardware.Cycle(4);
            },
            OpCode.JP_HL => (IProcessor hardware, ref Registers registers) =>
            {
                registers.ProgramCounter = registers.HL;
                hardware.Cycle(1);
            },
            OpCode.LD_a16_A => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(hardware.ReadWord(registers.ProgramCounter), registers.Accumulator);
                registers.ProgramCounter += 2;
                hardware.Cycle(4);
            },
            OpCode.XOR_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.Subtract = false;
                registers.Carry = false;
                registers.HalfCarry = false;
                registers.Accumulator ^= value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            OpCode.RST_5 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.ProgramCounter);
                registers.ProgramCounter = 0x0028;
                hardware.Cycle(4);
            },
            OpCode.LDH_A_a8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.ProgramCounter++;
                registers.Accumulator = hardware.ReadByte((ushort)(0xFF00 + value));
                hardware.Cycle(3);
            },
            OpCode.POP_AF => (IProcessor hardware, ref Registers registers) =>
            {
                registers.AF = hardware.PopWord();
                hardware.Cycle(3);
            },
            OpCode.LD_A_Cm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = hardware.ReadByte((ushort)(0xFF00 + registers.C));
                hardware.Cycle(2);
            },
            OpCode.DI => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.InterruptsEnabled = false;
                hardware.Cycle(1);
            },
            OpCode.PUSH_AF => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.AF);
                hardware.Cycle(4);
            },
            OpCode.OR_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.IncProgramCounter();
                registers.Subtract = false;
                registers.Carry = false;
                registers.HalfCarry = false;
                registers.Accumulator |= value;
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            OpCode.RST_6 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.ProgramCounter);
                registers.ProgramCounter = 0x0030;
                hardware.Cycle(4);
            },
            OpCode.LD_HL_SPs8 => (IProcessor hardware, ref Registers registers) =>
            {
                var sp = registers.StackPointer;
                var value = (sbyte)hardware.ReadByte(registers.ProgramCounter);
                registers.Subtract = false;
                registers.Zero = false;
                registers.Carry = ((sp ^ value ^ ((sp + value) & 0xFFFF)) & 0x100) == 0x100;
                registers.HalfCarry = ((sp ^ value ^ ((sp + value) & 0xFFFF)) & 0x10) == 0x10;
                registers.ProgramCounter++;
                registers.HL = (ushort)(registers.StackPointer + value);
                hardware.Cycle(3);
            },
            OpCode.LD_SP_HL => (IProcessor hardware, ref Registers registers) =>
            {
                registers.StackPointer = registers.HL;
                hardware.Cycle(2);
            },
            OpCode.LD_A_a16 => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = hardware.ReadByte(hardware.ReadWord(registers.ProgramCounter));
                registers.ProgramCounter += 2;
                hardware.Cycle(4);
            },
            OpCode.EI => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.InterruptsEnabled = true;
                hardware.Cycle(1);
            },
            OpCode.CP_d8 => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.ProgramCounter);
                registers.ProgramCounter++;
                registers.Zero = registers.Accumulator == value;
                registers.Subtract = true;
                registers.SetSubHalfCarry(registers.Accumulator, value);
                registers.SetSubCarry(registers.Accumulator, value);
                hardware.Cycle(2);
            },
            OpCode.RST_7 => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.PushWord(registers.ProgramCounter);
                registers.ProgramCounter = 0x0038;
                hardware.Cycle(4);
            },
            _ => throw new PanicException() // Unused opcodes
        };
    }
    
    private void BuildExtendedInstructions()
    {
        foreach (var opcode in Enum.GetValues<ExtendedOpCode>())
        {
            this.extendedInstructions[(byte)opcode] = GetExtendedInstruction(opcode);
        }
    }

    private Instruction GetExtendedInstruction(ExtendedOpCode opCode)
    {
        return opCode switch
        {
            ExtendedOpCode.RLC_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.Carry = (registers.B & 0x80) != 0;
                registers.HalfCarry = false;
                registers.B = (byte)((registers.B << 1) | (registers.Carry ? 1 : 0));
                registers.Zero = registers.B == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RLC_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.Carry = (registers.C & 0x80) != 0;
                registers.HalfCarry = false;
                registers.C = (byte)((registers.C << 1) | (registers.Carry ? 1 : 0));
                registers.Zero = registers.C == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RLC_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.Carry = (registers.D & 0x80) != 0;
                registers.HalfCarry = false;
                registers.D = (byte)((registers.D << 1) | (registers.Carry ? 1 : 0));
                registers.Zero = registers.D == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RLC_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.Carry = (registers.E & 0x80) != 0;
                registers.HalfCarry = false;
                registers.E = (byte)((registers.E << 1) | (registers.Carry ? 1 : 0));
                registers.Zero = registers.E == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RLC_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.Carry = (registers.H & 0x80) != 0;
                registers.HalfCarry = false;
                registers.H = (byte)((registers.H << 1) | (registers.Carry ? 1 : 0));
                registers.Zero = registers.H == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RLC_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.Carry = (registers.L & 0x80) != 0;
                registers.HalfCarry = false;
                registers.L = (byte)((registers.L << 1) | (registers.Carry ? 1 : 0));
                registers.Zero = registers.L == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RLC_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.Subtract = false;
                registers.Carry = (value & 0x80) != 0;
                registers.HalfCarry = false;
                value = (byte)((value << 1) | (registers.Carry ? 1 : 0));
                registers.Zero = value == 0;
                hardware.WriteByte(registers.HL, value);
                hardware.Cycle(4);
            },
            ExtendedOpCode.RLC_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Subtract = false;
                registers.Carry = (registers.Accumulator & 0x80) != 0;
                registers.HalfCarry = false;
                registers.Accumulator = (byte)((registers.Accumulator << 1) | (registers.Carry ? 1 : 0));
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RRC_B => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.B;
                registers.Subtract = false;
                registers.Carry = (value & 0x01) != 0;
                registers.HalfCarry = false;
                registers.B = (byte)((value >> 1) | (registers.Carry ? 0x80 : 0));
                registers.Zero = registers.B == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RRC_C => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.C;
                registers.Subtract = false;
                registers.Carry = (value & 0x01) != 0;
                registers.HalfCarry = false;
                registers.C = (byte)((value >> 1) | (registers.Carry ? 0x80 : 0));
                registers.Zero = registers.C == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RRC_D => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.D;
                registers.Subtract = false;
                registers.Carry = (value & 0x01) != 0;
                registers.HalfCarry = false;
                registers.D = (byte)((value >> 1) | (registers.Carry ? 0x80 : 0));
                registers.Zero = registers.D == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RRC_E => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.E;
                registers.Subtract = false;
                registers.Carry = (value & 0x01) != 0;
                registers.HalfCarry = false;
                registers.E = (byte)((value >> 1) | (registers.Carry ? 0x80 : 0));
                registers.Zero = registers.E == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RRC_H => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.H;
                registers.Subtract = false;
                registers.Carry = (value & 0x01) != 0;
                registers.HalfCarry = false;
                registers.H = (byte)((value >> 1) | (registers.Carry ? 0x80 : 0));
                registers.Zero = registers.H == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RRC_L => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.L;
                registers.Subtract = false;
                registers.Carry = (value & 0x01) != 0;
                registers.HalfCarry = false;
                registers.L = (byte)((value >> 1) | (registers.Carry ? 0x80 : 0));
                registers.Zero = registers.L == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RRC_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.Subtract = false;
                registers.Carry = (value & 0x01) != 0;
                registers.HalfCarry = false;
                value = (byte)((value >> 1) | (registers.Carry ? 0x80 : 0));
                registers.Zero = value == 0;
                hardware.WriteByte(registers.HL, value);
                hardware.Cycle(4);
            },
            ExtendedOpCode.RRC_A => (IProcessor hardware, ref Registers registers) =>
            {
                var value = registers.Accumulator;
                registers.Subtract = false;
                registers.Carry = (value & 0x01) != 0;
                registers.HalfCarry = false;
                registers.Accumulator = (byte)((value >> 1) | (registers.Carry ? 0x80 : 0));
                registers.Zero = registers.Accumulator == 0;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RL_B => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.B & 0b10000000) != 0;
                registers.B = (byte)((registers.B << 1) | (carry ? 1 : 0));
                registers.Zero = registers.B == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RL_C => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.C & 0x80) != 0;
                registers.C = (byte)(registers.C << 1 | (carry ? 1 : 0));
                registers.Zero = registers.C == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RL_D => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.D & 0x80) != 0;
                registers.D = (byte)(registers.D << 1 | (carry ? 1 : 0));
                registers.Zero = registers.D == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RL_E => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.E & 0x80) != 0;
                registers.E = (byte)(registers.E << 1 | (carry ? 1 : 0));
                registers.Zero = registers.E == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RL_H => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.H & 0x80) != 0;
                registers.H = (byte)(registers.H << 1 | (carry ? 1 : 0));
                registers.Zero = registers.H == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RL_L => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.L & 0x80) != 0;
                registers.L = (byte)(registers.L << 1 | (carry ? 1 : 0));
                registers.Zero = registers.L == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RL_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                var value = hardware.ReadByte(registers.HL);
                registers.Carry = (value & 0x80) != 0;
                value = (byte)(value << 1 | (carry ? 1 : 0));
                registers.Zero = value == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.WriteByte(registers.HL, value);
                hardware.Cycle(4);
            },
            ExtendedOpCode.RL_A => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.Accumulator & 0x80) != 0;
                registers.Accumulator = (byte)(registers.Accumulator << 1 | (carry ? 1 : 0));
                registers.Zero = registers.Accumulator == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RR_B => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.B & 0x01) != 0;
                registers.B = (byte)(registers.B >> 1 | (carry ? 0x80 : 0));
                registers.Zero = registers.B == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RR_C => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.C & 0x01) != 0;
                registers.C = (byte)(registers.C >> 1 | (carry ? 0x80 : 0));
                registers.Zero = registers.C == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RR_D => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.D & 0x01) != 0;
                registers.D = (byte)(registers.D >> 1 | (carry ? 0x80 : 0));
                registers.Zero = registers.D == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RR_E => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.E & 0x01) != 0;
                registers.E = (byte)(registers.E >> 1 | (carry ? 0x80 : 0));
                registers.Zero = registers.E == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RR_H => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.H & 0x01) != 0;
                registers.H = (byte)(registers.H >> 1 | (carry ? 0x80 : 0));
                registers.Zero = registers.H == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RR_L => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.L & 0x01) != 0;
                registers.L = (byte)(registers.L >> 1 | (carry ? 0x80 : 0));
                registers.Zero = registers.L == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RR_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                var value = hardware.ReadByte(registers.HL);
                registers.Carry = (value & 0x01) != 0;
                value = (byte)(value >> 1 | (carry ? 0x80 : 0));
                registers.Zero = value == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.WriteByte(registers.HL, value);
                hardware.Cycle(4);
            },
            ExtendedOpCode.RR_A => (IProcessor hardware, ref Registers registers) =>
            {
                var carry = registers.Carry;
                registers.Carry = (registers.Accumulator & 0x01) != 0;
                registers.Accumulator = (byte)(registers.Accumulator >> 1 | (carry ? 0x80 : 0));
                registers.Zero = registers.Accumulator == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SLA_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.B & 0x80) != 0;
                registers.B = (byte)((registers.B << 1) & 0b11111110);
                registers.Zero = registers.B == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SLA_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.C & 0x80) != 0;
                registers.C = (byte)((registers.C << 1) & 0b11111110);
                registers.Zero = registers.C == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SLA_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.D & 0x80) != 0;
                registers.D = (byte)((registers.D << 1) & 0b11111110);
                registers.Zero = registers.D == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SLA_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.E & 0x80) != 0;
                registers.E = (byte)((registers.E << 1) & 0b11111110);
                registers.Zero = registers.E == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SLA_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.H & 0x80) != 0;
                registers.H = (byte)((registers.H << 1) & 0b11111110);
                registers.Zero = registers.H == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SLA_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.L & 0x80) != 0;
                registers.L = (byte)((registers.L << 1) & 0b11111110);
                registers.Zero = registers.L == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SLA_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.Carry = (value & 0x80) != 0;
                value = (byte)((value << 1) & 0b11111110);
                registers.Zero = value == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.WriteByte(registers.HL, value);
                hardware.Cycle(4);
            },
            ExtendedOpCode.SLA_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.Accumulator & 0x80) != 0;
                registers.Accumulator = (byte)((registers.Accumulator << 1) & 0b11111110);
                registers.Zero = registers.Accumulator == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRA_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.B & 0x01) != 0;
                registers.B = (byte)((registers.B >> 1) | (registers.B & 0x80));
                registers.Zero = registers.B == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRA_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.C & 0x01) != 0;
                registers.C = (byte)((registers.C >> 1) | (registers.C & 0x80));
                registers.Zero = registers.C == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRA_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.D & 0x01) != 0;
                registers.D = (byte)((registers.D >> 1) | (registers.D & 0x80));
                registers.Zero = registers.D == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRA_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.E & 0x01) != 0;
                registers.E = (byte)((registers.E >> 1) | (registers.E & 0x80));
                registers.Zero = registers.E == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRA_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.H & 0x01) != 0;
                registers.H = (byte)((registers.H >> 1) | (registers.H & 0x80));
                registers.Zero = registers.H == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);  
            },
            ExtendedOpCode.SRA_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.L & 0x01) != 0;
                registers.L = (byte)((registers.L >> 1) | (registers.L & 0x80));
                registers.Zero = registers.L == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRA_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.Carry = (value & 0x01) != 0;
                value = (byte)(((value >> 1)) | (value & 0x80));
                hardware.WriteByte(registers.HL, value);
                registers.Zero = value == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.SRA_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.Accumulator & 0x01) != 0;
                registers.Accumulator = (byte)((registers.Accumulator >> 1) | (registers.Accumulator & 0x80));
                registers.Zero = registers.Accumulator == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SWAP_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)((registers.B << 4) | (registers.B >> 4));
                registers.Zero = registers.B == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SWAP_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)((registers.C << 4) | (registers.C >> 4));
                registers.Zero = registers.C == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SWAP_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)((registers.D << 4) | (registers.D >> 4));
                registers.Zero = registers.D == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SWAP_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)((registers.E << 4) | (registers.E >> 4));
                registers.Zero = registers.E == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SWAP_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)((registers.H << 4) | (registers.H >> 4));
                registers.Zero = registers.H == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SWAP_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)((registers.L << 4) | (registers.L >> 4));
                registers.Zero = registers.L == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SWAP_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                value = (byte)((value << 4) | (value >> 4));
                registers.Zero = value == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                registers.Carry = false;
                hardware.WriteByte(registers.HL, value);
                hardware.Cycle(4);
            },
            ExtendedOpCode.SWAP_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)((registers.Accumulator << 4) | (registers.Accumulator >> 4));
                registers.Zero = registers.Accumulator == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                registers.Carry = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRL_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.B & 0x01) != 0;
                registers.B = (byte)((registers.B >> 1) & 0b01111111);
                registers.Zero = registers.B == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRL_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.C & 0x01) != 0;
                registers.C = (byte)((registers.C >> 1) & 0b01111111);
                registers.Zero = registers.C == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRL_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.D & 0x01) != 0;
                registers.D = (byte)((registers.D >> 1) & 0b01111111);
                registers.Zero = registers.D == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRL_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.E & 0x01) != 0;
                registers.E = (byte)((registers.E >> 1) & 0b01111111);
                registers.Zero = registers.E == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRL_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.H & 0x01) != 0;
                registers.H = (byte)((registers.H >> 1) & 0b01111111);
                registers.Zero = registers.H == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRL_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.L & 0x01) != 0;
                registers.L = (byte)((registers.L >> 1) & 0b01111111);
                registers.Zero = registers.L == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.SRL_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                var value = hardware.ReadByte(registers.HL);
                registers.Carry = (value & 0x01) != 0;
                value = (byte)((value >> 1) & 0b01111111);
                registers.Zero = value == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.WriteByte(registers.HL, value);
                hardware.Cycle(4);
            },
            ExtendedOpCode.SRL_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Carry = (registers.Accumulator & 0x01) != 0;
                registers.Accumulator = (byte)((registers.Accumulator >> 1) & 0b01111111);
                registers.Zero = registers.Accumulator == 0;
                registers.HalfCarry = false;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_0_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.B >> 0) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_0_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.C >> 0) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_0_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.D >> 0) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_0_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.E >> 0) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_0_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.H >> 0) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_0_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.L >> 0) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_0_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((hardware.ReadByte(registers.HL) >> 0) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.BIT_0_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.Accumulator >> 0) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_1_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.B >> 1) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_1_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.C >> 1) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_1_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.D >> 1) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_1_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.E >> 1) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_1_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.H >> 1) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_1_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.L >> 1) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_1_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((hardware.ReadByte(registers.HL) >> 1) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.BIT_1_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.Accumulator >> 1) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_2_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.B >> 2) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_2_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.C >> 2) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_2_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.D >> 2) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_2_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.E >> 2) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_2_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.H >> 2) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_2_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.L >> 2) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_2_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((hardware.ReadByte(registers.HL) >> 2) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.BIT_2_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.Accumulator >> 2) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_3_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.B >> 3) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_3_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.C >> 3) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_3_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.D >> 3) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_3_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.E >> 3) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_3_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.H >> 3) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_3_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.L >> 3) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_3_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((hardware.ReadByte(registers.HL) >> 3) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.BIT_3_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.Accumulator >> 3) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_4_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.B >> 4) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_4_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.C >> 4) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_4_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.D >> 4) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_4_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.E >> 4) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_4_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.H >> 4) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_4_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.L >> 4) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_4_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((hardware.ReadByte(registers.HL) >> 4) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.BIT_4_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.Accumulator >> 4) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_5_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.B >> 5) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_5_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.C >> 5) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_5_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.D >> 5) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_5_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.E >> 5) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_5_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.H >> 5) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_5_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.L >> 5) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_5_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((hardware.ReadByte(registers.HL) >> 5) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.BIT_5_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.Accumulator >> 5) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_6_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.B >> 6) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_6_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.C >> 6) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_6_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.D >> 6) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_6_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.E >> 6) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_6_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.H >> 6) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_6_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.L >> 6) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_6_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((hardware.ReadByte(registers.HL) >> 6) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.BIT_6_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.Accumulator >> 6) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_7_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.B >> 7) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_7_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.C >> 7) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_7_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.D >> 7) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_7_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.E >> 7) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_7_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.H >> 7) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_7_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.L >> 7) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.BIT_7_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((hardware.ReadByte(registers.HL) >> 7) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(4);
            },
            ExtendedOpCode.BIT_7_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Zero = ((registers.Accumulator >> 7) & 0x01) == 0;
                registers.HalfCarry = true;
                registers.Subtract = false;
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_0_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B & ~(1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_0_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C & ~(1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_0_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D & ~(1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_0_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E & ~(1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_0_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H & ~(1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_0_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L & ~(1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_0_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) & ~(1 << 0)));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_0_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator & ~(1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_1_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B & ~(1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_1_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C & ~(1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_1_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D & ~(1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_1_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E & ~(1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_1_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H & ~(1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_1_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L & ~(1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_1_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) & ~(1 << 1)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.RES_1_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator & ~(1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_2_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B & ~(1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_2_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C & ~(1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_2_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D & ~(1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_2_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E & ~(1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_2_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H & ~(1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_2_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L & ~(1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_2_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) & ~(1 << 2)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.RES_2_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator & ~(1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_3_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B & ~(1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_3_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C & ~(1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_3_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D & ~(1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_3_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E & ~(1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_3_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H & ~(1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_3_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L & ~(1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_3_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) & ~(1 << 3)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.RES_3_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator & ~(1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_4_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B & ~(1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_4_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C & ~(1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_4_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D & ~(1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_4_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E & ~(1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_4_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H & ~(1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_4_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L & ~(1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_4_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) & ~(1 << 4)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.RES_4_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator & ~(1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_5_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B & ~(1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_5_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C & ~(1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_5_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D & ~(1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_5_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E & ~(1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_5_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H & ~(1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_5_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L & ~(1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_5_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) & ~(1 << 5)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.RES_5_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator & ~(1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_6_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B & ~(1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_6_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C & ~(1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_6_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D & ~(1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_6_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E & ~(1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_6_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H & ~(1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_6_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L & ~(1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_6_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) & ~(1 << 6)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.RES_6_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator & ~(1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_7_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B & ~(1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_7_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C & ~(1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_7_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D & ~(1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_7_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E & ~(1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_7_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H & ~(1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_7_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L & ~(1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.RES_7_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) & ~(1 << 7)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.RES_7_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator & ~(1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_0_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B | (1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_0_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C | (1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_0_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D | (1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_0_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E | (1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_0_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H | (1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_0_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L | (1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_0_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) | (1 << 0)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.SET_0_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator | (1 << 0));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_1_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B | (1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_1_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C | (1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_1_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D | (1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_1_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E | (1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_1_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H | (1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_1_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L | (1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_1_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) | (1 << 1)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.SET_1_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator | (1 << 1));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_2_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B | (1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_2_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C | (1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_2_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D | (1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_2_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E | (1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_2_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H | (1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_2_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L | (1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_2_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) | (1 << 2)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.SET_2_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator | (1 << 2));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_3_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B | (1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_3_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C | (1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_3_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D | (1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_3_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E | (1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_3_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H | (1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_3_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L | (1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_3_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) | (1 << 3)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.SET_3_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator | (1 << 3));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_4_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B | (1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_4_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C | (1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_4_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D | (1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_4_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E | (1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_4_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H | (1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_4_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L | (1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_4_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) | (1 << 4)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.SET_4_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator | (1 << 4));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_5_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B | (1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_5_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C | (1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_5_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D | (1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_5_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E | (1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_5_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H | (1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_5_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L | (1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_5_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) | (1 << 5)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.SET_5_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator | (1 << 5));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_6_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B | (1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_6_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C | (1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_6_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D | (1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_6_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E | (1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_6_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H | (1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_6_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L | (1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_6_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) | (1 << 6)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.SET_6_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator | (1 << 6));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_7_B => (IProcessor hardware, ref Registers registers) =>
            {
                registers.B = (byte)(registers.B | (1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_7_C => (IProcessor hardware, ref Registers registers) =>
            {
                registers.C = (byte)(registers.C | (1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_7_D => (IProcessor hardware, ref Registers registers) =>
            {
                registers.D = (byte)(registers.D | (1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_7_E => (IProcessor hardware, ref Registers registers) =>
            {
                registers.E = (byte)(registers.E | (1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_7_H => (IProcessor hardware, ref Registers registers) =>
            {
                registers.H = (byte)(registers.H | (1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_7_L => (IProcessor hardware, ref Registers registers) =>
            {
                registers.L = (byte)(registers.L | (1 << 7));
                hardware.Cycle(2);
            },
            ExtendedOpCode.SET_7_HLm => (IProcessor hardware, ref Registers registers) =>
            {
                hardware.WriteByte(registers.HL, (byte)(hardware.ReadByte(registers.HL) | (1 << 7)));
                hardware.Cycle(4);
            },
            ExtendedOpCode.SET_7_A => (IProcessor hardware, ref Registers registers) =>
            {
                registers.Accumulator = (byte)(registers.Accumulator | (1 << 7));
                hardware.Cycle(2);
            }
        };
    }
}
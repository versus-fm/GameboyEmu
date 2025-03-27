using System.Diagnostics;
using GameboyEmulator.Architecture.Interface;

namespace GameboyEmulator.Architecture;

public class Processor : IProcessor
{
    private const int CyclesPerSecond = 4194304 / 2;
    private Registers registers;
    private uint cycleCount;
    private InstructionSet instructionSet;

    private IMemory memory;

    /* IME */
    private bool interruptsEnabled;
    public ref Registers Registers => ref registers;

    public bool InterruptsEnabled
    {
        get => interruptsEnabled;
        set => interruptsEnabled = value;
    }

    public bool GlobalInterruptOverride { get; set; } = false;

    private bool halted = false;

    private uint currentSpeed = CyclesPerSecond;
    private bool doubleSpeed = false;

    private uint dividerCycles = 0;
    private uint timerCycles = 0;

    private uint frameCount = 0;


    public uint Frequency => currentSpeed;
    public uint CycleCount => cycleCount;
    public CpuState State { get; set; }

    private static readonly InterruptFlags[] InterruptPriorities = {
        InterruptFlags.VBlank,
        InterruptFlags.LCDStat,
        InterruptFlags.Timer,
        InterruptFlags.Serial,
        InterruptFlags.Joypad
    };

    public Processor(IMemory memory, CpuState state = CpuState.Run)
    {
        this.memory = memory;
        this.cycleCount = 0;
        this.registers = new Registers();
        this.instructionSet = new InstructionSet();
        this.InterruptsEnabled = true;
        this.State = state;
    }

    public SpeedFlags ReadSpeed()
    {
        return (SpeedFlags)memory.ReadByte(0xFF4D);
    }
    
    public void Halt()
    {
        this.halted = true;
    }


    public void WriteSpeed(SpeedFlags speed)
    {
        if (speed.HasFlag(SpeedFlags.CurrentSpeed))
        {
            currentSpeed = CyclesPerSecond * 2;
            doubleSpeed = true;
        }
        else
        {
            currentSpeed = CyclesPerSecond;
            doubleSpeed = false;
        }

        memory.WriteByte(0xFF04, 0x00);
        memory.WriteByte(0xFF4D, (byte)speed);
    }

    public uint ExecuteNextInstruction()
    {
        if (State == CpuState.Halt)
            return 0;
        
        uint delta = 1;
        if (!halted)
        {
            var start = cycleCount;
            var opcode = ReadByte(registers.ProgramCounter);
            
            registers.IncProgramCounter();
            instructionSet.ExecuteInstruction(this, opcode);


            delta = cycleCount - start;
            if (delta > 0)
            {
                dividerCycles += delta;
                timerCycles += delta;
            }

            if (dividerCycles >= currentSpeed / 256)
            {
                dividerCycles -= currentSpeed / 256;
                memory.IncrementDivider();
            }

            if (timerCycles >= currentSpeed / memory.TimerSpeed)
            {
                timerCycles -= currentSpeed / memory.TimerSpeed;
                if (memory.TimerEnabled)
                {
                    memory.IncrementTimer();
                }
            }

            if (State == CpuState.Step)
                State = CpuState.Halt;
        }
        else
        {
            cycleCount++;
        }
        if (interruptsEnabled && !GlobalInterruptOverride)
            HandleInterrupt();
        return delta;
    }

    public void Cycle(uint cycles = 1)
    {
        cycleCount += cycles;
    }

    public byte ReadByte(ushort address)
    {
        return memory[address];
    }

    public ushort ReadWord(ushort address)
    {
        return (ushort)(memory[address] | (ushort)(memory[(ushort)(address + 1)] << 8));
    }

    public void WriteByte(ushort address, byte value)
    {
        memory[address] = value;
        if (address == 0xFF46)
        {
            var source = (ushort)(value << 8);
            var destination = 0xFE00;
            for (var i = 0; i < 160; i++)
            {
                memory.WriteByte((ushort)(destination + i), memory.ReadByte((ushort)(source + i)));
            }
        }
    }

    public void WriteWord(ushort address, ushort value)
    {
        memory[address] = (byte)(value & 0xFF);
        memory[(ushort)(address + 1)] = (byte)(value >> 8);
    }

    public Span<byte> ReadBytes(ushort address, int length)
    {
        return memory.ReadBytes(address, length);
    }

    public void WriteBytes(ushort address, Span<byte> bytes)
    {
        memory.WriteBytes(address, bytes);
    }

    public void PushWord(ushort value)
    {
        registers.StackPointer -= 2;
        memory[registers.StackPointer] = (byte)(value & 0xFF);
        memory[(ushort)(registers.StackPointer + 1)] = (byte)(value >> 8);
    }

    public ushort PopWord()
    {
        var value = (ushort)(memory[registers.StackPointer] |
                             (ushort)(memory[(ushort)(registers.StackPointer + 1)] << 8));
        registers.StackPointer += 2;
        return value;
    }

    public void PushByte(byte value)
    {
        memory[registers.StackPointer] = value;
        registers.StackPointer++;
    }

    public byte PopByte()
    {
        registers.StackPointer--;
        return memory[registers.StackPointer];
    }

    public void HandleInterrupt()
    {
        for (int i = 0; i < InterruptPriorities.Length; i++)
        {
            var interrupt = InterruptPriorities[i];
            if (memory.CheckHardwareRegister(Addresses.IE, (byte)interrupt) &&
                memory.CheckHardwareRegister(Addresses.IF, (byte)interrupt))
            {
                halted = false;
                memory.ClearInterrupt(interrupt);
                InterruptsEnabled = false;
                PushWord(registers.ProgramCounter);
                registers.ProgramCounter = (ushort)interrupt.GetInterruptVector();
                cycleCount += 5;
                break;
            }
        }
    }
}
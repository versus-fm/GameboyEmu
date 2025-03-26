using System.Diagnostics;
using GameboyEmulator.Architecture.Interface;

namespace GameboyEmulator.Architecture;

public class Hardware
{
    private Memory memory;
    private IProcessor cpu;
    private IPictureProcessor ppu;
    private Stopwatch timer;

    private uint extraCycles = 0;


    public Hardware(Memory memory, IProcessor cpu, IPictureProcessor ppu)
    {
        this.memory = memory;
        this.cpu = cpu;
        this.ppu = ppu;
        this.timer = new Stopwatch();
    }

    public void Initialize()
    {
        cpu.Registers.ProgramCounter = 0x100;
        cpu.Registers.StackPointer = 0xFFFE;
        memory.WriteByte(0xFF00, 0xCF);
        memory.WriteByte(0xFF05, 0x0);
        memory.WriteByte(0xFF06, 0x0);
        memory.WriteByte(0xFF07, 0x0);
        memory.WriteByte(0xFF10, 0x80);
        memory.WriteByte(0xFF11, 0xBF);
        memory.WriteByte(0xFF12, 0xF3);
        memory.WriteByte(0xFF14, 0xBF);
        memory.WriteByte(0xFF16, 0x3F);
        memory.WriteByte(0xFF17, 0x00);
        memory.WriteByte(0xFF19, 0xBF);
        memory.WriteByte(0xFF1A, 0x7F);
        memory.WriteByte(0xFF1B, 0xFF);
        memory.WriteByte(0xFF1C, 0x9F);
        memory.WriteByte(0xFF1E, 0xBF);
        memory.WriteByte(0xFF20, 0xFF);
        memory.WriteByte(0xFF21, 0x00);
        memory.WriteByte(0xFF22, 0x00);
        memory.WriteByte(0xFF23, 0xBF);
        memory.WriteByte(0xFF24, 0x77);
        memory.WriteByte(0xFF25, 0xF3);
        memory.WriteByte(0xFF26, 0xF1);
        memory.WriteByte(0xFF40, 0x91);
        memory.WriteByte(0xFF42, 0x00);
        memory.WriteByte(0xFF43, 0x00);
        memory.WriteByte(0xFF44, 0x00);
        memory.WriteByte(0xFF45, 0x00);
        memory.WriteByte(0xFF47, 0xFC);
        memory.WriteByte(0xFF48, 0xFF);
        memory.WriteByte(0xFF49, 0xFF);
        memory.WriteByte(0xFF4A, 0x00);
        memory.WriteByte(0xFF4B, 0x00);
        memory.WriteByte(0xFF0F, 0x00);
        memory.WriteByte(0xFFFF, 0x00);
    }

    /**
     * Runs a single frame of the gameboy, if sleep is true it will busy wait until the frame is supposed to end. Useful
     * if emulator isn't run under an existing fixed update system.
     */
    public void ProcessFrame(bool sleep = true)
    {
        timer.Restart();
        var ticksPerCycle = TimeSpan.TicksPerSecond / cpu.Frequency;
        var cycleCount = extraCycles;
        var targetCycles = cpu.Frequency / 60;
        while (cycleCount < targetCycles && cpu.State != CpuState.Halt)
        {
            var cycles = cpu.ExecuteNextInstruction();

            if (cycles == 0)
            {
                break;
            }
            
            ppu.RunCycles(cycles * 4); //T-cycles, converted
            cycleCount += cycles;
        }
        
        if (targetCycles < cycleCount)
        {
            extraCycles = cycleCount - targetCycles;
        }
        var elapsed = timer.ElapsedTicks;
        var targetTicks = ticksPerCycle * cycleCount * 4;
        
        while (elapsed < targetTicks && sleep)
        {
            elapsed = timer.ElapsedTicks;
        }

    }
}
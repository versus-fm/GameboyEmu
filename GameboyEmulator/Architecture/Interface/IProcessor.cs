namespace GameboyEmulator.Architecture.Interface;

public interface IProcessor
{
    ref Registers Registers { get; }
    bool InterruptsEnabled { get; set; }
    bool GlobalInterruptOverride { get; set; }
    void Cycle(uint cycles = 1);
    byte ReadByte(ushort address);
    ushort ReadWord(ushort address);
    void WriteByte(ushort address, byte value);
    void WriteWord(ushort address, ushort value);
    void Halt();
    uint Frequency { get; }
    uint CycleCount { get; }
    CpuState State { get; set; }

    Span<byte> ReadBytes(ushort address, int length);
    void WriteBytes(ushort address, Span<byte> bytes);

    SpeedFlags ReadSpeed();
    void WriteSpeed(SpeedFlags speed);
    
    uint ExecuteNextInstruction();

    #region Stack
    void PushWord(ushort value);
    ushort PopWord();
    void PushByte(byte value);
    byte PopByte();
    #endregion
    
}
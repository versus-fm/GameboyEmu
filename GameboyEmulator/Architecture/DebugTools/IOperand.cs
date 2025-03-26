namespace GameboyEmulator.Architecture.DebugTools;

public interface IOperand
{
    uint Size { get; }
    void Read(ref Registers registers, Memory memory, ushort pc);
    
    string ToString(ByteFormatting formatting);
}
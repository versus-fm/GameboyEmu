namespace GameboyEmulator.Architecture.DebugTools.Operands;

public struct MemoryOperand : IOperand
{
    private IOperand inner;
    public uint Size => inner.Size;
    
    public MemoryOperand(IOperand inner)
    {
        this.inner = inner;
    }
    
    public void Read(ref Registers registers, Memory memory, ushort pc)
    {
        inner.Read(ref registers, memory, pc);
    }

    public string ToString(ByteFormatting formatting)
    {
        return $"({inner.ToString(formatting)})";
    }
}
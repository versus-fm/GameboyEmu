namespace GameboyEmulator.Architecture.DebugTools.Operands;

public struct None : IOperand
{
    public uint Size => 0;
    public void Read(ref Registers registers, Memory memory, ushort pc)
    {
        return;
    }

    public string ToString(ByteFormatting formatting)
    {
        return string.Empty;
    }
}
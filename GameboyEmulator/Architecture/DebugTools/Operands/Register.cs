namespace GameboyEmulator.Architecture.DebugTools.Operands;

public struct Register : IOperand
{
    private string name;
    public uint Size => 0;

    public Register(string name)
    {
        this.name = name;
    }

    public void Read(ref Registers registers, Memory memory, ushort pc)
    {
        return;
    }

    public string ToString(ByteFormatting formatting)
    {
        return name;
    }
}
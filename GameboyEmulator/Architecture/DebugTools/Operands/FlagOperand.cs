namespace GameboyEmulator.Architecture.DebugTools.Operands;

public struct FlagOperand : IOperand
{
    private Flags flag;
    private bool active;

    public FlagOperand(Flags flag, bool active)
    {
        this.flag = flag;
        this.active = active;
    }

    public uint Size => 0;
    public void Read(ref Registers registers, Memory memory, ushort pc)
    {
        return; 
    }

    public string ToString(ByteFormatting formatting)
    {
        return active ? flag.ToString() : $"!{flag}";
    }
}
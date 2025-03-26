namespace GameboyEmulator.Architecture.DebugTools.Operands;

public struct RegisterOffset : IOperand
{
    private byte data;
    private string name;
    public uint Size => 1;
    
    public RegisterOffset(string name)
    {
        this.name = name;
    }
    public void Read(ref Registers registers, Memory memory, ushort pc)
    {
        data = memory.ReadByte(pc);
    }

    public string ToString(ByteFormatting formatting)
    {
        return formatting switch
        {
            ByteFormatting.Hex => $"{name}+0x{data:X2}",
            ByteFormatting.Decimal => $"{name}+{data}",
            ByteFormatting.SignedDecimal => $"{name}+{(sbyte)data}",
            ByteFormatting.Binary => $"{name}+{Convert.ToString(data, 2).PadLeft(8, '0')}",
            ByteFormatting.Ascii => $"{name}+{(char)data}",
            _ => throw new ArgumentOutOfRangeException(nameof(formatting), formatting, null)
        };
    }
}
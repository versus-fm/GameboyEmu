namespace GameboyEmulator.Architecture.DebugTools.Operands;

public struct WordOperand : IOperand
{
    private ushort data;
    public uint Size => 2;
    public void Read(ref Registers registers, Memory memory, ushort pc)
    {
        data = (ushort)(memory.ReadByte(pc) | (ushort)(memory.ReadByte((ushort)(pc + 1)) << 8));
    }

    public string ToString(ByteFormatting formatting)
    {
        return formatting switch
        {
            ByteFormatting.Hex => $"0x{data:X4}",
            ByteFormatting.Decimal => $"{data}",
            ByteFormatting.SignedDecimal => $"{(short)data}",
            ByteFormatting.Binary => Convert.ToString(data, 2).PadLeft(16, '0'),
            ByteFormatting.Ascii => $"{(char)data & 0b1111111100000000}{(char)data & 0b0000000011111111}",
            _ => throw new ArgumentOutOfRangeException(nameof(formatting), formatting, null)
        };
    }
}
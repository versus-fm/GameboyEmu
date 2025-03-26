namespace GameboyEmulator.Architecture.DebugTools.Operands;

public struct ByteOperand : IOperand
{
    private byte data;
    public uint Size => 1;
    public void Read(ref Registers registers, Memory memory, ushort pc)
    {
        data = memory.ReadByte(pc);
    }

    public string ToString(ByteFormatting formatting)
    {
        return formatting switch
        {
            ByteFormatting.Hex => $"0x{data:X2}",
            ByteFormatting.Decimal => $"{data}",
            ByteFormatting.SignedDecimal => $"{(sbyte)data}",
            ByteFormatting.Binary => Convert.ToString(data, 2).PadLeft(8, '0'),
            ByteFormatting.Ascii => $"{(char)data}",
            _ => throw new ArgumentOutOfRangeException(nameof(formatting), formatting, null)
        };
    }
}
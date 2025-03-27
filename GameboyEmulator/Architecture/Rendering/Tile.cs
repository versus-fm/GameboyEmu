namespace GameboyEmulator.Architecture.Rendering;

public unsafe struct Tile
{
    private fixed byte data[16];

    public byte GetColorIndex(uint x, uint y)
    {
        if (x > 7 || y > 7)
            throw new ArgumentException($"Invalid coordinates {x}, {y}");
        var mostSignificantByte = data[y * 2 + 1];
        var leastSignificantByte = data[y * 2];
        var mostSignificantBit = (byte)((mostSignificantByte >> (byte)(7 - x)) & 0b1);
        var leastSignificantBit = (byte)((leastSignificantByte >> (byte)(7 - x)) & 0b1);
        return (byte)((mostSignificantBit << 1) | leastSignificantBit);
    }
}
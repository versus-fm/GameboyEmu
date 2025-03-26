using System.Runtime.InteropServices;

namespace GameboyEmulator.Architecture.Rendering;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct TileAttribute
{
    private byte data;

    public TileAttribute(byte data)
    {
        this.data = data;
    }
    
    public bool VerticalFlip => (data & 0x80) != 0;
    public bool HorizontalFlip => (data & 0x20) != 0;
    public byte Palette => (byte)(data & 0x3);
    public bool Bank => (data & 0x4) != 0;
    public bool Priority => (data & 0b10000000) != 0;
}
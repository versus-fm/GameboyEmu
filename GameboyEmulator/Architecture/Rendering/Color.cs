using System.Runtime.InteropServices;

namespace GameboyEmulator.Architecture.Rendering;

[StructLayout(LayoutKind.Sequential)]
public struct Color
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public Color(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static Color FromRgb(byte r, byte g, byte b)
    {
        return new Color(r, g, b, 255);
    }

    public static Color FromRgba(byte r, byte g, byte b, byte a)
    {
        return new Color(r, g, b, a);
    }

    public static Color IndexedGrayscale(byte color)
    {
        return (color & 0b11) switch
        {
            0 => FromRgb(255, 255, 255),
            1 => FromRgb(165, 165, 165),
            2 => FromRgb(85, 85, 85),
            3 => FromRgb(0, 0, 0),
            _ => throw new ArgumentException("Invalid color")
        };
    }

    public static Color PaletteColor(ushort color)
    {
        var r = (byte)((color & 0b0000000000011111));
        var g = (byte)((color & 0b0000011111100000) >> 5);
        var b = (byte)((color & 0b1111100000000000) >> 10);
        return FromRgb((byte)(r * 8), (byte)(g * 8), (byte)(b * 8));
    }
}
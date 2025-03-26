using System.Runtime.InteropServices;

namespace GameboyEmulator.Architecture.Rendering;

[StructLayout(LayoutKind.Sequential, Size = 1024)]
public unsafe struct Tilemap
{
    private fixed byte tiles[1024];
}
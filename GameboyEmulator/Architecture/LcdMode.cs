namespace GameboyEmulator.Architecture;

public enum LcdMode : byte
{
    HBlank = 0x00,
    VBlank = 0x01,
    OamSearch = 0x02,
    PixelTransfer = 0x03
}
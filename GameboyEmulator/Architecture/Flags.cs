namespace GameboyEmulator.Architecture;

[Flags]
public enum Flags : byte
{
    Carry = 1 << 4,
    HalfCarry = 1 << 5,
    Subtract = 1 << 6,
    Zero = 1 << 7
}
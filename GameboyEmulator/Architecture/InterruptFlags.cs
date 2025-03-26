namespace GameboyEmulator.Architecture;

[Flags]
public enum InterruptFlags : byte
{
    VBlank = 0b1,
    LCDStat = 0b10,
    Timer = 0b100,
    Serial = 0b1000,
    Joypad = 0b10000
}

public static class InterruptFlagsExtension
{
    public static Addresses GetInterruptVector(this InterruptFlags flags)
    {
        return flags switch
        {
            InterruptFlags.VBlank => Addresses.VBlankVector,
            InterruptFlags.LCDStat => Addresses.LcdVector,
            InterruptFlags.Timer => Addresses.TimerVector,
            InterruptFlags.Serial => Addresses.SerialVector,
            InterruptFlags.Joypad => Addresses.InputVector,
            _ => throw new ArgumentOutOfRangeException(nameof(flags), flags, null)
        };
    }
}
namespace GameboyEmulator.Architecture;

[Flags]
public enum SpeedFlags : byte
{
    CurrentSpeed = 0x80,
    PrepareSpeedSwitch = 0x01
}
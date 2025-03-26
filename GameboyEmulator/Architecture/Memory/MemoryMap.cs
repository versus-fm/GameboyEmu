namespace GameboyEmulator.Architecture;

public enum MemoryMap : ushort
{
    InterruptEnable = 0xFFFF,
    HighRam = 0xFF80,
    IoRegisters = 0xFF00,
    ObjectAttributeMemory = 0xFE00,
    EchoRam = 0xE000,
    InternalRam = 0xC000,
    InternalSwitchableRamBank = 0xD000,
    VideoRam = 0x8000,
    CartridgeRam = 0xA000,
    CartridgeRomBank = 0x4000,
    CartridgeRomBank0 = 0x0000
}

public static class MemoryMapExtensions
{
    public static bool IsIn(this MemoryMap memoryMap, ushort address)
    {
        return address >= (ushort)memoryMap && address < (ushort)(memoryMap + 0x1000);
    }
    public static MemoryMap GetMemoryMap(this ushort address)
    {
        return address switch
        {
            >= 0x0000 and < 0x4000 => MemoryMap.CartridgeRomBank0,
            >= 0x4000 and < 0x8000 => MemoryMap.CartridgeRomBank,
            >= 0x8000 and < 0xA000 => MemoryMap.VideoRam,
            >= 0xA000 and < 0xC000 => MemoryMap.CartridgeRam,
            >= 0xC000 and < 0xD000 => MemoryMap.InternalRam,
            >= 0xD000 and < 0xE000 => MemoryMap.InternalSwitchableRamBank,
            >= 0xE000 and < 0xFE00 => MemoryMap.EchoRam,
            >= 0xFE00 and < 0xFEA0 => MemoryMap.ObjectAttributeMemory,
            >= 0xFF00 and < 0xFF80 => MemoryMap.IoRegisters,
            >= 0xFF80 and < 0xFFFF => MemoryMap.HighRam,
            _ => MemoryMap.InterruptEnable
        };
    }
}
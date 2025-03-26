namespace GameboyEmulator.IO.HeaderFlags;

public enum RamSize : byte
{
    NO_RAM = 0x00,
    UNUSED = 0x01,
    SIZE_8KB = 0x02,
    SIZE_32KB = 0x03,
    SIZE_128KB = 0x04,
    SIZE_64KB = 0x05
}

public static class RamSizeExtensions
{
    public static uint GetRamSize(this RamSize ramSize)
    {
        return ramSize switch
        {
            RamSize.NO_RAM => 0,
            RamSize.UNUSED => 0,
            RamSize.SIZE_8KB => 8192,
            RamSize.SIZE_32KB => 32768,
            RamSize.SIZE_128KB => 131072,
            RamSize.SIZE_64KB => 65536,
            _ => throw new ArgumentOutOfRangeException(nameof(ramSize), ramSize, null)
        };  
    }
    
    public static uint GetNumberOfBanks(this RamSize ramSize)
    {
        return ramSize switch
        {
            RamSize.NO_RAM => 0,
            RamSize.UNUSED => 0,
            RamSize.SIZE_8KB => 1,
            RamSize.SIZE_32KB => 4,
            RamSize.SIZE_128KB => 16,
            RamSize.SIZE_64KB => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(ramSize), ramSize, null)
        };
    }
}
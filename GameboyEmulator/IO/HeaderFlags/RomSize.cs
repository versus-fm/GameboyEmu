namespace GameboyEmulator.IO.HeaderFlags;

public enum RomSize : byte
{
    SIZE_32KB = 0x00,
    SIZE_64KB = 0x01,
    SIZE_128KB = 0x02,
    SIZE_256KB = 0x03,
    SIZE_512KB = 0x04,
    SIZE_1MB = 0x05,
    SIZE_2MB = 0x06,
    SIZE_4MB = 0x07,
    SIZE_8MB = 0x08,
    SIZE_1_1MB = 0x52,
    SIZE_1_2MB = 0x53,
    SIZE_1_5MB = 0x54
}

public static class RomSizeExtensions
{
    public static uint GetRomSize(this RomSize romSize)
    {
        return romSize switch
        {
            RomSize.SIZE_32KB => 32768,
            RomSize.SIZE_64KB => 65536,
            RomSize.SIZE_128KB => 131072,
            RomSize.SIZE_256KB => 262144,
            RomSize.SIZE_512KB => 524288,
            RomSize.SIZE_1MB => 1048576,
            RomSize.SIZE_2MB => 2097152,
            RomSize.SIZE_4MB => 4194304,
            RomSize.SIZE_8MB => 8388608,
            RomSize.SIZE_1_1MB => 1179648,
            RomSize.SIZE_1_2MB => 1310720,
            RomSize.SIZE_1_5MB => 1572864,
            _ => throw new ArgumentOutOfRangeException(nameof(romSize), romSize, null)
        };
    }

    public static uint GetNumberOfBanks(this RomSize romSize)
    {
        return romSize switch
        {
            RomSize.SIZE_32KB => 2,
            RomSize.SIZE_64KB => 4,
            RomSize.SIZE_128KB => 8,
            RomSize.SIZE_256KB => 16,
            RomSize.SIZE_512KB => 32,
            RomSize.SIZE_1MB => 64,
            RomSize.SIZE_2MB => 128,
            RomSize.SIZE_4MB => 256,
            RomSize.SIZE_8MB => 512,
            RomSize.SIZE_1_1MB => 72,
            RomSize.SIZE_1_2MB => 80,
            RomSize.SIZE_1_5MB => 96,
            _ => throw new ArgumentOutOfRangeException(nameof(romSize), romSize, null)
        };
    }
}
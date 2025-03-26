using System.Runtime.InteropServices;
using GameboyEmulator.IO.Cartridges;
using GameboyEmulator.IO.HeaderFlags;
using GameboyEmulator.IO.MBC;

namespace GameboyEmulator.IO;

public class CartridgeReader
{
    public static ICartridge Read(ReadOnlySpan<byte> data)
    {
        var header = GetHeader(data);
        ICartridge cartridge = header.CartridgeType switch
        {
            MbcType.ROM_ONLY => new Cartridges.Cartridge(false),
            MbcType.ROM_RAM => new Cartridges.Cartridge(true),
            MbcType.MBC1 => new MemoryBankedCartridgeMbc1(header.RomSize.GetNumberOfBanks(), header.RamSize.GetNumberOfBanks()),
            _ => throw new NotImplementedException()
        };
        cartridge.CopyFrom(data);
        return cartridge;
    }

    public static ICartridge ReadFile(string filename)
    {
        return Read(File.ReadAllBytes(filename).AsSpan());
    }
    
    public static ReadOnlySpan<byte> GetHeaderData(ReadOnlySpan<byte> data)
    {
        return data.Slice(0x100, 86);
    }
    
    public static CartridgeHeader GetHeader(ReadOnlySpan<byte> data)
    {
        return MemoryMarshal.Read<CartridgeHeader>(GetHeaderData(data));
    }
}
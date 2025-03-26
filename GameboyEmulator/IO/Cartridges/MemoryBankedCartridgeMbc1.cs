using GameboyEmulator.IO.MBC;

namespace GameboyEmulator.IO.Cartridges;

public class MemoryBankedCartridgeMbc1 : ICartridge
{
    private readonly byte[] romBanks;
    private readonly byte[] ramBanks;
    private byte romBankNumber;
    private byte ramBankNumber;
    private bool ramEnabled;

    public MemoryBankedCartridgeMbc1(uint numberOfRomBanks, uint numberOfRamBanks)
    {
        romBanks = new byte[numberOfRomBanks * 16384];
        ramBanks = new byte[numberOfRamBanks * 8192];
    }
    public ReadOnlySpan<byte> GetReadOnlySpan(uint address, uint length)
    {
        throw new NotImplementedException();
    }

    public Span<byte> GetSpan(uint address, uint length)
    {
        throw new NotImplementedException();
    }

    public byte ReadByte(ushort address)
    {
        throw new NotImplementedException();
    }

    public ushort ReadWord(ushort address)
    {
        return (ushort)(ReadByte(address) | (ushort) (ReadByte((ushort) (address + 1)) << 8));
    }

    public void WriteByte(ushort address, byte value)
    {
        if (address is >= 0x0000 and <= 0x1FFF)
        {
            ramEnabled = (value & 0x0A) == 0x0A;
            return;
        }
        else if (address is >= 0x2000 and <= 0x3FFF)
        {
            romBankNumber = (byte) (value & 0x1F);
            return;
        }
        else if (address is >= 0x4000 and <= 0x5FFF)
        {
            ramBankNumber = (byte) (value & 0x03);
            return;
        }
        else
        {
            
        }
    }

    public void WriteWord(ushort address, ushort value)
    {
        WriteByte(address, (byte) (value & 0xFF));
        WriteByte((ushort) (address + 1), (byte) (value >> 8));
    }

    public ReadOnlySpan<byte> GetEntryPoint()
    {
        return romBanks.AsSpan(0x0100, 0x0103 - 0x0100);
    }

    public byte this[ushort address]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public void CopyFrom(ReadOnlySpan<byte> bytes)
    {
        throw new NotImplementedException();
    }

    public MbcType MBCType => MbcType.MBC1;
    public ushort GetMemoryBankCount()
    {
        throw new NotImplementedException();
    }

    public Span<byte> GetMemoryBank()
    {
        throw new NotImplementedException();
    }
}
using GameboyEmulator.IO.MBC;

namespace GameboyEmulator.IO.Cartridges;

public class MemoryBankedCartridgeMbc1 : ICartridge
{
    private readonly byte[] romBanks;
    private readonly byte[] ramBanks;
    private byte romBankNumber;
    private byte ramBankNumber;
    private bool ramEnabled;

    // 0 = BANK2 affects only accesses to 0x4000-0x7FFF
    // 1 = BANK2 affects accesses to 0x0000-0x3FFF, 0x4000-0x7FFF, 0xA000-0xBFFF
    private byte bankMode;

    public MemoryBankedCartridgeMbc1(uint numberOfRomBanks, uint numberOfRamBanks)
    {
        romBanks = new byte[numberOfRomBanks * 16384];
        ramBanks = new byte[numberOfRamBanks * 8192];
        
        Console.WriteLine(numberOfRamBanks);
        Console.WriteLine(numberOfRomBanks);
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
        if (address is >= 0x0000 and <= 0x7FFF)
        {
            return romBanks[GetRomMappedAddress(address)];
        }
        else if (address is >= 0xA000 and <= 0xBFFF && ramEnabled)
        {
            return ramBanks[GetRamMappedAddress(address)];
        }

        throw new NotImplementedException("Invalid address range");
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
        } 
        else if (address is >= 0x2000 and <= 0x3FFF)
        {
            byte mappedValue = value == 0b00 ? (byte)0b01 : value;
            romBankNumber = (byte)(mappedValue & 0x1F);
        }
        else if (address is >= 0x4000 and <= 0x5FFF)
        {
            ramBankNumber = (byte)(value & 0x03);
        }
        else if (address is >= 0x6000 and <= 0x7FFF)
        {
            bankMode = (byte)(value & 0b1);
        }
        else if (address is >= 0xA000 and <= 0xBFFF)
        {
            if (!ramEnabled) return;
            
            ramBanks[GetRamMappedAddress(address)] = value;
        }
    }

    public byte GetEffectiveRomBank(ushort address)
    {
        if (address is >= 0x0000 and <= 0x3FFF)
        {
            return bankMode == 0b1 ? (byte)(ramBankNumber << 5) : (byte)0;
        }
        else
        {
            return (byte)((romBankNumber & 0b11111) | (ramBankNumber << 5));
        }
    }

    public void WriteRamStatus(bool ramStatus)
    {
        WriteByte(0x0000, (byte)(ramStatus ? 0x0A : 0x00));
    }

    public void WriteRomBank(byte romBank)
    {
        WriteByte(0x2000, romBank);
    }

    public void WriteRamBank(byte ramBank)
    {
        WriteByte(0x4000, ramBank);
    }

    public void WriteBankMode(bool bankMode)
    {
        WriteByte(0x6000, bankMode ? (byte)0b1 : (byte)0b0);
    }

    public uint GetRamMappedAddress(ushort address)
    {
        var bank2 = (byte)(bankMode == 0b0 ? 0b00 : ramBankNumber & 0b11);
        var mappedAddress = (uint)((address & 0b1111111111111) | (bank2 << 13));
        return mappedAddress;
    }

    public uint GetRomMappedAddress(ushort address)
    {
        return (uint)((address & 0b11111111111111) | (GetEffectiveRomBank(address) << 14));
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
        get => ReadByte(address);
        set => WriteByte(address, value);
    }

    public void CopyFrom(ReadOnlySpan<byte> bytes)
    {
        bytes.CopyTo(romBanks);
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

    public void Initialize()
    {
        WriteRomBank(0);
        WriteRamBank(0);
        WriteBankMode(false);
        WriteRamStatus(false);
        
    }
}
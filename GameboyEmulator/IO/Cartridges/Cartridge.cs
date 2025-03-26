using GameboyEmulator.IO.MBC;

namespace GameboyEmulator.IO.Cartridges;

public class Cartridge : ICartridge
{
    private readonly byte[] memory;
    private readonly byte[] ram;
    private bool ramEnabled;

    public Cartridge(bool ramEnabled)
    {
        this.ramEnabled = ramEnabled;
        memory = new byte[32768];
        ram = new byte[8192];
    }
    
    public ReadOnlySpan<byte> GetReadOnlySpan(uint address, uint length)
    {
        return memory.AsSpan((int) address, (int) length);
    }

    public Span<byte> GetSpan(uint address, uint length)
    {
        return memory.AsSpan((int) address, (int) length);
    }

    public byte ReadByte(ushort address)
    {
        if (address >= 0xA000)
        {
            return ram[address - 0xA000];
        }
        return memory[address];
    }

    public ushort ReadWord(ushort address)
    {
        if (address >= 0xA000)
        {
            return (ushort) (ram[address - 0xA000] | (ram[address - 0xA000 + 1] << 8));
        }
        return (ushort) (memory[address] | (memory[address + 1] << 8));
    }

    public void WriteByte(ushort address, byte value)
    {
        if (address >= 0xA000)
        {
            ram[address - 0xA000] = value;
            return;
        }
        memory[address] = value;
    }

    public void WriteWord(ushort address, ushort value)
    {
        if (address >= 0xA000)
        {
            ram[address - 0xA000] = (byte) (value & 0xFF);
            ram[address - 0xA000 + 1] = (byte) (value >> 8);
            return;
        }
        memory[address] = (byte) (value & 0xFF);
        memory[address + 1] = (byte) (value >> 8);
    }

    public ReadOnlySpan<byte> GetEntryPoint()
    {
        return memory.AsSpan(0x0100, 0x0103 - 0x0100);
    }

    public byte this[ushort address]
    {
        get => ReadByte(address);
        set => WriteByte(address, value);
    }

    public void CopyFrom(ReadOnlySpan<byte> bytes)
    {
        bytes.CopyTo(memory.AsSpan());
    }

    public MbcType MBCType => MbcType.ROM_ONLY | MbcType.ROM_RAM;
    public ushort GetMemoryBankCount()
    {
        throw new NotImplementedException();
    }

    public Span<byte> GetMemoryBank()
    {
        throw new NotImplementedException();
    }
}
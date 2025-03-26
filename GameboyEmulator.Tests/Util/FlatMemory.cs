using GameboyEmulator.Architecture;
using GameboyEmulator.Architecture.Interface;

namespace GameboyEmulator.Tests.Util;

public class FlatMemory : IMemory
{
    private byte[] memory;
    public FlatMemory()
    {
        memory = new byte[ushort.MaxValue + 1];
    }
    public void WriteByte(ushort address, byte value)
    {
        this[address] = value;
    }

    public byte this[ushort address]
    {
        get => memory[address];
        set => memory[address] = value;
    }

    public byte ReadByte(ushort address)
    {
        return this[address];
    }

    public Span<byte> ReadBytes(ushort address, int length)
    {
        return memory.AsSpan(address, length);
    }

    public void WriteBytes(ushort address, Span<byte> bytes)
    {
        bytes.CopyTo(memory.AsSpan(address, bytes.Length));
    }

    public ushort GetMemoryBankCount()
    {
        throw new NotImplementedException();
    }

    public Span<byte> GetMemoryBank(ushort index)
    {
        throw new NotImplementedException();
    }
}
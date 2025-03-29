using System.Runtime.InteropServices;
using GameboyEmulator.IO.MBC;

namespace GameboyEmulator.IO;

public interface ICartridge
{
    ReadOnlySpan<byte> GetReadOnlySpan(uint address, uint length);
    Span<byte> GetSpan(uint address, uint length);
    byte ReadByte(ushort address);
    ushort ReadWord(ushort address);
    void WriteByte(ushort address, byte value);
    void WriteWord(ushort address, ushort value);
    ReadOnlySpan<byte> GetEntryPoint();
    byte this[ushort address] { get; set; }
    
    void CopyFrom(ReadOnlySpan<byte> bytes);

    MbcType MBCType { get; }
    ushort GetMemoryBankCount();
    Span<byte> GetMemoryBank();

    void Initialize();
}
namespace GameboyEmulator.Architecture.Interface;

public interface IMemory
{
    void WriteByte(ushort address, byte value);
    byte this[ushort address] { get; set; }
    byte ReadByte(ushort address);
    Span<byte> ReadBytes(ushort address, int length);
    void WriteBytes(ushort address, Span<byte> bytes);

    bool CheckHardwareRegister(Addresses register, byte n)
    {
        return (ReadByte((ushort)register) & n) != 0;
    }
    
    void WriteHardwareRegister(Addresses register, byte n, bool b)
    {
        if (b)
        {
            this[(ushort)register] |= (byte)(1 << n);
        }
        else
        {
            this[(ushort)register] &= (byte)~(1 << n);
        }
    }
    
    bool ReadHardwareRegister(Addresses register, byte n)
    {
        return (ReadByte((ushort)register) & (1 << n)) != 0;
    }

    void ClearInterrupt(InterruptFlags interrupt)
    {
        this[(ushort)Addresses.IF] &= (byte)~interrupt;
    }
    
    void RequestInterrupt(InterruptFlags interrupt)
    {
        this[(ushort)Addresses.IF] |= (byte)interrupt;
    }

    void IncrementDivider()
    {
        this[(ushort)Addresses.DIV]++;
    }

    bool TimerEnabled => (this[0xFF07] & 0b100) == 1;

    uint TimerSpeed => (this[0xFF07] & 0b11) switch
    {
        0b00 => 1024,
        0b01 => 16,
        0b10 => 64,
        0b11 => 256,
        _ => throw new Exception()
    };

    void IncrementTimer()
    {
        var value = this[0xFF05];
        if (value == 0xFF)
        {
            value = this[0xFF06];
            RequestInterrupt(InterruptFlags.Timer);
        }
        else
        {
            value = (byte)(value + 1);
        }

        this[0xFF05] = value;
    }

    ushort GetMemoryBankCount();
    Span<byte> GetMemoryBank(ushort index);
}
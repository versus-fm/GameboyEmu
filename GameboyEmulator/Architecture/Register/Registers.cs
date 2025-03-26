namespace GameboyEmulator.Architecture;

public struct Registers
{
    private byte accumulator;
    private Flags flags;
    private ushort programCounter;
    private ushort stackPointer;
    
    private ushort bc;
    private ushort de;
    private ushort hl;

    public Registers()
    {
        accumulator = 11;
        flags = 0;
        programCounter = 0;
        stackPointer = 0;
        bc = 0;
        de = 0;
        hl = 0;
    }
    
    public byte Accumulator
    {
        get => accumulator;
        set => accumulator = value;
    }

    public Flags Flags
    {
        get => flags;
        set => flags = value;
    }
    
    public bool Carry
    {
        get => (flags & Flags.Carry) == Flags.Carry;
        set => flags = value ? flags | Flags.Carry : flags & ~Flags.Carry;
    }
    
    public bool HalfCarry
    {
        get => (flags & Flags.HalfCarry) == Flags.HalfCarry;
        set => flags = value ? flags | Flags.HalfCarry : flags & ~Flags.HalfCarry;
    }
    
    public bool Subtract
    {
        get => (flags & Flags.Subtract) == Flags.Subtract;
        set => flags = value ? flags | Flags.Subtract : flags & ~Flags.Subtract;
    }
    
    public bool Zero
    {
        get => (flags & Flags.Zero) == Flags.Zero;
        set 
        {
            if (value)
            {
                flags |= Flags.Zero;
            }
            else
            {
                flags &= ~Flags.Zero;
            }
        }
    }
    
    public ushort ProgramCounter
    {
        get => programCounter;
        set => programCounter = value;
    }
    
    public ushort StackPointer
    {
        get => stackPointer;
        set => stackPointer = value;
    }
    
    public ushort BC
    {
        get => bc;
        set => bc = value;
    }
    
    public ushort DE
    {
        get => de;
        set => de = value;
    }
    
    public ushort HL
    {
        get => hl;
        set => hl = value;
    }

    public ushort AF
    {
        get => (ushort) ((accumulator << 8) | (byte) flags);
        set
        {
            accumulator = (byte) (value >> 8);
            flags = (Flags) (value & 0b11110000);
        }
    }

    public byte B
    {
        get => (byte) (bc >> 8);
        set => bc = (ushort) ((bc & 0x00FF) | (value << 8));
    }
    
    public byte C
    {
        get => (byte) (bc & 0x00FF);
        set => bc = (ushort) ((bc & 0xFF00) | value);
    }
    
    public byte D
    {
        get => (byte) (de >> 8);
        set => de = (ushort) ((de & 0x00FF) | (value << 8));
    }
    
    public byte E
    {
        get => (byte) (de & 0x00FF);
        set => de = (ushort) ((de & 0xFF00) | value);
    }
    
    public byte H
    {
        get => (byte) (hl >> 8);
        set => hl = (ushort) ((hl & 0x00FF) | (value << 8));
    }
    
    public byte L
    {
        get => (byte) (hl & 0x00FF);
        set => hl = (ushort) ((hl & 0xFF00) | value);
    }
    
    public void IncProgramCounter(ushort size = 1)
    {
        programCounter+=size;
    }
    
    public void DecProgramCounter(ushort size = 1)
    {
        programCounter-=size;
    }
    
    public void IncStackPointer()
    {
        stackPointer++;
    }
    
    public void DecStackPointer()
    {
        stackPointer--;
    }

    public void SetAddHalfCarry(byte a, byte b)
    {
        HalfCarry = (((a & 0xf) + (b & 0xf)) & 0x10) == 0x10;
    }
    
    public void SetAddHalfCarry(byte a, byte b, byte c)
    {
        HalfCarry = (((a & 0xf) + (b & 0xf) + (c & 0xf)) & 0x10) == 0x10;
    }
    
    public void SetAddHalfCarry(ushort a, ushort b)
    {
        HalfCarry = (((a & 0b1111_1111_1111) + (b & 0b1111_1111_1111)) & 0b1_0000_0000_0000) == 0b1_0000_0000_0000;
    }
    
    public void SetSubHalfCarry(byte a, byte b)
    {
        HalfCarry = (byte)(a & 0xf) - (byte)(b & 0xf) < 0;
    }
    
    public void SetSubHalfCarry(byte a, byte b, byte c)
    {
        HalfCarry = (((a & 0xf) - (b & 0xf) - (c & 0xf)) & 0x10) == 0x10;
    }
    
    public void SetSubHalfCarry(ushort a, ushort b)
    {
        HalfCarry = (short)(a & 0xfff) - (short)(b & 0xfff) < 0;
    }
    
    public void SetAddCarry(byte a, byte b)
    {
        Carry = (a & 0xff) + (b & 0xff) > 0xff;
    }
    
    public void SetAddCarry(ushort a, ushort b)
    {
        Carry = (a & 0xffff) + (b & 0xffff) > 0xffff;
    }
    
    public void SetSubCarry(byte a, byte b)
    {
        Carry = b > a;
    }

    public void SetSubCarry(byte a, byte b, byte c)
    {
        Carry = b + c > a;
    }
    
    public void SetSubCarry(ushort a, ushort b)
    {
        Carry = b > a;
    }

    public void SetZNHC(bool z, bool n, bool h, bool c)
    {
        Zero = z;
        Subtract = n;
        HalfCarry = h;
        Carry = c;
    }
    
    
}
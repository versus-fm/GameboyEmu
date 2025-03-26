using System.Runtime.InteropServices;
using GameboyEmulator.Architecture.Interface;
using GameboyEmulator.Architecture.Rendering;
using GameboyEmulator.Exceptions;
using GameboyEmulator.IO;
using GameboyEmulator.IO.HeaderFlags;

namespace GameboyEmulator.Architecture;

/**
 * Not actually hardware memory of the Gameboy, but rather the addressable memory of the Gameboy which also includes any memory on the cartridge.
 */
public class Memory : IMemory
{
    private readonly byte[] memory;
    private readonly byte[] backgroundPalette;
    private readonly byte[] objectPalette;
    private readonly ICartridge cartridge;
    
    private CgbFlag cgbFlag => (CgbFlag)memory[0x143];

    private byte wramBank => (byte)(memory[0xFF70] & 0b111);
    private byte vramBank => (byte)(memory[0xFF4F] & 0b1);

    public Memory(ICartridge cartridge)
    {
        this.cartridge = cartridge;
        memory = new byte[0x10000];
        backgroundPalette = new byte[64];
        objectPalette = new byte[64];
    }

    public byte this[ushort address]
    {
        get => ReadByte(address);
        set => WriteByte(address, value);
    }

    public Span<byte> ReadBytes(ushort address, int length)
    {
        return memory.AsSpan(address, length);
    }

    public void WriteBytes(ushort address, Span<byte> bytes)
    {
        bytes.CopyTo(memory.AsSpan(address));
    }

    public byte ReadByte(ushort address)
    {
        return address.GetMemoryMap() switch
        {
            MemoryMap.InterruptEnable => memory[address],
            MemoryMap.HighRam => memory[address],
            MemoryMap.IoRegisters => memory[address],
            MemoryMap.ObjectAttributeMemory => memory[address],
            MemoryMap.EchoRam => ReadByte((ushort)(address - (0xE000 - 0xC000))),
            MemoryMap.InternalRam => memory[address - 0xC000],
            MemoryMap.InternalSwitchableRamBank => memory[GetSwitchableRamBankAddress(address)],
            MemoryMap.VideoRam => memory[GetSwitchableVRamBankAddress(address)],
            MemoryMap.CartridgeRam => cartridge.ReadByte(address),
            MemoryMap.CartridgeRomBank => cartridge.ReadByte(address),
            MemoryMap.CartridgeRomBank0 => cartridge.ReadByte(address),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public byte ReadByte(Addresses addresses)
    {
        return ReadByte((ushort)addresses);
    }

    public void WriteByte(Addresses addresses, byte value)
    {
        WriteByte((ushort)addresses, value);
    }

    private ushort GetSwitchableRamBankAddress(ushort address)
    {
        return (ushort)(address - 0xC000 + 0x1000 + (wramBank * 0x1000));
    }

    private ushort GetSwitchableVRamBankAddress(ushort address)
    {
        // Keep the address within VRAM range (0x8000-0x9FFF)
        var vramOffset = address - 0x8000;
        // Add bank offset (0x2000 for bank 1)
        return (ushort)(0x8000 + vramOffset + (vramBank * 0x2000));
    }

    public void WriteByte(ushort address, byte value)
    {
        switch (address.GetMemoryMap())
        {
            case MemoryMap.IoRegisters:
            case MemoryMap.InterruptEnable:
            case MemoryMap.HighRam:
            case MemoryMap.ObjectAttributeMemory:
                if (address == 0xFF04)
                {
                    memory[address] = 0;
                    break;
                }
                memory[address] = value;
                switch (address)
                {
                    case (ushort)Addresses.BCPS_BGPI or (ushort)Addresses.BCPD_BGPD:
                    {
                        var colorAddress = (ushort)(memory[0xFF68] & 0b111111);
                        if (address is (ushort)Addresses.BCPS_BGPI)
                        {
                            memory[0xFF69] = backgroundPalette[colorAddress];
                        }

                        if (address is (ushort)Addresses.BCPD_BGPD)
                        {
                            var autoIncrement = (memory[0xFF68] & 0b10000000) != 0;
                            backgroundPalette[colorAddress] = memory[0xFF69];
                            if (autoIncrement)
                            {
                                colorAddress++;
                                colorAddress &= 0b111111;
                                memory[0xFF68] = (byte)((memory[0xFF68] & 0b10000000) | colorAddress);
                            }
                        }

                        break;
                    }
                    case (ushort)Addresses.OCPS_OBPI or (ushort)Addresses.OCPD_OBPD:
                    {
                        var colorAddress = (ushort)(memory[0xFF6A] & 0b111111);
                        if (address is (ushort)Addresses.OCPS_OBPI)
                        {
                            memory[0xFF6B] = objectPalette[colorAddress];
                        }

                        if (address is (ushort)Addresses.OCPD_OBPD)
                        {
                            var autoIncrement = (memory[0xFF6A] & 0b10000000) != 0;
                            objectPalette[colorAddress] = memory[0xFF6B];
                            if (autoIncrement)
                            {
                                colorAddress++;
                                colorAddress &= 0b111111;
                                memory[0xFF6A] = (byte)((memory[0xFF6A] & 0b10000000) | colorAddress);
                            }
                        }

                        break;
                    }
                }
                break;
            case MemoryMap.EchoRam:
                // DO NOTHING
                break;
            case MemoryMap.InternalRam:
                memory[address - 0xC000] = value;
                break;
            case MemoryMap.InternalSwitchableRamBank:
                memory[GetSwitchableRamBankAddress(address)] = value;
                break;
            case MemoryMap.VideoRam:
                memory[GetSwitchableVRamBankAddress(address)] = value;
                break;
            case MemoryMap.CartridgeRam:
                cartridge.WriteByte((ushort)(address), value);
                break;
            case MemoryMap.CartridgeRomBank:
                cartridge.WriteByte(address, value);
                break;
            case MemoryMap.CartridgeRomBank0:
                cartridge.WriteByte(address, value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void IncrementDivider()
    {
        this[(ushort)Addresses.DIV]++;
    }

    public ushort GetMemoryBankCount()
    {
        throw new NotImplementedException();
    }

    public Span<byte> GetMemoryBank(ushort index)
    {
        throw new NotImplementedException();
    }

    public void RequestInterrupt(InterruptFlags interrupt)
    {
        this[(ushort)Addresses.IF] |= (byte)interrupt;
    }
    
    public void ClearInterrupt(InterruptFlags interrupt)
    {
        this[(ushort)Addresses.IF] &= (byte)~interrupt;
    }

    public bool ReadHardwareRegister(Addresses register, byte n)
    {
        return (ReadByte((ushort)register) & (1 << n)) != 0;
    }
    
    public bool CheckHardwareRegister(Addresses register, byte n)
    {
        return (ReadByte((ushort)register) & n) != 0;
    }

    public void WriteHardwareRegister(Addresses register, byte n, bool b)
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

    public LcdMode ReadLcdMode()
    {
        return (LcdMode)(this[(ushort)Addresses.STAT] & 0b11);
    }

    public void WriteLcdMode(LcdMode mode)
    {
        this[(ushort)Addresses.STAT] = (byte)((this[(ushort)Addresses.STAT] & ~0b11) | ((byte)mode & 0b11));
    }

    public Color GetBackgroundColor(byte index)
    {
        return cgbFlag switch
        {
            CgbFlag.CGB_EXCLUSIVE => Color.PaletteColor(backgroundPalette[2 * (byte)(index & 0b11)]),
            CgbFlag.CGB_COMPATIBLE => Color.IndexedGrayscale((byte)(ReadByte(Addresses.BGP) >> (2 * (byte)(index & 0b11)))),
            _ => Color.IndexedGrayscale((byte)(ReadByte(Addresses.BGP) >> (2 * (byte)(index & 0b11))))
        };
    }
    
    public Color GetObjectColor(byte index, bool usePalette1 = false)
    {
        var strippedIndex = (byte)(index & 0b11);
        if (strippedIndex == 0)
        {
            return Color.FromRgba(255, 255, 255, 0);
        }
        else
        {
            return cgbFlag switch
            {
                CgbFlag.CGB_EXCLUSIVE => Color.PaletteColor(objectPalette[2 * strippedIndex]),
                CgbFlag.CGB_COMPATIBLE => Color.IndexedGrayscale(
                    (byte)(ReadByte(usePalette1 ? Addresses.OBP1 : Addresses.OBP0) >> (2 * strippedIndex))),
                _ => Color.IndexedGrayscale(
                    (byte)(ReadByte(usePalette1 ? Addresses.OBP1 : Addresses.OBP0) >> (2 * strippedIndex)))
            };
        }
    }

    /**
     * Note: x and y are actual tile indices, not pixels
     */
    public byte GetBackgroundTileIndex(uint x, uint y)
    {
        var upperTilemap = ReadHardwareRegister(Addresses.LCDC, 3);
        var baseAddress = upperTilemap ? 0x9C00 : 0x9800;
        var offset = y * 32 + x;
        return memory[baseAddress + offset];
    }
    
    /**
     * Note: x and y are actual tile indices, not pixels
     */
    public byte GetWindowTileIndex(uint x, uint y)
    {
        var upperTilemap = ReadHardwareRegister(Addresses.LCDC, 6);
        var baseAddress = upperTilemap ? 0x9C00 : 0x9800;
        var offset = y * 32 + x;
        return memory[baseAddress + offset];
    }
    
    public Tile GetBackgroundTile(byte address)
    {
        var addressMode = ReadHardwareRegister(Addresses.LCDC, 4);
        ushort tileAddress;
        if (addressMode)
        {
            // Mode 1: 0x8000 + address * 16
            tileAddress = (ushort)(0x8000 + address * 16);
        }
        else
        {
            // Mode 0: 0x8800 + (signed)address * 16
            tileAddress = (ushort)(0x8800 + ((sbyte)address * 16));
        }
        return MemoryMarshal.Read<Tile>(memory.AsSpan(GetSwitchableVRamBankAddress(tileAddress), 16));
    }

    public Tile GetWindowTile(byte address)
    {
        var addressMode = ReadHardwareRegister(Addresses.LCDC, 4);
        var tileAddress = GetSwitchableVRamBankAddress(addressMode ? (ushort)(0x8000 + address * 16) : (ushort)(0x8800 + (sbyte)address * 16));
        return MemoryMarshal.Read<Tile>(memory.AsSpan(tileAddress, 16));
    }

    public Tile GetObjectTile(byte address)
    {
        var tileAddress = GetSwitchableVRamBankAddress((ushort)(0x8000 + address * 16));
        return MemoryMarshal.Read<Tile>(memory.AsSpan(tileAddress, 16));
    }
}
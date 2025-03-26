using System.Runtime.InteropServices;
using System.Text;
using GameboyEmulator.IO.HeaderFlags;
using GameboyEmulator.IO.MBC;

namespace GameboyEmulator.IO;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CartridgeHeader
{
    private fixed byte entry[4];
    private fixed byte nintendoLogo[48];
    private fixed byte title[11];
    private fixed byte manufacturerCode[4];
    private CgbFlag cgbFlag;
    private fixed byte newLicenseeCode[2];
    private byte sgbFlag;
    private MbcType cartridgeType;
    private RomSize romSize;
    private RamSize ramSize;
    private DestinationCode destinationCode;
    private byte oldLicenseeCode;
    private byte maskRomVersionNumber;
    private byte headerChecksum;
    private ushort globalChecksum;

    public string GetTitle()
    {
        const int start = 0;
        var end = 0;
        while (end < 16 && title[end] != 0)
        {
            end++;
        }

        return Encoding.ASCII.GetString(Title.Slice(start, end));
    }

    public ReadOnlySpan<byte> Entry
    {
        get
        {
            fixed(byte* entryPtr = entry)
            {
                return new ReadOnlySpan<byte>(entryPtr, 4);
            }
        }
    }

    public ReadOnlySpan<byte> NintendoLogo
    {
        get
        {
            fixed(byte* logoPtr = nintendoLogo)
            {
                return new ReadOnlySpan<byte>(logoPtr, 48);
            }
        }
    }

    public ReadOnlySpan<byte> Title
    {
        get
        {
            fixed(byte* titlePtr = title)
            {
                return new ReadOnlySpan<byte>(titlePtr, 16);
            }
        }
    }

    public ReadOnlySpan<byte> ManufacturerCode
    {
        get
        {
            fixed(byte* manufacturerCodePtr = manufacturerCode)
            {
                return new ReadOnlySpan<byte>(manufacturerCodePtr, 4);
            }
        }
    }

    public CgbFlag CgbFlag => cgbFlag;

    public ReadOnlySpan<byte> NewLicenseeCode
    {
        get
        {
            fixed(byte* newLicenseeCodePtr = newLicenseeCode)
            {
                return new ReadOnlySpan<byte>(newLicenseeCodePtr, 2);
            }
        }
    }

    public byte SgbFlag => sgbFlag;

    public MbcType CartridgeType => cartridgeType;

    public RomSize RomSize => romSize;

    public RamSize RamSize => ramSize;

    public DestinationCode DestinationCode => destinationCode;

    public byte OldLicenseeCode => oldLicenseeCode;

    public byte MaskRomVersionNumber => maskRomVersionNumber;

    public byte HeaderChecksum => headerChecksum;

    public ushort GlobalChecksum => globalChecksum;
}
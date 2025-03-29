using GameboyEmulator.IO.Cartridges;
using Xunit;
using Xunit.Sdk;

namespace GameboyEmulator.Tests;

public class MBC1Tests
{
    private MemoryBankedCartridgeMbc1 mbc1;

    public MBC1Tests()
    {
        mbc1 = new MemoryBankedCartridgeMbc1(1, 1);
    }
    
    

    [Fact]
    public void TestEffectiveBankNumber()
    {
        mbc1.WriteRomBank(0x12);
        mbc1.WriteRamBank(0b01);
        AssertByte(0x32, mbc1.GetEffectiveRomBank(0x4000));
        AssertByte(0, mbc1.GetEffectiveRomBank(0x3000));
        mbc1.WriteBankMode(true);
        AssertByte(0x20, mbc1.GetEffectiveRomBank(0x3000));
    }

    [Fact]
    public void TestRomMappedAddress()
    {
        mbc1.WriteRomBank(0b00100);
        mbc1.WriteRamBank(0b10);
        AssertByte(0x44, mbc1.GetEffectiveRomBank(0x72A7));
        AssertAddress(0x1132A7, mbc1.GetRomMappedAddress(0x72A7));
    }

    [Fact]
    public void TestRamMappedAddress()
    {
        mbc1.WriteRamStatus(true);
        mbc1.WriteRamBank(0b10);
        mbc1.WriteBankMode(true);
        AssertAddress(0x5123, mbc1.GetRamMappedAddress(0xB123));
    }

    private void AssertAddress(ushort expected, ushort actual)
    {
        AssertEquals(expected, actual, (e, a) =>
        {
            return $@"Expected {e}, was {a}
HEX
    E: {e:x8}
    A: {a:x8}
BIN
    E: {Convert.ToString(e, 2).PadLeft(16, '0')}
    A: {Convert.ToString(a, 2).PadLeft(16, '0')}
";
        });
    }
    
    private void AssertAddress(uint expected, uint actual)
    {
        AssertEquals(expected, actual, (e, a) =>
        {
            return $@"Expected {e}, was {a}
HEX
    E: {e:x8}
    A: {a:x8}
BIN
    E: {Convert.ToString(e, 2).PadLeft(32, '0')}
    A: {Convert.ToString(a, 2).PadLeft(32, '0')}
";
        });
    }
    
    private void AssertByte(byte expected, byte actual)
    {
        AssertEquals(expected, actual, (e, a) =>
        {
            return $@"Expected {e}, was {a}
HEX
    E: {e:x8}
    A: {a:x8}
BIN
    E: {Convert.ToString(e, 2).PadLeft(8, '0')}
    A: {Convert.ToString(a, 2).PadLeft(8, '0')}
";
        });
    }
    
    private void AssertEquals<T>(T expected, T actual, Func<T, T, string> messageSupplier)
    {
        try
        {
            Assert.Equal(expected, actual);
        }
        catch (Exception e)
        {
            throw new XunitException(messageSupplier(expected, actual), e);
        }
    }
}
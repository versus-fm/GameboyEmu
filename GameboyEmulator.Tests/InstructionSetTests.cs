using System.Text.Json;
using System.Text.Json.Serialization;
using GameboyEmulator.Architecture;
using GameboyEmulator.Architecture.Interface;
using GameboyEmulator.Tests.Model;
using GameboyEmulator.Tests.Util;
using Xunit;
using Xunit.Sdk;

namespace GameboyEmulator.Tests;

public class InstructionSetTests
{
    private IProcessor _cpu;
    private JsonSerializerOptions options;

    public InstructionSetTests()
    {
        _cpu = new Processor(new FlatMemory());
        options = new JsonSerializerOptions()
        {
            IncludeFields = true
        };
    }

    [Fact]
    public void TestNOP()
    {
        var opcode = (OpCode)0x00;
        var states = ReadState($"TestData/00.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_BC_d16()
    {
        var opcode = (OpCode)0x01;
        var states = ReadState($"TestData/01.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_BC_A()
    {
        var opcode = (OpCode)0x02;
        var states = ReadState($"TestData/02.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_BC()
    {
        var opcode = (OpCode)0x03;
        var states = ReadState($"TestData/03.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_B()
    {
        var opcode = (OpCode)0x04;
        var states = ReadState($"TestData/04.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_B()
    {
        var opcode = (OpCode)0x05;
        var states = ReadState($"TestData/05.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_d8()
    {
        var opcode = (OpCode)0x06;
        var states = ReadState($"TestData/06.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRLCA()
    {
        var opcode = (OpCode)0x07;
        var states = ReadState($"TestData/07.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_a16_SP()
    {
        var opcode = (OpCode)0x08;
        var states = ReadState($"TestData/08.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_HL_BC()
    {
        var opcode = (OpCode)0x09;
        var states = ReadState($"TestData/09.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_BC()
    {
        var opcode = (OpCode)0x0A;
        var states = ReadState($"TestData/0A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_BC()
    {
        var opcode = (OpCode)0x0B;
        var states = ReadState($"TestData/0B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_C()
    {
        var opcode = (OpCode)0x0C;
        var states = ReadState($"TestData/0C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_C()
    {
        var opcode = (OpCode)0x0D;
        var states = ReadState($"TestData/0D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_d8()
    {
        var opcode = (OpCode)0x0E;
        var states = ReadState($"TestData/0E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRRCA()
    {
        var opcode = (OpCode)0x0F;
        var states = ReadState($"TestData/0F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSTOP()
    {
        var opcode = (OpCode)0x10;
        var states = ReadState($"TestData/10.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_DE_d16()
    {
        var opcode = (OpCode)0x11;
        var states = ReadState($"TestData/11.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_DE_A()
    {
        var opcode = (OpCode)0x12;
        var states = ReadState($"TestData/12.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_DE()
    {
        var opcode = (OpCode)0x13;
        var states = ReadState($"TestData/13.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_D()
    {
        var opcode = (OpCode)0x14;
        var states = ReadState($"TestData/14.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_D()
    {
        var opcode = (OpCode)0x15;
        var states = ReadState($"TestData/15.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_d8()
    {
        var opcode = (OpCode)0x16;
        var states = ReadState($"TestData/16.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRLA()
    {
        var opcode = (OpCode)0x17;
        var states = ReadState($"TestData/17.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJR_s8()
    {
        var opcode = (OpCode)0x18;
        var states = ReadState($"TestData/18.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_HL_DE()
    {
        var opcode = (OpCode)0x19;
        var states = ReadState($"TestData/19.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_DE()
    {
        var opcode = (OpCode)0x1A;
        var states = ReadState($"TestData/1A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_DE()
    {
        var opcode = (OpCode)0x1B;
        var states = ReadState($"TestData/1B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_E()
    {
        var opcode = (OpCode)0x1C;
        var states = ReadState($"TestData/1C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_E()
    {
        var opcode = (OpCode)0x1D;
        var states = ReadState($"TestData/1D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_d8()
    {
        var opcode = (OpCode)0x1E;
        var states = ReadState($"TestData/1E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRRA()
    {
        var opcode = (OpCode)0x1F;
        var states = ReadState($"TestData/1F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJR_NZ_s8()
    {
        var opcode = (OpCode)0x20;
        var states = ReadState($"TestData/20.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HL_d16()
    {
        var opcode = (OpCode)0x21;
        var states = ReadState($"TestData/21.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLI_A()
    {
        var opcode = (OpCode)0x22;
        var states = ReadState($"TestData/22.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_HL()
    {
        var opcode = (OpCode)0x23;
        var states = ReadState($"TestData/23.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_H()
    {
        var opcode = (OpCode)0x24;
        var states = ReadState($"TestData/24.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_H()
    {
        var opcode = (OpCode)0x25;
        var states = ReadState($"TestData/25.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_d8()
    {
        var opcode = (OpCode)0x26;
        var states = ReadState($"TestData/26.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDAA()
    {
        var opcode = (OpCode)0x27;
        var states = ReadState($"TestData/27.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJR_Z_s8()
    {
        var opcode = (OpCode)0x28;
        var states = ReadState($"TestData/28.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_HL_HL()
    {
        var opcode = (OpCode)0x29;
        var states = ReadState($"TestData/29.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_HLI()
    {
        var opcode = (OpCode)0x2A;
        var states = ReadState($"TestData/2A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_HL()
    {
        var opcode = (OpCode)0x2B;
        var states = ReadState($"TestData/2B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_L()
    {
        var opcode = (OpCode)0x2C;
        var states = ReadState($"TestData/2C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_L()
    {
        var opcode = (OpCode)0x2D;
        var states = ReadState($"TestData/2D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_d8()
    {
        var opcode = (OpCode)0x2E;
        var states = ReadState($"TestData/2E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCPL()
    {
        var opcode = (OpCode)0x2F;
        var states = ReadState($"TestData/2F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJR_NC_s8()
    {
        var opcode = (OpCode)0x30;
        var states = ReadState($"TestData/30.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_SP_d16()
    {
        var opcode = (OpCode)0x31;
        var states = ReadState($"TestData/31.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLD_A()
    {
        var opcode = (OpCode)0x32;
        var states = ReadState($"TestData/32.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_SP()
    {
        var opcode = (OpCode)0x33;
        var states = ReadState($"TestData/33.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_HLm()
    {
        var opcode = (OpCode)0x34;
        var states = ReadState($"TestData/34.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_HLm()
    {
        var opcode = (OpCode)0x35;
        var states = ReadState($"TestData/35.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLm_d8()
    {
        var opcode = (OpCode)0x36;
        var states = ReadState($"TestData/36.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSCF()
    {
        var opcode = (OpCode)0x37;
        var states = ReadState($"TestData/37.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJR_C_s8()
    {
        var opcode = (OpCode)0x38;
        var states = ReadState($"TestData/38.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_HL_SP()
    {
        var opcode = (OpCode)0x39;
        var states = ReadState($"TestData/39.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_HLD()
    {
        var opcode = (OpCode)0x3A;
        var states = ReadState($"TestData/3A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_SP()
    {
        var opcode = (OpCode)0x3B;
        var states = ReadState($"TestData/3B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestINC_A()
    {
        var opcode = (OpCode)0x3C;
        var states = ReadState($"TestData/3C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDEC_A()
    {
        var opcode = (OpCode)0x3D;
        var states = ReadState($"TestData/3D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_d8()
    {
        var opcode = (OpCode)0x3E;
        var states = ReadState($"TestData/3E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCCF()
    {
        var opcode = (OpCode)0x3F;
        var states = ReadState($"TestData/3F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_B()
    {
        var opcode = (OpCode)0x40;
        var states = ReadState($"TestData/40.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_C()
    {
        var opcode = (OpCode)0x41;
        var states = ReadState($"TestData/41.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_D()
    {
        var opcode = (OpCode)0x42;
        var states = ReadState($"TestData/42.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_E()
    {
        var opcode = (OpCode)0x43;
        var states = ReadState($"TestData/43.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_H()
    {
        var opcode = (OpCode)0x44;
        var states = ReadState($"TestData/44.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_L()
    {
        var opcode = (OpCode)0x45;
        var states = ReadState($"TestData/45.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_HLm()
    {
        var opcode = (OpCode)0x46;
        var states = ReadState($"TestData/46.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_B_A()
    {
        var opcode = (OpCode)0x47;
        var states = ReadState($"TestData/47.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_B()
    {
        var opcode = (OpCode)0x48;
        var states = ReadState($"TestData/48.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_C()
    {
        var opcode = (OpCode)0x49;
        var states = ReadState($"TestData/49.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_D()
    {
        var opcode = (OpCode)0x4A;
        var states = ReadState($"TestData/4A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_E()
    {
        var opcode = (OpCode)0x4B;
        var states = ReadState($"TestData/4B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_H()
    {
        var opcode = (OpCode)0x4C;
        var states = ReadState($"TestData/4C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_L()
    {
        var opcode = (OpCode)0x4D;
        var states = ReadState($"TestData/4D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_HLm()
    {
        var opcode = (OpCode)0x4E;
        var states = ReadState($"TestData/4E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_C_A()
    {
        var opcode = (OpCode)0x4F;
        var states = ReadState($"TestData/4F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_B()
    {
        var opcode = (OpCode)0x50;
        var states = ReadState($"TestData/50.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_C()
    {
        var opcode = (OpCode)0x51;
        var states = ReadState($"TestData/51.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_D()
    {
        var opcode = (OpCode)0x52;
        var states = ReadState($"TestData/52.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_E()
    {
        var opcode = (OpCode)0x53;
        var states = ReadState($"TestData/53.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_H()
    {
        var opcode = (OpCode)0x54;
        var states = ReadState($"TestData/54.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_L()
    {
        var opcode = (OpCode)0x55;
        var states = ReadState($"TestData/55.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_HLm()
    {
        var opcode = (OpCode)0x56;
        var states = ReadState($"TestData/56.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_D_A()
    {
        var opcode = (OpCode)0x57;
        var states = ReadState($"TestData/57.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_B()
    {
        var opcode = (OpCode)0x58;
        var states = ReadState($"TestData/58.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_C()
    {
        var opcode = (OpCode)0x59;
        var states = ReadState($"TestData/59.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_D()
    {
        var opcode = (OpCode)0x5A;
        var states = ReadState($"TestData/5A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_E()
    {
        var opcode = (OpCode)0x5B;
        var states = ReadState($"TestData/5B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_H()
    {
        var opcode = (OpCode)0x5C;
        var states = ReadState($"TestData/5C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_L()
    {
        var opcode = (OpCode)0x5D;
        var states = ReadState($"TestData/5D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_HLm()
    {
        var opcode = (OpCode)0x5E;
        var states = ReadState($"TestData/5E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_E_A()
    {
        var opcode = (OpCode)0x5F;
        var states = ReadState($"TestData/5F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_B()
    {
        var opcode = (OpCode)0x60;
        var states = ReadState($"TestData/60.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_C()
    {
        var opcode = (OpCode)0x61;
        var states = ReadState($"TestData/61.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_D()
    {
        var opcode = (OpCode)0x62;
        var states = ReadState($"TestData/62.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_E()
    {
        var opcode = (OpCode)0x63;
        var states = ReadState($"TestData/63.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_H()
    {
        var opcode = (OpCode)0x64;
        var states = ReadState($"TestData/64.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_L()
    {
        var opcode = (OpCode)0x65;
        var states = ReadState($"TestData/65.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_HLm()
    {
        var opcode = (OpCode)0x66;
        var states = ReadState($"TestData/66.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_H_A()
    {
        var opcode = (OpCode)0x67;
        var states = ReadState($"TestData/67.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_B()
    {
        var opcode = (OpCode)0x68;
        var states = ReadState($"TestData/68.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_C()
    {
        var opcode = (OpCode)0x69;
        var states = ReadState($"TestData/69.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_D()
    {
        var opcode = (OpCode)0x6A;
        var states = ReadState($"TestData/6A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_E()
    {
        var opcode = (OpCode)0x6B;
        var states = ReadState($"TestData/6B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_H()
    {
        var opcode = (OpCode)0x6C;
        var states = ReadState($"TestData/6C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_L()
    {
        var opcode = (OpCode)0x6D;
        var states = ReadState($"TestData/6D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_HLm()
    {
        var opcode = (OpCode)0x6E;
        var states = ReadState($"TestData/6E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_L_A()
    {
        var opcode = (OpCode)0x6F;
        var states = ReadState($"TestData/6F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLm_B()
    {
        var opcode = (OpCode)0x70;
        var states = ReadState($"TestData/70.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLm_C()
    {
        var opcode = (OpCode)0x71;
        var states = ReadState($"TestData/71.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLm_D()
    {
        var opcode = (OpCode)0x72;
        var states = ReadState($"TestData/72.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLm_E()
    {
        var opcode = (OpCode)0x73;
        var states = ReadState($"TestData/73.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLm_H()
    {
        var opcode = (OpCode)0x74;
        var states = ReadState($"TestData/74.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLm_L()
    {
        var opcode = (OpCode)0x75;
        var states = ReadState($"TestData/75.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestHALT()
    {
        var opcode = (OpCode)0x76;
        var states = ReadState($"TestData/76.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HLm_A()
    {
        var opcode = (OpCode)0x77;
        var states = ReadState($"TestData/77.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_B()
    {
        var opcode = (OpCode)0x78;
        var states = ReadState($"TestData/78.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_C()
    {
        var opcode = (OpCode)0x79;
        var states = ReadState($"TestData/79.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_D()
    {
        var opcode = (OpCode)0x7A;
        var states = ReadState($"TestData/7A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_E()
    {
        var opcode = (OpCode)0x7B;
        var states = ReadState($"TestData/7B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_H()
    {
        var opcode = (OpCode)0x7C;
        var states = ReadState($"TestData/7C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_L()
    {
        var opcode = (OpCode)0x7D;
        var states = ReadState($"TestData/7D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_HLm()
    {
        var opcode = (OpCode)0x7E;
        var states = ReadState($"TestData/7E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_A()
    {
        var opcode = (OpCode)0x7F;
        var states = ReadState($"TestData/7F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_B()
    {
        var opcode = (OpCode)0x80;
        var states = ReadState($"TestData/80.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_C()
    {
        var opcode = (OpCode)0x81;
        var states = ReadState($"TestData/81.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_D()
    {
        var opcode = (OpCode)0x82;
        var states = ReadState($"TestData/82.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_E()
    {
        var opcode = (OpCode)0x83;
        var states = ReadState($"TestData/83.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_H()
    {
        var opcode = (OpCode)0x84;
        var states = ReadState($"TestData/84.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_L()
    {
        var opcode = (OpCode)0x85;
        var states = ReadState($"TestData/85.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_HLm()
    {
        var opcode = (OpCode)0x86;
        var states = ReadState($"TestData/86.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_A()
    {
        var opcode = (OpCode)0x87;
        var states = ReadState($"TestData/87.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_B()
    {
        var opcode = (OpCode)0x88;
        var states = ReadState($"TestData/88.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_C()
    {
        var opcode = (OpCode)0x89;
        var states = ReadState($"TestData/89.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_D()
    {
        var opcode = (OpCode)0x8A;
        var states = ReadState($"TestData/8A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_E()
    {
        var opcode = (OpCode)0x8B;
        var states = ReadState($"TestData/8B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_H()
    {
        var opcode = (OpCode)0x8C;
        var states = ReadState($"TestData/8C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_L()
    {
        var opcode = (OpCode)0x8D;
        var states = ReadState($"TestData/8D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_HLm()
    {
        var opcode = (OpCode)0x8E;
        var states = ReadState($"TestData/8E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_A()
    {
        var opcode = (OpCode)0x8F;
        var states = ReadState($"TestData/8F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_B()
    {
        var opcode = (OpCode)0x90;
        var states = ReadState($"TestData/90.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_C()
    {
        var opcode = (OpCode)0x91;
        var states = ReadState($"TestData/91.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_D()
    {
        var opcode = (OpCode)0x92;
        var states = ReadState($"TestData/92.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_E()
    {
        var opcode = (OpCode)0x93;
        var states = ReadState($"TestData/93.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_H()
    {
        var opcode = (OpCode)0x94;
        var states = ReadState($"TestData/94.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_L()
    {
        var opcode = (OpCode)0x95;
        var states = ReadState($"TestData/95.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_HLm()
    {
        var opcode = (OpCode)0x96;
        var states = ReadState($"TestData/96.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_A()
    {
        var opcode = (OpCode)0x97;
        var states = ReadState($"TestData/97.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_B()
    {
        var opcode = (OpCode)0x98;
        var states = ReadState($"TestData/98.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_C()
    {
        var opcode = (OpCode)0x99;
        var states = ReadState($"TestData/99.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_D()
    {
        var opcode = (OpCode)0x9A;
        var states = ReadState($"TestData/9A.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_E()
    {
        var opcode = (OpCode)0x9B;
        var states = ReadState($"TestData/9B.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_H()
    {
        var opcode = (OpCode)0x9C;
        var states = ReadState($"TestData/9C.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_L()
    {
        var opcode = (OpCode)0x9D;
        var states = ReadState($"TestData/9D.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_HLm()
    {
        var opcode = (OpCode)0x9E;
        var states = ReadState($"TestData/9E.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_A()
    {
        var opcode = (OpCode)0x9F;
        var states = ReadState($"TestData/9F.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_B()
    {
        var opcode = (OpCode)0xA0;
        var states = ReadState($"TestData/A0.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_C()
    {
        var opcode = (OpCode)0xA1;
        var states = ReadState($"TestData/A1.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_D()
    {
        var opcode = (OpCode)0xA2;
        var states = ReadState($"TestData/A2.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_E()
    {
        var opcode = (OpCode)0xA3;
        var states = ReadState($"TestData/A3.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_H()
    {
        var opcode = (OpCode)0xA4;
        var states = ReadState($"TestData/A4.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_L()
    {
        var opcode = (OpCode)0xA5;
        var states = ReadState($"TestData/A5.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_HLm()
    {
        var opcode = (OpCode)0xA6;
        var states = ReadState($"TestData/A6.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_A()
    {
        var opcode = (OpCode)0xA7;
        var states = ReadState($"TestData/A7.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_B()
    {
        var opcode = (OpCode)0xA8;
        var states = ReadState($"TestData/A8.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_C()
    {
        var opcode = (OpCode)0xA9;
        var states = ReadState($"TestData/A9.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_D()
    {
        var opcode = (OpCode)0xAA;
        var states = ReadState($"TestData/AA.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_E()
    {
        var opcode = (OpCode)0xAB;
        var states = ReadState($"TestData/AB.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_H()
    {
        var opcode = (OpCode)0xAC;
        var states = ReadState($"TestData/AC.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_L()
    {
        var opcode = (OpCode)0xAD;
        var states = ReadState($"TestData/AD.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_HLm()
    {
        var opcode = (OpCode)0xAE;
        var states = ReadState($"TestData/AE.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_A()
    {
        var opcode = (OpCode)0xAF;
        var states = ReadState($"TestData/AF.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_B()
    {
        var opcode = (OpCode)0xB0;
        var states = ReadState($"TestData/B0.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_C()
    {
        var opcode = (OpCode)0xB1;
        var states = ReadState($"TestData/B1.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_D()
    {
        var opcode = (OpCode)0xB2;
        var states = ReadState($"TestData/B2.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_E()
    {
        var opcode = (OpCode)0xB3;
        var states = ReadState($"TestData/B3.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_H()
    {
        var opcode = (OpCode)0xB4;
        var states = ReadState($"TestData/B4.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_L()
    {
        var opcode = (OpCode)0xB5;
        var states = ReadState($"TestData/B5.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_HLm()
    {
        var opcode = (OpCode)0xB6;
        var states = ReadState($"TestData/B6.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_A()
    {
        var opcode = (OpCode)0xB7;
        var states = ReadState($"TestData/B7.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_B()
    {
        var opcode = (OpCode)0xB8;
        var states = ReadState($"TestData/B8.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_C()
    {
        var opcode = (OpCode)0xB9;
        var states = ReadState($"TestData/B9.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_D()
    {
        var opcode = (OpCode)0xBA;
        var states = ReadState($"TestData/BA.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_E()
    {
        var opcode = (OpCode)0xBB;
        var states = ReadState($"TestData/BB.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_H()
    {
        var opcode = (OpCode)0xBC;
        var states = ReadState($"TestData/BC.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_L()
    {
        var opcode = (OpCode)0xBD;
        var states = ReadState($"TestData/BD.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_HLm()
    {
        var opcode = (OpCode)0xBE;
        var states = ReadState($"TestData/BE.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_A()
    {
        var opcode = (OpCode)0xBF;
        var states = ReadState($"TestData/BF.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRET_NZ()
    {
        var opcode = (OpCode)0xC0;
        var states = ReadState($"TestData/C0.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPOP_BC()
    {
        var opcode = (OpCode)0xC1;
        var states = ReadState($"TestData/C1.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJP_NZ_a16()
    {
        var opcode = (OpCode)0xC2;
        var states = ReadState($"TestData/C2.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJP_a16()
    {
        var opcode = (OpCode)0xC3;
        var states = ReadState($"TestData/C3.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCALL_NZ_a16()
    {
        var opcode = (OpCode)0xC4;
        var states = ReadState($"TestData/C4.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPUSH_BC()
    {
        var opcode = (OpCode)0xC5;
        var states = ReadState($"TestData/C5.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_A_d8()
    {
        var opcode = (OpCode)0xC6;
        var states = ReadState($"TestData/C6.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRST_0()
    {
        var opcode = (OpCode)0xC7;
        var states = ReadState($"TestData/C7.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRET_Z()
    {
        var opcode = (OpCode)0xC8;
        var states = ReadState($"TestData/C8.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRET()
    {
        var opcode = (OpCode)0xC9;
        var states = ReadState($"TestData/C9.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJP_Z_a16()
    {
        var opcode = (OpCode)0xCA;
        var states = ReadState($"TestData/CA.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPREFIX_CB()
    {
        var opcode = (OpCode)0xCB;
        var states = ReadState($"TestData/CB.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCALL_Z_a16()
    {
        var opcode = (OpCode)0xCC;
        var states = ReadState($"TestData/CC.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCALL_a16()
    {
        var opcode = (OpCode)0xCD;
        var states = ReadState($"TestData/CD.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADC_A_d8()
    {
        var opcode = (OpCode)0xCE;
        var states = ReadState($"TestData/CE.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRST_1()
    {
        var opcode = (OpCode)0xCF;
        var states = ReadState($"TestData/CF.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRET_NC()
    {
        var opcode = (OpCode)0xD0;
        var states = ReadState($"TestData/D0.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPOP_DE()
    {
        var opcode = (OpCode)0xD1;
        var states = ReadState($"TestData/D1.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJP_NC_a16()
    {
        var opcode = (OpCode)0xD2;
        var states = ReadState($"TestData/D2.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCALL_NC_a16()
    {
        var opcode = (OpCode)0xD4;
        var states = ReadState($"TestData/D4.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPUSH_DE()
    {
        var opcode = (OpCode)0xD5;
        var states = ReadState($"TestData/D5.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSUB_d8()
    {
        var opcode = (OpCode)0xD6;
        var states = ReadState($"TestData/D6.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRST_2()
    {
        var opcode = (OpCode)0xD7;
        var states = ReadState($"TestData/D7.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRET_C()
    {
        var opcode = (OpCode)0xD8;
        var states = ReadState($"TestData/D8.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRETI()
    {
        var opcode = (OpCode)0xD9;
        var states = ReadState($"TestData/D9.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJP_C_a16()
    {
        var opcode = (OpCode)0xDA;
        var states = ReadState($"TestData/DA.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCALL_C_a16()
    {
        var opcode = (OpCode)0xDC;
        var states = ReadState($"TestData/DC.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestSBC_A_d8()
    {
        var opcode = (OpCode)0xDE;
        var states = ReadState($"TestData/DE.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRST_3()
    {
        var opcode = (OpCode)0xDF;
        var states = ReadState($"TestData/DF.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLDH_a8_A()
    {
        var opcode = (OpCode)0xE0;
        var states = ReadState($"TestData/E0.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPOP_HL()
    {
        var opcode = (OpCode)0xE1;
        var states = ReadState($"TestData/E1.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_Cm_A()
    {
        var opcode = (OpCode)0xE2;
        var states = ReadState($"TestData/E2.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPUSH_HL()
    {
        var opcode = (OpCode)0xE5;
        var states = ReadState($"TestData/E5.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestAND_d8()
    {
        var opcode = (OpCode)0xE6;
        var states = ReadState($"TestData/E6.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRST_4()
    {
        var opcode = (OpCode)0xE7;
        var states = ReadState($"TestData/E7.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestADD_SP_s8()
    {
        var opcode = (OpCode)0xE8;
        var states = ReadState($"TestData/E8.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestJP_HL()
    {
        var opcode = (OpCode)0xE9;
        var states = ReadState($"TestData/E9.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_a16_A()
    {
        var opcode = (OpCode)0xEA;
        var states = ReadState($"TestData/EA.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestXOR_d8()
    {
        var opcode = (OpCode)0xEE;
        var states = ReadState($"TestData/EE.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRST_5()
    {
        var opcode = (OpCode)0xEF;
        var states = ReadState($"TestData/EF.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLDH_A_a8()
    {
        var opcode = (OpCode)0xF0;
        var states = ReadState($"TestData/F0.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPOP_AF()
    {
        var opcode = (OpCode)0xF1;
        var states = ReadState($"TestData/F1.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_Cm()
    {
        var opcode = (OpCode)0xF2;
        var states = ReadState($"TestData/F2.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestDI()
    {
        var opcode = (OpCode)0xF3;
        var states = ReadState($"TestData/F3.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestPUSH_AF()
    {
        var opcode = (OpCode)0xF5;
        var states = ReadState($"TestData/F5.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestOR_d8()
    {
        var opcode = (OpCode)0xF6;
        var states = ReadState($"TestData/F6.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestRST_6()
    {
        var opcode = (OpCode)0xF7;
        var states = ReadState($"TestData/F7.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_HL_SPs8()
    {
        var opcode = (OpCode)0xF8;
        var states = ReadState($"TestData/F8.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_SP_HL()
    {
        var opcode = (OpCode)0xF9;
        var states = ReadState($"TestData/F9.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestLD_A_a16()
    {
        var opcode = (OpCode)0xFA;
        var states = ReadState($"TestData/FA.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestEI()
    {
        var opcode = (OpCode)0xFB;
        var states = ReadState($"TestData/FB.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    [Fact]
    public void TestCP_d8()
    {
        var opcode = (OpCode)0xFE;
        var states = ReadState($"TestData/FE.json");
        foreach (var state in states)
            TestState(_cpu, state, opcode);
    }

    private void TestState(IProcessor cpu, TestState state, OpCode opCode)
    {
        cpu.GlobalInterruptOverride = true;
        SetInitialState(cpu, state, opCode);
        var nextOpcode = cpu.ReadByte(cpu.Registers.ProgramCounter);
        var followingOpcode = nextOpcode == 0xCB ? cpu.ReadByte((ushort)(cpu.Registers.ProgramCounter + 1)) : 0;
        cpu.ExecuteNextInstruction();
        try
        {
            AssertState(cpu, state, opCode);
        }
        catch (Exception e)
        {
            if (nextOpcode == 0xCB)
            {
                throw new XunitException($"Extended instruction {(ExtendedOpCode)followingOpcode}", e);
            }
            else
            {
                throw;
            }
        }
    }

    private void SetInitialState(IProcessor cpu, TestState state, OpCode opCode)
    {
        cpu.Registers.Accumulator = state.initial.A;
        cpu.Registers.B = state.initial.B;
        cpu.Registers.C = state.initial.C;
        cpu.Registers.D = state.initial.D;
        cpu.Registers.E = state.initial.E;
        cpu.Registers.H = state.initial.H;
        cpu.Registers.L = state.initial.L;
        cpu.Registers.Flags = (Flags)state.initial.F;
        cpu.Registers.ProgramCounter = state.initial.PC;
        cpu.Registers.StackPointer = state.initial.SP;

        foreach (var ramTuple in state.initial.GetRamState())
        {
            cpu.WriteByte(ramTuple.address, ramTuple.data);
        }
    }

    private void AssertState(IProcessor cpu, TestState state, OpCode opCode)
    {
        try
        {
            AssertEquals(state.final.A, cpu.Registers.Accumulator, (expected, actual) => $"A differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals(state.final.B, cpu.Registers.B, (expected, actual) => $"B differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals(state.final.C, cpu.Registers.C, (expected, actual) => $"C differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals(state.final.D, cpu.Registers.D, (expected, actual) => $"D differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals(state.final.E, cpu.Registers.E, (expected, actual) => $"E differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals((Flags)state.final.F, cpu.Registers.Flags, (expected, actual) => $"F differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals(state.final.H, cpu.Registers.H, (expected, actual) => $"H differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals(state.final.L, cpu.Registers.L, (expected, actual) => $"L differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals(state.final.PC, cpu.Registers.ProgramCounter, (expected, actual) => $"PC differ; \n\tExpected: {expected}\n\tActual: {actual}");
            AssertEquals(state.final.SP, cpu.Registers.StackPointer, (expected, actual) => $"SP differ; \n\tExpected: {expected}\n\tActual: {actual}");

            foreach (var ramTuple in state.final.GetRamState())
            {
                AssertEquals(ramTuple.data, cpu.ReadByte(ramTuple.address), (expected, actual) => $"RamTuple @{ramTuple.address} differ; \n\tExpected: {expected}\n\tActual: {actual}");
            }
        }
        catch (Exception e)
        {
            throw new XunitException($"Test failed with executing test for {opCode} with name {state.name}", e);
        }
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

    private IEnumerable<TestState> ReadState(string path)
    {
        return JsonSerializer.Deserialize<IEnumerable<TestState>>(File.ReadAllText(path), options)!;
    }
}
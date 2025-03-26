using System.Globalization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GameboyEmulator.Tests.Model;

public class TestState
{
    [JsonProperty("name")]
    [JsonInclude]
    public string name;
    [JsonInclude]
    [JsonProperty("initial")]
    public MemoryState initial;
    [JsonInclude]
    [JsonProperty("final")]
    public MemoryState final;
    [JsonInclude]
    [JsonProperty("cycles")]
    public string[][] cycles;
    
    public CycleTuple[] GetCycles()
    {
        var tuples = new CycleTuple[cycles.Length];
        for (var i = 0; i < cycles.Length; i++)
        {
            var tuple = cycles[i];
            tuples[i] = new CycleTuple()
            {
                address = ushort.Parse(tuple[0][2..], NumberStyles.HexNumber),
                data = byte.Parse(tuple[1][2..], NumberStyles.HexNumber),
                type = tuple[2]
            };
        }

        return tuples;
    }
}

public class RegisterState
{
    [JsonInclude]
    [JsonProperty("a")]
    public string a;
    [JsonInclude]
    [JsonProperty("b")]
    public string b;
    [JsonInclude]
    [JsonProperty("c")]
    public string c;
    [JsonInclude]
    [JsonProperty("d")]
    public string d;
    [JsonInclude]
    [JsonProperty("e")]
    public string e;
    [JsonInclude]
    [JsonProperty("f")]
    public string f;
    [JsonInclude]
    [JsonProperty("h")]
    public string h;
    [JsonInclude]
    [JsonProperty("l")]
    public string l;
    [JsonInclude]
    [JsonProperty("pc")]
    public string pc;
    [JsonInclude]
    [JsonProperty("sp")]
    public string sp;
}

public class MemoryState
{
    [JsonProperty("cpu")] 
    [JsonInclude] 
    public RegisterState cpu;
    [JsonProperty("ram")]
    [JsonInclude]
    public string[][] ram;

    public byte A => byte.Parse(cpu.a[2..], NumberStyles.HexNumber);
    public byte B => byte.Parse(cpu.b[2..], NumberStyles.HexNumber);
    public byte C => byte.Parse(cpu.c[2..], NumberStyles.HexNumber);
    public byte D => byte.Parse(cpu.d[2..], NumberStyles.HexNumber);
    public byte E => byte.Parse(cpu.e[2..], NumberStyles.HexNumber);
    public byte F => byte.Parse(cpu.f[2..], NumberStyles.HexNumber);
    public byte H => byte.Parse(cpu.h[2..], NumberStyles.HexNumber);
    public byte L => byte.Parse(cpu.l[2..], NumberStyles.HexNumber);
    public ushort PC => ushort.Parse(cpu.pc[2..], NumberStyles.HexNumber);
    public ushort SP => ushort.Parse(cpu.sp[2..], NumberStyles.HexNumber);

    public RamTuple[] GetRamState()
    {
        var tuples = new RamTuple[ram.Length];
        for (var i = 0; i < ram.Length; i++)
        {
            var tuple = ram[i];
            tuples[i] = new RamTuple()
            {
                address = ushort.Parse(tuple[0][2..], NumberStyles.HexNumber),
                data = byte.Parse(tuple[1][2..], NumberStyles.HexNumber)
            };
        }

        return tuples;
    }
}

public struct RamTuple
{
    public ushort address;
    public byte data;
}

public struct CycleTuple
{
    public ushort address;
    public byte data;
    public string type;
}
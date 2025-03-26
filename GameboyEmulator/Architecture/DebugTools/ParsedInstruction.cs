namespace GameboyEmulator.Architecture.DebugTools;

public struct ParsedInstruction
{
    private readonly byte opcode;
    private readonly IOperand operand1;
    private readonly IOperand operand2;
    private readonly string mnemonic;
    private ushort address;

    public ParsedInstruction(byte opcode, string mnemonic, IOperand operand1, IOperand operand2)
    {
        this.opcode = opcode;
        this.mnemonic = mnemonic;
        this.operand1 = operand1;
        this.operand2 = operand2;
    }

    public uint Size => operand1.Size + operand2.Size;

    public string ToString(ByteFormatting byteFormatting)
    {
        var op1 = operand1.ToString(byteFormatting);
        var op2 = operand2.ToString(byteFormatting);
        if (op1.Length > 0 && op2.Length > 0)
        {
            return $"{opcode:X2}:\t{mnemonic} {op1}, {op2}";
        }
        else if (op1.Length > 0 && op2.Length <= 0)
        {
            return $"{opcode:X2}:\t{mnemonic} {op1}";
        }
        else
        {
            return $"{opcode:X2}:\t{mnemonic}";
        }
    }

    public void Read(ref Registers registers, Memory memory, ushort pc)
    {
        address = pc;
        operand1.Read(ref registers, memory, (ushort)(pc + 1));
        operand2.Read(ref registers, memory, (ushort)(pc + 1 + operand1.Size));
    }

    public byte OpCode => opcode;

    public ushort Address => address;
}
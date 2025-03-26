using GameboyEmulator.Architecture.DebugTools.Operands;

namespace GameboyEmulator.Architecture.DebugTools;

public class InstructionParser
{
    private delegate IOperand OperandFactory();

    private readonly OperandFactory None = () => new None();
    private readonly OperandFactory ByteOperand = () => new ByteOperand();
    private readonly OperandFactory WordOperand = () => new WordOperand();
    private readonly OperandFactory RegisterA = () => new Register("A");
    private readonly OperandFactory RegisterB = () => new Register("B");
    private readonly OperandFactory RegisterC = () => new Register("C");
    private readonly OperandFactory RegisterD = () => new Register("D");
    private readonly OperandFactory RegisterE = () => new Register("E");
    private readonly OperandFactory RegisterH = () => new Register("H");
    private readonly OperandFactory RegisterL = () => new Register("L");
    private readonly OperandFactory RegisterAF = () => new Register("AF");
    private readonly OperandFactory RegisterBC = () => new Register("BC");
    private readonly OperandFactory RegisterDE = () => new Register("DE");
    private readonly OperandFactory RegisterHL = () => new Register("HL");
    private readonly OperandFactory RegisterSP = () => new Register("SP");
    private readonly OperandFactory RegisterPC = () => new Register("PC");
    private readonly Memory memory;

    public InstructionParser(Memory memory)
    {
        this.memory = memory;
    }
    
    public List<ParsedInstruction> ParseInstructions(ref Registers registers, ushort start, uint count)
    {
        var instructions = new List<ParsedInstruction>();
        var pc = start;
        while (instructions.Count < count)
        {
            var instruction = ParseInstruction(ref pc);
            instruction.Read(ref registers, memory, pc);
            instructions.Add(instruction);
            pc += (ushort)(instruction.Size + 1);
        }

        return instructions;
    }

    private ParsedInstruction ParseInstruction(ref ushort pc)
    {
        var opcode = (OpCode)memory.ReadByte(pc);
        if (opcode == OpCode.PREFIX_CB)
        {
            pc++;
            return ParseCBInstruction(ref pc);
        }
        else
        {
            return opcode switch
            {
                _ => TryParse(opcode)
            };
        }
    }

    private ParsedInstruction TryParse(OpCode opCode)
    {
        var name = Enum.GetName(opCode);
        if (name == null)
        {
            return new ParsedInstruction((byte)opCode, "??", None(), None());
        }

        var parts = name.Split("_");
        var mnemonic = parts[0];
        var op1 = parts.Length > 1 ? parts[1] : "";
        var op2 = parts.Length > 2 ? parts[2] : "";
        return new ParsedInstruction((byte)opCode, mnemonic, ParseOperand(op1), ParseOperand(op2));
    }

    private ParsedInstruction TryParse(ExtendedOpCode extendedOpCode)
    {
        var name = Enum.GetName(extendedOpCode);
        if (name == null)
        {
            return new ParsedInstruction((byte)extendedOpCode, "??", None(), None());
        }

        var parts = name.Split("_");
        var mnemonic = parts[0];
        if (mnemonic == "BIT" || mnemonic == "RES" || mnemonic == "SET")
        {
            mnemonic = $"{mnemonic} {parts[1]}";
            parts = parts.Skip(1).ToArray();
        }
        var op1 = parts.Length > 0 ? parts[0] : "";
        var op2 = parts.Length > 1 ? parts[1] : "";
        return new ParsedInstruction((byte)extendedOpCode, mnemonic, ParseOperand(op1), ParseOperand(op2));
    }

    private IOperand ParseOperand(string operandName)
    {
        if (operandName == "")
            return None();
        if (operandName == "A")
            return RegisterA();
        if (operandName == "B")
            return RegisterB();
        if (operandName == "C")
            return RegisterC();
        if (operandName == "D")
            return RegisterD();
        if (operandName == "E")
            return RegisterE();
        if (operandName == "H")
            return RegisterH();
        if (operandName == "L")
            return RegisterL();
        if (operandName == "BC")
            return RegisterBC();
        if (operandName == "DE")
            return RegisterDE();
        if (operandName == "HL")
            return RegisterHL();
        if (operandName == "SP")
            return RegisterSP();
        if (operandName == "AF")
            return RegisterAF();
        if (operandName == "HLm")
            return new MemoryOperand(RegisterHL());
        if (operandName == "d8")
            return ByteOperand();
        if (operandName == "s8")
            return ByteOperand();
        if (operandName == "a8")
            return new MemoryOperand(ByteOperand());
        if (operandName == "a16")
            return new MemoryOperand(WordOperand());
        if (operandName == "d16")
            return WordOperand();
        if (operandName == "s16")
            return WordOperand();
        if (operandName == "Cm")
            return new MemoryOperand(RegisterC());
        if (operandName == "NZ")
            return new FlagOperand(Flags.Zero, false);
        if (operandName == "Z")
            return new FlagOperand(Flags.Zero, true);
        if (operandName == "NC")
            return new FlagOperand(Flags.Carry, false);
        if (operandName == "C")
            return new FlagOperand(Flags.Carry, true);
        if (operandName == "SPs8")
            return new RegisterOffset("SP");
        return None();
    }

    private ParsedInstruction ParseCBInstruction(ref ushort pc)
    {
        var opcode = (ExtendedOpCode)memory.ReadByte(pc);
        return opcode switch
        {
            _ => TryParse(opcode)
        };
    }
}
using System.Text;
using GameboyEmulator.Architecture;
using GameboyEmulator.Architecture.Debug;
using GameboyEmulator.Frontend.UI;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace GameboyEmulator.Frontend;

public class EmulatorGame : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private EmulatorComponent emulatorComponent;
    private ImGuiRenderer imGuiRenderer;
    
    public SpriteBatch SpriteBatch => spriteBatch;

    private IntPtr backgroundTexture;
    private IntPtr windowTexture;
    private IntPtr spriteTexture;

    public EmulatorGame()
    {
        this.IsMouseVisible = true;
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        this.graphics.PreferredBackBufferWidth = 1280;
        this.graphics.PreferredBackBufferHeight = 720;
        this.graphics.ApplyChanges();

        var defaultRomPath = "roms/lycscx.gb";
        Encoding.UTF8.GetBytes(defaultRomPath).CopyTo(romFileBuffer, 0);
        
        spriteBatch = new SpriteBatch(GraphicsDevice);
        emulatorComponent = new EmulatorComponent(this);
        imGuiRenderer = new ImGuiRenderer(this);
        
        
        imGuiRenderer.AddLayout(LayoutBegin);
        imGuiRenderer.AddLayout(Layout);
        imGuiRenderer.AddLayout(LayoutRomLoad);
        imGuiRenderer.AddLayout(LayoutRegisters);
        imGuiRenderer.AddLayout(LayoutDebug);
        imGuiRenderer.AddLayout(LayoutDebuggerSettings);
        imGuiRenderer.AddLayout(LayoutGameWindow);
        imGuiRenderer.AddLayout(LayoutEnd);
        
        
        Components.Add(emulatorComponent);
        Components.Add(imGuiRenderer);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        base.LoadContent();
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        if (windowTexture == IntPtr.Zero && emulatorComponent.GetWindowTexture() != null)
        {
            windowTexture = imGuiRenderer.BindTexture(emulatorComponent.GetWindowTexture());
        }
        
        if (backgroundTexture == IntPtr.Zero && emulatorComponent.GetBackgroundTexture() != null)
        {
            backgroundTexture = imGuiRenderer.BindTexture(emulatorComponent.GetBackgroundTexture());
        }

        if (spriteTexture == IntPtr.Zero && emulatorComponent.GetSpriteTexture() != null)
        {
            spriteTexture = imGuiRenderer.BindTexture(emulatorComponent.GetSpriteTexture());
        }

        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    protected void LayoutBegin(GameTime gameTime)
    {
        ImGui.SetNextWindowPos(Vector2.Zero);
        ImGui.SetNextWindowSize(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));
        ImGui.Begin("Emulator", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBringToFrontOnFocus);
    }

    protected void LayoutEnd(GameTime gameTime)
    {
        ImGui.End();
    }

    private bool openDebug = true;
    private bool openRegisters = true;
    private bool openSettings = false;
    protected void Layout(GameTime gameTime)
    {
        var openLoad = false;
        if (ImGui.BeginMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Load Rom", "Ctrl+O"))
                {
                    openLoad = true;
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Debug"))
            {
                if (ImGui.MenuItem("Show Debugger", "Ctrl+D"))
                {
                    openDebug = true;
                }
                if (ImGui.MenuItem("Show Registers", "Ctrl+R"))
                {
                    openRegisters = true;
                }
                if (ImGui.MenuItem("Show Settings"))
                {
                    openSettings = true;
                }
            }
            ImGui.EndMenuBar();
        }
        if (openLoad)
            ImGui.OpenPopup("LoadRom");
    }


    private byte[] romFileBuffer = new byte[256];
    private bool debugMode = false;
    protected void LayoutRomLoad(GameTime gameTime)
    {
        ImGui.SetNextWindowSize(new Vector2(300, 200));
        if (ImGui.BeginPopupModal("LoadRom"))
        {
            ImGui.InputText("Rom Path", romFileBuffer, 256);
            ImGui.Checkbox("Debug Mode", ref debugMode);
            if (ImGui.Button("Load"))
            {
                emulatorComponent.LoadRom(GetNullTerminatedString(romFileBuffer), debugMode ? CpuState.Halt : CpuState.Run);
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
    }

    protected void LayoutRegisters(GameTime gameTime)
    {
        if (openRegisters && ImGui.Begin("Registers", ImGuiWindowFlags.MenuBar))
        {
            if (ImGui.BeginMenuBar())
            {     
                if (ImGui.MenuItem("Close"))
                {
                    openRegisters = false;
                }
                ImGui.EndMenuBar();
            }
            if (ImGui.BeginTable("RegisterTable", 2))
            {
                if (emulatorComponent.cpu != null)
                {
                    RegisterRow("AF", emulatorComponent.cpu.Registers.AF);
                    RegisterRow("BC", emulatorComponent.cpu.Registers.BC);
                    RegisterRow("DE", emulatorComponent.cpu.Registers.DE);
                    RegisterRow("HL", emulatorComponent.cpu.Registers.HL);
                    RegisterRow("SP", emulatorComponent.cpu.Registers.StackPointer);
                    RegisterRow("PC", emulatorComponent.cpu.Registers.ProgramCounter);
                    ImGui.Spacing();
                    RegisterRow("A", emulatorComponent.cpu.Registers.Accumulator);
                    RegisterRow("F", (byte)emulatorComponent.cpu.Registers.Flags);
                    RegisterRow("B", emulatorComponent.cpu.Registers.B);
                    RegisterRow("C", emulatorComponent.cpu.Registers.C);
                    RegisterRow("D", emulatorComponent.cpu.Registers.D);
                    RegisterRow("E", emulatorComponent.cpu.Registers.E);
                    RegisterRow("H", emulatorComponent.cpu.Registers.H);
                    RegisterRow("L", emulatorComponent.cpu.Registers.L);
                    ImGui.Spacing();
                    RegisterRow("Zero", emulatorComponent.cpu.Registers.Zero.ToString());
                    RegisterRow("Subtract", emulatorComponent.cpu.Registers.Subtract.ToString());
                    RegisterRow("HalfCarry", emulatorComponent.cpu.Registers.HalfCarry.ToString());
                    RegisterRow("Carry", emulatorComponent.cpu.Registers.Carry.ToString());
                    ImGui.Spacing();
                    RegisterRow("IME", emulatorComponent.cpu.InterruptsEnabled.ToString());
                    RegisterRow("IF", emulatorComponent.memory.ReadByte(Addresses.IF));
                    RegisterRow("IE", emulatorComponent.memory.ReadByte(Addresses.IE));
                }
                ImGui.EndTable();
            }
            ImGui.End();
        }
    }
    
    protected void RegisterRow(string name, ushort value)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        ImGui.Text(FormatWord(Formatting, value));
    }
    
    protected void RegisterRow(string name, string value)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        ImGui.Text(value);
    }
    
    protected void RegisterRow(string name, byte value)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        ImGui.Text(FormatByte(Formatting, value));
    }

    private ushort lastPc = ushort.MaxValue;
    private List<ParsedInstruction> instructions = new List<ParsedInstruction>();
    protected void LayoutDebug(GameTime gameTime)
    {
        if (openDebug && ImGui.Begin("Debugger", ImGuiWindowFlags.MenuBar))
        {
            if (emulatorComponent.cpu != null && ImGui.BeginMenuBar())
            {
                if (ImGui.MenuItem("Run"))
                {
                    emulatorComponent.cpu.State = CpuState.Run;
                }
                if (ImGui.Button("Step"))
                {
                    emulatorComponent.cpu.State = CpuState.Step;
                }
                
                ImGui.Text("Current State: " + emulatorComponent.cpu.State.ToString());
                ImGui.EndMenuBar();
            }

            if (emulatorComponent.cpu != null && emulatorComponent.instructionParser != null && emulatorComponent.cpu.State == CpuState.Halt)
            {
                var cpu = emulatorComponent.cpu;
                var parser = emulatorComponent.instructionParser;
                if (lastPc != cpu.Registers.ProgramCounter)
                {
                    instructions = parser.ParseInstructions(ref cpu.Registers, cpu.Registers.ProgramCounter, 25);
                    lastPc = cpu.Registers.ProgramCounter;
                }
            }

            if (ImGui.BeginTable("InstructionTable", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.ScrollY))
            {
                ImGui.TableSetupColumn("Index");
                ImGui.TableSetupColumn("Address");
                ImGui.TableSetupColumn("Instruction");
                ImGui.TableHeadersRow();
                
                for (int i = 0; i < instructions.Count; i++)
                {
                    var instruction = instructions[i];
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(i.ToString());
                    ImGui.TableNextColumn();
                    ImGui.Text(FormatWord(Formatting, instruction.Address));
                    ImGui.TableNextColumn();
                    ImGui.Text(instruction.ToString(Formatting));
                }
                ImGui.EndTable();
            }
        }
    }

    private int formattingIndex = 0;

    private ByteFormatting Formatting => formattingIndex switch
    {
        0 => ByteFormatting.Hex,
        1 => ByteFormatting.Decimal,
        2 => ByteFormatting.SignedDecimal,
        3 => ByteFormatting.Binary,
        4 => ByteFormatting.Ascii,
    };
    
    protected void LayoutDebuggerSettings(GameTime gameTime)
    {
        if (openSettings && ImGui.Begin("Debugger Settings"))
        {
            ImGui.Combo("Formatting", ref formattingIndex, "Hex\0Decimal\0Signed Decimal\0Binary\0Ascii\0");
            ImGui.End();
        }
    }

    protected void LayoutGameWindow(GameTime gameTime)
    {
        if (ImGui.Begin("Game"))
        {
            var _windowTexture = emulatorComponent.GetWindowTexture();
            var _backgroundTexture = emulatorComponent.GetBackgroundTexture();
            var _spriteTexture = emulatorComponent.GetSpriteTexture();
            
            // Draw background first
            ImGui.Image(this.backgroundTexture, new Vector2(_backgroundTexture.Width, _backgroundTexture.Height));
            
            // Draw window on top
            ImGui.SetCursorPos(new Vector2(0, 0));
            ImGui.Image(this.windowTexture, new Vector2(_windowTexture.Width, _windowTexture.Height));
            
            // Draw sprites on top
            ImGui.SetCursorPos(new Vector2(0, 0));
            ImGui.Image(this.spriteTexture, new Vector2(_spriteTexture.Width, _spriteTexture.Height));
            
            ImGui.End();
        }
    }

    protected string GetNullTerminatedString(byte[] buffer)
    {
        var length = GetNullTerminatedStringLength(buffer, 0);
        return Encoding.UTF8.GetString(buffer, 0, length);
    }
    
    protected int GetNullTerminatedStringLength(byte[] buffer, int offset)
    {
        var length = 0;
        while (buffer[offset + length] != 0)
            length++;
        return length;
    }

    protected string FormatWord(ByteFormatting formatting, ushort word)
    {
        return formatting switch
        {
            ByteFormatting.Hex => $"0x{word:X4}",
            ByteFormatting.Decimal => $"{word}",
            ByteFormatting.SignedDecimal => $"{(short)word}",
            ByteFormatting.Binary => Convert.ToString(word, 2).PadLeft(16, '0'),
            ByteFormatting.Ascii => $"{(char)word & 0b1111111100000000}{(char)word & 0b0000000011111111}",
            _ => throw new ArgumentOutOfRangeException(nameof(formatting), formatting, null)
        };
    }
    
    protected string FormatByte(ByteFormatting formatting, byte b)
    {
        return formatting switch
        {
            ByteFormatting.Hex => $"0x{b:X2}",
            ByteFormatting.Decimal => $"{b}",
            ByteFormatting.SignedDecimal => $"{(sbyte)b}",
            ByteFormatting.Binary => Convert.ToString(b, 2).PadLeft(8, '0'),
            ByteFormatting.Ascii => $"{(char)b}",
            _ => throw new ArgumentOutOfRangeException(nameof(formatting), formatting, null)
        };
    }
}
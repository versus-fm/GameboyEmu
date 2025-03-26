using System.Runtime.InteropServices;
using GameboyEmulator.Architecture;
using GameboyEmulator.Architecture.DebugTools;
using GameboyEmulator.Architecture.Interface;
using GameboyEmulator.Architecture.Rendering;
using GameboyEmulator.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace GameboyEmulator.Frontend;

public class EmulatorComponent : DrawableGameComponent
{
    public Hardware? hardware;
    public Memory? memory;
    public IProcessor? cpu;
    public IPictureProcessor? ppu;
    public InstructionParser? instructionParser;

    private SpriteBatch spriteBatch;
    
    private Texture2D backgroundTexture;
    private Texture2D windowTexture;
    private Texture2D spriteTexture;

    private Color[] backgroundBuffer;
    private Color[] windowBuffer;
    
    public EmulatorComponent(EmulatorGame game) : base(game)
    {
        this.spriteBatch = game.SpriteBatch;
    }

    public override void Initialize()
    {
        this.backgroundTexture = new Texture2D(this.GraphicsDevice, 256, 256);
        this.windowTexture = new Texture2D(this.GraphicsDevice, 160, 144);
        this.spriteTexture = new Texture2D(this.GraphicsDevice, 160, 144);
        this.backgroundBuffer = new Color[256 * 256];
        this.windowBuffer = new Color[160 * 144];
        base.Initialize();
    }

    public void LoadRom(string path, CpuState state = CpuState.Run)
    {
        var cartridge = CartridgeReader.ReadFile(path);
        memory = new Memory(cartridge);
        cpu = new Processor(memory);
        ppu = new PictureProcessor(memory);
        instructionParser = new InstructionParser(memory);
        hardware = new Hardware(memory, cpu, ppu);
        cpu.State = state;
        hardware.Initialize();
    }
    
    public override void Update(GameTime gameTime)
    {
        if (hardware != null)
        {
            hardware.ProcessFrame(false);
            CopyFrame();
        }
        base.Update(gameTime);
    }

    private void CopyFrame()
    {
        CopyBackground();
        CopyWindow();
        CopySprites();
    }

    private void CopyBackground()
    {
        var emulatorColorData = ppu.BackgroundPixels;
        var colorData = MemoryMarshal.Cast<GameboyEmulator.Architecture.Rendering.Color, Color>(emulatorColorData);
        colorData.CopyTo(backgroundBuffer);
        backgroundTexture.SetData(backgroundBuffer);
    }
    
    private void CopyWindow()
    {
        var emulatorColorData = ppu.WindowPixels;
        var colorData = MemoryMarshal.Cast<GameboyEmulator.Architecture.Rendering.Color, Color>(emulatorColorData);
        colorData.CopyTo(windowBuffer);
        windowTexture.SetData(windowBuffer);
    }
    
    private void CopySprites()
    {
        var emulatorColorData = ppu.SpritePixels;
        var colorData = MemoryMarshal.Cast<GameboyEmulator.Architecture.Rendering.Color, Color>(emulatorColorData);
        spriteTexture.SetData(colorData.ToArray());
    }

    public Texture2D GetWindowTexture()
    {
        return windowTexture;
    }

    public Texture2D GetBackgroundTexture()
    {
        return backgroundTexture;
    }

    public Texture2D GetSpriteTexture()
    {
        return spriteTexture;
    }
}
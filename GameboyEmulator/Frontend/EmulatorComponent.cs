﻿using System.Runtime.InteropServices;
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
    private Color[] spriteBuffer;
    
    public EmulatorComponent(EmulatorGame game) : base(game)
    {
        this.spriteBatch = game.SpriteBatch;
    }

    public override void Initialize()
    {
        this.backgroundTexture = new Texture2D(this.GraphicsDevice, 160, 144);
        this.windowTexture = new Texture2D(this.GraphicsDevice, 160, 144);
        this.spriteTexture = new Texture2D(this.GraphicsDevice, 160, 144);
        this.backgroundBuffer = new Color[160 * 144];
        this.windowBuffer = new Color[160 * 144];
        this.spriteBuffer = new Color[160 * 144];
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

    public override void Draw(GameTime gameTime)
    {
        if (hardware != null)
        {
            var scaling = 2;
            var targetSize = new Point(160 * scaling, 144 * scaling);
            var middle = new Point(this.GraphicsDevice.Viewport.Width / 2 - targetSize.X / 2,
                this.GraphicsDevice.Viewport.Height / 2 - targetSize.Y / 2);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(backgroundTexture, new Rectangle(middle, targetSize), Color.White);
            spriteBatch.Draw(windowTexture, new Rectangle(middle, targetSize), Color.White);
            spriteBatch.Draw(spriteTexture, new Rectangle(middle - new Point(8 * scaling), targetSize), Color.White);
            spriteBatch.End();
        }
        base.Draw(gameTime);
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
        colorData.CopyTo(spriteBuffer);
        spriteTexture.SetData(spriteBuffer);
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
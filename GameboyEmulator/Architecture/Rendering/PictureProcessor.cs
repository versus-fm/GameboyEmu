using GameboyEmulator.Architecture.Interface;

namespace GameboyEmulator.Architecture.Rendering;

public class PictureProcessor : IPictureProcessor
{
    private Memory memory;
    private uint cycleCount = 0;
    
    private Color[] backgroundPixels = new Color[160 * 144];
    private Color[] windowPixels = new Color[160 * 144];
    private Color[] spritePixels = new Color[160 * 144];
    
    public ReadOnlySpan<Color> BackgroundPixels => backgroundPixels;
    public ReadOnlySpan<Color> WindowPixels => windowPixels;
    public ReadOnlySpan<Color> SpritePixels => spritePixels;

    private bool last = false;
    
    public PictureProcessor(Memory memory)
    {
        this.memory = memory;
    }
    public void RunCycles(uint cycles)
    {
        if (!memory.ReadHardwareRegister(Addresses.LCDC, 7))
        {
            if (last)
            {
                //Console.WriteLine("LCD Disabled - Clearing screen");
                //Array.Clear(backgroundPixels, 0, backgroundPixels.Length);
                //Array.Clear(windowPixels, 0, windowPixels.Length);
                //Array.Clear(spritePixels, 0, spritePixels.Length);
            }
            last = false;
            return;
        }

        if (!last)
        {
            //Console.WriteLine("LCD Enabled - Initializing screen");
            //Array.Clear(backgroundPixels, 0, backgroundPixels.Length);
            //Array.Clear(windowPixels, 0, windowPixels.Length);
            //Array.Clear(spritePixels, 0, spritePixels.Length);
        }
        last = true;

        var mode = memory.ReadLcdMode();
        cycleCount += cycles;
        switch (mode)
        {
            case LcdMode.VBlank:
                RenderVBlank();
                break;
            case LcdMode.HBlank:
                RenderHBlank();
                break;
            case LcdMode.OamSearch:
                PerformOamSearch();
                break;
            case LcdMode.PixelTransfer:
                PerformPixelTransfer();
                break;
        }
    }

    private void RenderVBlank()
    {
        if (cycleCount >= 456)
        {
            if (memory.ReadByte(Addresses.LY) == 144)
            {
                memory.RequestInterrupt(InterruptFlags.VBlank);
                //Console.WriteLine("VBlank");
                // TODO: Render frame
            }
            
            if (memory.ReadByte(Addresses.LY) >= 153)
            {
                memory.WriteByte(Addresses.LY, 0);
                memory.WriteLcdMode(LcdMode.OamSearch);
            }
            else
            {
                memory.WriteByte(Addresses.LY, (byte)(memory.ReadByte(Addresses.LY) + 1));
            }
            
            cycleCount -= 456;
        }
    }

    private void RenderHBlank()
    {
        if (cycleCount >= 204)
        {
            //Console.WriteLine(Convert.ToString(memory.ReadByte(Addresses.STAT), 2));
            if (memory.ReadHardwareRegister(Addresses.STAT, 3))
            {
                memory.RequestInterrupt(InterruptFlags.LCDStat);
            }
            
            // Render background first
            RenderBackground();
            
            // Then render window on top
            RenderWindow();
            
            RenderSprites();

            if (memory.ReadByte(Addresses.LY) == 143)
            {
                memory.WriteLcdMode(LcdMode.VBlank);
            }
            else
            {
                memory.WriteLcdMode(LcdMode.OamSearch);
            }
            memory.WriteByte(Addresses.LY, (byte)(memory.ReadByte(Addresses.LY) + 1));
            cycleCount -= 204;
        }
    }

    private void PerformOamSearch()
    {
        if (cycleCount >= 80)
        {
            if (memory.ReadHardwareRegister(Addresses.STAT, 5))
            {
                memory.RequestInterrupt(InterruptFlags.LCDStat);
            }
            var ly = memory.ReadByte(Addresses.LY);
            var lyc = memory.ReadByte(Addresses.LYC);
            if (ly == lyc)
            {
                memory.WriteHardwareRegister(Addresses.STAT, 2, true);
                if (memory.ReadHardwareRegister(Addresses.STAT, 6))
                {
                    memory.RequestInterrupt(InterruptFlags.LCDStat);
                }
            }
            else
            {
                memory.WriteHardwareRegister(Addresses.STAT, 2, false);
            }
            memory.WriteLcdMode(LcdMode.PixelTransfer);
            cycleCount -= 80;
        }
    }

    private void PerformPixelTransfer()
    {
        if (cycleCount >= 173)
        {
            memory.WriteLcdMode(LcdMode.HBlank);
            cycleCount -= 173;
        }
    }

    private void RenderBackground()
    {
        var lcdc = memory.ReadByte(Addresses.LCDC);
        var backgroundEnabled = memory.ReadHardwareRegister(Addresses.LCDC, 0);
        var windowEnabled = memory.ReadHardwareRegister(Addresses.LCDC, 5);
        var tileDataMode = memory.ReadHardwareRegister(Addresses.LCDC, 4);
        var backgroundTileMap = memory.ReadHardwareRegister(Addresses.LCDC, 3);
        var windowTileMap = memory.ReadHardwareRegister(Addresses.LCDC, 6);

        // Console.WriteLine($"LCD Control: 0x{lcdc:X2}");
        // Console.WriteLine($"Background: {(backgroundEnabled ? "Enabled" : "Disabled")}");
        // Console.WriteLine($"Window: {(windowEnabled ? "Enabled" : "Disabled")}");
        // Console.WriteLine($"Tile Data Mode: {(tileDataMode ? "8000-8FFF" : "8800-97FF")}");
        // Console.WriteLine($"Background Tile Map: {(backgroundTileMap ? "9C00-9FFF" : "9800-9BFF")}");
        // Console.WriteLine($"Window Tile Map: {(windowTileMap ? "9C00-9FFF" : "9800-9BFF")}");

        if (!backgroundEnabled) return;
        
        var y = memory.ReadByte(Addresses.LY);
        var scrollY = memory.ReadByte(Addresses.SCY);
        var scrollX = memory.ReadByte(Addresses.SCX);
        
        
        for (var x = 0u; x < 160; x++)
        {
            var backgroundX = (x + scrollX) % 256;
            var backgroundY = (y + scrollY) % 256;
            var tileX = backgroundX / 8;
            var tileY = backgroundY / 8;
            var tile = memory.GetBackgroundTileIndex(tileX, (uint)tileY);
            var tileData = memory.GetBackgroundTile(tile);
            var colorIndex = tileData.GetColorIndex((uint)(backgroundX % 8), (uint)(backgroundY % 8));
            var color = memory.GetBackgroundColor(colorIndex);
            if (y < 144)
            {
                backgroundPixels[x + 160 * y] = color;
            }
        }
    }
    
    private void RenderWindow()
    {
        if (!memory.ReadHardwareRegister(Addresses.LCDC, 5)) {
            return;
        }
        
        var y = memory.ReadByte(Addresses.LY);
        var windowY = memory.ReadByte(Addresses.WY);
        var windowX = memory.ReadByte(Addresses.WX);
        
        //Console.WriteLine($"Window position: ({windowX}, {windowY})");
        
        if (y < windowY || y >= 144) return;
        
        var adjustedWindowX = windowX - 7;
        if (adjustedWindowX >= 160) return;
        
        //Console.WriteLine($"Rendering window line {y} at X offset {adjustedWindowX}");
        
        for (var x = 0u; x < 160u; x++)
        {
            if (x + adjustedWindowX >= 160) break;
            var windowPixelX = x;
            var windowPixelY = y - windowY;
            var tileX = windowPixelX / 8;
            var tileY = windowPixelY / 8;
            var tile = memory.GetWindowTileIndex(tileX, (uint)tileY);
            var tileData = memory.GetWindowTile(tile);
            var colorIndex = tileData.GetColorIndex((uint)(windowPixelX % 8), (uint)(windowPixelY % 8));
            var color = memory.GetBackgroundColor(colorIndex);
            windowPixels[x + 160 * y] = color;
        }
    }
    
    private void RenderSprites()
    {
        var y = memory.ReadByte(Addresses.LY);
        if (y >= 144) return;


        // Get sprite data from OAM
        for (int i = 0; i < 40; i++)
        {
            var spriteY = memory.ReadByte((ushort)(0xFE00 + i * 4));
            var spriteX = memory.ReadByte((ushort)(0xFE00 + i * 4 + 1));
            var tileNumber = memory.ReadByte((ushort)(0xFE00 + i * 4 + 2));
            var attributes = new TileAttribute(memory.ReadByte((ushort)(0xFE00 + i * 4 + 3)));

            // Check if sprite is on current scanline
            if (spriteY <= y + 8 && spriteY > y)
            {
                var tileData = memory.GetObjectTile(tileNumber);
                var pixelY = y - spriteY + 8;
                if (attributes.VerticalFlip)
                {
                    pixelY = 7 - pixelY;
                }

                for (int x = 0; x < 8; x++)
                {
                    var pixelX = x;
                    if (attributes.HorizontalFlip)
                    {
                        pixelX = 7 - x;
                    }

                    var colorIndex = tileData.GetColorIndex((uint)pixelX, (uint)pixelY);
                    var color = memory.GetObjectColor(colorIndex);
                    
                    if (spriteX + x >= 0 && spriteX + x < 160)
                        SetSpritePixel(spriteX + x, y, color);
                }
            }
        }
    }
    
    private void SetSpritePixel(int x, int y, Color color)
    {
        spritePixels[y * 160 + x] = color;
    }
    
    private void SetBackgroundPixel(int x, int y, Color color)
    {
        backgroundPixels[y * 256 + x] = color;
    }
    
    private void SetWindowPixel(int x, int y, Color color)
    {
        windowPixels[y * 160 + x] = color;
    }
    
    public Color[] GetSpritePixels()
    {
        return spritePixels;
    }
    
    public Color[] GetBackgroundPixels()
    {
        return backgroundPixels;
    }
    
    public Color[] GetWindowPixels()
    {
        return windowPixels;
    }
    
    private Color SampleBackgroundPixel(uint x, uint y)
    {
        if (x > 255 || y > 255)
            throw new ArgumentException("Invalid coordinates");
        var tileX = x / 8;
        var tileY = y / 8;
        var tile = memory.GetBackgroundTileIndex(tileX, tileY);
        var tileData = memory.GetBackgroundTile(tile);
        var colorIndex = tileData.GetColorIndex(x % 8, y % 8);
        return memory.GetBackgroundColor(colorIndex);
    }
    
    private Color SampleWindowPixel(uint x, uint y)
    {
        if (x > 255 || y > 255)
            throw new ArgumentException("Invalid coordinates");
        var tileX = x / 8;
        var tileY = y / 8;
        var tile = memory.GetWindowTileIndex(tileX, tileY);
        var tileData = memory.GetWindowTile(tile);
        var colorIndex = tileData.GetColorIndex(x % 8, y % 8);
        return memory.GetBackgroundColor(colorIndex);
    }
}
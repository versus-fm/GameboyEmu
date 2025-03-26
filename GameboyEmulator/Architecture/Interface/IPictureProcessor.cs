using GameboyEmulator.Architecture.Rendering;

namespace GameboyEmulator.Architecture.Interface;

public interface IPictureProcessor
{
    void RunCycles(uint cycles);
    
    public ReadOnlySpan<Color> BackgroundPixels { get; }
    public ReadOnlySpan<Color> WindowPixels { get; }
    public ReadOnlySpan<Color> SpritePixels { get; }
}
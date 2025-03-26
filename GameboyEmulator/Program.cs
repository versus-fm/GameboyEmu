using GameboyEmulator.Architecture;
using GameboyEmulator.Architecture.Rendering;
using GameboyEmulator.Frontend;
using GameboyEmulator.IO;

namespace GameboyEmulator;

public class Program
{
    private byte[] boot = { };
    public static void Main(string[] args)
    {
        using var game = new EmulatorGame();
        game.Run();
    }
}
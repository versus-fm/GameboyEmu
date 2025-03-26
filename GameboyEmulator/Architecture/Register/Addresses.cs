namespace GameboyEmulator.Architecture;

public enum Addresses : ushort
{
    /// <summary>
    /// <para/>Usage: Joypad
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    JOYP = 0xFF00,

    /// <summary>
    /// <para/>Usage: Serial transfer data
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    SB = 0xFF01,

    /// <summary>
    /// <para/>Usage: Serial transfer control
    /// <para/>Access: R/W
    /// <para/>Model: Mixed
    /// </summary>
    SC = 0xFF02,

    /// <summary>
    /// <para/>Usage: Divider register
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    DIV = 0xFF04,

    /// <summary>
    /// <para/>Usage: Timer counter
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    TIMA = 0xFF05,

    /// <summary>
    /// <para/>Usage: Timer modulo
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    TMA = 0xFF06,

    /// <summary>
    /// <para/>Usage: Timer control
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    TAC = 0xFF07,

    /// <summary>
    /// <para/>Usage: Interrupt flag
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    IF = 0xFF0F,

    /// <summary>
    /// <para/>Usage: Sound channel 1 sweep
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR10 = 0xFF10,

    /// <summary>
    /// <para/>Usage: Sound channel 1 length timer & duty cycle
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    NR11 = 0xFF11,

    /// <summary>
    /// <para/>Usage: Sound channel 1 volume & envelope
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR12 = 0xFF12,

    /// <summary>
    /// <para/>Usage: Sound channel 1 period low
    /// <para/>Access: W
    /// <para/>Model: All
    /// </summary>
    NR13 = 0xFF13,

    /// <summary>
    /// <para/>Usage: Sound channel 1 period high & control
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    NR14 = 0xFF14,

    /// <summary>
    /// <para/>Usage: Sound channel 2 length timer & duty cycle
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    NR21 = 0xFF16,

    /// <summary>
    /// <para/>Usage: Sound channel 2 volume & envelope
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR22 = 0xFF17,

    /// <summary>
    /// <para/>Usage: Sound channel 2 period low
    /// <para/>Access: W
    /// <para/>Model: All
    /// </summary>
    NR23 = 0xFF18,

    /// <summary>
    /// <para/>Usage: Sound channel 2 period high & control
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    NR24 = 0xFF19,

    /// <summary>
    /// <para/>Usage: Sound channel 3 DAC enable
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR30 = 0xFF1A,

    /// <summary>
    /// <para/>Usage: Sound channel 3 length timer
    /// <para/>Access: W
    /// <para/>Model: All
    /// </summary>
    NR31 = 0xFF1B,

    /// <summary>
    /// <para/>Usage: Sound channel 3 output level
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR32 = 0xFF1C,

    /// <summary>
    /// <para/>Usage: Sound channel 3 period low
    /// <para/>Access: W
    /// <para/>Model: All
    /// </summary>
    NR33 = 0xFF1D,

    /// <summary>
    /// <para/>Usage: Sound channel 3 period high & control
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    NR34 = 0xFF1E,

    /// <summary>
    /// <para/>Usage: Sound channel 4 length timer
    /// <para/>Access: W
    /// <para/>Model: All
    /// </summary>
    NR41 = 0xFF20,

    /// <summary>
    /// <para/>Usage: Sound channel 4 volume & envelope
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR42 = 0xFF21,

    /// <summary>
    /// <para/>Usage: Sound channel 4 frequency & randomness
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR43 = 0xFF22,

    /// <summary>
    /// <para/>Usage: Sound channel 4 control
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    NR44 = 0xFF23,

    /// <summary>
    /// <para/>Usage: Master volume & VIN panning
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR50 = 0xFF24,

    /// <summary>
    /// <para/>Usage: Sound panning
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    NR51 = 0xFF25,

    /// <summary>
    /// <para/>Usage: Sound on/off
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    NR52 = 0xFF26,

    /// <summary>
    /// <para/>Usage: Storage for one of the sound channels’ waveform
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// <para/>Size: 16 bytes (0xFF30-0xFF3F)
    /// </summary>
    WaveRAM = 0xFF30,

    /// <summary>
    /// <para/>Usage: LCD control
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    LCDC = 0xFF40,

    /// <summary>
    /// <para/>Usage: LCD status
    /// <para/>Access: Mixed
    /// <para/>Model: All
    /// </summary>
    STAT = 0xFF41,

    /// <summary>
    /// <para/>Usage: Viewport Y position
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    SCY = 0xFF42,

    /// <summary>
    /// <para/>Usage: Viewport X position
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    SCX = 0xFF43,

    /// <summary>
    /// <para/>Usage: LCD Y coordinate
    /// <para/>Access: R
    /// <para/>Model: All
    /// </summary>
    LY = 0xFF44,

    /// <summary>
    /// <para/>Usage: LY compare
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    LYC = 0xFF45,

    /// <summary>
    /// <para/>Usage: OAM DMA source address & start
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    DMA = 0xFF46,

    /// <summary>
    /// <para/>Usage: BG palette data
    /// <para/>Access: R/W
    /// <para/>Model: DMG
    /// </summary>
    BGP = 0xFF47,

    /// <summary>
    /// <para/>Usage: OBJ palette 0 data
    /// <para/>Access: R/W
    /// <para/>Model: DMG
    /// </summary>
    OBP0 = 0xFF48,

    /// <summary>
    /// <para/>Usage: OBJ palette 1 data
    /// <para/>Access: R/W
    /// <para/>Model: DMG
    /// </summary>
    OBP1 = 0xFF49,

    /// <summary>
    /// <para/>Usage: Window Y position
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    WY = 0xFF4A,

    /// <summary>
    /// <para/>Usage: Window X position plus 7
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    WX = 0xFF4B,

    /// <summary>
    /// <para/>Usage: Prepare speed switch
    /// <para/>Access: Mixed
    /// <para/>Model: CGB
    /// </summary>
    KEY1 = 0xFF4D,

    /// <summary>
    /// <para/>Usage: VRAM bank
    /// <para/>Access: R/W
    /// <para/>Model: CGB
    /// </summary>
    VBK = 0xFF4F,

    /// <summary>
    /// <para/>Usage: VRAM DMA source high
    /// <para/>Access: W
    /// <para/>Model: CGB
    /// </summary>
    HDMA1 = 0xFF51,

    /// <summary>
    /// <para/>Usage: VRAM DMA source low
    /// <para/>Access: W
    /// <para/>Model: CGB
    /// </summary>
    HDMA2 = 0xFF52,

    /// <summary>
    /// <para/>Usage: VRAM DMA destination high
    /// <para/>Access: W
    /// <para/>Model: CGB
    /// </summary>
    HDMA3 = 0xFF53,

    /// <summary>
    /// <para/>Usage: VRAM DMA destination low
    /// <para/>Access: W
    /// <para/>Model: CGB
    /// </summary>
    HDMA4 = 0xFF54,

    /// <summary>
    /// <para/>Usage: VRAM DMA length/mode/start
    /// <para/>Access: R/W
    /// <para/>Model: CGB
    /// </summary>
    HDMA5 = 0xFF55,

    /// <summary>
    /// <para/>Usage: Infrared communications port
    /// <para/>Access: Mixed
    /// <para/>Model: CGB
    /// </summary>
    RP = 0xFF56,

    /// <summary>
    /// <para/>Usage: Background color palette specification / Background palette index
    /// <para/>Access: R/W
    /// <para/>Model: CGB
    /// </summary>
    BCPS_BGPI = 0xFF68,

    /// <summary>
    /// <para/>Usage: Background color palette data / Background palette data
    /// <para/>Access: R/W
    /// <para/>Model: CGB
    /// </summary>
    BCPD_BGPD = 0xFF69,

    /// <summary>
    /// <para/>Usage: OBJ color palette specification / OBJ palette index
    /// <para/>Access: R/W
    /// <para/>Model: CGB
    /// </summary>
    OCPS_OBPI = 0xFF6A,

    /// <summary>
    /// <para/>Usage: OBJ color palette data / OBJ palette data
    /// <para/>Access: R/W
    /// <para/>Model: CGB
    /// </summary>
    OCPD_OBPD = 0xFF6B,

    /// <summary>
    /// <para/>Usage: Object priority mode
    /// <para/>Access: R/W
    /// <para/>Model: CGB
    /// </summary>
    OPRI = 0xFF6C,

    /// <summary>
    /// <para/>Usage: WRAM bank
    /// <para/>Access: R/W
    /// <para/>Model: CGB
    /// </summary>
    SVBK = 0xFF70,

    /// <summary>
    /// <para/>Usage: Audio digital outputs 1 & 2
    /// <para/>Access: R
    /// <para/>Model: CGB
    /// </summary>
    PCM12 = 0xFF76,

    /// <summary>
    /// <para/>Usage: Audio digital outputs 3 & 4
    /// <para/>Access: R
    /// <para/>Model: CGB
    /// </summary>
    PCM34 = 0xFF77,

    /// <summary>
    /// <para/>Usage: Interrupt enable
    /// <para/>Access: R/W
    /// <para/>Model: All
    /// </summary>
    IE = 0xFFFF,
    
    VBlankVector = 0x0040,
    LcdVector = 0x0048,
    TimerVector = 0x0050,
    SerialVector = 0x0058,
    InputVector = 0x0060
}
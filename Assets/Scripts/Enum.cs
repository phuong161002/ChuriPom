using System;

public enum PieceColor
{
    WHITE,
    GREEN,
    RED,
    CYAN,
    YELLOW,
    PURPLE,
    ORANGE,
    COUNT,
    NONE,
}

public enum PieceType
{
    NORMAL,
    HUGE,
    EXPLOSIVE,
    LOCKED,
    FREEZING,
    EMPTY,
    COUNT,
}

public enum EyeColor
{
    Green, 
    Blue,
    Red, 
    Orange, 
    Black,
}

public enum RollType
{
    NONE,
    HORIZONTAL,
    VERTICAL,
}

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    COUNT
}

public enum ComboType
{
    Combo,
    Super,
    x2,
    x3,
    x4,
    x5,
    x6,
    Wow,
    Count,
}

public enum HintType
{
    Normal,
    Huge,
}

public enum OutOfRangeType
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
}

public enum LevelType
{
    FillBottle,
    FillColor,
    UnlockPiece,
    Instruction,
}
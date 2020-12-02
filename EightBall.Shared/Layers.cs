using System;

namespace EightBall.Shared
{
    [Flags]
    public enum Layers
    {
        None = 0,
        Ball = 1,
        Rail = 2,
        Hole = 3,
        Cue = 4,
        WhiteBall = 5,
        //None = 0,
        //Ball = 1 << 0,
        //Rail = 1 << 1,
        //Hole = 1 << 2,
        //Cue = 1 << 3,
        //WhiteBall = 1 << 4,
    }
}

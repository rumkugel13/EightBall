using System;
using System.Collections.Generic;
using System.Text;

namespace EightBall.Shared
{
    public struct GameState
    {
        public Player ActivePlayer;
        public Player LeftPlayer;
        public Player RightPlayer;
        public Player Winner;
        public Player InactivePlayer => this.ActivePlayer == Player.One ? Player.Two : Player.One;

        public int InactiveBalls;

        public void Reset()
        {
            this.ActivePlayer = Player.One;
            this.LeftPlayer = Player.None;
            this.RightPlayer = Player.None;
            this.Winner = Player.None;

            this.InactiveBalls = 0;
        }
    }
}

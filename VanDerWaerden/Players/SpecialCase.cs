using System;

namespace VanDerWaerden.Players
{
    public class SpecialCasePlayer : Player
    {
        public SpecialCasePlayer(Configuration config, int id) : base(config, id) { }

        protected override int Strategy(Game game)
        {
            int prev = (int)game.lastChosen;
            int chosen = n - prev + 1;
            if (game.board[chosen] != null)
            {
                throw new ArgumentException($"Unexpected strategy error: chosen number {chosen} has been already selected!");
            }
            return chosen;
        }
    }
}

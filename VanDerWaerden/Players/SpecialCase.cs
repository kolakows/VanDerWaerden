using System;

namespace VanDerWaerden.Players
{
    public class SpecialCasePlayer : Player
    {
        public SpecialCasePlayer(Configuration config, int id) : base(config, id) { }

        protected override int Strategy(Game game)
        {
            int prev = game.lastChosen.Value;
            int chosen = n - prev - 1;
            Console.WriteLine($"Prev move: {prev}, n: {n}, chosen: n - prev - 1 = {chosen}");
            if (game.board[chosen] != null)
            {
                throw new ArgumentException($"Unexpected strategy error: chosen number {chosen} has been already selected!");
            }
            return chosen;
        }
    }
}

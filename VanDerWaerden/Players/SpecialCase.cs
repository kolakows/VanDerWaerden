using System;

namespace VanDerWaerden.Players
{
    public class SpecialCasePlayer : Player
    {
        public SpecialCasePlayer(Configuration config, int id) : base(config, id) { }


        protected override int Strategy(Game game)
        {
            int prev = game.LastChosen.Value;
            int chosen = n - prev - 1;
            Console.WriteLine($"Prev move: {prev}, n: {n}, chosen: n - prev - 1 = {chosen}");
            if (game.Board[chosen] != null)
            {
                throw new ArgumentException($"Unexpected strategy error: chosen number {chosen} has been already selected!");
            }
            return chosen;
        }

        public override Player Clone()
        {
            var config = new Configuration()
            {
                k = this.k,
                n = this.n
            };
            var player = new SpecialCasePlayer(config, id);
            CopyPlayerStatusTo(player);
            return player;
        }
    }
}

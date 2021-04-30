using System;
using System.Linq;

namespace VanDerWaerden.Players
{
    public class RandomPlayer : Player
    {
        public Random Random { get; set; }

        public RandomPlayer(Configuration config, int id, int seed) : base(config, id)
        {
            Random = new Random(seed);
        }

        protected override int Strategy(Game game)
        {
            var board = game.board;
            var freeIndices = Enumerable.Range(0, board.Length).Where(i => board[i] == null).ToList();
            return freeIndices[Random.Next(freeIndices.Count)];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanDerWaerden.Players
{
	public class RandomPlayer : Player
	{
		public Random Random { get; set; }

		public RandomPlayer(Configuration config, int seed) : base(config)
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

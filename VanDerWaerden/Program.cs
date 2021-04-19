using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanDerWaerden.Players;

namespace VanDerWaerden
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = new Configuration { n = 9, k = 3 };
			var first = new RandomPlayer(config, 420);
			var second = new RandomPlayer(config, 123);
			var game = new Game(config, first, second);
			game.Play();
			Console.ReadKey();
		}
	}
}

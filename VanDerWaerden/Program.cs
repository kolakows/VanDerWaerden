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
			int n = 9, k = 3;
			Console.WriteLine("Hello in VAN DER WAERDEN MISÈRE game!");

			while(true)
			{
				Console.WriteLine("1. Demo (single game)");
				Console.WriteLine("2. Test (multiple games)");
				Console.WriteLine("3. Exit");

				int option = 3;
				if (int.TryParse(Console.ReadLine(), out int opt))
					option = opt;

				if (option == 3)
					break;

				Console.WriteLine("Choose value of n (default n = 9):");
				if(int.TryParse(Console.ReadLine(), out int nn))
					n = nn;
				Console.WriteLine($"n = {n}");
				Console.WriteLine("Choose value of k (default k = 3):");
				if (int.TryParse(Console.ReadLine(), out int kk))
					k = kk;
				Console.WriteLine($"k = {k} ");

				var config = new Configuration { n = n, k = k };
				var first = new RandomPlayer(config, 420);
				var second = new RandomPlayer(config, 123);

				//DEMO
				if(option == 1)
				{
					var game = new Game(config, first, second);
					game.Play(verbose: true);
				}
				//TEST
				else
				{
					Console.WriteLine("Type number of games:");
					int numberOfGames = 0;
					if(int.TryParse(Console.ReadLine(), out int number))
						numberOfGames = number;

					int[] results = new int[3];
					for(int i = 0; i < numberOfGames; i++)
					{
						var game = new Game(config, first, second);
						results[game.Play(verbose: false)]++;
					}

					if(numberOfGames > 0)
					{
						Console.WriteLine($"First player: {results[1]} times");
						Console.WriteLine($"Second player: {results[2]} times");
						Console.WriteLine($"Draw: {results[0]} times");
						Console.WriteLine();
					}
				}
			}
		}
	}
}

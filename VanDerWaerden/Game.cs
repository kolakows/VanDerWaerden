using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanDerWaerden
{
	public struct Configuration
	{
		public int n;
		public int k;
	}

	public abstract class Player
	{
		public int n, k;
		public List<int> playerNumbers;
		public List<Progression> progressions;

		public Player(Configuration config)
		{
			this.n = config.n;
			this.k = config.k;
			playerNumbers = new List<int>();
			progressions = new List<Progression>();
		}

		protected abstract int Strategy(Player[] board);

		public int ChooseNumber(Player[] board)
		{
			var chosen = Strategy(board);
			playerNumbers.Add(chosen);
			UpdateProgressions(chosen);
			return chosen;
		}

		private void UpdateProgressions(int chosen)
		{
			foreach (var progression in progressions)
				progression.ExtendBy(chosen);

			// generate progressions of length 2
			foreach (var number in playerNumbers)
			{
				if(number != chosen)
				{
					var pair = new Progression(Math.Abs(number - chosen))
					{
						number,
						chosen
					};
					pair.Sort();
					progressions.Add(pair);
				}
			}

			Collate();
		}

		private void Collate()
		{
			foreach (var group in progressions.GroupBy(x => x.stride))
			{
				foreach (var a in group)
					foreach (var b in group)
					{
						if(a.Last() == b.First())
						{
							var collated = new Progression(group.Key);
							collated.AddRange(a);
							collated.AddRange(b.Skip(1));
							collated.Sort();
							progressions.Add(collated);
							progressions.Remove(a);
							progressions.Remove(b);
						}
					}
			}
		}

		private bool InRange(int i)
		{
			return i >= 0 && i < n;
		}
	}

	public class Progression : List<int>
	{
		public int stride;

		public Progression(int stride)
		{
			this.stride = stride;
		}

		public void ExtendBy(int number)
		{
			if (this.Last() + stride == number)
				this.Add(number);
			if (this.First() - stride == number)
				this.Insert(0, number);
		}

		public override string ToString()
		{
			return string.Join(" ", this.Select(x => x.ToString()).ToArray());
		}
	}

	public class Game
	{
		public int n, k;
		// indexing from 0 to n-1
		public Player[] board;
		public List<Player> players;
		public Player first;
		public Player second;
		public Player active;
		public Player winner;
		public bool done;

		public Game(Configuration config, Player first, Player second)
		{
			this.n = config.n;
			this.k = config.k;
			board = new Player[n];
			active = first;
			this.first = first;
			this.second = second;
			players = new List<Player> { first, second };
			done = false;
		}

		public void Play()
		{
			while (!done)
				Step();
			Console.WriteLine($"And the winner is {(active == first ? "first" : "second")}");
			Console.WriteLine($"Winning progression {active.progressions.Where(x => x.Count >= k).First()}");
			Console.WriteLine();
			Console.WriteLine($"Board: {string.Join(" ", Enumerable.Range(0, n).Select(x => x.ToString()))}");
			Console.WriteLine($"First player numbers: {string.Join(" ",first.playerNumbers.Select(x => x.ToString()))}");
			Console.WriteLine($"Second player numbers: {string.Join(" ",second.playerNumbers.Select(x => x.ToString()))}");
		}

		public void Step()
		{
			if (done)
				return;

			var chosen = active.ChooseNumber(board);
			board[chosen] = active;
			if (active.progressions.Any(x => x.Count >= k))
			{
				done = true;
				winner = active;
				return;
			}

			active = players.Where(x => x != active).Single();
			if (board.All(x => x != null))
				done = true;
		}
		
	}
}

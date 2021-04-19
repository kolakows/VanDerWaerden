using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanDerWaerden.Players;

namespace VanDerWaerden
{
	public struct Configuration
	{
		public int n;
		public int k;
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

		private int? lastChosen;

		public Player NotActive { get { return players.Where(x => x != active).Single(); } }

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
			var chosen = active.ChooseNumber(this.Clone());
			TakeNumber(chosen);
		}

		public void TakeNumber(int chosen)
		{
			lastChosen = chosen;
			board[chosen] = active;
			if (active.progressions.Any(x => x.Count >= k))
			{
				active = NotActive;
				winner = active; // Misère
				done = true;
				return;
			}

			active = NotActive;
			if (board.All(x => x != null))
				done = true;
		}

		// undo last move
		public void Undo()
		{
			if(lastChosen.HasValue)
			{
				board[lastChosen.Value] = null;
				active = NotActive;
				done = false;
				lastChosen = null;
				return;
			}
			throw new InvalidOperationException();
		}

		public List<int> AvailableNumbers()
		{
			return Enumerable.Range(0, board.Length).Where(i => board[i] == null).ToList();
		}

		public List<int> NotLosingNumbers()
		{
			var numbers = AvailableNumbers();
			var notLosingNumbers = new List<int>();
			foreach (var i in numbers)
			{
				TakeNumber(i);
				if (!done)
					notLosingNumbers.Add(i);
				Undo();
			}
			return notLosingNumbers;
		} 

		public Game Clone()
		{
			throw new NotImplementedException();
		}
	}
}

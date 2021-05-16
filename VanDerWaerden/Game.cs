using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool extended;

        public Progression(int stride)
        {
            this.stride = stride;
        }

        public void ExtendBy(int number)
        {
            extended = false;
            if (this.Last() + stride == number)
            {
                Add(number);
                extended = true;
            }
            if (this.First() - stride == number)
            {
                Insert(0, number);
                extended = true;
            }
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
        public Player[] Board { get; set; }
        public Player first;
        public Player second;
        public Player active;
        public Player winner;
        public bool done;
        public int? LastChosen { get; set; }
        private int? PrevChosen { get; set; }

        public Player NotActive { get { if (active == first) return second; else return first; } }

		private Game()
		{
		}

        public Game(Configuration config, Player first, Player second)
        {
            n = config.n;
            k = config.k;
            Board = new Player[n];
            active = first;
            this.first = first;
            this.second = second;
            done = false;
            winner = null;
        }

        public int Play(bool verbose = false)
        {
            while (!done)
                Step(verbose);

            int result = 2; // draw
            if (winner != null)
                result = winner.id;
            if (verbose)
            {
                if (result < 2)
                {
                    Console.Write("And the winner is ");
                    Console.ForegroundColor = winner.color;
                    Console.WriteLine($"{winner.name}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Losing progression ");
                    Console.ForegroundColor = winner.id == 0 ? second.color : first.color;
                    Console.WriteLine($"{NotActive.progressions.Where(x => x.Count >= k).First()}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                }
                else
                    Console.WriteLine("DRAW!");
            }

            return result;
        }

        public void Step(bool verbose = false)
        {
            int chosen = active.ChooseNumber(this.Clone());
            TakeNumber(chosen);
            if (verbose)
            {
                PrintBoard(chosen);
                Console.Write("Press key...");
                Console.ReadLine();
            }
        }

		public void ForcedStep(int number)
		{
			active.TakeNumber(number);
			TakeNumber(number);
		}

        public void PrintBoard(int chosen)
        {
            for (int i = 0; i < n; i++)
            {
                Console.ForegroundColor = Board[i] == null ? ConsoleColor.White : Board[i].color;
                Console.Write($"{i} ");
            }
            Console.ForegroundColor = Board[chosen].color;
            Console.Write($" - {chosen} - ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void TakeNumber(int chosen)
        {
            PrevChosen = LastChosen;
            LastChosen = chosen;
            Board[chosen] = active;

            if (active.progressions.Any(x => x.Count >= k))
            {
                active = NotActive;
                winner = active; // Misère
                done = true;
                return;
            }

            active = NotActive;
            if (Board.All(x => x != null))
                done = true;
        }

        public List<int> AvailableNumbers()
        {
            return Enumerable.Range(0, Board.Length).Where(i => Board[i] == null).ToList();
        }

        public List<int> LosingNumbers()
        {
            List<int> numbers = AvailableNumbers();
            List<int> losingNumbers = new List<int>();
            foreach (int i in numbers)
            {
				var game = this.Clone();
                game.ForcedStep(i);
                if (game.done && game.winner != null && game.winner != active)
                    losingNumbers.Add(i);
            }
            return losingNumbers;
        }

        public Game Clone()
        {
			var game = new Game();
			game.first = this.first.Clone();
			game.second = this.second.Clone();
			game.active = this.active == this.first ? game.first : game.second;
			game.Board = (Player[])Board.Clone();
			game.done = this.done;
			if (this.winner != null)
				game.winner = this.winner == this.first ? first : second;
			game.n = this.n;
			game.k = this.k;
			return game;
        }
    }
}
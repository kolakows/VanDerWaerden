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
        public Player[] board;
        public Player first;
        public Player second;
        public Player active;
        public Player winner;
        public bool done;

        public int? lastChosen; // TODO: Why it was private, is there some good reason for that?

        public Player NotActive { get { if (active == first) return second; else return first; } }

        public Game(Configuration config, Player first, Player second)
        {
            n = config.n;
            k = config.k;
            board = new Player[n];
            active = first;
            this.first = first;
            this.second = second;
            done = false;
        }

        public int Play(bool verbose = false)
        {
            while (!done)
                Step(verbose);

            int result = 0;
            if (winner != null)
                result = winner == first ? 1 : 2;
            if (verbose)
            {
                if (NotActive.progressions.Any(x => x.Count >= k))
                {
                    Console.Write("And the winner is ");
                    Console.ForegroundColor = result == 1 ? ConsoleColor.Red : ConsoleColor.Blue;
                    Console.WriteLine($"{(result == 1 ? "first" : "second")}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Losing progression ");
                    Console.ForegroundColor = result == 1 ? ConsoleColor.Blue : ConsoleColor.Red;
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
            if (done)
                return;
            var chosen = active.ChooseNumber(this.Clone());
            TakeNumber(chosen);
            if (verbose)
            {
                for (int i = 0; i < this.n; i++)
                {
                    Console.ForegroundColor = board[i] == null ? ConsoleColor.White : board[i] == first ? ConsoleColor.Red : ConsoleColor.Blue;
                    Console.Write($"{i} ");
                }
                Console.ForegroundColor = board[chosen] == first ? ConsoleColor.Red : ConsoleColor.Blue;
                Console.Write($" - {chosen} - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Press key...");
                Console.ReadLine();

            }
        }

        public void TakeNumber(int chosen)
        {
            lastChosen = chosen;
            board[chosen] = active;
            active = NotActive;

            if (active.progressions.Any(x => x.Count >= k))
            {
                winner = active; // Misère
                done = true;
                return;
            }

            if (board.All(x => x != null))
                done = true;
        }

        // undo last move
        public void Undo()
        {
            if (lastChosen.HasValue)
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
            return new Game(new Configuration() { n = this.n, k = this.k }, first, second) { board = this.board };
        }
    }
}
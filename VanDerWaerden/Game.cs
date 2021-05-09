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
        public int? lastChosen;
        private int? prevChosen;

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

        public void PrintBoard(int chosen)
        {
            for (int i = 0; i < n; i++)
            {
                Console.ForegroundColor = board[i] == null ? ConsoleColor.White : board[i].color;
                Console.Write($"{i} ");
            }
            Console.ForegroundColor = board[chosen].color;
            Console.Write($" - {chosen} - ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void TakeNumber(int chosen)
        {
            prevChosen = lastChosen;
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
            if (lastChosen.HasValue)
            {
                board[lastChosen.Value] = null;
                active = NotActive;
                done = false;
                lastChosen = prevChosen;
                prevChosen = null;
                return;
            }
            throw new InvalidOperationException("There is no move to undo!");
        }

        public List<int> AvailableNumbers()
        {
            return Enumerable.Range(0, board.Length).Where(i => board[i] == null).ToList();
        }

        public List<int> LosingNumbers()
        {
            List<int> numbers = AvailableNumbers();
            List<int> losingNumbers = new List<int>();
            foreach (int i in numbers)
            {
                TakeNumber(i);
                if (done && winner != null && winner.id != active.id)
                    losingNumbers.Add(i);
                Undo();
            }
            return losingNumbers;
        }

        public Game Clone()
        {
            return new Game(new Configuration() { n = this.n, k = this.k }, first, second)
            {
                board = this.board,
                lastChosen = this.lastChosen,
                prevChosen = this.prevChosen,
                done = this.done,
                winner = this.winner,
                active = this.active
            };
        }
    }
}
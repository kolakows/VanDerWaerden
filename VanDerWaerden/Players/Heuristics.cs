using System;
using System.Collections.Generic;
using System.Linq;

namespace VanDerWaerden.Players
{
    public class HeuristicPlayer : Player
    {
        private double alpha;
        private double beta;
        private double gamma;
        public Random Random { get; set; }

        public HeuristicPlayer(Configuration config, int id, int seed, double alpha, double beta, double gamma) : base(config, id)
        {
            this.alpha = alpha;
            this.beta = beta;
            this.gamma = gamma;
            Random = new Random(seed);
        }

        protected override int Strategy(Game game)
        {
            var board = game.board;
            var freeIndices = Enumerable.Range(0, board.Length).Where(i => board[i] == null).ToList();
            List<int> chosen = new List<int>();
            double chosen_val = double.MaxValue;
            foreach (int m in freeIndices)
            {
                double f = alpha * h1(game, m) + beta * h2(game, m) + gamma * h3(game, m);
                if (f > chosen_val)
                {
                    chosen_val = f;
                    chosen.Clear();
                    chosen.Add(m);
                }
                else if (f == chosen_val)
                {
                    chosen.Add(m);
                }
            }
            return chosen[Random.Next(chosen.Count)];
        }

        // h1(m) = -q, gdzie q jest długością ciągu arytmetycznego powstałego poprzez pokolorowanie liczby m
        private int h1(Game game, int m)
        {
            game.TakeNumber(m);
            int q = 0;
            foreach (var p in progressions)
            {
                if (p.extended && p.Count > q)
                {
                    q = p.Count;
                }
            }
            game.Undo();
            return -q;
        }

        // h2(m) = 0.5, gdy liczba dozwolona dla przeciwnika, 0 wpp
        private double h2(Game game, int m)
        {
            double allowed = 0.5;
            game.active = game.NotActive; // switch sides, imagine the opponent
            game.TakeNumber(m);
            if (game.done && game.winner == this) allowed = 0; // the opponent loses the game
            //if (game.active.progressions.Any(p => p.extended)) allowed = 0; // at least one of opponent's progressions increased
            game.Undo();
            game.active = this;
            return allowed;
        }

        // h3(m) = 1/(3+p), gdzie p jest ilością liczb, które będą dla nas niedozwolone w kolejnym ruchu po wybraniu liczby m
        private double h3(Game game, int m)
        {
            Console.WriteLine($"This heuristic player: {id}.");
            game.TakeNumber(m);
            game.PrintBoard(m);
            var nextNumbers = game.AvailableNumbers();
            int p = 0;
            Console.WriteLine($"Game active player: {game.active.id}");
            foreach (int x in nextNumbers)
            {
                game.active = this;
                Console.WriteLine($"Game switched active player: {game.active.id}");
                game.TakeNumber(x);
                game.PrintBoard(x);
                if (game.done && game.winner != this)
                {
                    p++; // we lose after choosing m and x
                    Console.WriteLine($"Player lose after choosing x: {x}.");
                }
                game.Undo();
                Console.WriteLine($"Game active player after undo: {game.active.id}");
            }
            Console.WriteLine($"Game active player after x loop: {game.active.id}");
            game.active = this;
            Console.WriteLine($"Game switched active player: {game.active.id}");
            game.Undo();
            Console.WriteLine($"Game active player after undo: {game.active.id}");
            return 1.0 / (3.0 + p);
        }
    }
}
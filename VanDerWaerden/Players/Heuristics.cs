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
            var board = game.Board;
            var freeIndices = Enumerable.Range(0, board.Length).Where(i => board[i] == null).ToList();
            List<int> chosen = new List<int>();
            double chosen_val = double.MinValue;
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
            //Console.Write($"Heuristics possible choices: {chosen.Count}:");
            //foreach (int i in chosen) Console.Write($" {i}");
            Console.WriteLine();
            return chosen[Random.Next(chosen.Count)];
        }

        // h1(m) = -q, gdzie q jest długością ciągu arytmetycznego powstałego poprzez pokolorowanie liczby m
        private int h1(Game game, int m)
        {
            game = game.Clone();
            game.ForcedStep(m);
            var pClone = game.first == this ? game.first : game.second;
            int q = 0;
            foreach (var p in pClone.progressions)
            {
                if (p.extended && p.Count > q)
                {
                    q = p.Count;
                }
            }
            //Console.WriteLine($"h1({m}) = {-q}");
            return -q;
        }

        // h2(m) = 0.5, gdy liczba dozwolona dla przeciwnika, 0 wpp
        private double h2(Game game, int m)
        {
            double allowed = 0.5;
            game = game.Clone();
            game.active = game.NotActive; // switch sides, imagine the opponent
            game.ForcedStep(m);
            if (game.done && game.winner == this) allowed = 0; // the opponent loses the game
            //if (game.active.progressions.Any(p => p.extended)) allowed = 0; // at least one of opponent's progressions increased
            //Console.WriteLine($"h2({m}) = {allowed}");
            return allowed;
        }

        // h3(m) = 1/(3+p), gdzie p jest ilością liczb, które będą dla nas niedozwolone w kolejnym ruchu po wybraniu liczby m
        private double h3(Game game, int m)
        {
            game = game.Clone();
            game.ForcedStep(m);
            game.active = this;
            int p = game.LosingNumbers().Count;
            //Console.WriteLine($"h3({m}) = {1.0 / (3.0 + p)}, p = {p}");
            return 1.0 / (3.0 + p);
        }

        public override Player Clone()
        {
            var config = new Configuration()
            {
                k = this.k,
                n = this.n
            };
            var player = new HeuristicPlayer(config, id, Random.Next(), alpha, beta, gamma);
            CopyPlayerStatusTo(player);
            return player;
        }
    }
}
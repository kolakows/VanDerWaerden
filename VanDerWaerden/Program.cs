using System;
using System.Collections.Generic;
using VanDerWaerden.Players;

namespace VanDerWaerden
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 9, k = 3;
            Console.WriteLine("Welcome to the VAN DER WAERDEN MISÈRE game!");

            try
            {
                while (true)
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
                    if (int.TryParse(Console.ReadLine(), out int nn))
                        n = nn;
                    Console.WriteLine($"n = {n}");
                    Console.WriteLine("Choose value of k (default k = 3):");
                    if (int.TryParse(Console.ReadLine(), out int kk))
                        k = kk;
                    Console.WriteLine($"k = {k} ");

                    var config = new Configuration { n = n, k = k };

                    string default_player = "random";
                    List<string> player_str = new List<string> { "first", "second" };
                    List<int> seeds = new List<int> { 420, 123 };
                    List<Player> players = new List<Player>();
                    for (int i = 0; i < 2; i++)
                    {
                        Console.WriteLine($"Choose {player_str[i]} player (default: {default_player}).");
                        Console.WriteLine("Available types: random, MCTS, heuristic, special (only for second player, n=2k).");
                        string choice = Console.ReadLine();
                        if (choice == "") choice = default_player;
                        players.Add(GetPlayer(choice, config, i, seeds[i]));
                    }

                    //DEMO
                    if (option == 1)
                    {
                        var game = new Game(config, players[0], players[1]);
                        game.Play(verbose: true);
                    }
                    //TEST
                    else
                    {
                        Console.WriteLine("Type number of games:");
                        int numberOfGames = 0;
                        if (int.TryParse(Console.ReadLine(), out int number))
                            numberOfGames = number;

                        int[] results = new int[3];
                        for (int i = 0; i < numberOfGames; i++)
                        {
                            var game = new Game(config, players[0], players[1]);
                            results[game.Play(verbose: false)]++;
                        }

                        if (numberOfGames > 0)
                        {
                            Console.WriteLine($"First player: {results[1]} times");
                            Console.WriteLine($"Second player: {results[2]} times");
                            Console.WriteLine($"Draw: {results[0]} times");
                            Console.WriteLine();
                        }
                    }
                }
            }
            finally
            { // Keep console open also in case of an uncaught exception
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();

            }
        }

        static Player GetPlayer(string choice, Configuration config, int id, int seed)
        {
            if (choice == "random")
            {
                return new RandomPlayer(config, id, seed);
            }
            if (choice == "MCTS" || choice == "mcts")
            {
                return new MCTSRandomPlayer(config, id, seed, rolloutLimit: 5);
            }
            if (choice == "heuristic")
            {
                return new HeuristicPlayer(config, id, seed, alpha: 1.0, beta: 1.0, gamma: 1.0);
            }
            if (choice == "special")
            {
                if (config.n != 2 * config.k) throw new ArgumentException("Special player cannot play in a game where n != 2k!");
                if (id != 1) throw new ArgumentException("Only second player can be of 'special' type!");
                return new SpecialCasePlayer(config, id);
            }
            throw new ArgumentException("Unknown player type!");
        }
    }
}
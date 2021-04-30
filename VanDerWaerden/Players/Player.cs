using System;
using System.Collections.Generic;
using System.Linq;

namespace VanDerWaerden.Players
{
    public abstract class Player
    {
        public int n, k;
        public List<int> playerNumbers;
        public List<Progression> progressions;

        public Player(Configuration config)
        {
            n = config.n;
            k = config.k;
            playerNumbers = new List<int>();
            progressions = new List<Progression>();
        }

        protected abstract int Strategy(Game game);

        public int ChooseNumber(Game game)
        {
            var chosen = Strategy(game);
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
                if (number != chosen)
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
                        if (a.Last() == b.First())
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
}
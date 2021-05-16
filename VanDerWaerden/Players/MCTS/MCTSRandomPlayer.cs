using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanDerWaerden.Players
{
    public class MCTSRandomPlayer : MCTS.MCTS
    {

        public MCTSRandomPlayer(Configuration config, int id, int seed, int rolloutLimit) : base(config, id, seed, rolloutLimit)
        {
        }

        protected override MoveSelection MoveSelection { get => MoveSelection.BestScore; }

        public override TreeNode SelectNextNode(TreeNode treeNode)
        {
            var availableNodes = treeNode.Children.Where(x => x != null).ToList();
            return availableNodes[Generator.Next(availableNodes.Count)];
        }

        public override string ToString()
        {
            return $"MCTSRandomPlayer with id:{id}";
        }

        public override Player Clone()
        {
            var player = new MCTSRandomPlayer(Config, id, Generator.Next(), RolloutLimit);
            CopyPlayerStatusTo(player);
            return player;
        }

    }
}
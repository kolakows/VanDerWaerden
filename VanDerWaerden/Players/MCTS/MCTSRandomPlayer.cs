using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanDerWaerden.Players.MCTS
{
	public class MCTSRandomPlayer : MCTS
	{
		public MCTSRandomPlayer(Configuration config, int seed, int rolloutLimit) : base(config, seed, rolloutLimit)
		{
		}

		protected override MoveSelection MoveSelection { get => MoveSelection.BestScore; }

		public override TreeNode SelectNextNode(TreeNode treeNode)
		{
			throw new NotImplementedException();
		}

	}
}

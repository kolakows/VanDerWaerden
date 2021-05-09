using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VanDerWaerden;
using VanDerWaerden.Players;

namespace Tests
{
	[TestClass]
	public class MCTSTests
	{
		[TestMethod]
		public void MCTSRandomPlayer()
		{
			Configuration gameConfiguration = new Configuration()
			{
				k = 3,
				n = 9
			};

			var p1 = new MCTSRandomPlayer(config: gameConfiguration, id: 0, seed: 123, rolloutLimit: 10000);
			var p2 = new MCTSRandomPlayer(config: gameConfiguration, id: 1, seed: 420, rolloutLimit: 10000);

			var game = new Game(gameConfiguration, p1, p2);
			game.Play(true);

		}
	}
}

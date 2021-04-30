﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanDerWaerden.Players;

namespace VanDerWaerden.Players.MCTS
{
    public abstract class MCTS : Player
    {
        private Random Generator { get; set; }
        private TreeNode Root { get; set; }
        private int RolloutLimit { get; set; }
        private Configuration Config { get; set; }
        internal int Iterations { get; set; }
        abstract protected MoveSelection MoveSelection { get; }

        public MCTS(Configuration config, int seed, int rolloutLimit) : base(config)
        {
            Generator = new Random(seed);
            RolloutLimit = rolloutLimit;
        }

        private void Expand()
        {
            //find leaf node in tree
            var currNode = Root;
            while (currNode.AllActionsTested() && !currNode.Game.done)
            {
                currNode = SelectNextNode(currNode);
            }

            // take winning move if possible
            for (int i = 0; i < Config.n; i++)
            {
                // !!!!!!!!!!!! misere, we avoid done games
                if (currNode.Children[i] != null && currNode.Children[i].Game.done)
                {
                    currNode = currNode.Children[i];
                    currNode.PropagadeScoreUp(Score(currNode.Game));
                    return;
                }
            }

            // if there wasnt a winning move then

            var game = currNode.Game.Clone(); //create new child node
            var unvisitedChildren = game.AvailableNumbers();
            var chosen = unvisitedChildren[Generator.Next(unvisitedChildren.Count)];
            game.TakeNumber(chosen);

            var child = currNode.CreateChild(game);
            var rollout = Rollout(child);
            var score = Score(rollout);
            child.PropagadeScoreUp(score);
            currNode.Children[chosen] = child;
            currNode.ActionsTaken++;
        }

        private int Score(Game game)
        {
            if (game.done)
            {
                if (game.winner != null)
                    if (game.winner == Root.Game.active)
                        return 1;
                    else
                        return -1;
                return 0;
            }
            throw new InvalidOperationException();
        }

        private Game Rollout(TreeNode node)
        {
            var game = node.Game.Clone();
            while (!game.done)
            {
                MakeRandomMove(game);
            }
            return game;
        }

        private int MakeRandomMove(Game game)
        {
            var numbers = game.AvailableNumbers();
            var move = numbers[Generator.Next(numbers.Count)];
            game.TakeNumber(move);
            return move;
        }

        public abstract TreeNode SelectNextNode(TreeNode treeNode);

        protected override int Strategy(Game game)
        {
            //clean start every time
            Root = new TreeNode(game);
            Iterations = 0;
            while (Iterations < RolloutLimit)
            {
                Iterations++;
                Expand();
            }
            switch (MoveSelection)
            {
                case MoveSelection.MostVisited:
                    return MaxVisitAction();
                case MoveSelection.BestScore:
                    return BestScoreAction();
                default:
                    throw new NotImplementedException();
            }
        }

        //robust child https://ai.stackexchange.com/questions/16905/mcts-how-to-choose-the-final-action-from-the-root
        private int MaxVisitAction()
        {
            var visitMax = 0;
            var indexOfMax = 0;
            for (int i = 0; i < Config.n; i++)
            {
                if (Root.Children[i] != null && Root.Children[i].VisitedCount > visitMax)
                {
                    visitMax = Root.Children[i].VisitedCount;
                    indexOfMax = i;
                }
            }
            return indexOfMax;
        }

        //max child
        private int BestScoreAction()
        {
            var maxScore = double.MinValue;
            var indexOfMax = 0;
            for (int i = 0; i < Config.n; i++)
            {
                if (Root.Children[i] != null && Root.Children[i].MeanScore > maxScore)
                {
                    maxScore = Root.Children[i].MeanScore;
                    indexOfMax = i;
                }
            }
            return indexOfMax;
        }

        public void SetSeed(int seed)
        {
            Generator = new Random(seed);
        }

    }
}
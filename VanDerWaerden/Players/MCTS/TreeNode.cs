using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanDerWaerden
{
    public class TreeNode
    {
        internal Game Game { get; set; }
        internal TreeNode Parent { get; set; }
        internal TreeNode[] Children { get; set; }
        public int ActionsTaken { get; set; }
        public int VisitedCount { get; set; }
        public double CumulativeScore { get; set; }
        public double CumulativeSquaredScore { get; set; }
        public double MeanScore
        {
            get
            {
                return CumulativeScore / VisitedCount;
            }
        }

        public double Var
        {
            get
            {
                return CumulativeSquaredScore / VisitedCount;
            }
        }

        public TreeNode(Game game)
        {
            Game = game;
            Children = new TreeNode[game.n];

            for (int i = 0; i < game.n; i++)
            {
                if (game.Board[i] != null)
                    ActionsTaken++;
            }
        }

        public bool AllActionsTested()
        {
            if (ActionsTaken > Game.n)
                throw new OverflowException("There can't be more actions taken than there is numbers in game");
            return ActionsTaken == Game.n;
        }

        public void PropagadeScoreUp(int score)
        {
            var node = this;
            while (node.Parent != null)
            {
                node.VisitedCount++;
                node.CumulativeScore += score;
                //node.UpdateNodeStats(score);
                score = -score;
                node = node.Parent;
            }
        }

        public TreeNode CreateChild(Game game)
        {
            var node = new TreeNode(game)
            {
                Parent = this
            };


            return node;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{MeanScore.ToString("0.00")} ");
            foreach (var child in Children)
            {
                if (child == null)
                    sb.Append("null ");
                else
                    sb.Append($"{child.VisitedCount} ");
            }
            return sb.ToString();
        }

        public string PrintPretty(int index = 0, string indent = "", bool last = false, StringBuilder text = null)
        {
            if (text == null)
                text = new StringBuilder();

            text.Append(indent);
            if (last)
            {
                text.Append("\\-");
                indent += "  ";
            }
            else
            {
                text.Append("|-");
                indent += "| ";
            }
            if (Parent != null)
                text.AppendLine($"Take {index + 1}, Total score: {CumulativeScore}, Mean score: {MeanScore}");

            int lastChild = 0;
            for (int i = 0; i < Children.Length; i++)
                if (Children[i] != null)
                    lastChild = i;
            for (int i = 0; i < Children.Length; i++)
                if (Children[i] != null)
                    Children[i].PrintPretty(i, indent, i == lastChild, text);
            return text.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckersGame
{
    internal class UCTNode : ICloneable
    {
        public int Visits { get; set; }
        public int Score { get; set; }
        public List<UCTNode> Children { get; set; }
        public int PlayerId { get; set; }
        public Move Move { get; set; }

        public UCTNode(Move move, int player)
        {
            Visits = 1;
            Score = 0;
            Children = new List<UCTNode>();
            PlayerId = player;
            Move = (Move)move.Clone();
        }

        public int getOpponent()
        {
            return PlayerId == 1 ? 2 : 1;
        }

        public int playFor(Game game)
        {
            return game.move(Move);
        }

        public void expand(Game game)
        {
            List<Move> moves = game.getMoveList();

            for (var i = 0; i < moves.Count; i++)
            {
                var child = new UCTNode(moves[i], game.getPlayer());
                Children.Add(child);
            }
        }

        public double getUCB(double coeff)
        {
            return (double)Score / Visits + Math.Sqrt(coeff / Visits);
        }

        public UCTNode findBestChild()
        {
            double coeff = 20 * Math.Log(Visits);
            double bestScore = -1e9;
            UCTNode bestChild = null;
            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i].getUCB(coeff) > bestScore)
                {
                    bestScore = Children[i].getUCB(coeff);
                    bestChild = Children[i];
                }
            }
            return bestChild;
        }

        public object Clone()
        {
            UCTNode node = new UCTNode(Move, PlayerId);
            node.Visits = Visits;
            node.Score = Score;
            node.Children = new List<UCTNode>();
            for(int i = 0; i < Children.Count; i++)
            {
                node.Children.Add((UCTNode)Children[i].Clone());
            }
            return node;
        }
    }

    internal class UCTBot
    {
        public UCTBot(Game game, int level)
        {
            _root = new UCTNode(new Move((-1, -1), (-1, -1)), 2);
            _original = game;
            _maturityThreshold = 200;
            switch (level)
            {
                case 1:
                    _maxTime = 1;
                    break;
                case 2:
                    _maxTime = 100;
                    break;
                case 3:
                    _maxTime = 500;
                    break;
                case 4:
                    _maxTime = 1000;
                    break;
                case 5:
                    _maxTime = 6000;
                    break;
            }

            _root.expand(game);
            _history = new Dictionary<int, UCTNode>();
            _history[0] = _root;
        }

        private int playout(Game game)
        {
            int nmoves = 0;
            while (++nmoves < 60)
            {
                int result = game.moveRandom();
                if (result != 0)
                    return result;
            }
            int p1 = game.getScore(true), p2 = game.getScore(false);
            if (p1 > p2)
                return 1;
            if (p1 < p2)
                return 2;
            return 0;
        }

        private void run()
        {
            int depth = 1;
            Game board = (Game)_original.Clone();
            var node = _root;
            int winner = 0;

            while (true)
            {
                if (node.Children.Count == 0)
                {
                    if (node.Visits >= _maturityThreshold)
                    {
                        node.expand(board);
                        if (node.Children.Count == 0)
                        {
                            winner = node.getOpponent();
                            _history[depth++] = node;
                            break;
                        }
                        continue;
                    }
                    winner = playout(board);
                    break;
                }
                node = node.findBestChild();
                _history[depth++] = node;
                if (node.playFor(board) != 0)
                {
                    winner = board.getPlayer();
                    break;
                }
            }

            for (var i = 0; i < depth; i++)
            {
                node = _history[i];
                node.Visits++;
                if (winner != node.PlayerId)
                    node.Score += 1;
                else if (winner != 0)
                    node.Score -= 1;
            }
        }

        public (Move, long) suggest()
        {
            if (_maxTime == 1)
            {
                var moves = _original.getMoveList();
                if (moves.Count != 0) {
                    int ind = (int)(DateTime.UtcNow.Ticks % moves.Count);
                    return (moves[ind], 1);
                }
            }
            long TotalPlayouts = 0, start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long elapsed = 0;

            while (true)
            {
                int i = 0;
                for (; i < 500; i++)
                {
                    run();
                    elapsed = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - start;
                    if (elapsed >= _maxTime)
                        break;
                }
                TotalPlayouts += i + 1;
                if (elapsed >= _maxTime)
                    break;
            }

            UCTNode bestChild = null;
            double bestScore = -1e9;
            for (var i = 0; i < _root.Children.Count; i++)
            {
                var child = _root.Children[i];
                if (child.Visits > bestScore)
                {
                    bestChild = child;
                    bestScore = child.Visits;
                }
            }

            return (bestChild.Move, TotalPlayouts);
        }

        private UCTNode _root;
        private int _maturityThreshold, _maxTime;
        private Dictionary<int, UCTNode> _history;
        private Game _original;
    }
}

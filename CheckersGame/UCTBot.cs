using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame
{
    internal class UCTNode
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
            Move = move;
        }

        public int getOpponent()
        {
            return PlayerId == 1 ? 2 : 1;
        }

        public int playFor(Game game)
        {
            return (game.Clone() as Game).move(Move);
        }

        public void expand(Game game)
        {
            MoveList moves = game.getMoveList();

            for (var i = 0; i < moves.moves.Count; i++)
            {
                var child = new UCTNode(moves.moves[i], game.getPlayer());
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
    }

    internal class UCTBot
    {
        public UCTBot(Game game, int maxTime)
        {
            _root = new UCTNode(null, 0);
            _original = (Game)game.Clone();
            _maturityThreshold = 200;
            _maxTime = maxTime;

            _root.expand(game);
            _history = new List<UCTNode>();
            _history.Add(_root);
        }

        private int playout(Game game)
        {
            int nmoves = 0;
            while (++nmoves < 60)
            {
                try
                {
                    MoveList moveList = game.getMoveList();
                    int result = game.move(moveList.moves[new Random((int)DateTime.Now.Ticks).Next() % moveList.moves.Count]);
                    if (result != 0)
                        return result;
                }
                catch (Exception) { }
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
                            if (_history.Count <= depth)
                                _history.Add(node);
                            else
                                _history[depth++] = node;
                            break;
                        }
                        continue;
                    }
                    winner = playout((Game)board.Clone());
                    break;
                }
                node = node.findBestChild();
                if (_history.Count <= depth)
                    _history.Add(node);
                else
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
                if (winner == node.PlayerId)
                    node.Score += 1;
                else if (winner != 0)
                    node.Score -= 1;
            }
        }

        public Move suggest()
        {
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
            int bestScore = (int)-1e9;
            for (var i = 0; i < _root.Children.Count; i++)
            {
                var child = _root.Children[i];
                if (child.Visits > bestScore)
                {
                    bestChild = child;
                    bestScore = child.Visits;
                }
            }

            return bestChild.Move;
        }

        private UCTNode _root;
        private int _maturityThreshold, _maxTime;
        private List<UCTNode> _history;
        private Game _original;
    }
}

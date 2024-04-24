using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame
{
    internal class Game : ICloneable
    {
        public Game(int rows, int columns)
        {
            if (rows < 4 || columns < 4)
            {
                throw new Exception("Слишком маленькое поле. Минимальный размер 4x4.");
            }

            if (rows % 2 != 0)
            {
                throw new Exception("Количество строк должно быть четным числом.");
            }

            _rows = rows;
            _columns = columns;
            _figureCount.Item1 = _figureCount.Item2 = (_rows - 2) / 2 * (_columns / 2);
            _kingCount.Item1 = _kingCount.Item2 = 0;
            _board = new List<List<Cell>>();

            for (int i = 0; i < (_rows - 2) / 2; i++)
            {
                _board.Add(new List<Cell>());
                bool was = i % 2 != 0;

                for (int j = 0; j < _columns; j++)
                {
                    _board[i].Add(new Cell(i, j, was ? 2 : 0));
                    was = !was;
                }
            }

            for (int i = (_rows - 2) / 2; i < (_rows - 2) / 2 + 2; i++)
            {
                _board.Add(new List<Cell>());
                for (int j = 0; j < _columns; j++)
                {
                    _board[i].Add(new Cell(i, j, 0));
                }
            }

            for (int i = (_rows - 2) / 2 + 2; i < _rows; i++)
            {
                _board.Add(new List<Cell>());
                bool was = (i - ((_rows - 2) / 2 + 2)) % 2 == 0;

                for (int j = 0; j < _columns; j++)
                {
                    _board[i].Add(new Cell(i, j, was ? 1 : 0));
                    was = !was;
                }
            }
        }

        public int getPlayer()
        {
            return _current_player;
        }

        private void remove(int row, int column)
        {
            if (_board[row][column].playerId == 0)
                throw new Exception("Невозможно убрать фигуру с пустой клетки.");

            int player = _board[row][column].playerId;
            _board[row][column].playerId = 0;

            if (player == 1)
            {
                _figureCount.Item1--;
                if (_board[row][column].isKing)
                    _kingCount.Item1--;
            }
            else
            {
                _figureCount.Item2--;
                if (_board[row][column].isKing)
                    _kingCount.Item2--;
            }
            _board[row][column].isKing = false;
        }

        public int getScore(bool is_first)
        {
            return is_first ? _figureCount.Item1 + _kingCount.Item1 * 5 : _figureCount.Item2 + _kingCount.Item2 * 5;
        }

        public List<List<Cell>> getBoard()
        {
            return _board;
        }

        public int move(Move move)
        {
            validateMove(move);
            if (makeMove(move))
                return _current_player;
            var moves = getMoveList();
            if (moves.moves.Count == 0)
                return getOppenent(_current_player);
            return 0;
        }

        public void validateMove(Move move)
        {
            if (_board[move.from.Item1][move.from.Item2].playerId != _current_player)
                throw new Exception("Выбранная фигура вам не принадлежит.");
            if (_board[move.to.Item1][move.to.Item2].playerId != 0)
                throw new Exception("Выбранная клетка не является пустой.");
            if (move.from == move.to)
                throw new Exception("Игрок обязан сделать ход.");

            int row_delta = Math.Abs(move.from.Item1 - move.to.Item1), column_delta = Math.Abs(move.from.Item2 - move.to.Item2);
            if (row_delta != column_delta)
                throw new Exception("Движение может осуществляться только по диагонали.");

            if (!_board[move.from.Item1][move.from.Item2].isKing)
            {
                if (_current_player == 1 && move.from.Item1 < move.to.Item1 || _current_player == 2 && move.from.Item1 > move.to.Item1)
                    throw new Exception("Только дамки могут ходить назад.");
            }

            if (row_delta > 1)
            {
                if (row_delta > 2)
                    throw new Exception("Вы не можете делать прыжок больше, чем на 2 клетки.");
                if (!validJump(move))
                    throw new Exception("Вы можете переместиться только на 2 клетки, убрав другую фигуру.");
                return;
            }

            if (_is_multiTurn)
                throw new Exception("Вы обязаны срубить фигуру, если это возможно.");

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    if (_board[row][col].playerId != _current_player) continue;
                    if (canCapture(row, col))
                        throw new Exception("Вы обязаны срубить фигуру, если это возможно.");
                }
            }
        }

        private bool makeMove(Move move)
        {
            Cell figure = (Cell)_board[move.from.Item1][move.from.Item2].Clone();
            _board[move.from.Item1][move.from.Item2].playerId = 0;
            _board[move.from.Item1][move.from.Item2].isKing = false;

            if (!figure.isKing && (_current_player == 1 && move.to.Item1 == 0 || _current_player == 2 && move.to.Item1 == _rows - 1))
            {
                figure.isKing = true;
                if (_current_player == 1)
                    _kingCount.Item1++;
                else
                    _kingCount.Item2++;
            }

            _board[move.to.Item1][move.to.Item2] = figure;

            if (Math.Abs(move.to.Item1 - move.from.Item1) == 2)
            {
                var middleRow = (move.from.Item1 + move.to.Item1) / 2;
                var middleCol = (move.from.Item2 + move.to.Item2) / 2;
                remove(middleRow, middleCol);
                if (_current_player == 1 && _figureCount.Item1 == 0 || _current_player == 2 && _figureCount.Item2 == 0)
                    return true;

                _is_multiTurn = canCapture(move.to.Item1, move.to.Item2);
                if (_is_multiTurn) return false;
            }

            _current_player = getOppenent(_current_player);
            return false;
        }

        public MoveList getMoveList()
        {
            MoveList list = new MoveList();
            bool canCapture = false;

            for (int fromRow = 0; fromRow < _rows; fromRow++)
            {
                for (int fromCol = 0; fromCol < _columns; fromCol++)
                {
                    if (_board[fromRow][fromCol].playerId == 0) continue;
                    bool isKing = _board[fromRow][fromCol].isKing;

                    if ((isKing || _current_player == 2) && fromRow != _rows - 1)
                    {
                        if (fromCol != 0)
                        {
                            if (_board[fromRow + 1][fromCol - 1].playerId == 0 && !canCapture)
                                list.add(new Move((fromRow, fromCol), (fromRow + 1, fromCol - 1)));
                        }
                        if (fromCol != _columns - 1)
                        {
                            if (_board[fromRow + 1][fromCol + 1].playerId == 0 && !canCapture)
                                list.add(new Move((fromRow, fromCol), (fromRow + 1, fromCol + 1)));
                        }

                        if (fromRow != _rows - 2)
                        {
                            if (fromCol != 0 && _board[fromRow + 1][fromCol - 1].playerId == getOppenent(_current_player) && fromCol >= 2 &&
                                _board[fromRow + 2][fromCol - 2].playerId == 0)
                            {
                                if (!canCapture)
                                {
                                    list.clear();
                                    canCapture = true;
                                }
                                list.add(new Move((fromRow, fromCol), (fromRow + 2, fromCol - 2)));
                            }
                            if (fromCol != _columns - 1 && _board[fromRow + 1][fromCol + 1].playerId == getOppenent(_current_player) && fromCol < _columns - 2 &&
                                 _board[fromRow + 2][fromCol + 2].playerId == 0)
                            {
                                if (!canCapture)
                                {
                                    list.clear();
                                    canCapture = true;
                                }
                                list.add(new Move((fromRow, fromCol), (fromRow + 2, fromCol + 2)));
                            }
                        }
                    }

                    if ((isKing || _current_player == 1) && fromRow != 0)
                    {
                        if (fromCol != 0)
                        {
                            if (_board[fromRow - 1][fromCol - 1].playerId == 0 && !canCapture)
                                list.add(new Move((fromRow, fromCol), (fromRow - 1, fromCol - 1)));
                        }
                        if (fromCol != _columns - 1)
                        {
                            if (_board[fromRow - 1][fromCol + 1].playerId == 0 && !canCapture)
                                list.add(new Move((fromRow, fromCol), (fromRow - 1, fromCol + 1)));
                        }

                        if (fromRow >= 2)
                        {
                            if (fromCol != 0 && _board[fromRow - 1][fromCol - 1].playerId == getOppenent(_current_player) && fromCol >= 2 &&
                                _board[fromRow - 2][fromCol - 2].playerId == 0)
                            {
                                if (!canCapture)
                                {
                                    list.clear();
                                    canCapture = true;
                                }
                                list.add(new Move((fromRow, fromCol), (fromRow - 2, fromCol - 2)));
                            }
                            if (fromCol != _columns - 1 && _board[fromRow - 1][fromCol + 1].playerId == getOppenent(_current_player) && fromCol < _columns - 2 &&
                                 _board[fromRow - 2][fromCol + 2].playerId == 0)
                            {
                                if (!canCapture)
                                {
                                    list.clear();
                                    canCapture = true;
                                }
                                list.add(new Move((fromRow, fromCol), (fromRow - 2, fromCol + 2)));
                            }
                        }
                    }
                }
            }

            return list;
        }

        public object Clone() => MemberwiseClone();

        private int getOppenent(int player)
        {
            return player == 1 ? 2 : 1;
        }

        private bool validJump(Move move)
        {
            var middleRow = (move.from.Item1 + move.to.Item1) / 2;
            var middleCol = (move.from.Item2 + move.to.Item2) / 2;
            return _board[middleRow][middleCol].playerId == getOppenent(_current_player);
        }

        private bool validHop(int rowFrom, int colFrom, int rowTo, int colTo)
        {
            if (_board[rowTo][colTo].playerId != 0)
                return false;
            return validJump(new Move((rowFrom, colFrom), (rowTo, colTo)));
        }

        private bool canCapture(int row, int col)
        {
            bool isKing = _board[row][col].isKing;
            return (row >= 2 && col >= 2 && (isKing || _current_player == 1) && validHop(row, col, row - 2, col - 2)) ||
                  (row >= 2 && col < _columns - 2 && (isKing || _current_player == 1) && validHop(row, col, row - 2, col + 2)) ||
                  (row < _rows - 2 && col >= 2 && (isKing || _current_player == 2) && validHop(row, col, row + 2, col - 2)) ||
                  (row < _rows - 2 && col < _columns - 2 && (isKing || _current_player == 2) && validHop(row, col, row + 2, col + 2));
        }

        private int _rows, _columns;
        private ValueTuple<int, int> _figureCount, _kingCount;
        private List<List<Cell>> _board;
        private bool _is_multiTurn = false;
        private int _current_player = 1;
    }
}

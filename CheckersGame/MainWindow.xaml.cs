using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CheckersGame
{
    public partial class MainWindow : Window
    {
        private Game game = new Game(8, 8);
        private (int, int) moveFigure = (-1, -1);
        private List<(int, int)> lastMoves = new List<(int, int)>();
        private int prevPlayer = 2;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool is_white = true;
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    BoardCell cell = new BoardCell();
                    cell.IsWhite = is_white;
                    cell.BorderThickness = new Thickness(0, 0, 0, 0);
                    cell.Tag = (i, j);
                    cell.PreviewMouseDown += (se, ev) =>
                    {
                        (int, int) pos = ((int, int))(se as BoardCell).Tag;
                        if (game.getBoard()[pos.Item1][pos.Item2].playerId != game.getPlayer())
                        {
                            if (game.getBoard()[pos.Item1][pos.Item2].playerId != 0 || moveFigure == (-1, -1)) return;
                            try
                            {
                                game.move(new Move(moveFigure, pos));
                                lastMoves.Add(pos);
                                lastMoves.Add(moveFigure);
                                (se as BoardCell).Activate(true);
                                (Board.Children[8 * moveFigure.Item1 + moveFigure.Item2] as BoardCell).Activate(true);
                                changePlayer();
                            } catch (Exception ex) { }
                            return;
                        }
                        moveFigure = pos;
                        for (int toRow = 0; toRow < 8; toRow++)
                        {
                            for (int toCol = 0; toCol < 8; toCol++)
                            {
                                bool can_make = false;
                                try
                                {
                                    game.validateMove(new Move(pos, (toRow, toCol)));
                                    can_make = true;
                                } catch(Exception) {}

                                (Board.Children[8 * toRow + toCol] as BoardCell).SetMove(can_make);
                            }
                        }
                    };

                    Board.Children.Add(cell);
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    is_white = !is_white;
                }
                is_white = !is_white;
            }

            changePlayer();
        }

        private void Cell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void updateBoard()
        {
            var board = game.getBoard();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int type = -1;
                    if (board[i][j].playerId == 2) type = 0b00;
                    else if (board[i][j].playerId == 1) type = 0b10;
                    else type = -1;

                    if (board[i][j].isKing) type |= 0b01;

                    (Board.Children[8 * i + j] as BoardCell).CellType = type;
                    (Board.Children[8 * i + j] as BoardCell).SetMove(false);
                    if (!lastMoves.Contains((i, j)))
                        (Board.Children[8 * i + j] as BoardCell).Activate(false);
                    (Board.Children[8 * i + j] as BoardCell).Cursor = game.getPlayer() == board[i][j].playerId ? Cursors.Hand : Cursors.Arrow;
                }
            }
        }

        private void changePlayer()
        {
            moveFigure = (-1, -1);
            updateBoard();
            if (prevPlayer != game.getPlayer())
            {
                lastMoves.Clear();
                prevPlayer = game.getPlayer();
            }
        }
    }
}

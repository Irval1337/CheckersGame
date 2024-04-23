using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CheckersGame
{
    /// <summary>
    /// Логика взаимодействия для BoardCell.xaml
    /// </summary>
    public partial class BoardCell : UserControl
    {
        private bool _is_white;
        private int _cell_type;

        public bool IsWhite
        {
            get { return _is_white; }
            set
            {
                _is_white = value;
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_is_white ? "#eeeed5" : "#4d5391"));
            }
        }

        public int CellType
        {
            get { return _cell_type; }
            set
            {
                _cell_type = value;
                if (_cell_type == -1)
                {
                    figure.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7175a7"));
                    figure.Height = figure.Width = 40;
                    return;
                }
                ImageBrush brush = new ImageBrush();
                if ((_cell_type >> 1) == 1)
                {
                    if ((_cell_type & 1) == 1)
                        brush.ImageSource = new BitmapImage(ResourceAccessor.Get("Resources/red-checkers-piece-king.png"));
                    else
                        brush.ImageSource = new BitmapImage(ResourceAccessor.Get("Resources/red-checkers-piece-small.png"));
                }
                else
                {
                    if ((_cell_type & 1) == 1)
                        brush.ImageSource = new BitmapImage(ResourceAccessor.Get("Resources/black-checkers-piece-king.png"));
                    else
                        brush.ImageSource = new BitmapImage(ResourceAccessor.Get("Resources/black-checkers-piece-small.png"));
                }

                figure.Fill = brush;
                figure.Height = figure.Width = 60;
                figure.Visibility = Visibility.Visible;
            }
        }

        public void SetMove(bool move)
        {
            if (CellType != -1) return;
            if (move)
            {
                this.Cursor = Cursors.Hand;
                figure.Visibility = Visibility.Visible;
            }
            else
            {
                this.Cursor = Cursors.Arrow;
                figure.Visibility = Visibility.Collapsed;
            }
        }

        public void Activate(bool val)
        {
            btn.BorderThickness = new Thickness(val ? 4 : 0);
            if (val)
                figure.Margin = new Thickness(-4, -4, 0, 0);
            else
                figure.Margin = new Thickness(0);
        }

        public BoardCell()
        {
            InitializeComponent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для ScoreLine.xaml
    /// </summary>
    public partial class ScoreLine : UserControl
    {
        public void SetScore(int figures, int score, bool is_white)
        {
            stackPanel.Children.Clear();
            for(int i = 0; i < figures; i++)
            {
                BoardCell cell = new BoardCell();
                cell.IsWhite = is_white;
                cell.CellType = is_white ? 0b10 : 0b00;
                cell.BorderThickness = new Thickness(0, 0, 0, 0);
                cell.Width = cell.Height = 62;
                cell.SetTransparent();
                if (i != 0)
                    cell.Margin = new Thickness(-30, 0, 0, 0);
                stackPanel.Children.Add(cell);
            }
            if (score > 0)
            {
                Label scoreLabel = new Label();
                scoreLabel.Content = "+" + score.ToString();
                scoreLabel.VerticalAlignment = VerticalAlignment.Center;
                scoreLabel.FontSize = 20;
                scoreLabel.Padding = new Thickness(0);
                stackPanel.Children.Add(scoreLabel);
            }
        }

        public ScoreLine()
        {
            InitializeComponent();
        }
    }
}

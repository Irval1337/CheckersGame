using System.Windows;
using System.Windows.Controls;

namespace CheckersGame
{
    /// <summary>
    /// Логика взаимодействия для PlayerPanel.xaml
    /// </summary>
    public partial class PlayerPanel : UserControl
    {
        public PlayerPanel()
        {
            InitializeComponent();
        }

        public void SetMove(bool is_bot)
        {
            moveLabel1.Visibility = moveLabel2.Visibility = Visibility.Collapsed;
            if (is_bot)
                moveLabel1.Visibility = Visibility.Visible;
            else
                moveLabel2.Visibility = Visibility.Visible;
        }
    }
}

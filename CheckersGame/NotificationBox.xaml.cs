using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CheckersGame
{
    /// <summary>
    /// Логика взаимодействия для NotificationBox.xaml
    /// </summary>
    public partial class NotificationBox : UserControl
    {
        public NotificationBox()
        {
            InitializeComponent();
        }

        private void RestartBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public void Win()
        {
            GameText.Text = "Вы сделали это! РобоТакса побеждена! Попробуйте повысить уровень сложности ИИ, чтобы сделать игру более увлекательной.";
            ResultText.Content = "Вы победили!";
        }
    }
}

using System.Windows;
using System.Windows.Input;

namespace Binary_Engine.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TearableTabControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                Top = 0;
            }

            if (WindowState == WindowState.Normal)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            }
        }

        private void TearableTabControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                e.Handled = true;
            }
        }
    }
}

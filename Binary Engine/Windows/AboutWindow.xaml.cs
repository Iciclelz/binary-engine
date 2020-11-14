using System.Windows;
using System.Windows.Input;

namespace Binary_Engine.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void aboutWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}

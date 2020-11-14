using Binary_Engine.ViewModel;
using Binary_Engine.Windows;
using System.Windows;

namespace Binary_Engine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new MainWindow()
            {
                DataContext = MainWindowViewModel.Initialize()

            }.Show();
        }
    }
}

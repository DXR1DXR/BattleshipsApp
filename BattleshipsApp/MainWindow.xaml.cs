using BattleshipsApp.Model;
using BattleshipsApp.Pages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BattleshipsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //var tempBoats = new List<Boat>() { new Boat() { X1 = 0, X2 = 0, Y1 = 0, Y2 = 3 }, new Boat() { X1 = 2, X2 = 3, Y1 = 1, Y2 = 1} };
            MainFrame.Navigate(new LoginPage());
            //MainFrame.Navigate(new SetShipsPage());

        }
    }
}
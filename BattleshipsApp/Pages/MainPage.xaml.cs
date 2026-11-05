using System;
using System.Collections.Generic;
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

namespace BattleshipsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            
        }

        private void ProfileBT_Click(object sender, RoutedEventArgs e)
        {
            InFrame.Navigate(new ProfilePage(NavigationService));
        }

        private void FindGameBT_Click(object sender, RoutedEventArgs e)
        {
            InFrame.Navigate(new FindGamePage(NavigationService));
        }

        private void CreateGameBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreateGamePage());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InFrame.Navigate(new FindGamePage(NavigationService));
        }
    }
}

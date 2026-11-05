using BattleshipsApp.Classes;
using BattleshipsApp.Model;
using BattleshipsApp.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace BattleshipsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private ProfilePage ProfilePage { get; set; }
        public ProfileViewModel(ProfilePage page)
        {
            ProfilePage = page;
            StartUpdate();
        }
        private Player? _player = null;
        public Player? Player { get => _player; set { _player = value; OnPropertyChanged("Player"); } }
        private async Task UpdateAsync()
        {
            try
            {
                Player = await ApiHandler.Me();
            }
            catch (ConnectionException)
            {
                MessageBox.Show("Соединение с сервером потеряно");
                await ProfilePage.ExitAsync();
                return;
            }
            catch (AuthorizationException)
            {
                MessageBox.Show("Сессия истекла");
                await ProfilePage.ExitAsync();
                return;
            }
            
        }
        public void StartUpdate()
        {
            Task.Run(UpdateAsync);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
    public partial class ProfilePage : Page
    {
        private ProfileViewModel ViewModel { get; set; }
        public NavigationService MainNavigationService { get; set; }
        public ProfilePage(NavigationService mainNavigationService)
        {
            InitializeComponent();
            MainNavigationService = mainNavigationService;
            ViewModel = new ProfileViewModel(this);
            DataContext = ViewModel;
        }

        private void ChangeNameBT_Click(object sender, RoutedEventArgs e)
        {
            if (new ChangeNameWindow(MainNavigationService).ShowDialog() == true)
            {
                ViewModel.StartUpdate();
            }
        }

        private void UpdateBT_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartUpdate();
        }

        private void ChangePasswordBT_Click(object sender, RoutedEventArgs e)
        {
            if (new ChangePasswordWindow(MainNavigationService).ShowDialog() == true)
            {
                ViewModel.StartUpdate();
            }
        }
        public void Exit()
        {
            MainNavigationService.Navigate(new LoginPage());
        }
        public async Task ExitAsync()
        {
            Dispatcher.Invoke(Exit);
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }
    }
}

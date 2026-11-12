using BattleshipsApp.Classes;
using BattleshipsApp.Model;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для FindGamePage.xaml
    /// </summary>
    public class FindGameViewModel
    {
        private Dispatcher Dispatcher;
        public FindGameViewModel(Dispatcher dispatcher)
        {
            Games = new ObservableCollection<GameDTO>();
            Dispatcher = dispatcher;
            Task.Run(Update);
        }
        private async void Update()
        {
            var list = await ApiHandler.GetGames();
            Dispatcher.Invoke(() => list.ForEach(Games.Add));
        }
        public ObservableCollection<GameDTO> Games { get; set; }
        public async void AddGame(GameDTO gameDTO)
        {
            Dispatcher.Invoke(() => Games.Add(gameDTO));
        }
        public async void RemoveGame(GameDTO game)
        {
            Dispatcher.Invoke(() => Games.Remove(Games.FirstOrDefault(x => x.CreatorNickname == game.CreatorNickname) ?? new GameDTO()));
        }
    }
    public partial class FindGamePage : Page
    {
        public FindGameViewModel ViewModel { get; set; }
        private NavigationService MainNavigationService;
        public FindGamePage(NavigationService navigationService)
        {
            InitializeComponent();
            MainNavigationService = navigationService;
            ViewModel = new FindGameViewModel(Dispatcher);
            DataContext = ViewModel;
            var connection = new HubConnectionBuilder()
                .WithUrl(ApiHandler.BaseSignalUrl)
                .Build();
            connection.On<GameDTO>("NewGame", ViewModel.AddGame);
            connection.On<GameDTO>("RemovedGame", game => ViewModel.RemoveGame(game));
            connection.StartAsync();
        }

        private async void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var nickName = ((sender as ListViewItem)!.DataContext as GameDTO)!.CreatorNickname;
            await ApiHandler.ConnectToGame(nickName);
            MainNavigationService.Navigate(new SetShipsPage(false));
        }
    }
}

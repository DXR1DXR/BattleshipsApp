using BattleshipsApp.Classes;
using BattleshipsApp.Model;
using Microsoft.AspNetCore.SignalR.Client;
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
    /// Логика взаимодействия для CreateGamePage.xaml
    /// </summary>
    public partial class CreateGamePage : Page
    {
        public CreateGamePage()
        {
            InitializeComponent();
            Connection = new HubConnectionBuilder()
                .WithUrl(ApiHandler.BaseSignalUrl + $"?login={Uri.EscapeDataString(ApiHandler.Login ?? "")}&path=CreateGame")
                .Build();
        }

        private async void CancelBT_Click(object sender, RoutedEventArgs e)
        {
            await Connection.DisposeAsync();
            NavigationService.Navigate(new MainPage());
        }
        HubConnection Connection;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ApiHandler.CreateGame();
            Connection.On<string>("OpponentConnected", async data =>
            {
                Dispatcher.Invoke(()=> NavigationService.Navigate(new SetShipsPage(true)));
                await Connection.DisposeAsync();
            });
            await Connection.StartAsync();
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            await Connection.DisposeAsync();
        }
    }
}

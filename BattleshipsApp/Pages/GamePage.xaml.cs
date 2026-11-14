using BattleshipsApp.Classes;
using BattleshipsApp.Model;
using Microsoft.AspNetCore.SignalR.Client;
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

namespace BattleshipsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для GamePage.xaml
    /// </summary>
    public class GameCellViewModel : INotifyPropertyChanged
    {
        public GameCellViewModel(GamePage page)
        {
            _IsPressed = false;
            Page = page;
        }
        GamePage Page;
        bool _IsPressed;
        public bool IsPressed
        {
            get => _IsPressed; set
            {
                _IsPressed = value;
                OnPropertyChanged("Background");
            }
        }
        public SolidColorBrush Background
        {
            get =>
                (SolidColorBrush)(IsPressed ? Page.FindResource("SecondaryDarkBrush") : Page.FindResource("PrimaryHueLightBrush"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class GamePageViewModel : INotifyPropertyChanged
    {
        List<Border> SelfCells;
        List<Border> OpponentCells;
        GamePage Page;
        bool isMyTurn;
        public bool IsMyTurn { get => isMyTurn; set
            {
                isMyTurn = value;
                OnPropertyChanged("SelfVisibility");
                OnPropertyChanged("OpponentVisibility");
            }
        }
        int timeLeft;
        public int TimeLeft { get => timeLeft; set
            {
                timeLeft = value;
                OnPropertyChanged(nameof(TimeLeft));
            }
        }
        public bool SelfVisibility { get => isMyTurn;}
        public bool OpponentVisibility { get => !isMyTurn;}
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public GamePageViewModel(GamePage page, List<Boat> selfBoats, bool isHost)
        {
            Page = page;
            isMyTurn = isHost;
            timeLeft = 30;
            SelfCells = new List<Border>();
            OpponentCells = new List<Border>();
            FillGrid(ref page.SelfShipsGrid, selfBoats);
            FillGrid(ref page.OpponentShipsGrid, null);
        }
        public void FillGrid(ref Grid grid, List<Boat>? boats)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var border = new Border();
                    border.Style = (Style)Page.FindResource("ClickableBorder");
                    border.Width = 10;
                    border.Height = 10;
                    border.Margin = new Thickness(0.25);
                    border.CornerRadius = new CornerRadius(1);
                    SelfCells.Add(border);
                    var vm = new GameCellViewModel(Page);
                    if (boats is not null)
                        vm.IsPressed = boats.Any(x => x.Y1 <= j && j <= x.Y2 && x.X1 <= i && i <= x.X2);
                    border.DataContext = vm;
                    border.SetBinding(Border.BackgroundProperty, new Binding("Background"));
                    if (boats is null)
                        border.MouseLeftButtonUp += Border_MouseLeftButtonUp;
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    grid.Children.Add(border);
                }
            }
        }
        private async void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = (sender as Border)!;
            var row = Grid.GetRow(border);
            var column = Grid.GetColumn(border);
            GameCellViewModel cell = ((sender as Border)!.DataContext as GameCellViewModel)!;
            //cell.IsPressed = !cell.IsPressed;
            //OnPropertyChanged("IsReadyActive");
            try
            {
                var result = await ApiHandler.Shoot(row, column);
                border.Background = result.IsHit ? Brushes.Red : (SolidColorBrush)Page.FindResource("PrimaryHueSuperLightBrush");
                (sender as Border).Style = (Style)Page.FindResource("InactiveBorder");
                (sender as Border).MouseLeftButtonUp -= Border_MouseLeftButtonUp;
            }
            catch (ApiException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

    }
    public partial class GamePage : Page
    {
        GamePageViewModel? ViewModel;
        HubConnection Connection;
        List<Boat> SelfBoats;
        bool IsHost;
        public GamePage(bool isHost)
        {
            InitializeComponent();
            IsHost = isHost;
            Connection = new HubConnectionBuilder()
                .WithUrl(ApiHandler.BaseSignalUrl + $"?login={Uri.EscapeDataString(ApiHandler.Login ?? "")}&path=Game")
                .Build();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SelfBoats = await ApiHandler.GetBoats();
            ViewModel = new GamePageViewModel(this, SelfBoats, IsHost);
            DataContext = ViewModel;
            Connection.On<int>("OpponentMissed", x =>
            {
                //Dispatcher.Invoke(() => TimerTB.Text = x.ToString());
            });
            Connection.On<int>("OpponentHit", x =>
            {
                //Dispatcher.Invoke(() => TimerTB.Text = x.ToString());
            });
            Connection.On<int>("GameTimer", x =>
            {
                ViewModel.TimeLeft = x;
            });
            Connection.On<string>("GameWon", x =>
            {
                MessageBox.Show("Вы победили");
                Dispatcher.Invoke(() => NavigationService.Navigate(new MainPage()));
                Connection.DisposeAsync();
            });
            Connection.On<string>("GameLost", x =>
            {
                MessageBox.Show("Вы проиграли");
                Dispatcher.Invoke(() => NavigationService.Navigate(new MainPage()));
                Connection.DisposeAsync();
            });
            await Connection.StartAsync();
        }
    }
}

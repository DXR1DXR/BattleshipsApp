using BattleshipsApp.Classes;
using BattleshipsApp.Model;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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
    /// Логика взаимодействия для SetShipsPage.xaml
    /// </summary>
    public class CellViewModel : INotifyPropertyChanged
    {
        public CellViewModel(SetShipsPage page)
        {
            _IsPressed = false;
            Page = page;
        }
        SetShipsPage Page;
        bool _IsPressed;
        public bool IsPressed { get => _IsPressed; set
            {
                _IsPressed = value;
                OnPropertyChanged("Background");
            }
        }
        public SolidColorBrush Background {  get => 
                (SolidColorBrush)(IsPressed ? Page.FindResource("SecondaryDarkBrush") : Page.FindResource("PrimaryHueLightBrush"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null) 
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class SetShipsViewModel : INotifyPropertyChanged
    {
        List<Border> Cells;
        SetShipsPage Page;
        public bool IsReadyActive { get => IsValid(); }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public SetShipsViewModel(SetShipsPage page)
        {
            Page = page;
            Cells = new List<Border>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var border = new Border();
                    border.Style = (Style)page.FindResource("ClickableBorder");
                    border.Width = 10;
                    border.Height = 10;
                    border.Margin = new Thickness(0.25);
                    border.CornerRadius = new CornerRadius(1);
                    Cells.Add(border);
                    border.DataContext = new CellViewModel(Page);
                    border.SetBinding(Border.BackgroundProperty, new Binding("Background"));
                    border.MouseLeftButtonUp += Border_MouseLeftButtonUp;
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    page.ShipsGrid.Children.Add(border);
                }
            }
        }
        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CellViewModel cell = ((sender as Border)!.DataContext as CellViewModel)!;
            cell.IsPressed = !cell.IsPressed;
            OnPropertyChanged("IsReadyActive");
        }
        public bool IsValid()
        {
            var boats = GetBoats();
            if (boats == null)
                return false;

            int[] expected = new int[5];
            expected[1] = 4;
            expected[2] = 3;
            expected[3] = 2;
            expected[4] = 1;

            int[] actual = new int[5];
            int[,] owner = new int[10, 10];

            // 0 means unoccupied; boat index + 1 identifies its boat.
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    owner[y, x] = 0;

            for (int i = 0; i < boats.Count; i++)
            {
                Boat boat = boats[i];
                if (boat == null)
                    return false;

                int minX = Math.Min(boat.X1, boat.X2);
                int maxX = Math.Max(boat.X1, boat.X2);
                int minY = Math.Min(boat.Y1, boat.Y2);
                int maxY = Math.Max(boat.Y1, boat.Y2);

                // Boat must be inside the 10x10 board.
                if (minX < 0 || maxX >= 10 || minY < 0 || maxY >= 10)
                    return false;

                // A boat must be horizontal, vertical, or a single cell.
                if (minX != maxX && minY != maxY)
                    return false;

                int length = Math.Max(maxX - minX, maxY - minY) + 1;
                if (length > 4)
                    return false;

                actual[length]++;

                // Mark every cell occupied by this boat.
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        if (owner[y, x] != 0)
                            return false; // Overlap.

                        owner[y, x] = i + 1;
                    }
                }
            }

            for (int length = 1; length <= 4; length++)
            {
                if (actual[length] != expected[length])
                    return false;
            }

            // Different boats must not touch, including diagonally.
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (owner[y, x] == 0)
                        continue;

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int ny = y + dy;
                            int nx = x + dx;

                            if (ny < 0 || ny >= 10 || nx < 0 || nx >= 10)
                                continue;

                            if (owner[ny, nx] != 0 && owner[ny, nx] != owner[y, x])
                                return false;
                        }
                    }
                }
            }

            return true;
        }
        public List<Boat> GetBoats()
        {
            var field = new int[10, 10];
            Cells.ForEach(x => field[Grid.GetColumn(x), Grid.GetRow(x)] = (x.DataContext as CellViewModel)!.IsPressed ? 1 : 0);
            int rows = field.GetLength(0);
            int cols = field.GetLength(1);
            var boats = new List<Boat>();
            var visited = new bool[rows, cols];

            int[] dy = { -1, 1, 0, 0 };
            int[] dx = { 0, 0, -1, 1 };

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (field[y, x] != 1 || visited[y, x])
                        continue;

                    var queue = new Queue<(int Y, int X)>();
                    queue.Enqueue((y, x));
                    visited[y, x] = true;

                    int minY = y, maxY = y;
                    int minX = x, maxX = x;

                    while (queue.Count > 0)
                    {
                        var (currentY, currentX) = queue.Dequeue();

                        minY = System.Math.Min(minY, currentY);
                        maxY = System.Math.Max(maxY, currentY);
                        minX = System.Math.Min(minX, currentX);
                        maxX = System.Math.Max(maxX, currentX);

                        for (int i = 0; i < 4; i++)
                        {
                            int nextY = currentY + dy[i];
                            int nextX = currentX + dx[i];

                            if (nextY < 0 || nextY >= rows ||
                                nextX < 0 || nextX >= cols ||
                                visited[nextY, nextX] ||
                                field[nextY, nextX] != 1)
                                continue;

                            visited[nextY, nextX] = true;
                            queue.Enqueue((nextY, nextX));
                        }
                    }

                    boats.Add(new Boat
                    {
                        X1 = minX,
                        X2 = maxX,
                        Y1 = minY,
                        Y2 = maxY
                    });
                }
            }

            return boats;
        }

    }
    public partial class SetShipsPage : Page
    {
        SetShipsViewModel? ViewModel;
        HubConnection Connection;
        public SetShipsPage()
        {
            InitializeComponent();
            Connection = new HubConnectionBuilder()
                .WithUrl(ApiHandler.BaseSignalUrl + $"?login={Uri.EscapeDataString(ApiHandler.Login ?? "")}&path=SetShips")
                .Build();
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new SetShipsViewModel(this);
            DataContext = ViewModel;
            Connection.On<int>("ShipsSet", x =>
            {
                Dispatcher.Invoke(() => TimerTB.Text = x.ToString());
            });
        }

        
        
        private async void ReadyBT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ApiHandler.PostBoats(ViewModel!.GetBoats());
                MessageBox.Show("Успешный успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

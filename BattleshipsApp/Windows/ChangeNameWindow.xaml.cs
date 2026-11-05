using BattleshipsApp.Classes;
using BattleshipsApp.Pages;
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

namespace BattleshipsApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangeNameWindow.xaml
    /// </summary>
    public partial class ChangeNameWindow : Window
    {
        public NavigationService MainNavigationService { get; set; }
        public ChangeNameWindow(NavigationService mainNavigationService)
        {
            InitializeComponent();
            MainNavigationService = mainNavigationService;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTB.Text.Trim();
            if (name.Length > 20 || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите правильное имя");
                return;
            }
            try
            {
                await ApiHandler.ChangeName(name);
            }
            catch (ConnectionException)
            {
                MessageBox.Show("Соединение с сервером потеряно");
                MainNavigationService.Navigate(new LoginPage());
                this.DialogResult = false;
                return;
            }
            catch (AuthorizationException)
            {
                MessageBox.Show("Сессия истекла");
                MainNavigationService.Navigate(new LoginPage());
                this.DialogResult = false;
                return;
            }
            catch (ApiException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            this.DialogResult = true;
        }
    }
}

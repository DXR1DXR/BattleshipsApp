using BattleshipsApp.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        public LoginPage(string login)
        {
            InitializeComponent();
            LoginTB.Text = login;
        }

        private async void EnterBT_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTB.Text;
            var password = PasswordPB.Password;
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }
            try
            {
                await ApiHandler.Authorize(login, password);
            }
            catch (ConnectionException)
            {
                MessageBox.Show("Ошибка соединения с сервером");
                return;
            }
            catch (ApiException ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            NavigationService.Navigate(new MainPage());
        }

        private void RegisterBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}

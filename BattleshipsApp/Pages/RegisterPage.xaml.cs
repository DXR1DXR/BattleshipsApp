using BattleshipsApp.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }
        private async void EnterBT_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTB.Text;
            var password = PasswordPB.Password;
            var password2 = Password2PB.Password;
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
            if (string.IsNullOrEmpty(password2))
            {
                MessageBox.Show("Введите повтор пароля");
                return;
            }
            if (password != password2)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }
            try
            {
                await ApiHandler.Register(login, password);
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
            MessageBox.Show("Учпех регистрации");
            NavigationService.Navigate(new LoginPage(login));
        }

        private void Authorize_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}

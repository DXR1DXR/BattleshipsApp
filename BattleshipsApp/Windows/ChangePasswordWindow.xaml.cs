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
    /// Логика взаимодействия для ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        public NavigationService MainNavigationService { get; set; }
        public ChangePasswordWindow(NavigationService mainNavigationService)
        {
            InitializeComponent();
            MainNavigationService = mainNavigationService;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var oldPassword = OldPasswordPB.Password;
            var newPassword = NewPasswordPB.Password;
            var newPassword2 = NewPasswordPB2.Password;
            if (oldPassword.IsEmpty() || newPassword.IsEmpty() || newPassword2.IsEmpty())
            {
                MessageBox.Show("Введите все значения");
                return;
            }
            if (newPassword != newPassword2)
            {
                MessageBox.Show("Пароли должны совпадать");
                return;
            }
            try
            {
                await ApiHandler.ChangePassword(oldPassword, newPassword);
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

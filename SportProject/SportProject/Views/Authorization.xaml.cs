using SportProject.Model;
using SportProject.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SportProject.Views
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
            App.Entities = new Model.Entities();
            
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGuest_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            ProductCatalog catalog = new ProductCatalog();
            this.Hide();
            catalog.ShowDialog();
            this.ShowDialog();
        }

        private void btnAuthorize_Click(object sender, RoutedEventArgs e)
        {
            List<Users> users = App.Entities.Users.ToList();

            string login = tbxLogin.Text, password = tbxPassword.Text;
            
            if (login.Length == 0 || login.Length == 0)
            {
                MessageBox.Show("Поля ввода логина или пароля пустые", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            App.CurrentUser = users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (App.CurrentUser == null)
            {
                MessageBox.Show("Неправильный логин или пароль", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Добрый день, {App.CurrentUser.FullName}! Вы успешно авторизованы. Ваша роль {App.CurrentUser.Roles.Name}", "Успешная авторизации", MessageBoxButton.OK, MessageBoxImage.Information);
                ProductCatalog catalog = new ProductCatalog();
                this.Hide();
                catalog.ShowDialog();
                this.ShowDialog();
            }

        }



    }
}

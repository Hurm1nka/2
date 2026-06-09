using SportProject.Model;
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
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders : Window
    {
        public Orders()
        {
            InitializeComponent();
            getOrders();
        }

        private void getOrders()
        {

            List<Model.Orders> orders = App.Entities.Orders.ToList();



            lbOrders.ItemsSource = orders;

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser.RoleId == 1)
            {

                Model.Orders order = new Model.Orders();
                AddEditOrder addEditOrder = new AddEditOrder();
                addEditOrder.getOrder("add", order);
                this.Hide();
                addEditOrder.ShowDialog();
                getOrders();
                this.ShowDialog();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lbOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (App.CurrentUser.RoleId == 1)
            {
                var order = lbOrders.SelectedItem as Model.Orders;
                if (order == null) { return; }

                AddEditOrder addEditOrder = new AddEditOrder();
                addEditOrder.getOrder("edit", order);
                this.Hide();
                addEditOrder.ShowDialog();
                getOrders();
                this.ShowDialog();
            }
        }
    }
}

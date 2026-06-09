using SportProject.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Логика взаимодействия для EditOrder.xaml
    /// </summary>
    public partial class AddEditOrder : Window
    {
        public AddEditOrder()
        {
            InitializeComponent();
        

            List<Model.OrderStatuses> statuses = App.Entities.OrderStatuses.ToList();
            List<Users> users = App.Entities.Users.Where(u => u.RoleId == 3).ToList();
            
            cbStatus.ItemsSource = statuses;
            cbStatus.DisplayMemberPath = "Name";
            cbStatus.SelectedValuePath = "Id";
          
            cbClient.ItemsSource = users;
            cbClient.DisplayMemberPath = "FullName";
            cbClient.SelectedValuePath = "Id";
        
        }

        private string _type = string.Empty;
        private Model.Orders _currentOrder = new Model.Orders();


        private int generateCode()
        {
            int code;

            Random random = new Random();

            do
            {
                code = random.Next(0, 10000);
            }
            while (App.Entities.Orders.Select(o => o.Code).Contains(code));
           
            return code;
        }

        public void getOrder(string type, Model.Orders order)
        {
            
            _type = type;
            _currentOrder = order;
            tbxCode.Text = order.Code.ToString();
            cbStatus.SelectedValue = order.OrderStatusId;
            dpDateOrder.SelectedDate = order.OrderDate;
            dpDateGet.SelectedDate = order.DeliveryDate;
            cbClient.SelectedValue = order.ClientId;

            cbStatus.SelectedIndex = 0;
            cbClient.SelectedIndex = 0;

            switch (type)
            {
                case "add":
                    tbTitle.Text = "Добавление заказа";
                    btnAddEdit.Content = "Добавить заказ";
                    tbxCode.Text = generateCode().ToString();
                    break;

                case "edit":
                    tbTitle.Text = "Изменение заказа";
                    btnAddEdit.Content = "Изменить заказ";
                    tbxPoint.Text = order.Points.Name;
                    break;
            }

        }

        private void btnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(tbxCode.Text, out var code)) { _currentOrder.Code = code; }
            else { MessageBox.Show("Код указан неверно!", "Ошибка заполнения товара", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            _currentOrder.OrderStatuses = cbStatus.SelectedItem as OrderStatuses;

            var point = App.Entities.Points.FirstOrDefault(p => p.Name == tbxPoint.Text);
            if(point == null)
            {   
                point = new Model.Points();
                point.Name = tbxPoint.Text;
                App.Entities.Points.Add(point);
                App.Entities.SaveChanges();
            }
            _currentOrder.Points = point;
            _currentOrder.OrderDate = (DateTime)dpDateOrder.SelectedDate;
            _currentOrder.DeliveryDate = ((DateTime)dpDateGet.SelectedDate).Date;
            _currentOrder.ClientId = (int)cbClient.SelectedValue;

            switch (_type)
            {
                case "add":
                    App.Entities.Orders.Add(_currentOrder);
                    try { App.Entities.SaveChanges(); }
                    catch (DbEntityValidationException ex) { MessageBox.Show($"{ex}", "Ошибка при сохранении данных в БД", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    MessageBox.Show("Заказ успешно добавлен", "Изменение заказа", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                case "edit":
                    try { App.Entities.SaveChanges(); }
                    catch (DbEntityValidationException ex) { MessageBox.Show($"{ex}", "Ошибка при сохранении данных в БД", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    MessageBox.Show("Заказ успешно изменен", "Изменение заказа", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

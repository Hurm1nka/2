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
    /// Логика взаимодействия для Catalog.xaml
    /// </summary>
    public partial class ProductCatalog : Window
    {
        public ProductCatalog()
        {
            InitializeComponent();


            if (App.CurrentUser != null)
            {
                tbUsername.Text = App.CurrentUser.FullName;
                roleDetection();
            }

            // Заполнение комбо-бокса с поставщиками
            List<Providers> providers = App.Entities.Providers.ToList();

            Providers allProviders = new Providers();

            allProviders.Name = "Все поставщики";
            providers.Insert(0, allProviders);

            cbProviders.ItemsSource = providers;
            cbProviders.DisplayMemberPath = "Name";
            cbProviders.SelectedValuePath = "Id";
            cbProviders.SelectedIndex = 0;

            getProducts();
        }

        // Настройка видимости элементов интерфейса в зависимости от роли пользователя
        private void roleDetection()
        {
            if (App.CurrentUser.RoleId < 3)
            {
                rowFooter.Height = new GridLength(1, GridUnitType.Star);
                gridFilters.Visibility = Visibility.Visible;
                gridFooter.Visibility = Visibility.Visible;

            }

            if (App.CurrentUser.RoleId == 1)
            {
                btnDelete.Visibility = Visibility.Visible;
                btnAdd.Visibility = Visibility.Visible;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Products product = new Products();
            AddEditProduct addEditProduct = new AddEditProduct();
            addEditProduct.getProduct("add", product);
            this.Hide();
            addEditProduct.ShowDialog();
            getProducts();
            this.ShowDialog();
        }


        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            Orders orders = new Orders();
            this.Hide();
            orders.ShowDialog();
            this.ShowDialog();
        }

        private void getProducts()
        {
            List<Products> products = App.Entities.Products.ToList();

            string search = tbxSearch.Text;


            // Реализация поиска по нескольким столбцам
            if (search.Length > 0)
            {

                products = products.Where(p => p.ProductsNames.Name.Contains(search)
                                             || p.Categories.Name.Contains(search)
                                             || p.Manufacturers.Name.Contains(search)
                                             || p.Providers.Name.Contains(search)
                                             || p.Description.Contains(search)).ToList();
            }

            // Сортировка по возрастанию/убыванию
            if (rbUp.IsChecked == true) { products = products.OrderBy(p => p.Count).ToList(); }
            if (rbDown.IsChecked == true) { products = products.OrderByDescending(p => p.Count).ToList(); }


            // Выбор поставщиков из комбо-бокса
            if (cbProviders.SelectedIndex > 0)
            {
                products = products.Where(p => p.ProviderId == (int)cbProviders.SelectedValue).ToList();
            }

            lbProducts.ItemsSource = products;
        }

        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            getProducts();
        }

        private void rbUp_Checked(object sender, RoutedEventArgs e)
        {
            getProducts();
        }

        private void rbDown_Checked(object sender, RoutedEventArgs e)
        {
            getProducts();
        }


        private void cbProviders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getProducts();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser.RoleId == 1)
            {
                if (lbProducts.SelectedItem != null)
                {
                    var currentProduct = lbProducts.SelectedItem as Products;



                    if (App.Entities.ProductsOrders.Select(o => o.ProductId).Contains(currentProduct.Id))
                    {
                        MessageBox.Show($"Товар не может быть удален, так как он содержится в заказах", "Ошибка удаления товара", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var resultQuest = MessageBox.Show("Вы уверены что хотите удалить это товар?", "Удаление товара", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (resultQuest == MessageBoxResult.Yes)
                    {
                        App.Entities.Products.Remove(currentProduct);
                        App.Entities.SaveChanges();
                        getProducts();
                        MessageBox.Show($"Товар успешно удален", "Удаление товара", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {

                    MessageBox.Show("Не выбрана строка для удаления! Выберите строку которую вы собираетесь удалить", "Ошибка удаления товара", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void lbProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (App.CurrentUser.RoleId == 1)
            {
                try
                {
                    var product = lbProducts.SelectedItem as Products;
                    if (product == null) { return; }
                    AddEditProduct addEditProduct = new AddEditProduct();
                    addEditProduct.getProduct("edit", product);
                    this.Hide();
                    addEditProduct.ShowDialog();
                    getProducts();
                    this.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка {ex}", "Ошибка при открытии товара", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

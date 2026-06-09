using Microsoft.Win32;
using SportProject.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
    /// Логика взаимодействия для AddProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Window
    {

        private string _currentType = string.Empty;
        private Products _currentProduct = new Products();

        public AddEditProduct()
        {
            InitializeComponent();

 
            List<Categories> categories = App.Entities.Categories.ToList();
            List<Manufacturers> manufacturers = App.Entities.Manufacturers.ToList();
            List<Providers> providers = App.Entities.Providers.ToList();
            List<Units> units = App.Entities.Units.ToList();


            cbCategory.ItemsSource = categories;
            cbCategory.DisplayMemberPath = "Name";
            cbCategory.SelectedValuePath = "Id";
            cbCategory.SelectedIndex = 0;

            cbManufacturer.ItemsSource = manufacturers;
            cbManufacturer.DisplayMemberPath = "Name";
            cbManufacturer.SelectedValuePath = "Id";
            cbManufacturer.SelectedIndex = 0;

            cbProvider.ItemsSource = providers;
            cbProvider.DisplayMemberPath = "Name";
            cbProvider.SelectedValuePath = "Id";
            cbProvider.SelectedIndex = 0;

            cbUnit.ItemsSource = units;
            cbUnit.DisplayMemberPath = "Name";
            cbUnit.SelectedValuePath = "Id";
            cbUnit.SelectedIndex = 0;

        }

   
        private string generateArticule()
        {
            string articule = string.Empty;
            List<string> articules = App.Entities.Products.Select(p => p.Articule).ToList();

            do
            {
                articule = Guid.NewGuid().ToString().Substring(0,6).ToUpper();
            }
            while (articules.Contains(articule));

            return articule;
        }
        
        
        // Получение продукта и заполнение полей
        public void getProduct(string type, Products product)
        {

           _currentType = type;
           _currentProduct = product;
            List<Products> products = App.Entities.Products.ToList();
           
            switch (type)
            {
                case "add":
                    tbTitle.Text = "Добавление товара";
                    btnAddEdit.Content = "Добавить товар";
                    btnAddPhoto.Content = "Добавить фото";
                    tbxArticule.Text = generateArticule();
                    break;

                case "edit":
                    tbTitle.Text = "Изменение товара";
                    btnAddEdit.Content = "Изменить товар";
                    btnAddPhoto.Content = "Изменить фото";
                    tbID.Visibility = Visibility.Visible;
                    tbxID.Visibility = Visibility.Visible;

                    if (product != null)
                    {
                        tbxID.Text = product.Id.ToString();
                        tbxArticule.Text = product.Articule;
                        tbxName.Text = product.ProductsNames.Name;
                        cbCategory.SelectedValue = product.CategoryId;
                        tbxDescription.Text = product.Description;
                        cbManufacturer.SelectedValue = product.ManufacturerId;
                        cbProvider.SelectedValue = product.ProviderId;
                        tbxCost.Text = product.Cost.ToString();
                        cbUnit.SelectedValue = product.Units.Id;
                        tbxCount.Text = product.Count.ToString();
                        tbxDiscount.Text = product.Discount.ToString();
                        tbImgPreview.Text = product.Image;

                        if(!(tbImgPreview.Text == ""))
                        {
                            BitmapImage image = new BitmapImage(new Uri(System.IO.Path.GetFullPath($"ProductImages/{tbImgPreview.Text}")));
                            ImgPreview.Source = image;
                        }
                    }

                    break;
            }
        }

        // Добавление или изменение товара
        private void btnAddEdit_Click(object sender, RoutedEventArgs e)
        {  

            ProductCatalog catalog = new ProductCatalog();

            var productName = App.Entities.ProductsNames.Where(pn => pn.Name == tbxName.Text).FirstOrDefault();
            if (productName == null) { 
                productName = new ProductsNames();
                productName.Name = tbxName.Text;
                App.Entities.ProductsNames.Add(productName);
                try { App.Entities.SaveChanges(); }
                catch (DbEntityValidationException ex) { MessageBox.Show($"{ex}", "Ошибка при сохранении данных в БД", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            }

            _currentProduct.Articule = tbxArticule.Text;
            _currentProduct.ProductsNames = productName;
            _currentProduct.Categories = cbCategory.SelectedItem as Categories;
            _currentProduct.Description = tbxDescription.Text;
            _currentProduct.Manufacturers = cbManufacturer.SelectedItem as Manufacturers;
            _currentProduct.Providers = cbProvider.SelectedItem as Providers;
            _currentProduct.Image = tbImgPreview.Text;
            if (decimal.TryParse(tbxCost.Text, out decimal cost)) { _currentProduct.Cost = cost; }
            else { MessageBox.Show("Цена указана неверно!", "Ошибка заполнения товара", MessageBoxButton.OK, MessageBoxImage.Information);  return; }
            _currentProduct.Units = cbUnit.SelectedItem as Units;
            if(int.TryParse(tbxCount.Text, out int count)) { _currentProduct.Count = count; }
            else{ MessageBox.Show("Количество товаров указано неверно!", "Ошибка заполнения товара", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            if (int.TryParse(tbxDiscount.Text, out int discount)) { _currentProduct.Discount = discount; }
            else { MessageBox.Show("Скидка заполнена неверно!", "Ошибка заполнения товара", MessageBoxButton.OK, MessageBoxImage.Information); return; }

            switch (_currentType)
            {
                case "add":
                    App.Entities.Products.Add(_currentProduct);
                    try { App.Entities.SaveChanges(); }
                    catch (DbEntityValidationException ex) { MessageBox.Show($"{ex}", "Ошибка при сохранении данных в БД", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    MessageBox.Show("Товар успешно добавлен", "Добавление товара", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                case "edit":
                    try { App.Entities.SaveChanges(); } 
                    catch (DbEntityValidationException ex) { MessageBox.Show($"{ex}", "Ошибка при сохранении данных в БД", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    MessageBox.Show("Товар успешно изменен", "Изменение товара", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }

        }

      
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png";

            if(dialog.ShowDialog() == true)
            {
                BitmapImage img = new BitmapImage(new Uri(dialog.FileName));

                if(img.PixelWidth > 300 || img.PixelHeight > 200)
                {
                    MessageBox.Show("Размер не должен превышать 300x200 пикселей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(dialog.FileName);

                string currentAppFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductImages");

                System.IO.Directory.CreateDirectory(currentAppFolder);

                string destPath = System.IO.Path.Combine(currentAppFolder, fileName);

                System.IO.File.Copy(dialog.FileName, destPath, true);

                tbImgPreview.Text = fileName;
                ImgPreview.Source = img;
            }  
        }
    }
}

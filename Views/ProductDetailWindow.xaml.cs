using Microsoft.Win32;
using Shoes.Models;
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
using System.Collections.Generic;
using System.IO;

namespace Shoes.Views
{
    public partial class ProductDetailWindow : Window
    {
        Product? editProduct;
        string filePath;
        string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "image");
        public ProductDetailWindow(Product? selectedProduct = null)
        {
            InitializeComponent();
            editProduct = selectedProduct;

            categoryComboBox.ItemsSource = DbControl.GetProductCategories();
            manufacturerComboBox.ItemsSource = DbControl.GetManufacturers();
            supplierComboBox.ItemsSource = DbControl.GetSuppliers();
            measurementComboBox.ItemsSource = DbControl.GetMesurmentrs();

            if (editProduct != null)
            {
                Title = "Редактирование товара";
                title.Text = "Редактирование товара";
                idTextBox.Text = "" + editProduct.Id;
                articleTextBox.Text = editProduct.Article;
                nameTextBox.Text = editProduct.Name;
                descriptionTextBox.Text = editProduct.Description;
                priceTextBox.Text = editProduct.Price.ToString();
                countTextBox.Text = editProduct.Count.ToString();
                discountTextBox.Text = editProduct.Discount.ToString();

                categoryComboBox.SelectedValue = editProduct.ProductCategoryId;
                manufacturerComboBox.SelectedValue = editProduct.ManufacturerId;
                supplierComboBox.SelectedValue = editProduct.SupplierId;
                measurementComboBox.SelectedValue = editProduct.MesurmentId;

                if (!string.IsNullOrEmpty(editProduct.Photo))
                {
                    imagePrint.Source = new BitmapImage(new Uri(System.IO.Path.Combine(folderPath, editProduct.Photo)));
                }
            }
            else
            {
                idBlock.Visibility = Visibility.Collapsed;
                idTextBox.Visibility = Visibility.Collapsed;
                deleteButton.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(priceTextBox.Text, out int price) ||
                !int.TryParse(countTextBox.Text, out int count) ||
                !int.TryParse(discountTextBox.Text, out int discount))
            {
                MessageBox.Show("Поля Цена, Количество и Скидка должны быть числовыми!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (categoryComboBox.SelectedItem == null || manufacturerComboBox.SelectedItem == null ||
                supplierComboBox.SelectedItem == null || measurementComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля со списками!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string fileName = editProduct != null ? editProduct.Photo : null;
            if (!string.IsNullOrEmpty(filePath))
            {
                string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "image");
                fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(filePath);
                string fullName = System.IO.Path.Combine(folderPath, fileName);
                File.Copy(filePath, fullName, true);
            }

            if (editProduct == null)
            {
                Product p = new Product()
                {
                    Article = articleTextBox.Text,
                    Name = nameTextBox.Text,
                    Description = descriptionTextBox.Text,
                    Photo = System.IO.Path.GetFileName(fileName),
                    ProductCategoryId = ((ProductCategory)categoryComboBox.SelectedItem).Id,
                    ManufacturerId = ((Manufacturer)manufacturerComboBox.SelectedItem).Id,
                    SupplierId = ((Supplier)supplierComboBox.SelectedItem).Id,
                    Price = Convert.ToInt32(priceTextBox.Text),
                    MesurmentId = ((Mesurment)measurementComboBox.SelectedItem).Id,
                    Count = Convert.ToInt32(countTextBox.Text),
                    Discount = Convert.ToInt32(discountTextBox.Text)
                };
                DbControl.AddProduct(p);
            }
            else
            {
                editProduct.Article = articleTextBox.Text;
                editProduct.Name = nameTextBox.Text;
                editProduct.Description = descriptionTextBox.Text;
                editProduct.Photo = fileName;
                editProduct.ProductCategory = (ProductCategory)categoryComboBox.SelectedItem;
                editProduct.Manufacturer = (Manufacturer)manufacturerComboBox.SelectedItem;
                editProduct.Supplier = (Supplier)supplierComboBox.SelectedItem;
                editProduct.Price = price;
                editProduct.Mesurment = (Mesurment)measurementComboBox.SelectedItem;
                editProduct.Count = count;
                editProduct.Discount = discount;

                DbControl.UpdateProduct(editProduct);
            }
            ((ProductsListWindow)this.Owner).UpdateList();
            this.Close();
        }

        private void ChangeImageClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                imagePrint.Source = new BitmapImage(new Uri(filePath));
            }
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (DbControl.HasLinkedOrders(editProduct))
            {
                MessageBox.Show(
                    "Невозможно удалить товар, так как он используется в существующих заказах.",
                    "Ошибка удаления",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить этот товар?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                DbControl.DeleteProduct(editProduct);
                ((ProductsListWindow)this.Owner).UpdateList();
                this.Close();
            }
        }
    }
}

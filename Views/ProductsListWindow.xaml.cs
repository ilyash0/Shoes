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

namespace Shoes.Views
{
    public partial class ProductsListWindow : Window
    {
        User? authUser = null;
        public ProductsListWindow(User? user = null)
        {
            InitializeComponent();
            authUser = user;
            if (authUser != null)
            {
                userName.Text = authUser.Name + " " + authUser.Surname + " " + authUser.Patronymic;
                if (authUser.RoleId == 1 || authUser.RoleId == 2)
                {
                    searchBox.Visibility = Visibility.Visible;
                    adminPanel.Visibility = Visibility.Visible;
                }
                if (authUser.RoleId == 1)
                {
                    addButton.Visibility = Visibility.Visible;
                }
            }

            List<Product> products = DbControl.GetProducts();
            productList.ItemsSource = products;

            Supplier allSuppliers = new Supplier()
            {
                Id = 0,
                Name = "Все поставщики"
            };
            List<Supplier> suppliers = DbControl.GetSuppliers();
            suppliers.Insert(0, allSuppliers);
            supplierComboBox.ItemsSource = suppliers;
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }

        public void UpdateList()
        {
            if (productList.ItemsSource != null)
            {
                productList.ItemsSource = null;
            }
            productList.ItemsSource = DbControl.GetProducts(searchText.Text, descSort.IsChecked, (Supplier)supplierComboBox.SelectedItem);
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (productList != null)
            {
                UpdateList();
            }
        }

        private void SortChecked(object sender, RoutedEventArgs e)
        {
            if (productList != null)
            {
                UpdateList();
            }
        }

        private void supplierComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productList != null)
            {
                UpdateList();
            }
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            ProductDetailWindow productDetailWindow = new ProductDetailWindow();
            productDetailWindow.Owner = this;
            productDetailWindow.ShowDialog();
        }

        private void OrderClick(object sender, RoutedEventArgs e)
        {
            OrdersListWindow ordersListWindow = new OrdersListWindow(authUser);
            ordersListWindow.Show();
            this.Close();
        }

        private void ProductListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (authUser != null && authUser.RoleId == 1)
            {
                ProductDetailWindow productDetailWindow = new ProductDetailWindow((Product)productList.SelectedItem);
                productDetailWindow.Owner = this;
                productDetailWindow.ShowDialog();
            }
        }
    }
}

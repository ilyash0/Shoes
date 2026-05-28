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
    public partial class OrdersListWindow : Window
    {
        public User authUser;
        public OrdersListWindow(User user)
        {
            InitializeComponent();
            authUser = user;
            userName.Text = authUser.Name + " " + authUser.Surname + " " + authUser.Patronymic;
            if (authUser.RoleId == 1 || authUser.RoleId == 2)
            {
                adminPanel.Visibility = Visibility.Visible;
            }
            if (authUser.RoleId == 1)
            {
                addButton.Visibility = Visibility.Visible;
            }

            List<OrderProduct> orders = DbControl.GetOrdersProducts();
            orderList.ItemsSource = orders;
        }

        public void UpdateList()
        {
            if (orderList.ItemsSource != null)
            {
                orderList.ItemsSource = null;
            }
            orderList.ItemsSource = DbControl.GetOrdersProducts();
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            OrderDetailWindow orderDetailWindow = new OrderDetailWindow();
            orderDetailWindow.Owner = this;
            orderDetailWindow.ShowDialog();
        }

        private void ProductsClick(object sender, RoutedEventArgs e)
        {
            ProductsListWindow productsListWindow = new ProductsListWindow(authUser);
            productsListWindow.Show();
            this.Close();
        }

        private void ProductListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (authUser != null && authUser.RoleId == 1)
            {
                OrderDetailWindow orderDetailWindow = new OrderDetailWindow((OrderProduct)orderList.SelectedItem);
                orderDetailWindow.Owner = this;
                orderDetailWindow.ShowDialog();
            }
        }
    }
}

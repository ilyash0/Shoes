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

namespace Shoes.Views
{
    public partial class OrderDetailWindow : Window
    {
        OrderProduct? editOrder;
        public OrderDetailWindow(OrderProduct? selectedOrder = null)
        {
            InitializeComponent();
            editOrder = selectedOrder;

            addressComboBox.ItemsSource = DbControl.GetPickupPoints();
            statusComboBox.ItemsSource = DbControl.GetOrderStatuses();
            articleComboBox.ItemsSource = DbControl.GetProducts();

            if (editOrder != null)
            {
                Title = "Редактирование заказа";
                title.Text = "Редактирование заказа";
                idTextBox.Text = "" + editOrder.Id;
                deliveryDateDatePicker.Text = editOrder.Order.DeliveryDate.ToString();
                orderDateDatePicker.Text = editOrder.Order.OrdrerDate.ToString();

                addressComboBox.SelectedValue = editOrder.Order.AddresId;
                statusComboBox.SelectedValue = editOrder.Order.OrderStatusId;
                articleComboBox.SelectedValue = editOrder.Product.Id;
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

            if (addressComboBox.SelectedItem == null || statusComboBox.SelectedItem == null ||
                articleComboBox.SelectedItem == null || deliveryDateDatePicker.SelectedDate == null || orderDateDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (editOrder == null)
            {
                Random rand = new Random();
                Order o = new Order()
                {
                    OrdrerDate = DateOnly.FromDateTime(orderDateDatePicker.SelectedDate.GetValueOrDefault(DateTime.Today)),
                    DeliveryDate = DateOnly.FromDateTime(deliveryDateDatePicker.SelectedDate.GetValueOrDefault(DateTime.Today.AddDays(3))),
                    AddresId = ((PickupPoint)addressComboBox.SelectedItem).Id,
                    UserId = ((OrdersListWindow)this.Owner).authUser.Id,
                    Code = rand.Next(1000, 10000),
                    OrderStatusId = ((OrderStatus)statusComboBox.SelectedItem).Id
                };
                DbControl.AddOrder(o);

                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.Id,
                    ProductId = ((Product)articleComboBox.SelectedItem).Id,
                    Count = 1
                };
                DbControl.AddOrderProduct(op);
            }
            else
            {
                editOrder.Order.OrdrerDate = DateOnly.FromDateTime(orderDateDatePicker.SelectedDate.GetValueOrDefault(DateTime.Today));
                editOrder.Order.DeliveryDate = DateOnly.FromDateTime(deliveryDateDatePicker.SelectedDate.GetValueOrDefault(DateTime.Today.AddDays(3)));
                editOrder.Order.AddresId = ((PickupPoint)addressComboBox.SelectedItem).Id;
                editOrder.Order.UserId = ((OrdersListWindow)this.Owner).authUser.Id;
                editOrder.Order.OrderStatusId = ((OrderStatus)statusComboBox.SelectedItem).Id;
                editOrder.ProductId = ((Product)articleComboBox.SelectedItem).Id;

                DbControl.UpdateOrder(editOrder.Order);
                DbControl.UpdateOrderProduct(editOrder);
            }
            ((OrdersListWindow)this.Owner).UpdateList();
            this.Close();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить этот заказ?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                DbControl.DeleteOrderProduct(editOrder);
                ((OrdersListWindow)this.Owner).UpdateList();
                this.Close();
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ООО_обувь.Models;

namespace ООО_обувь
{
    public partial class AddNewOrder : Window
    {
        AppDbContext _db = new AppDbContext();
        User _user;

        // Товары, добавленные в заказ
        private List<OrderItemDraft> _items = new List<OrderItemDraft>();

        private class OrderItemDraft
        {
            public Product Product { get; set; }
            public int Count { get; set; }
        }

        public AddNewOrder()
        {
            InitializeComponent();
            LoadComboBoxes();
            OrderDateBox.SelectedDate = DateTime.Now;
        }

        public AddNewOrder(User user) : this()
        {
            _user = user;
        }

        private void LoadComboBoxes()
        {
            StatusBox.ItemsSource = _db.Statuses.ToList();
            UserBox.ItemsSource = _db.Users
        .ToList()
        .Select(u => new
        {
            u.Id,
            Name = $"{u.Surname} {u.Name} {u.Patronyc}".Trim()
        })
        .ToList();
            PickupBox.ItemsSource = _db.PickUpPoints
                .Include(p => p.Street)
                .Select(p => new
                {
                    p.Id,
                    Name = p.Street.Name + ", д. " + p.House
                })
                .ToList();
        }

        //  Добавление товара в заказ 
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddProductToOrder { Owner = this };
            if (dialog.ShowDialog() == true)
            {
                var existing = _items.FirstOrDefault(i => i.Product.Id == dialog.SelectedProduct.Id);
                if (existing != null)
                {
                    existing.Count += dialog.SelectedCount;
                }
                else
                {
                    _items.Add(new OrderItemDraft
                    {
                        Product = dialog.SelectedProduct,
                        Count = dialog.SelectedCount
                    });
                }

                RefreshOrderProductsList();
            }
        }

        //  Удаление товара из заказа 
        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!(ListViewOrderProducts.SelectedItem is Border card) ||
                !(card.Tag is OrderItemDraft draft))
            {
                MessageBox.Show("Выберите товар для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _items.Remove(draft);
            RefreshOrderProductsList();
        }


        private void RefreshOrderProductsList()
        {
            ListViewOrderProducts.Items.Clear();

            if (_items.Count == 0)
            {
                ListViewOrderProducts.Items.Add(new TextBlock
                {
                    Text = "Товары не добавлены",
                    FontSize = 14,
                    Margin = new Thickness(10),
                    Foreground = Brushes.Gray
                });
                return;
            }

            decimal total = 0;

            foreach (var item in _items)
            {
                decimal price = item.Product.Price;
                decimal sum = price * item.Count;
                total += sum;

                var card = new Border
                {
                    Width = 400,
                    MinHeight = 80,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Background = Brushes.White,
                    Tag = item
                };

                card.Child = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    Text =
                        $"Артикул: {item.Product.Article}\n" +
                        $"Название: {item.Product.Name}\n" +
                        $"Количество: {item.Count} шт.\n" +
                        $"Цена: {price} руб.\n" +
                        $"Сумма: {sum} руб."
                };

                ListViewOrderProducts.Items.Add(card);
            }

            ListViewOrderProducts.Items.Add(new TextBlock
            {
                Text = $"Итого: {total} руб.",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                Foreground = Brushes.Black
            });
        }

        // Создание заказа =
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //  Валидация 
            if (!int.TryParse(CodeBox.Text, out int pickupCode))
            {
                MessageBox.Show("Введите корректный код получения (целое число).", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_db.Orders.Any(o => o.КодДляПолучения == pickupCode))
            {
                MessageBox.Show("Заказ с таким кодом получения уже существует. Введите другой код.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StatusBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите покупателя.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (OrderDateBox.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату заказа.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DeliveryDateBox.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату доставки.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PickupBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите пункт выдачи.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_items.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в заказ.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (OrderDateBox.SelectedDate.HasValue)
            {
                DeliveryDateBox.DisplayDateStart = OrderDateBox.SelectedDate.Value.Date;
                if (DeliveryDateBox.SelectedDate.HasValue &&
                    DeliveryDateBox.SelectedDate.Value.Date < OrderDateBox.SelectedDate.Value.Date)
                {
                    MessageBox.Show("Ошибка");
                   
                    return;
                }
            }
            else
            {

                MessageBox.Show("Ошибка");

                return;
            }


            try
            {
                var order = new Order
                {
                    КодДляПолучения = pickupCode,
                    StatusId = (int)StatusBox.SelectedValue,
                    UserId = (int)UserBox.SelectedValue,
                    ДатаЗаказа = OrderDateBox.SelectedDate.HasValue
        ? DateOnly.FromDateTime(OrderDateBox.SelectedDate.Value)
        : (DateOnly?)null,
                    ДатаДоставки = DeliveryDateBox.SelectedDate.HasValue
        ? DateOnly.FromDateTime(DeliveryDateBox.SelectedDate.Value)
        : (DateOnly?)null,
                    АдресПунктаВыдачи = (int)PickupBox.SelectedValue
                };
                _db.Orders.Add(order);
                _db.SaveChanges(); 

                // Добавляем товары 
                foreach (var item in _items)
                {
                    _db.OrdersProducts.Add(new OrdersProduct
                    {
                        OrdersId = order.Id,
                        ProductId = item.Product.Id,
                        Count = item.Count,
                        Price = item.Product.Price
                    });
                }

                _db.SaveChanges();

                MessageBox.Show("Заказ успешно создан.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании заказа:\n" + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnexit_Click(object sender, RoutedEventArgs e)
        {
            adminWindow adminWindow = new adminWindow(_user);
            adminWindow.Show();
            this.Close();

        }
    }
}
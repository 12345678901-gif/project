using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ООО_обувь.Models;

namespace ООО_обувь
{
    /// <summary>
    /// Логика взаимодействия для ListOrdersAdmin.xaml
    /// </summary>
    public partial class ListOrdersAdmin : Window
    {
        AppDbContext _db = new AppDbContext();
        User user;
        public ListOrdersAdmin(User user)
        {
            InitializeComponent();
            this.user = user;
            LoadOrdersFromDb();
        }
        private void LoadOrdersFromDb()
        {
            var orders = _db.Orders
         .Include(o => o.Status)
         .Include(o => o.User)
         .Include(o => o.АдресПунктаВыдачиNavigation)
             .ThenInclude(p => p.Street)
         .ToList();
            //  Очищаем ListView перед заполнением
            ListViewOrders.Items.Clear();

            //  Для каждого заказа создаём карточку
            foreach (var order in orders)
            {
                
                string address = "—";
                var pvz = order.АдресПунктаВыдачиNavigation;
                if (pvz != null)
                {
                    address = (pvz.Street?.Name ?? "") + ", д. " + pvz.House;
                }

                // Товары в заказе

                var card = new Border
                {
                    Width = 750,
                    MinHeight = 120,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Background = Brushes.White,
                    Tag = order
                };

                var info = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14,
                    Text =
                        $"Артикул: {order.КодДляПолучения}\n" +
                        $"Статус: {order.Status?.Name ?? "—"}\n" +
                        $"Адрес ПВЗ: {address}\n" +
                        $"Дата заказа: {order.ДатаЗаказа:dd.MM.yyyy}\n" +
                        $"Дата доставки: {order.ДатаДоставки:dd.MM.yyyy}\n"


                };

                card.Child = info;
                ListViewOrders.Items.Add(card);
            }
        }



        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(ListViewOrders.SelectedItem is Border card) || !(card.Tag is Order order))
            {
                MessageBox.Show("Выберите заказ для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить заказ №{order.Id}?",
                    "Подтверждение", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                // Сначала удаляем товары заказа
                var orderProducts = _db.OrdersProducts
                    .Where(op => op.OrdersId == order.Id)
                    .ToList();

                if (orderProducts.Count > 0)
                    _db.OrdersProducts.RemoveRange(orderProducts);

                // Потом сам заказ
                var orderToDelete = _db.Orders.FirstOrDefault(o => o.Id == order.Id);
                if (orderToDelete == null) return;

                _db.Orders.Remove(orderToDelete);
                _db.SaveChanges();

                MessageBox.Show("Заказ удалён.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoadOrdersFromDb();
                ListViewOrderProducts.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении:\n" + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListViewOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewOrderProducts.Items.Clear();

            // Достаём выбранный заказ из Tag
            if (!(ListViewOrders.SelectedItem is Border card) || !(card.Tag is Order order))
                return;

            // Загружаем товары этого заказа
            var orderProducts = _db.OrdersProducts
                .Include(op => op.Product)
                .Where(op => op.OrdersId == order.Id)
                .ToList();

            if (orderProducts.Count == 0)
            {
                ListViewOrderProducts.Items.Add(new TextBlock
                {
                    Text = "В заказе нет товаров",
                    FontSize = 14,
                    Margin = new Thickness(10),
                    Foreground = Brushes.Gray
                });
                return;
            }

            decimal total = 0;

            foreach (var op in orderProducts)
            {
                decimal price = op.Price ?? 0;
                total += price * op.Count;

                var productCard = new Border
                {
                    Width = 400,
                    MinHeight = 80,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Background = Brushes.White
                };

                productCard.Child = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    Text =
                        $"Артикул: {op.Product?.Article ?? "—"}\n" +
                        $"Название: {op.Product?.Name ?? "—"}\n" +
                        $"Количество: {op.Count} шт.\n"
                };

                ListViewOrderProducts.Items.Add(productCard);
            }
        }

        private void AddNewOrder_Click(object sender, RoutedEventArgs e)
        {
            AddNewOrder window = new AddNewOrder(user);
            window.ShowDialog();

            // После закрытия окна — обновить список
            LoadOrdersFromDb();
            ListViewOrderProducts.Items.Clear();
        }

        private void ExitClick_Click(object sender, RoutedEventArgs e)
        {
            adminWindow adminWindow = new adminWindow(user);
            adminWindow.Show();
            this.Close();
        }
    }
}

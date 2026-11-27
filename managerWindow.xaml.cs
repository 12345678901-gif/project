using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ООО_обувь.Models;

namespace ООО_обувь
{
    public partial class managerWindow : Window
    {
        User _user;
        AppDbContext _db = new AppDbContext();

        private List<ProductView> _allProducts = new List<ProductView>();

        private class ProductView
        {
            public Product Product { get; set; }
            public int StockCount { get; set; }
        }

        public managerWindow(User user)
        {
            InitializeComponent();
            _user = user;
            NameUserText.Content = $"{user.Surname} {user.Name} {user.Patronyc} менеджер";

            LoadProductsFromDb();
            ApplyFiltersAndSort();
        }

        private void LoadProductsFromDb()
        {
            var products = _db.Products
                .Include(item => item.Categories)
                .Include(item => item.Manufacturers)
                .Include(item => item.Supplier)
                .Include(item => item.Units)
                .Include(item => item.ProductStockItem)
                .ToList();

            _allProducts.Clear();
            foreach (var item in products)
            {
                var stockItem = _db.ProductStockItems
                    .FirstOrDefault(stock => stock.ProductId == item.Id);
                _allProducts.Add(new ProductView
                {
                    Product = item,
                    StockCount = stockItem?.Count ?? 0
                });
            }
        }

        private void ApplyFiltersAndSort()
        {
            if (ListViewProduct == null) return;

            IEnumerable<ProductView> query = _allProducts;

            // --- 1. ПОИСК ---
            string searchText = (TextBoxSearch?.Text ?? string.Empty).Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(p =>
                    (p.Product.Name ?? "").ToLower().Contains(searchText) ||
                    (p.Product.Description ?? "").ToLower().Contains(searchText) ||
                    (p.Product.Categories?.Name ?? "").ToLower().Contains(searchText) ||
                    (p.Product.Manufacturers?.Name ?? "").ToLower().Contains(searchText) ||
                    (p.Product.Supplier?.Name ?? "").ToLower().Contains(searchText));
            }

            // --- 2. ФИЛЬТР ПО СКИДКЕ ---
            int filterIndex = ComboBoxFilter?.SelectedIndex ?? 0;
            switch (filterIndex)
            {
                case 1: // 0–12,99%
                    query = query.Where(p => p.Product.Sale >= 0 && p.Product.Sale <= 12.99m);
                    break;
                case 2: // 13–16,99%
                    query = query.Where(p => p.Product.Sale >= 13 && p.Product.Sale <= 16.99m);
                    break;
                case 3: // 17% и более
                    query = query.Where(p => p.Product.Sale >= 17);
                    break;
                    // case 0 — "Все диапазоны", фильтр не применяется
            }

            // --- 3. СОРТИРОВКА ---
            int sortIndex = ComboBoxSort?.SelectedIndex ?? 0;
            switch (sortIndex)
            {
                case 0: // Цена по возрастанию
                    query = query.OrderBy(p => p.Product.Price);
                    break;
                case 1: // Цена по убыванию
                    query = query.OrderByDescending(p => p.Product.Price);
                    break;
                case 2: // Количество по возрастанию
                    query = query.OrderBy(p => p.StockCount);
                    break;
                case 3: // Количество по убыванию
                    query = query.OrderByDescending(p => p.StockCount);
                    break;
            }

          
            ListViewProduct.Items.Clear();
            foreach (var pv in query.ToList())
            {
                ListViewProduct.Items.Add(CreateProductCard(pv.Product, pv.StockCount));
            }
        }


        private WrapPanel CreateProductCard(Product item, int stockCount)
        {
            var productCard = new WrapPanel
            {
                Width = 750,
                MinHeight = 250,
                Margin = new Thickness(5)
            };

            if (stockCount <= 0)
            {
                productCard.Background = Brushes.LightGray;
            }
            else if (item.Sale > 25)
            {
                productCard.Background = new SolidColorBrush(Color.FromRgb(0x23, 0xE1, 0xEF));
            }
            else
            {
                productCard.Background = Brushes.White;
            }

            var productImage = new Image
            {
                Width = 180,
                Height = 220,
                Margin = new Thickness(10),
                Stretch = Stretch.Uniform,
                Source = LoadProductImage(item.Image)
            };

            var productInfo = new TextBlock
            {
                Width = 520,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text =
                    $"{item.Categories?.Name ?? "Без категории"} | {item.Name}\n" +
                    $"Описание товара: {item.Description}\n" +
                    $"Производитель: {item.Manufacturers?.Name ?? "Не указан"}\n" +
                    $"Поставщик: {item.Supplier?.Name ?? "Не указан"}\n" +
                    $"Цена: {item.Price}\n" +
                    $"Единица измерения: {item.Units?.Name ?? "Не указана"}\n" +
                    $"Количество на складе: {stockCount}\n" +
                    $"Скидка: {item.Sale}%"
            };

            productCard.Children.Add(productImage);
            productCard.Children.Add(productInfo);
            return productCard;
        }

        private static BitmapImage LoadProductImage(string? fileName)
        {
            string imagePath = System.IO.Path.Combine(
                AppContext.BaseDirectory, "Images", fileName ?? "");

            try
            {
                if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch { }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(
                    "pack://application:,,,/Images/picture.png", UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        private void ExitClick_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void OrdersButtn_Click(object sender, RoutedEventArgs e)
        {
            UserOrders userOrders= new UserOrders(_user);
            userOrders.Show();
            this.Close();
        }
    }
}
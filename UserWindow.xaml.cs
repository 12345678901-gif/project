using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        User _user;
        private readonly AppDbContext _db = new AppDbContext();
        public UserWindow(User user)
        {
            InitializeComponent();
            NameUserText.Content = user.Surname + " " + user.Name + " " + user.Patronyc; ;
            var products = _db.Products
                .Include(item => item.Categories)
                .Include(item => item.Manufacturers)
                .Include(item => item.Supplier)
                .Include(item => item.Units)
                .Include(item => item.ProductStockItem)
                .ToList();
            foreach (var item in products)
            {
                var stockItem = _db.ProductStockItems
                    .FirstOrDefault(stock => stock.ProductId == item.Id);

                var stockCount = stockItem?.Count ?? 0;

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
                    productCard.Background = new SolidColorBrush(
                        Color.FromRgb(0x23, 0xE1, 0xEF));
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

                ListViewProduct.Items.Add(productCard);
            }

            _user = user;
        }
        private static BitmapImage LoadProductImage(string? fileName)
        {
            string imagePath = System.IO.Path.Combine(
                AppContext.BaseDirectory,
                "Images",
                fileName ?? "");

            try
            {
                if (!string.IsNullOrWhiteSpace(fileName) &&
                    File.Exists(imagePath))
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
            catch
            {
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(
                    "pack://application:,,,/Images/picture.png",
                    UriKind.Absolute);
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
    }
}

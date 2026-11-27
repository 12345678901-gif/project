using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ООО_обувь.Models;

namespace ООО_обувь
{
    public partial class Добавление_нового_товара : Window
    {
        private readonly AppDbContext _db = new AppDbContext();
        User _user;

        private string _selectedImagePath;

        public Добавление_нового_товара(User user)
        {
            InitializeComponent();
            LoadComboBoxes();
            _user = user;
        }

        private void LoadComboBoxes()
        {
            UnitBox.ItemsSource = _db.Units.ToList();
            SupplierBox.ItemsSource = _db.Suppliers.ToList();
            ManufacturerBox.ItemsSource = _db.Manufacturers.ToList();
            CategoryBox.ItemsSource = _db.Categories.ToList();
        }


        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Выберите изображение товара",
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedImagePath = dialog.FileName;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(_selectedImagePath, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();

                image.Source = bitmap;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(ArticleBox.Text))
            {
                MessageBox.Show("Введите артикул.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название товара.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceBox.Text.Replace('.', ','), out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (неотрицательное число).", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal sale = 0;
            if (!string.IsNullOrWhiteSpace(SaleBox.Text))
            {
                if (!decimal.TryParse(SaleBox.Text.Replace('.', ','), out sale) ||
                    sale < 0 || sale > 100)
                {
                    MessageBox.Show("Скидка должна быть числом от 0 до 100.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (UnitBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите единицу измерения.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SupplierBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите поставщика.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ManufacturerBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите производителя.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- Проверка уникальности артикула ---
            string article = ArticleBox.Text.Trim();
            bool articleExists = _db.Products.Any(p => p.Article == article);
            if (articleExists)
            {
                MessageBox.Show("Товар с таким артикулом уже существует.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // --- Сохранение картинки в папку Images ---
                string imageFileName = null;
                if (!string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    string imagesDir = Path.Combine(AppContext.BaseDirectory, "Images");
                    if (!Directory.Exists(imagesDir))
                        Directory.CreateDirectory(imagesDir);

                    imageFileName = Guid.NewGuid().ToString("N") +
                                    Path.GetExtension(_selectedImagePath);

                    string destPath = Path.Combine(imagesDir, imageFileName);
                    File.Copy(_selectedImagePath, destPath, overwrite: true);
                }

                // --- Создание товара ---
                var product = new Product
                {
                    Article = article,
                    Name = NameBox.Text.Trim(),
                    Description = DescriptionBox.Text?.Trim(),
                    Price = price,
                    Sale = sale,
                    Image = imageFileName,
                    UnitsId = (int)UnitBox.SelectedValue,
                    SupplierId = (int)SupplierBox.SelectedValue,
                    ManufacturersId = (int)ManufacturerBox.SelectedValue,
                    CategoriesId = (int)CategoryBox.SelectedValue
                };

                _db.Products.Add(product);
                _db.SaveChanges();

                // --- Остаток на складе (по умолчанию 0) ---
                var stockItem = new ProductStockItem
                {
                    ProductId = product.Id,
                    Count = 0
                };
                _db.ProductStockItems.Add(stockItem);
                _db.SaveChanges();

                MessageBox.Show("Товар успешно добавлен.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                adminWindow adminWindow = new adminWindow(_user);
                adminWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении:\n" + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            adminWindow adminWindow = new adminWindow(_user);
            adminWindow.Show();
            this.Close();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ООО_обувь.Models;

namespace ООО_обувь
{
    /// <summary>
    /// Логика взаимодействия для редактирование_товара.xaml
    /// </summary>
    public partial class редактирование_товара : Window
    {
        private Product _tovar;
        User _user;
        AppDbContext _db = new AppDbContext();
        private string _selectedImagePath;

        public редактирование_товара(User user, Product product)
        {
            InitializeComponent();
            _user = user;
            _tovar = product;

            // --- Заполнение полей ---
            ArticleBox.Text = product.Article;
            NameBox.Text = product.Name;
            PriceBox.Text = product.Price.ToString();
            DescriptionBox.Text = product.Description;
            SaleBox.Text = product.Sale.ToString();

            // --- ComboBox'ы ---
            SupplierBox.ItemsSource = _db.Suppliers.ToList();
            SupplierBox.SelectedItem = _db.Suppliers.FirstOrDefault(x => x.Id == product.SupplierId);

            ManufacturerBox.ItemsSource = _db.Manufacturers.ToList();
            ManufacturerBox.SelectedItem = _db.Manufacturers.FirstOrDefault(x => x.Id == product.ManufacturersId);

            CategoryBox.ItemsSource = _db.Categories.ToList();
            CategoryBox.SelectedItem = _db.Categories.FirstOrDefault(x => x.Id == product.CategoriesId);

            UnitBox.ItemsSource = _db.Units.ToList();
            UnitBox.SelectedItem = _db.Units.FirstOrDefault(x => x.Id == product.UnitsId);

            // --- Текущее фото ---
            image.Source = LoadProductImage(product.Image);
        }


        private static BitmapImage LoadProductImage(string? fileName)
        {
            string imagePath = Path.Combine(AppContext.BaseDirectory, "Images", fileName ?? "");

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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            adminWindow adminWindow = new adminWindow(_user);
            adminWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // --- Валидация ---
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

            // --- Уникальность артикула (исключая текущий товар) ---
            string article = ArticleBox.Text.Trim();
            bool articleExists = _db.Products
                .Any(p => p.Article == article && p.Id != _tovar.Id);
            if (articleExists)
            {
                MessageBox.Show("Товар с таким артикулом уже существует.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // --- Получаем «свежий» товар из БД ---
                var product = _db.Products.FirstOrDefault(x => x.Id == _tovar.Id);
                if (product == null)
                {
                    MessageBox.Show("Товар не найден в базе данных.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // --- Картинка ---
                string oldImage = product.Image;
                string newImageFileName = oldImage;

                if (!string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    string imagesDir = Path.Combine(AppContext.BaseDirectory, "Images");
                    if (!Directory.Exists(imagesDir))
                        Directory.CreateDirectory(imagesDir);

                    newImageFileName = Guid.NewGuid().ToString("N") +
                                       Path.GetExtension(_selectedImagePath);

                    string destPath = Path.Combine(imagesDir, newImageFileName);
                    File.Copy(_selectedImagePath, destPath, overwrite: true);
                }

                // --- Обновление полей ---
                product.Article = article;
                product.Name = NameBox.Text.Trim();
                product.Description = DescriptionBox.Text?.Trim();
                product.Price = price;
                product.Sale = sale;
                product.Image = newImageFileName;
                product.UnitsId = (int)UnitBox.SelectedValue;
                product.SupplierId = (int)SupplierBox.SelectedValue;
                product.ManufacturersId = (int)ManufacturerBox.SelectedValue;
                product.CategoriesId = (int)CategoryBox.SelectedValue;

                _db.SaveChanges();

                // --- Удаляем старую картинку, если была заменена ---
                if (!string.IsNullOrWhiteSpace(_selectedImagePath) &&
                    !string.IsNullOrWhiteSpace(oldImage) &&
                    oldImage != newImageFileName)
                {
                    try
                    {
                        string oldPath = Path.Combine(AppContext.BaseDirectory, "Images", oldImage);
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                    }
                    catch { /* не критично */ }
                }

                MessageBox.Show("Товар успешно сохранён.", "Готово",
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
    }
}
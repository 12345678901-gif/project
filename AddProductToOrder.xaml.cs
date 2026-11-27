using System;
using System.Linq;
using System.Windows;
using ООО_обувь.Models;

namespace ООО_обувь
{
    public partial class AddProductToOrder : Window
    {
        AppDbContext _db = new AppDbContext();

        public Product SelectedProduct { get; private set; }
        public int SelectedCount { get; private set; }

        public AddProductToOrder()
        {
            InitializeComponent();
            ProductBox.ItemsSource = _db.Products.ToList();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите товар.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CountBox.Text, out int count) || count <= 0)
            {
                MessageBox.Show("Введите корректное количество (целое > 0).", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedProduct = product;
            SelectedCount = count;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
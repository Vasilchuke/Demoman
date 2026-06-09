using DemoExz22.DataBase;
using DemoExz22.Helpers;
using DemoExz22.Statics;
using DemoExz22.ViewModels;
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

namespace DemoExz22
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private ShoeShopDBEntities db = new ShoeShopDBEntities();
        private MessageHelper messageHelper = new MessageHelper();
        private List<ProductViewModel> _productViewModels = new List<ProductViewModel>();

        private string[] _sortOptions = new string[]
        {
            "По умолчанию",
            "По убыванию",
            "По возрастанию"
        };

        private List<string> _filterTypes = new List<string>()
        {
            "Все поставщики"
        };

        public ProductWindow()
        {
            InitializeComponent();
            LoadProducts();
            LoadData();
            LoadUI();
        }

        private void LoadUI()
        {
            Users user = CurrentSession.CurrentUser;

            AdminPanel.Visibility = Visibility.Collapsed;

            if (user != null && user.RoleID == 1)
            {
                AdminPanel.Visibility = Visibility.Visible;
                CreateButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadData()
        {
            SortComboBox.ItemsSource = _sortOptions;
            SortComboBox.SelectedIndex = 0;

            var supplier = db.Supplier.ToList();
            foreach (var item in supplier)
            {
                _filterTypes.Add(item.Name);
            }
            FilterComboBox.ItemsSource = _filterTypes;
            FilterComboBox.SelectedIndex = 0;

            Users user = CurrentSession.CurrentUser;
            
            if (user != null)
            {
                FullUserName.Text = $"{user.SurName} {user.FirstName} {user.Patronymic}";
            }
        }

        private void LoadProducts()
        {
            _productViewModels = db.Product
                .OrderBy(p => p.ID)
                .ToList()
                .Select(p => new ProductViewModel(p))
                .ToList();
            ProductList.ItemsSource = _productViewModels;
        }

        private void UpdateProducts()
        {
            ProductList.ItemsSource = null;
            ProductList.ItemsSource = _productViewModels;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.CurrentUser = null;
            new MainWindow().Show();
            Close();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string searchText = SearchTextBox.Text.ToLower();

            string supplierText = FilterComboBox.SelectedItem?.ToString();

            int sortingType = SortComboBox.SelectedIndex;

            var products = db.Product.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                products = products.Where(p =>
                    p.Category.Name.ToLower().Contains(searchText) ||
                    p.Name.ToLower().Contains(searchText) ||
                    p.Description.ToLower().Contains(searchText) ||
                    p.Supplier.Name.ToLower().Contains(searchText) ||
                    p.Manufacturer.Name.ToLower().Contains(searchText) ||
                    p.Unit.Name.ToLower().Contains(searchText)
                );
            }

            if (!string.IsNullOrWhiteSpace(supplierText) &&
                supplierText != "Все поставщики")
            {
                products = products.Where(p => p.Supplier.Name == supplierText);
            }

            if (sortingType == 1)
            {
                products = products.OrderByDescending(p => p.Quantity);
            }
            else if (sortingType == 2)
            {
                products = products.OrderBy(p => p.Quantity);
            }
            else
            {
                products = products.OrderBy(p => p.ID);
            }

            _productViewModels = products
                .ToList()
                .Select(p => new ProductViewModel(p))
                .ToList();

            UpdateProducts();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new AddEditProductWindow(null).ShowDialog();
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Users user = CurrentSession.CurrentUser;

            if (user == null || user.RoleID != 1)
            {
                return;
            }

            Border border = sender as Border;

            if (border == null)
                return;

            int productId = (int)border.Tag;

            new AddEditProductWindow(productId).Show();

            Close();
        }

        private void DeleteProduct(int productId)
        {
            Product product = db.Product.Find(productId);

            if (product == null)
            {
                messageHelper.ShowError("Товар не найден.");
                return;
            }

            bool productInOrder = db.ProductInOrder
                .Any(p => p.ProductID == productId);

            if (productInOrder)
            {
                messageHelper.ShowError(
                    "Нельзя удалить товар, который присутствует в заказе.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите удалить товар?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            db.Product.Remove(product);
            db.SaveChanges();

            messageHelper.ShowInfo("Товар удалён.");

            ApplyFilters();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            int productId = (int)button.Tag;

            DeleteProduct(productId);
        }
    }
}

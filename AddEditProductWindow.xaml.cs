using DemoExz22.DataBase;
using DemoExz22.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        private ShoeShopDBEntities db = new ShoeShopDBEntities();
        private bool _isEditMode;
        private Product product;
        private MessageHelper messageHelper = new MessageHelper();
        public AddEditProductWindow(int? id)
        {
            InitializeComponent();

            if (id == null)
            {
                _isEditMode = false;
            }
            else
            {
                _isEditMode = true;
                product = db.Product.Find(id);
            }
            LoadData();
        }

        private void LoadData()
        {
            var units = db.Unit.ToList();
            var categories = db.Category.ToList();
            var suppliers = db.Supplier.ToList();
            var manufacturers = db.Manufacturer.ToList();

            ProductUnit.ItemsSource = units;
            ProductUnit.DisplayMemberPath = "Name";
            ProductUnit.SelectedValuePath = "ID";
            ProductUnit.SelectedIndex = 0;
            ProductSupplier.ItemsSource = suppliers;
            ProductSupplier.DisplayMemberPath = "Name";
            ProductSupplier.SelectedValuePath = "ID";
            ProductSupplier.SelectedIndex = 0;
            ProductManufacturer.ItemsSource = manufacturers;
            ProductManufacturer.DisplayMemberPath = "Name";
            ProductManufacturer.SelectedValuePath = "ID";
            ProductManufacturer.SelectedIndex = 0;
            ProductCategory.ItemsSource = categories;
            ProductCategory.DisplayMemberPath = "Name";
            ProductCategory.SelectedValuePath = "ID";
            ProductCategory.SelectedIndex = 0;

            if (_isEditMode == true)
            {
                FillData();
            }
        }

        private void FillData()
        {
            ProductArticle.Text = product.Article;
            ProductName.Text = product.Name;
            ProductPrice.Text = product.Price.ToString();
            ProductDiscount.Text = product.Discount.ToString();
            ProductQuantity.Text = product.Quantity.ToString();
            ProductDescription.Text = product.Description;
            ProductPhoto.Text = product.Photo;

            ProductUnit.SelectedValue = product.UnitID;
            ProductSupplier.SelectedValue = product.SupplierID;
            ProductManufacturer.SelectedValue = product.ManufacturerID;
            ProductCategory.SelectedValue = product.CategoryID;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            if (_isEditMode == true)
                UpdateProduct();
            else
                CreateProduct();
        }

        private void CreateProduct()
        {
            Product product = new Product();

            string article = ProductArticle.Text;
            string name = ProductName.Text;
            decimal price = Convert.ToDecimal(ProductPrice.Text);
            int discount = Convert.ToInt32(ProductDiscount.Text);
            int quantity = Convert.ToInt32(ProductQuantity.Text);
            string description = ProductDescription.Text;
            //string photo = ProductPhoto.Text;

            product.Article = article;
            product.Name = name;
            product.Price = price;
            product.Discount = discount;
            product.Quantity = quantity;
            product.Description = description;
            //product.Photo = photo;

            product.UnitID = (int)ProductUnit.SelectedValue;
            product.ManufacturerID = (int)ProductManufacturer.SelectedValue;
            product.SupplierID = (int)ProductSupplier.SelectedValue;
            product.CategoryID = (int)ProductCategory.SelectedValue;

            try
            {
                db.Product.Add(product);
                db.SaveChanges();
                messageHelper.ShowInfo("Данные успешно сохранены.");
                CancelButton_Click(null, null);
            }
            catch (Exception ex)
            {
                messageHelper.ShowError("Ошибка при сохранении данных: " + ex.Message);
            }
        }

        private void UpdateProduct()
        {
            string article = ProductArticle.Text;
            string name = ProductName.Text;
            decimal price = Convert.ToDecimal(ProductPrice.Text);
            decimal discount = Convert.ToDecimal(ProductDiscount.Text);
            int quantity = Convert.ToInt32(ProductQuantity.Text);
            string description = ProductDescription.Text;
            //string photo = ProductPhoto.Text;

            product.Article = article;
            product.Name = name;
            product.Price = price;
            product.Discount = discount;
            product.Quantity = quantity;
            product.Description = description;
            //product.Photo = photo;

            product.UnitID = (int)ProductUnit.SelectedValue;
            product.ManufacturerID = (int)ProductManufacturer.SelectedValue;
            product.SupplierID = (int)ProductSupplier.SelectedValue;
            product.CategoryID = (int)ProductCategory.SelectedValue;

            try
            {
                db.SaveChanges();
                messageHelper.ShowInfo("Данные успешно изменены.");
                CancelButton_Click(null, null);
            }
            catch (Exception ex)
            {
                messageHelper.ShowError("Ошибка при сохранении данных: " + ex.Message);
            }
        }

        private bool ValidateInput()
        {
            StringBuilder errors = new StringBuilder();

            string article = ProductArticle.Text;
            string name = ProductName.Text;
            string price = ProductPrice.Text;
            string discount = ProductDiscount.Text;
            string quantity = ProductQuantity.Text;
            string description = ProductDescription.Text;
            //string photo = ProductPhoto.Text;

            if (string.IsNullOrWhiteSpace(article))
            {
                errors.AppendLine("Артикул не может быть пустым.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.AppendLine("Наименование не может быть пустым.");
            }
            if (string.IsNullOrWhiteSpace(price) || !decimal.TryParse(price, out decimal priceDecimal))
            {
                errors.AppendLine("Цена не может быть пустой.");
            }
            if (string.IsNullOrWhiteSpace(discount) || !decimal.TryParse(discount, out decimal discountDecimal) || discountDecimal > 100 || discountDecimal < 0)
            {
                errors.AppendLine("Скидка не может быть пустой.");
            }
            if (string.IsNullOrWhiteSpace(quantity) || !int.TryParse(quantity, out int quantityInt) || quantityInt < 0)
            {
                errors.AppendLine("Количество не может быть пустым.");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                errors.AppendLine("Описание не может быть пустым.");
            }
            /*if (string.IsNullOrWhiteSpace(photo))
            {
                errors.AppendLine("Фото не может быть пустым.");
            }*/

            if (errors.Length > 0)
            {
                messageHelper.ShowError(errors.ToString());
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new ProductWindow().Show();
            Close();
        }
    }
}

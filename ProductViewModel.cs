using DemoExz22.DataBase;
using DemoExz22.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DemoExz22.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel(Product product)
        {
            ID = product.ID;
            Article = product.Article;
            Name = product.Name;
            Price = product.Price;
            Discount = product.Discount;
            Quantity = product.Quantity;
            Description = product.Description;
            Photo = product.Photo;
            Category = product.Category;
            Manufacturer = product.Manufacturer;
            Supplier = product.Supplier;
            Unit = product.Unit;

            GetBackground();
            GetPhoto();
            GetPrice();

            AdminButtonsVisibility =
            CurrentSession.CurrentUser != null && CurrentSession.CurrentUser.RoleID == 1
            ? Visibility.Visible
            : Visibility.Collapsed;

        }

        public int ID { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public  Category Category { get; set; }
        public  Manufacturer Manufacturer { get; set; }
        public  Supplier Supplier { get; set; }
        public  Unit Unit { get; set; }
        public Brush Background { get; set; }
        public Visibility AdminButtonsVisibility { get; set; }

        private void GetBackground()
        {
            if (Discount >= 15)
            {
                Background = (Brush)new BrushConverter().ConvertFromString("#2e8b57");
                return;
            }
            else if (Quantity == 0)
            {
                Background = Brushes.LightBlue;
                return;
            }
            else
            {
                Background = (Brush)new BrushConverter().ConvertFromString("#7fff00");
                return;
            }
        }

        private void GetPhoto()
        {
            if (!string.IsNullOrEmpty(Photo))
            {
                return;
            }
            else
            {
                Photo = "/Res/picture.png";
            }
        }

        private void GetPrice()
        {
            if (Discount == 0)
            {
                return;
            }
            else
            {
                OldPrice = Price;
                Price = OldPrice * (1 - (decimal)Discount / 100);
            }
        }
    }
}

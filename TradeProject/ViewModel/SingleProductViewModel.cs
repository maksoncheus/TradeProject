using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using TradeProject.Model.DBO;
using TradeProject.Model.Utils;
using System.Reflection;
using System.IO;
using TradeProject.Model;
using Microsoft.EntityFrameworkCore;

namespace TradeProject.ViewModel
{
    internal delegate void Message(string? message);
    internal class SingleProductViewModel : PropertyChangedBase
    {
        internal event Message Message;
        private bool allPropertiesFilled;
        private int _selectedCategory;
        private bool _picChanged;
        private string _categoryText;
        private bool _isCategoryTextVisible;
        private ObservableCollection<string> _categories;
        private Visibility _visibility;
        //TODO: Check for admin rights
        public bool CanEdit
        {
            get => true;
        }
        public bool IsCategoryTextVisible
        {
            get => _isCategoryTextVisible;
            set
            {
                _isCategoryTextVisible = value;
                OnPropertyChanged(nameof(IsCategoryTextVisible));
            }
        }
        public string CategoryText
        {
            get => _categoryText;
            set
            {
                _categoryText = value;
                OnPropertyChanged(nameof(CategoryText));
            }
        }
        public ObservableCollection<string> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }
        public bool AllPropertiesFilled
        {
            get => allPropertiesFilled;
            set
            {
                allPropertiesFilled = value;
                OnPropertyChanged(nameof(AllPropertiesFilled));
            }
        }
        public int SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if(value == Categories.Count-1)
                {
                    IsCategoryTextVisible = true;
                }
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }
        public bool PictureChanged
        {
            get => _picChanged;
            set
            {
                _picChanged = value;
                OnPropertyChanged(nameof(PictureChanged));
            }
        }
        private BitmapImage _ImageSource;
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set
            {
                _ImageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }
        private bool isAdding;
        private SingleProductModel _model;
        public Product CurrentProduct
        {
            get; set;
        }
        public SingleProductViewModel(Product? prod)
        {
            _model = new();
            _isCategoryTextVisible = false;
            _categoryText = "";
            using TradeDbContext context = new();
            if (prod != null)
            {
                prod = context.Products.First(x => x.ArticleNumber == prod.ArticleNumber);
            }
            Categories = new ObservableCollection<string>(from cat in context.Categories select cat.Name)
            {
                "Новая..."
            };
            SelectedCategory = prod?.Category.Id - 1 ?? -1;
            Visibility = Visibility.Visible;
            if (prod == null)
            {
                CurrentProduct = new Product() { Photo = "picture.png" };
                Visibility = Visibility.Hidden;
                isAdding = true;
            }
            else
            {
                CurrentProduct = prod;
                isAdding = false;
            }
        }
        public CommonCommand SaveChanges
        {
            get => new CommonCommand(() =>
            {
                using TradeDbContext context = new();
                if(_isCategoryTextVisible && !string.IsNullOrEmpty(CategoryText))
                {
                    context.Categories.Add(new Category() { Name = CategoryText });
                    context.SaveChanges();
                    _selectedCategory = context.Categories.OrderBy(cat => cat.Id).Last().Id - 1;
                }
                if (isAdding)
                {
                    Product pr = new Product();
                    foreach (PropertyInfo property in typeof(Product).GetProperties().Where(p => p.CanWrite))
                    {
                        property.SetValue(pr, property.GetValue(CurrentProduct, null), null);
                    }
                    pr.Category = context.Categories.Where(c => c.Id == _selectedCategory + 1).First();
                    if (!PictureChanged) pr.Photo = null;
                    else
                    {
                        if (!_model.IsPathInResources(ImageSource.UriSource))
                            _model.CopyImageToResources(ImageSource.UriSource.ToString());
                        pr.Photo = Path.GetFileName(ImageSource.UriSource.ToString());
                    }
                    context.Products.Add(pr);
                }
                else
                {
                    Product pr = context.Products.Where(pr => pr.ArticleNumber == CurrentProduct.ArticleNumber).First();
                    foreach (PropertyInfo property in typeof(Product).GetProperties().Where(p => p.CanWrite))
                    {
                        property.SetValue(pr, property.GetValue(CurrentProduct, null), null);
                    }
                    pr.Category = context.Categories.Where(c => c.Id == SelectedCategory + 1).First();
                    if (PictureChanged)
                    {
                        if (!_model.IsPathInResources(ImageSource.UriSource))
                            _model.CopyImageToResources(ImageSource.UriSource.ToString());
                        pr.Photo = Path.GetFileName(ImageSource.UriSource.ToString());
                    }
                }
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (Message != null)
                        Message?.Invoke(ex.Message);
                }
            }
            , () => true);
        }
        public CommonCommand RemoveProduct
        {
            get => new CommonCommand(() =>
            {
                using TradeDbContext context = new();
                context.Products.Remove(context.Products.Where(pr => pr.ArticleNumber == CurrentProduct.ArticleNumber).First());
                context.SaveChanges();
            }
            , () => true);
        }
        public CommonCommand ChangeProductPicture
        {
            get => new CommonCommand(() =>
            {
                BitmapImage pic = _model.SetPicture();
                if (pic != null)
                {
                    ImageSource = pic;
                    PictureChanged = true;
                }
            });
        }
    }
}

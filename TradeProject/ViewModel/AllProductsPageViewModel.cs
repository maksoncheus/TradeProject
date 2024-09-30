using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TradeProject.Model.DBO;
using TradeProject.Model.Utils;
using TradeProject.Model;
using TradeProject.View;

namespace TradeProject.ViewModel
{
    internal class AllProductsPageViewModel : PropertyChangedBase
    {
        private ObservableCollection<string> _manufacturers;
        private ObservableCollection<Product> _products;
        private int _manufacturerSelectedIndex;
        private string _manufacturerSelectedName;
        private int _costSortingIndex;
        private string _searchString;
        private int _currentShowedProducts;
        private int _currentPage;
        private int _totalProducts;
        private ObservableCollection<PageModel> _pages;
        private AllProductsPageModel _model;
        public ObservableCollection<string> Manufacturers
        {
            get => _manufacturers;
            set
            {
                _manufacturers = value;
                OnPropertyChanged(nameof(Manufacturers));
            }
        }
        public int ManufacturerSelectedIndex
        {
            get => _manufacturerSelectedIndex;
            set
            {
                _manufacturerSelectedIndex = value;
                OnPropertyChanged(nameof(ManufacturerSelectedIndex));
            }
        }
        public string ManufacturerSelectedName
        {
            get => _manufacturerSelectedName;
            set
            {
                _manufacturerSelectedName = value;
                OnPropertyChanged(nameof(ManufacturerSelectedName));
                CurrentPage = 1;
                ApplyFilters();
            }
        }
        public int CostSortingIndex
        {
            get => _costSortingIndex;
            set
            {
                _costSortingIndex = value;
                OnPropertyChanged(nameof(CostSortingIndex));
                CurrentPage = 1;
                ApplyFilters();
            }
        }
        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged(nameof(SearchString));
                CurrentPage = 1;
                ApplyFilters();
            }
        }
        public int CurrentShowedProducts
        {
            get => _currentShowedProducts;
            set
            {
                _currentShowedProducts = value;
                OnPropertyChanged(nameof(CurrentShowedProducts));
            }
        }
        public int TotalProducts
        {
            get => _totalProducts;
            set
            {
                _totalProducts = value;
                OnPropertyChanged(nameof(TotalProducts));
            }
        }
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                ApplyFilters();
            }
        }
        public ObservableCollection<PageModel> Pages
        {
            get => _pages;
            set
            {
                _pages = value;
                OnPropertyChanged(nameof(Pages));
            }
        }
        public ObservableCollection<Product> ProductCollection
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(ProductCollection));
                TotalProducts = ProductCollection.Count;
                ApplyFilters();
            }
        }
        public AllProductsPageViewModel()
        {
            _model = new AllProductsPageModel();
            ProductCollection = _model.GetAllProducts();
            Manufacturers = _model.GetManufacturers();
            ManufacturerSelectedIndex = 0;
            ManufacturerSelectedName = "Все производители";
            CostSortingIndex = 0;
            CurrentPage = 1;
            Pages = _model.GetPages(TotalProducts, CurrentPage);
            SearchString = string.Empty;
        }
        public void ApplyFilters()
        {
            _model.ApplyFilterSortingSearch(
                ref _products,
                (ManufacturerSelectedIndex, ManufacturerSelectedName),
                CostSortingIndex,
                SearchString,
                CurrentPage,
                out _currentShowedProducts);
            Pages = _model.GetPages(CurrentShowedProducts, CurrentPage);
            OnPropertyChanged(nameof(ProductCollection));
            OnPropertyChanged(nameof(CurrentShowedProducts));
        }
        public CommonCommand<object> ChangePage
        {
            get => new CommonCommand<object>
                (sender =>
                {
                    CurrentPage = (int)sender;
                }
                );
        }
        public CommonCommand<object> OpenProductPage
        {
            get => new CommonCommand<object>(sender =>
            {
                SingleProductView productWindow = new((Product)sender);
                productWindow.ShowDialog();
                ProductCollection = _model.GetAllProducts();
                _model.DeleteUnusedPictures();
            }
            );
        }
        public CommonCommand<object> OpenAddProductPage
        {
            get => new CommonCommand<object>(sender =>
            {
                SingleProductView productWindow = new(null);
                productWindow.ShowDialog();
                ProductCollection = _model.GetAllProducts();
                _model.DeleteUnusedPictures();
            }
            );
        }
    }
}

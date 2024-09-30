using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using TradeProject.Model.DBO;

namespace TradeProject.Model
{
    internal class AllProductsPageModel
    {
        private int productsPerPage = 5;
        public ObservableCollection<string> GetManufacturers()
        {
            using TradeDbContext context = new();
            List<string> mf =
            [
                "Все производители",
                .. (
                            from product in context.Products
                            select product.Manufacturer
                        ).Distinct(),
            ];
            return new ObservableCollection<string>(mf);
        }
        public ObservableCollection<Product> GetAllProducts()
        {
            using TradeDbContext context = new();
            return new ObservableCollection<Product>(context.Products);
        }
        /// <summary>
        /// Метод для последовательного применения фильтров, сортировки и поиска
        /// </summary>
        /// <param name="products">Коллекция продуктов</param>
        /// <param name="manufacturerInfo">Информация о производителе в формате (selectedIndex, manufacturerName)</param>
        /// <param name="costSortingIndex">0 - нет сортировки, 1 - сортировка по возрастанию, 2 - сортировка по убыванию</param>
        /// <param name="searchString">Строка поиска</param>
        public void ApplyFilterSortingSearch(ref ObservableCollection<Product> products, (int Index, string Name) manufacturerInfo, int costSortingIndex, string searchString, int page, out int CurrentShowedProducts)
        {
            products = GetAllProducts();
            ApplyManufacturerFilter(ref products, manufacturerInfo);
            ApplyCostSorting(ref products, costSortingIndex);
            ApplySearch(ref products, searchString);
            CurrentShowedProducts = products.Count;
            ChangePage(ref products, page);
        }
        private void ApplyManufacturerFilter(ref ObservableCollection<Product> products, (int Index, string Name) manufacturerInfo)
        {
            using TradeDbContext context = new();
            if (manufacturerInfo.Index != 0)
            {
                products = new ObservableCollection<Product>
                    (from product in products where product.Manufacturer == manufacturerInfo.Name select product);
            }
        }
        private void ApplyCostSorting(ref ObservableCollection<Product> products, int costSortingIndex)
        {
            using TradeDbContext context = new();
            products = costSortingIndex switch
            {
                0 => products,
                1 => new ObservableCollection<Product>(from product in products orderby product.Cost ascending select product),
                2 => new ObservableCollection<Product>(from product in products orderby product.Cost descending select product),
                _ => throw new ArgumentException("Недопустимый индекс для сортировки по цене", nameof(costSortingIndex)),
            };
        }
        private void ApplySearch(ref ObservableCollection<Product> products, string searchString)
        {
            using TradeDbContext context = new();
            if (!String.IsNullOrEmpty(searchString))
            {
                Regex regex = new Regex(searchString.ToLower());
                PropertyInfo[] propertyInfo = typeof(Product).GetProperties();
                products = new ObservableCollection<Product>(from product in products
                                                             where propertyInfo.Any(x => regex.IsMatch(Convert.ToString(x.GetValue(product)).ToLower()))
                                                             select product);
            }
        }
        private void ChangePage(ref ObservableCollection<Product> products, int page)
        {
            products = new ObservableCollection<Product>(products.Skip((page - 1) * productsPerPage).Take(productsPerPage));
        }

        public ObservableCollection<PageModel> GetPages(int totalCount, int currentPage)
        {
            ObservableCollection<PageModel> pages = new();
            int pagesCount = totalCount / productsPerPage;
            if (totalCount % productsPerPage != 0)
                pagesCount++;
            for (int i = 1; i <= pagesCount; i++)
            {
                pages.Add(new PageModel() { PageNumber = i, IsUnChecked = (currentPage != i) });
            }
            return pages;
        }

        public void DeleteUnusedPictures()
        {
            using TradeDbContext context = new TradeDbContext();
            string pathToRes = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\res\\";
            Directory.GetFiles(pathToRes).Except(context.Products.Select(x => pathToRes + x.Photo)).Where(x => x != pathToRes + ApplicationContext.MOCK_PICTURE_NAME).ToList().ForEach(x => File.Delete(x));
        }
    }
}

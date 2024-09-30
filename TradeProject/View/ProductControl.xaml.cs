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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TradeProject.Model.DBO;

namespace TradeProject.View
{
    /// <summary>
    /// Логика взаимодействия для ProductControl.xaml
    /// </summary>
    public partial class ProductControl : UserControl
    {
        public Product Product
        {
            get { return (Product)GetValue(ProductProperty); }
            set { SetValue(ProductProperty, value); }
        }
        public static readonly DependencyProperty ProductProperty
            = DependencyProperty.Register(
                  "Product",
                  typeof(Product),
                  typeof(ProductControl),
                  new PropertyMetadata(null)
              );
        public ProductControl()
        {
            InitializeComponent();
        }
    }
}

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
using TradeProject.Model.DBO;
using TradeProject.ViewModel;

namespace TradeProject.View
{
    /// <summary>
    /// Логика взаимодействия для SingleProductView.xaml
    /// </summary>
    public partial class SingleProductView : Window
    {
        private SingleProductViewModel viewModel;
        public SingleProductView(Product prod)
        {
            InitializeComponent();
            DataContext = viewModel = new SingleProductViewModel(prod);
            viewModel.Message += ShowMessage;
        }
        private void ShowMessage(string? message)
        {
            if (message == null)
            {
                MessageBox.Show("Пришло сообщение без текста. Что-то пошло не так...");
            }
            else
            {
                MessageBox.Show(message);
            }
        }
    }
}

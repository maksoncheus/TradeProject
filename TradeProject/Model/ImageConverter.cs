using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TradeProject.Model
{
    internal class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                          object parameter, CultureInfo culture)
        {
            try
            {
                return new BitmapImage(new Uri($"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\res\\" + ((string?)value ?? "picture.png")));
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

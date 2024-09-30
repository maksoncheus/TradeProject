using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace TradeProject.Model
{
    internal class SingleProductModel
    {
        private readonly string _pathToRes = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\res\\";
        public BitmapImage SetPicture()
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            dialog.CheckPathExists = true;
            dialog.InitialDirectory = Path.GetFullPath("res\\");
            bool isSelected = dialog.ShowDialog().Value;
            if (isSelected)
            {
                if (Path.GetFileName(dialog.FileName) != ApplicationContext.MOCK_PICTURE_NAME)
                {
                    if (!Directory.GetFiles(_pathToRes).Any(x => Path.GetFileName(x) == Path.GetFileName(dialog.FileName)) || IsPathInResources(new Uri(dialog.FileName)))
                    {
                        BitmapImage selectedImage = new BitmapImage(new Uri(dialog.FileName));
                        if (selectedImage.PixelWidth > 300 || selectedImage.PixelHeight > 200)
                            throw new Exception("Разрешение файла не должно превышать 300x200 пикселей!");
                        else
                            return selectedImage;
                    }
                    throw new Exception($"Файл с именем '{Path.GetFileName(dialog.FileName)}' уже существует в базе данных. Переименуйте файл");
                }
                else
                {
                    throw new Exception($"ВНИМАНИЕ!!! имя '{ApplicationContext.MOCK_PICTURE_NAME}' используется для изображения-заглушки товаров без указанных фото. Переименуйте файл и попытайтесь снова");
                }
            }
            return null;
        }
        public bool IsPathInResources(Uri path) => path.ToString().Replace("file://", "").Replace('/', '\\') == _pathToRes + Path.GetFileName(path.ToString());
        public void DeleteOldImage(string path) => File.Delete(_pathToRes + path);
        public void CopyImageToResources(string initPath) => File.Copy(@initPath.Replace("file://", ""), _pathToRes + Path.GetFileName(initPath));
    }
}

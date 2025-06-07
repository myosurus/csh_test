using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TWpfApp.Models;
using TWpfApp.Services;

namespace ImageDesktopApp
{
    public partial class MainWindow : Window
    {
        private readonly ImageService _imageService = new();
        public ObservableCollection<ImageItem> Images { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadImages();
            ImageTable.SelectionChanged += (s, e) => UpdateButton.IsEnabled = ImageTable.SelectedItem != null;
        }

        private async void LoadImages()
        {
            try
            {
                var images = await _imageService.GetImagesAsync();
                Images.Clear();
                foreach (var image in images)
                {
                    image.ImageSource = LoadImage(image.Data);
                    Images.Add(image);
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка загрузки изображений", ex);
            }
        }

        private BitmapImage LoadImage(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            using var ms = new MemoryStream(data);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private string? OpenImageFileDialog(string title)
        {
            var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private void ShowError(string message, Exception ex) =>
            MessageBox.Show($"{message}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

        private async void AddImage_Click(object sender, RoutedEventArgs e)
        {
            var filePath = OpenImageFileDialog("Добавить изображение");
            if (filePath == null) return;

            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                var image = new ImageItem { Name = Path.GetFileName(filePath), Data = data };
                if (await _imageService.AddImageAsync(image))
                    LoadImages();
                else
                    MessageBox.Show("Ошибка при добавлении изображения.", "Ошибка");
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при добавлении изображения", ex);
            }
        }

        private async void EditImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageTable.SelectedItem is not ImageItem selectedImage) return;

            var filePath = OpenImageFileDialog("Выберите новое изображение");
            if (filePath == null) return;

            try
            {
                if (await _imageService.UpdateImageAsync(selectedImage.Id, filePath))
                    LoadImages();
                else
                    MessageBox.Show("Ошибка при обновлении изображения.", "Ошибка");
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при обновлении", ex);
            }
        }

        private async void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageTable.SelectedItem is not ImageItem selectedImage) return;

            try
            {
                if (await _imageService.DeleteImageAsync(selectedImage.Id))
                    LoadImages();
                else
                    MessageBox.Show("Ошибка при удалении изображения.", "Ошибка");
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при удалении", ex);
            }
        }
    }
}

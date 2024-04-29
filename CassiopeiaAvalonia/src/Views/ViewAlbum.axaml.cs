using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Cassiopeia.src.Model;
using System.IO;

namespace Cassiopeia.src.Views
{
    public partial class ViewAlbum : Window
    {
        public ViewAlbum()
        {
            InitializeComponent();
            Loaded += ViewAlbum_Loaded;

        }

        private void ViewAlbum_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            AlbumData album = DataContext as AlbumData;
            var file = File.OpenRead(album.CoverPath);
            Bitmap image = Bitmap.DecodeToWidth(file, 400);
            imageCover.Source = image;
        }
    }
}

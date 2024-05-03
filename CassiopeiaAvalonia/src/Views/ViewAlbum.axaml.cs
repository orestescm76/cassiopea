using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Cassiopeia.src.Model;
using Cassiopeia.src.VM;
using System;
using System.IO;
using System.Resources;

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
            AlbumVM album = DataContext as AlbumVM;
            album.Album.Title += " - TEST";
            try
            {
                var file = File.OpenRead(album.Album.CoverPath);
                Bitmap image = Bitmap.DecodeToWidth(file, 400);
                imageCover.Source = image;

            }
            catch (Exception)
            {
                var cover = new Bitmap(AssetLoader.Open(new Uri("avares://Cassiopeia/Assets/unknown_album.png")));
                imageCover.Source = cover;
            }
        }
    }
}

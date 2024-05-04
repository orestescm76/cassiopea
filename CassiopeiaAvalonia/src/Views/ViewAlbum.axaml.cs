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
            datagridSongs.SelectionChanged += DatagridSongs_SelectionChanged;
            textDurationSelected.Text = Kernel.LocalTexts.GetString("dur_total") + ": 00:00:00";
            
        }

        private void DatagridSongs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            TimeSpan duration = TimeSpan.Zero;
            foreach (var song in datagridSongs.SelectedItems)
            {
                Song s = song as Song;
                duration += s.Length;
            }
            textDurationSelected.Text = Kernel.LocalTexts.GetString("dur_total") + ": " + duration.ToString();
        }

        private void ViewAlbum_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            AlbumVM album = DataContext as AlbumVM;
            try
            {
                var file = File.OpenRead(album.Album.CoverPath);
                Bitmap image = Bitmap.DecodeToWidth(file, 800);
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

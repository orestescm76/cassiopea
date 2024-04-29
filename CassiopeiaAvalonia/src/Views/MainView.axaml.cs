using Avalonia.Controls;
using Cassiopeia.src.Views;

namespace CassiopeiaAvalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        dataGridAlbums.DoubleTapped += DataGridAlbums_DoubleTapped;
    }

    private void DataGridAlbums_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var album = dataGridAlbums.SelectedItem;
        ViewAlbum viewAlbumWindow = new ViewAlbum();
        viewAlbumWindow.DataContext = album;
        viewAlbumWindow.Show();
    }
}

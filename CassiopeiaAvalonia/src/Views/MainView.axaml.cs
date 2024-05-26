using Avalonia.Controls;
using Cassiopeia.src.Model;
using Cassiopeia.src.Views;
using Cassiopeia.VM;

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
        MainVM vm = this.DataContext as MainVM;
        vm.OpenViewAlbum(dataGridAlbums.SelectedItem as AlbumData);
    }
}

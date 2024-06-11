using Avalonia.Controls;
using Avalonia.Input;
using System.Diagnostics;
using System.Reflection;

namespace Cassiopeia.Views
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }
        public void TextBlock_PointerPressed(object? sender, PointerPressedEventArgs args)
        {
            Process.Start(new ProcessStartInfo("https://github.com/orestescm76/cassiopeia") { UseShellExecute=true });
        }
    }
}

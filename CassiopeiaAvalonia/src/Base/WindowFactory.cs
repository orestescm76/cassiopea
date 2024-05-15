using Avalonia.Controls;
using Cassiopeia.src.Interfaces;
using Cassiopeia.src.Model;
using Cassiopeia.src.ViewModels;
using Cassiopeia.src.Views;
using Cassiopeia.src.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassiopeia.Base
{
    public class WindowFactory : IWindowFactory
    {
        private readonly Dictionary<Type, Func<Window>> _registeredWindows;

        public WindowFactory()
        {
            _registeredWindows = new Dictionary<Type, Func<Window>>();
        }
        public void RegisterWindow<TWindow>() where TWindow : Window, new()
        {
            _registeredWindows.Add(typeof(TWindow), () => new TWindow());
        }

        public Window CreateWindow<TViewModel>(Type window) where TViewModel : BaseVM, new()
        {
            if (_registeredWindows.TryGetValue(window, out var creator))
            {
                var newWindow = creator();
                newWindow.DataContext = new TViewModel();
                return newWindow;
            }
            else return null;
        }
        public Window CreateWindow<TViewModel>(ref AlbumData album) where TViewModel : AlbumVM
        {
            if (_registeredWindows.TryGetValue(typeof(ViewAlbum), out var creator))
            {
                var newWindow = creator();
                newWindow.DataContext = Activator.CreateInstance(typeof(AlbumVM), album);
                return newWindow;
            }
            else return null;
        }
    }
}

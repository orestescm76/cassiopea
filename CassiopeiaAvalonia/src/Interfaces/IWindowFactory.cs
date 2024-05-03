using Avalonia.Controls;
using Cassiopeia.src.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassiopeia.src.Interfaces
{
    internal interface IWindowFactory
    {
        Window CreateWindow<TViewModel>(Type window) where TViewModel : BaseVM, new();
    }
}

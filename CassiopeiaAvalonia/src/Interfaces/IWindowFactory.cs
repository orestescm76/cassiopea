using Avalonia.Controls;
using Cassiopeia.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassiopeia.Interfaces
{
    internal interface IWindowFactory
    {
        Window CreateWindow<TViewModel>(Type window) where TViewModel : BaseVM, new();
    }
}

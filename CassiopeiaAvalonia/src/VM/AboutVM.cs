using Cassiopeia.src.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cassiopeia.VM
{
    public class AboutVM : BaseVM
    {
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    }
}

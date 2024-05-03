using Cassiopeia.src.Model;
using Cassiopeia.src.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassiopeia.src.VM
{
    //viewmodel for editing and viewing model
    public class AlbumVM : BaseVM
    {
        public AlbumData Album { get; set; }
        public List<Song> Songs {get { return Album.Songs; } }
        public string Description { get { return "Test"; } }
        public AlbumVM()
        {
            Album = new AlbumData();
        }
        public AlbumVM(ref AlbumData album)
        {
            Album = album;
        }
    }
}

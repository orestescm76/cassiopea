using Cassiopeia.src.Model;
using Cassiopeia.src.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;

namespace Cassiopeia.VM
{
    //viewmodel for editing and viewing model
    public class AlbumVM : BaseVM
    {
        private enum AlbumInfo
        {
            Artist = 0,
            Title = 1,
            Year = 2,
            Length = 3,
            Genre = 4,
            Type = 5,
            Location = 6,
            Format = 6,
            PublishYear = 7,
            PublishCountry = 8,
            CoverWear = 9,
            MediaWear = 10
        }
        public AlbumData Album { get; set; }
        public List<Song> Songs {get { return Album.Songs; } }
        public string Description { get => GetDescription();  }

        private string GetDescription()
        {
            //retuns the complete description
            var bonusLen = Album.BonusLength;
            string[] data = new string[10];
            string res = string.Empty;
            data[(int)AlbumInfo.Artist] = Kernel.LocalTexts.GetString("artista") + ": " + Album.Artist + Environment.NewLine;
            data[(int)AlbumInfo.Title] = Kernel.LocalTexts.GetString("titulo") + ": " + Album.Title + Environment.NewLine;
            data[(int)AlbumInfo.Year] = Kernel.LocalTexts.GetString("año") + ": " + Album.Year + Environment.NewLine;
            if (bonusLen.TotalMilliseconds != 0)
                data[(int)AlbumInfo.Length] = Kernel.LocalTexts.GetString("duracion") + ": " + Album.Length.ToString() + " (" + bonusLen.ToString() + ")" + Environment.NewLine;
            else
                data[(int)AlbumInfo.Length] = Kernel.LocalTexts.GetString("duracion") + ": " + Album.Length.ToString() + Environment.NewLine;
            for (int i = 0; i < 3; i++)
            {
                res += data[i];
            }
            return res;
        }

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

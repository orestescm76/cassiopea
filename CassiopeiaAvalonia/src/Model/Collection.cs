﻿using System;
using System.Linq;
using System.Collections.Generic;
using Cassiopeia.VM;
using System.Collections.ObjectModel;
using Cassiopeia.Base;
using Cassiopeia;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Avalonia;
using Avalonia.Threading;

namespace Cassiopeia.Model
{
    public class Collection : NotifyBase
    {
        public ObservableCollection<AlbumData> Albums { get; set; }
       // public List<AlbumData> FilteredAlbums { get; set; }
        public List<CompactDisc> CDS { get; private set; }
        public List<VinylAlbum> Vinyls { get; private set; }
        public List<CassetteTape> Tapes { get; private set; }
        public ObservableCollection<AlbumData> FilteredAlbums { get; set; } 
        private string _filter;

        public string Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value); FilterData(); }
        }

        private void FilterData()
        {
            if (Filter is null)
                return;
            IEnumerable<AlbumData> query = Albums;
            HashSet<AlbumData> albumContainsSong = new();
            albumContainsSong = Utils.GetAlbumsWithSongTitle(query.ToList(), Filter);
            query = from album in query where album.ID.ToLower().Contains(Filter) || album.ID.ToLower().Contains(Filter) select album;
            List<AlbumData> list = query.ToList();
            list.AddRange(albumContainsSong);
            FilteredAlbums = new ObservableCollection<AlbumData>(
                list
            );
            OnPropertyChanged(nameof(FilteredAlbums)); // Notificar el cambio
        }

        public Collection()
        {
            Albums = new ObservableCollection<AlbumData>();
            CDS = new List<CompactDisc>();
            Vinyls = new();
            Tapes = new();
        }
        public bool AddAlbum(ref AlbumData album)
        {
            try
            {
                Albums.Add(/*album.Artist + Kernel.SearchSeparator + album.Title,*/ album);
            }
            catch (ArgumentException)
            {
                Log.Instance.PrintMessage("already added!", MessageType.Warning);
                Log.Instance.PrintMessage(album.Artist + " - " + album.Title, MessageType.Warning);
                return false;
            }
            return true;
        }
        public void RemoveAlbum(ref AlbumData album)
        {
            //if (album.CanBeRemoved)
            //    Albums.Remove(album.Artist + Kernel.SearchSeparator + album.Title);
            //else
            //    throw new InvalidOperationException(album.Artist + Kernel.SearchSeparator + album.Title);
        }
        public List<AlbumData> SearchAlbum(string title)
        {
            List<AlbumData> encontrados = new List<AlbumData>();
            encontrados = Albums.Where(pair => pair.Title.Contains(title)).Select(pair => pair).ToList();
            //foreach (AlbumData a in Albums)
            //{
            //    if (a.Title == title)
            //        encontrados.Add(a);
            //}
            return encontrados;
        }
        //public bool IsInCollection(AlbumData referenceAlbum)
        //{
        //    return Albums.Contains(referenceAlbum);
        //    //foreach (AlbumData album in Albums)
        //    //{
        //    //    if (album == referenceAlbum)
        //    //        return true;
        //    //}
        //    //return false;
        //}

        //public AlbumData GetAlbum(string s) //s is equal to Black Sabbath/**/Paranoid
        //{
        //    return Albums[s];

        //    //String[] busqueda = s.Split(Kernel.SearchSeparator);

        //    //foreach (AlbumData album in Albums)
        //    //{
        //    //    if (album.Artist == busqueda[0] && album.Title == busqueda[1])
        //    //        return album;
        //    //}

        //    //return null;
        //}
        //public AlbumData GetAlbum(int index, bool filtered)
        //{
        //    if (!filtered)
        //        return Albums.Values.ToArray()[index];
        //    else
        //        return FilteredAlbums[index];
        //}
        //public void GetAlbum(string s, out CompactDisc cd)
        //{
        //    cd = null;
        //    String[] busqueda = s.Split(Kernel.SearchSeparator);
        //    foreach (CompactDisc cdd in CDS)
        //    {
        //        if (cdd.Album.Artist == busqueda[0] && cdd.Album.Title == busqueda[1])
        //            cd = cdd;
        //    }
        //}
        //public void ChangeAlbums(ref Dictionary<string, AlbumData> n)
        //{
        //    Albums = n;
        //}
        public void Clear()
        {
            Albums.Clear();
            CDS.Clear();
        }
        public void AddCD(ref CompactDisc cd)
        {
            CDS.Add(cd);
        }
        public CompactDisc GetCDById(string i)
        {
            CompactDisc busqueda = null;
            for (int j = 0; j < CDS.Count; j++)
            {
                if (i == CDS[j].Id)
                    busqueda = CDS[j];
            }
            return busqueda;
        }
        public void DeleteCD(String id)
        {
            foreach (CompactDisc item in CDS)
            {
                if (item.Id == id)
                {
                    CDS.Remove(item);
                    ReleaseLock(item.Album);
                    return;
                }
            }
        }
        public void DeleteCD(ref CompactDisc cd)
        {
            CDS.Remove(cd);
        }
        public void DeleteVinyl(string id)
        {
            foreach (var v in Vinyls)
            {
                if(v.Id == id)
                {
                    Vinyls.Remove(v);
                    ReleaseLock(v.Album);
                    return;
                }
            }
        }
        private void ReleaseLock(AlbumData a)
        {
            //First release the lock
            a.CanBeRemoved = true;
            foreach (var cd in CDS)
            {
                //If one copy refers that album, we cannot remove
                if (cd.Album == a)
                    a.CanBeRemoved = false;
            }
            foreach (var vinyl in Vinyls)
            {
                if (vinyl.Album == a)
                    a.CanBeRemoved = false;
            }
        }
        public void AddVinyl(ref VinylAlbum v)
        {
            Vinyls.Add(v);
            v.Album.CanBeRemoved = false;
        }
        public void AddTape(ref CassetteTape t)
        {
            Tapes.Add(t);
            t.Album.CanBeRemoved = false;
        }
        public VinylAlbum GetVinylByID(string id)
        {
            for (int i = 0; i < Vinyls.Count; i++)
            {
                if (Vinyls[i].Id == id)
                    return Vinyls[i];
            }
            return null;
        }
        public CassetteTape GetTapeByID(string id)
        {
            for (int i = 0; i < Tapes.Count; i++)
            {
                if (Tapes[i].Id == id)
                    return Tapes[i];
            }
            return null;
        }
        public TimeSpan GetTotalTime(List<AlbumData> albums)
        {
            TimeSpan time = new TimeSpan();
            foreach (AlbumData album in albums)
            {
                time += album.Length;
            }
            return time;
        }
        public TimeSpan GetTotalTime(List<CompactDisc> CDS)
        {
            TimeSpan time = new TimeSpan();
            foreach (CompactDisc cd in CDS)
            {
                time += cd.Length;
            }
            return time;
        }
        public TimeSpan GetTotalTime(List<VinylAlbum> Vinyl)
        {
            TimeSpan time = new TimeSpan();
            foreach (var v in Vinyl)
            {
                time += v.Length;
            }
            return time;
        }
        public List<AlbumData> GetCDAlbums()
        {
            HashSet<AlbumData> albums = new HashSet<AlbumData>();
            foreach (var cd in CDS)
            {
                albums.Add(cd.Album);
            }
            return albums.ToList();
        }
        public List<AlbumData> GetVinylAlbums()
        {
            HashSet<AlbumData> albums = new HashSet<AlbumData>();
            foreach (var vinyl in Vinyls)
            {
                albums.Add(vinyl.Album);
            }
            return albums.ToList();
        }
    }
}

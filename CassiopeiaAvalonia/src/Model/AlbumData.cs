﻿using Cassiopeia.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Cassiopeia.Model
{
    public enum AlbumType
    {
        Studio,
        Live,
        Compilation,
        EP,
        Single
    }
    public partial class AlbumData : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _artist;
        [ObservableProperty]
        private string _title;
        public short Year { get; set; }
        internal Genre Genre { get; set; }

        internal List<Song> Songs { get; set; }

        internal String ID { get => Artist + " " + Title; }
        internal String IdSpotify { get; set; }

        internal String CoverPath { get; set; }
        internal String SoundFilesPath { get; set; }
        public AlbumType Type { get; set; }
       // public string Key { get => Artist + Kernel.SearchSeparator + Title; }

        [JsonIgnore] internal int NumberOfSongs { get { return Songs.Count; } }
        [JsonIgnore] public TimeSpan Length { get => GetLength(false); }
        [JsonIgnore] internal TimeSpan BonusLength { get => GetLength(true); }
        [JsonIgnore] internal bool CanBeRemoved { get; set; }

        public AlbumData()
        {
            //example album
            Artist = "Nektar";
            Title = "Remember the future";
            Year = 1973;
            Songs = new List<Song> { 
                new("Part 1", 16*60*1000+40*1000, false),
                new("Part 2", 18*60*1000+59*1000, false)
            };
            CoverPath = Environment.CurrentDirectory + "\\" + "Remember The Future_Nektar.jpg";
            //Genre = Kernel.Genres.Last();
        }

        public AlbumData(Genre genre, string title = "", string artist = "", short year = 0, string coverPath = "")
        {
            Title = title;
            Artist = artist;
            Year = year;
            Songs = new List<Song>();
            CoverPath = coverPath;
            Genre = genre;
            CanBeRemoved = true;
        }

        public AlbumData(string title = "", string artist = "", short year = 0, string coverPath = "")
        {
            Title = title;
            Artist = artist;
            Year = year;
            CoverPath = coverPath;
            Genre = new Genre("");
            Songs = new List<Song>();
            CanBeRemoved = true;
        }

        public AlbumData(AlbumData other)
        {
            Title = other.Title;
            Artist = other.Artist;
            Year = other.Year;
            Songs = other.Songs;
            CoverPath = other.CoverPath;
            CanBeRemoved = true;
        }

        //---SONGS MANAGEMENT---
        public void AddSong(Song song)
        {
            Songs.Add(song);
            song.SetAlbum(this);
        }

        public void AddSong(Song song, int index)
        {
            Songs.Insert(index, song);
            song.SetAlbum(this);
        }

        public void RemoveSong(string title)
        {
            Song song = GetSong(title);
            Songs.Remove(song);
        }

        public Song GetSong(string title)
        {
            Song song = null;

            foreach (Song s in Songs)
            {
                if (s.Title.Equals(title))
                {
                    song = s;
                    break;
                }
            }

            return song;
        }

        public Song GetSong(int n)
        {
            return Songs[n];
        }

        public int GetSongPosition(string title)
        {
            int songPos = -1;

            for (int i = 0; i < Songs.Count; i++)
            {
                if (Songs[i].Title.Equals(title))
                {
                    songPos = i;
                    break;
                }
            }

            return songPos;
        }

        //---DATA---
        private TimeSpan GetLength(bool bonus)
        {
            TimeSpan length = new TimeSpan();
            TimeSpan lengthBonus = new TimeSpan();
            foreach (Song song in Songs)
            {
                if (!song.IsBonus) //If this song is bonus don't add it
                    length += song.Length;
                else
                    lengthBonus += song.Length; //If we want the total bonus song length we would add it here
            }
            if (!bonus)
                return length;
            return lengthBonus;
        }

        public override string ToString()
        {
            //Returns whatever the clipboard string is.
            return string.Empty;//ToClipboard();
        }
        //public static bool operator ==(AlbumData a, AlbumData b)
        //{
        //    if (a.Artist == b.Artist && a.Title == b.Title)
        //        return true;
        //    return false;
        //}
        //public static bool operator !=(AlbumData a, AlbumData b)
        //{
        //    if (a.Artist != b.Artist && a.Title != b.Title)
        //        return true;
        //    return false;
        //}
        public String[] ToStringArray()
        {
            String[] datos = { Artist, Title, Year.ToString(), Length.ToString(), Genre.Name };
            return datos;
        }

        public String GetSpotifySearchLabel()
        {
            return Artist + " " + Title;
        }

        //public string ToClipboard()
        //{
        //    string clipboardText = Config.Clipboard.Replace("%artist%", Artist); //Es seguro.

        //    try
        //    {
        //        clipboardText = clipboardText.Replace("%title%", Title);
        //        clipboardText = clipboardText.Replace("%year%", Year.ToString());
        //        clipboardText = clipboardText.Replace("%genre%", Genre.Name);
        //        clipboardText = clipboardText.Replace("%length%", Length.ToString());
        //        clipboardText = clipboardText.Replace("%length_seconds%", ((int)Length.TotalSeconds).ToString());
        //        clipboardText = clipboardText.Replace("%length_seconds%", ((int)Length.TotalSeconds).ToString());
        //        clipboardText = clipboardText.Replace("%length_min%", Length.TotalMinutes.ToString());
        //        clipboardText = clipboardText.Replace("%totaltracks%", NumberOfSongs.ToString());
        //        clipboardText = clipboardText.Replace("%path%", SoundFilesPath);
        //        return clipboardText;
        //    }
        //    catch (NullReferenceException)
        //    {
        //        return clipboardText;
        //    }
        //}

        public bool NeedsMetadata()
        {
            return string.IsNullOrEmpty(Artist) || string.IsNullOrEmpty(Title);
        }
    }
}
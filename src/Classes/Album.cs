﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace aplicacion_musica
{
    public class Album
    {
        public String nombre { get; set; }
        public String artista { get; set; }
        public short year { get; set; }
        [JsonIgnore]
        public short numCanciones { get; set; }
        [JsonIgnore]
        public TimeSpan duracion { get; set; }
        public List<Cancion> canciones { get; set; }
        public String caratula { get; set; }
        public Genre genero { get; set; }
        public String IdSpotify { get; set; }
        public String DirectorioSonido { get; set; }
        [JsonIgnore]
        public bool PuedeBorrarse { get; private set; }
        public Album()
        {
            canciones = new List<Cancion>();
            genero = Programa.genres.Last();
        }
        public Album(Genre g, string n = "", string a = "", short y = 0, short nc = 0, string c = "")
        {
            duracion = new TimeSpan();
            nombre = n;
            artista = a;
            year = y;
            numCanciones = nc;
            canciones = new List<Cancion>(nc);
            caratula = c;
            genero = g;
            PuedeBorrarse = true;
        }
        public Album(string n = "", string a = "", short y = 0, short nc = 0, string c = "")
        {
            duracion = new TimeSpan();
            nombre = n;
            artista = a;
            year = y;
            numCanciones = nc;
            caratula = c;
            genero = new Genre("");
            PuedeBorrarse = true;
        }
        public Album(Album a)
        {
            duracion = a.duracion;
            nombre = a.nombre;
            artista = a.artista;
            year = a.year;
            numCanciones = a.numCanciones;
            canciones = a.canciones;
            caratula = a.caratula;
            PuedeBorrarse = true;
        }
        public void agregarCancion(Cancion c)
        {
            canciones.Add(c);
            if (!c.IsBonus)
                duracion += c.duracion;
            numCanciones = (short)canciones.Count;
        }
        public void agregarCancion(Cancion c, int cual)
        {
            canciones.Insert(cual, c);
            if (!c.IsBonus)
                duracion += c.duracion;
            numCanciones = (short)canciones.Count;
        }
        public String[] ToStringArray()
        {
            String[] datos = { artista, nombre, year.ToString(), duracion.ToString(), genero.Name };
            return datos;
        }
        public String[] SongsToStringArray()
        {
            String[] datos = new string[numCanciones];
            for (int i = 0; i < canciones.Count; i++)
            {
                datos[i] = canciones[i].titulo;
            }
            return datos;
        }
        private string getID()
        {
            return artista + nombre;
        }
        public bool sonIguales(Album otro)
        {
            if (getID() == otro.getID())
                return true;
            else return false;
        }
        public int buscarCancion(string t)
        {
            int i = 0;
            while (t != canciones[i].titulo)
                i++;
            return i;
        }
        public Cancion DevolverCancion(string t)
        {
            Cancion c = null;
            int i = 0;
            c = canciones[i];
            while (t != canciones[i].titulo)
            {
                i++;
                c = canciones[i];
            }

            return c;
        }
        public void RefrescarDuracion()
        {
            duracion = new TimeSpan();
            for (int i = 0; i < canciones.Count; i++)
            {
                if (!canciones[i].IsBonus)
                    duracion += canciones[i].duracion;
            }
        }
        public Cancion getCancion(int n)
        {
            return canciones[n];
        }

        public Cancion getCancion(String b)
        {
            for (int i = 0; i < canciones.Count; i++)
            {
                if (b == canciones[i].titulo)
                    return canciones[i];
            }
            return null;
        }
        public override string ToString()
        {
            //artista - nombre (dur) (gen) 
            return artista + " - " + nombre + "(" + duracion + ") (" + genero.Name + ")";
        }
        public void BorrarCancion(int cual)
        {
            if (!canciones[cual].IsBonus)
                duracion -= canciones[cual].duracion;
            canciones.RemoveAt(cual);
            numCanciones--;
        }
        public void BorrarCancion(Cancion cancion)
        {
            if (!cancion.IsBonus)
                duracion -= cancion.duracion;
            canciones.Remove(cancion);
            numCanciones--;
        }
        public void ConfigurarCanciones()
        {
            foreach (Cancion cancion in canciones)
            {
                cancion.SetAlbum(this);
            }
        }
        public void SetSpotifyID(string id)
        {
            IdSpotify = id;
        }
        public String GetTerminoBusqueda()
        {
            return artista + " " + nombre;
        }
        public void ProtegerBorrado()
        {
            PuedeBorrarse = false;
        }
        public void LevantarBorrado()
        {
            PuedeBorrarse = true;
        }
        public void QuitarCancion(Cancion c)
        {
            canciones.Remove(c);
            numCanciones--;
            duracion -= c.duracion;
        }
        public string GetPortapapeles()
        {
            string val = Config.Portapapeles.Replace("%artist%", artista); //Es seguro.
            try
            {
                val = val.Replace("%title%", nombre);
                val = val.Replace("%year%", year.ToString());
                val = val.Replace("%genre%", genero.Name);
                val = val.Replace("%length%", duracion.ToString());
                val = val.Replace("%length_seconds%", ((int)duracion.TotalSeconds).ToString());
                return val;
            }
            catch (NullReferenceException)
            {
                return val;
            }
        }
    }
}
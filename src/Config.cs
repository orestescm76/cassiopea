﻿using System;
using System.Drawing;
using System.IO;
using System.Resources;

namespace aplicacion_musica
{
    //Clase para temas de configuración
    public static class Config
    {
        public static string Idioma;
        public static string TipografiaLyrics;
        public static bool VinculadoConSpotify;
        public static string UltimoDirectorioAbierto;
        private static ResXResourceSet cargador;
        private static ResXResourceWriter guardador;
        public static void CargarConfiguracion()
        {
            if(File.Exists("config.cfg"))
            {
                cargador = new ResXResourceSet("config.cfg");
                Idioma = cargador.GetString("Idioma");
                TipografiaLyrics = cargador.GetString("TipografiaLyrics");
                UltimoDirectorioAbierto = cargador.GetString("UltimoDirectorioAbierto");
                VinculadoConSpotify = Convert.ToBoolean(cargador.GetString("VinculadoConSpotify"));
            }
            else
            {
                Idioma = Properties.Resources.Idioma;
                TipografiaLyrics = Properties.Resources.TipografiaLyrics;
                VinculadoConSpotify = Convert.ToBoolean(Properties.Resources.VinculadoConSpotify);
                UltimoDirectorioAbierto = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            guardador = new ResXResourceWriter("config.cfg");
        }
        public static void GuardarConfiguracion()
        {
            guardador.AddResource("Idioma", Idioma);
            guardador.AddResource("TipografiaLyrics", TipografiaLyrics);
            guardador.AddResource("VinculadoConSpotify", VinculadoConSpotify.ToString());
            guardador.AddResource("UltimoDirectorioAbierto", UltimoDirectorioAbierto);
            guardador.Close();
        }
        public static Image GetIconoBandera()
        {
            switch (Idioma)
            {
                case "es":
                    return Properties.Resources.es;
                case "ca":
                    return Properties.Resources.ca;
                case "en":
                    return Properties.Resources.en;
                case "el":
                    return Properties.Resources.el;
            }
            return null;
        }
    }
}

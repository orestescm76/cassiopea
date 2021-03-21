﻿using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Net;

/* VERSION 1.6.xx CODENAME COCKROACH
* Reproductor:
*  Soporte CD Audio
*  Rework del sistema de playlists
* Gestor:
*  Ahora se puede redimensonar la ventana principal
*  Nuevo botón, abrir una disquetera para reproducir un CD
*  Visor de lyrics
*  Visor de log
* Misc:
*  Argumentos de lanzamiento en inglés
*/

namespace Cassiopeia
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        public static ResXResourceSet LocalTexts;
        public static String[] idGeneros = {"clasica", "hardrock", "rockprog", "progmetal", "rockpsicodelico", "heavymetal", "blackmetal", "electronica", "postrock", "indierock",
            "stoner", "pop", "jazz", "disco", "vaporwave", "chiptune", "punk", "postpunk", "folk", "blues" ,"funk", "new wave", "rocksinfonico", "ska", "flamenquito", "jazz fusion", ""};
        public static Collection Collection;
        public static Genre[] Genres = new Genre[idGeneros.Length];
        private static Version ver = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string Version = "v" + ver.ToString()+ "-beta";
        public static string[] idiomas;
        public static Spotify _spotify;
        private static principal principal = null;
        public static bool ModoOscuro = false;
        public static readonly string CodeName = "Cockroach";
        public static bool SpotifyActivado = true;
        public static bool ModoReproductor = false;
        public static Thread tareaRefrescoToken;
        public static bool ModoStream = false;
        public static int NumIdiomas = 0;
        private static bool ComprobarActualizaciones = true;
        private static bool Spotify = true;
        private static bool InicioReproductor = false;
        private static bool Console = false;

        private enum CSV_Albums
        {
            Title,
            Artist,
            Year,
            NumSongs,
            Genre,
            Cover_PATH,
            SpotifyID,
            SongFiles_DIR
        }
        private enum CSV_Songs
        {
            Title,
            TotalSeconds,
            IsBonus
        }
        private enum CSV_PATHS_LYRICS
        {
            Artist,
            SongTitle,
            Album
        }
        public static void RefreshSpotifyToken()
        {
            while(true)
            {
                if (_spotify.TokenExpirado())
                {
                    _spotify.RefrescarToken();
                }

                Thread.Sleep(TimeSpan.FromSeconds(15));
            }
        }

        //método mierdoso por temas de privado público
        public static void HayInternet(bool i)
        {
            principal.HayInternet(i);
        }
        public static void ChangeLanguage(String idioma)
        {
            Log.Instance.PrintMessage("Cambiando idioma al " + idioma, MessageType.Info);
            LocalTexts = new ResXResourceSet(@"./idiomas/" + "original." + idioma + ".resx");
            Config.Language = idioma;
            ReloadGenres();
            ReloadView();
            src.Forms.Reproductor.Instancia.RefrescarTextos();
        }
        public static void ActivarReproduccionSpotify()
        {
            principal.ActivarReproduccionSpotify();
        }
        public static void ReloadView()
        {
            principal.Refrescar();
        }
        public static int FindGenre(string g)
        {
            for (int i = 0; i < idGeneros.Length; i++)
            {
                if (g == idGeneros[i])
                    return i;
            }
            return -1;
        }
        public static int FindTranslatedGenre(string g)
        {
            for (int i = 0; i < Genres.Length; i++)
            {
                if (g == Genres[i].Name)
                    return i;
            }
            return -1;
        }
        public static void ReloadGenres()
        {
            for (int i = 0; i < Genres.Length-1; i++)
            {
                Genres[i].Name = LocalTexts.GetString("genero_" + Genres[i].Id);
            }
        }

        public static void CargarAlbumes(string fichero)
        {
            Log.Instance.PrintMessage("Cargando álbumes almacenados en " + fichero, MessageType.Info, "cargarAlbumes(string)");
            Stopwatch crono = Stopwatch.StartNew();
            using (StreamReader lector = new StreamReader(fichero))
            {
                string LineaJson = "";
                while (!lector.EndOfStream)
                {
                    LineaJson = lector.ReadLine();
                    AlbumData a = JsonConvert.DeserializeObject<AlbumData>(LineaJson);
                    a.Genre = Genres[FindGenre(a.Genre.Id)];
                    Collection.AddAlbum(ref a);
                    a.CanBeRemoved = true;
                }
            }
            crono.Stop();
            Log.Instance.PrintMessage("Cargados " + Collection.Albums.Count + " álbumes correctamente", MessageType.Correct, crono, TimeType.Milliseconds);
            ReloadView();
        }
        private static void SendErrorLoading(int line, string file)
        {
            Log.Instance.PrintMessage("Error cargando el álbum. Revise la línea " + line + " del fichero " + file, MessageType.Error);
            MessageBox.Show(LocalTexts.GetString("errorLoadingAlbums1") + line + " "+ LocalTexts.GetString("errorLoadingAlbums2")+ file, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void LoadCSVAlbums(string fichero)
        {
            Log.Instance.PrintMessage("Cargando álbumes CSV almacenados en " + fichero, MessageType.Info, "cargarAlbumesLegacy(string)");
            Stopwatch crono = Stopwatch.StartNew();
            //cargando CSV a lo bestia
            int lineaC = 1;
            using (StreamReader lector = new StreamReader(fichero))
            {
                string linea;
                while (!lector.EndOfStream)
                {
                    linea = lector.ReadLine();
                    while (linea == "")
                    {
                        linea = lector.ReadLine();
                        lineaC++;
                    }

                    if (linea == null) continue; //si no hay nada tu sigue, que hemos llegado al final del fichero, después del nulo porque siempre al terminar un disco pongo línea nueva.
                    string[] datos = linea.Split(';');
                    if (datos.Length != 8)
                    {
                        SendErrorLoading(lineaC, fichero);
                        Environment.Exit(-1);
                    }
                    short nC = 0;
                    int gen = FindGenre(datos[(int)CSV_Albums.Genre]);
                    Genre g = Genres[gen];
                    if (string.IsNullOrEmpty(datos[(int)CSV_Albums.Cover_PATH])) datos[(int)CSV_Albums.Cover_PATH] = string.Empty;
                    AlbumData a = null;
                    try
                    {
                        nC = Convert.ToInt16(datos[(int)CSV_Albums.NumSongs]);
                        a = new AlbumData(g, datos[(int)CSV_Albums.Title], datos[(int)CSV_Albums.Artist], Convert.ToInt16(datos[(int)CSV_Albums.Year]), datos[(int)CSV_Albums.Cover_PATH]);
                    }
                    catch (FormatException)
                    {
                        SendErrorLoading(lineaC, fichero);
                        Environment.Exit(-1);
                    }
                    if (!string.IsNullOrEmpty(datos[(int)CSV_Albums.SpotifyID]))
                        a.IdSpotify = datos[(int)CSV_Albums.SpotifyID];
                    if (!string.IsNullOrEmpty(datos[(int)CSV_Albums.SongFiles_DIR]))
                        a.SoundFilesPath = datos[(int)CSV_Albums.SongFiles_DIR];
                    bool exito = false;
                    for (int i = 0; i < nC; i++)
                    {
                        exito = false;
                        linea = lector.ReadLine();
                        lineaC++;
                        if (string.IsNullOrEmpty(linea))
                        {
                            /*System.Windows.Forms.MessageBox.Show("mensajeError"+Environment.NewLine
                                + a.nombre + " - " + a.nombre + Environment.NewLine
                                + "saltarAlSiguiente", "error", System.Windows.Forms.MessageBoxButtons.OK);*/
                            break; //no sigue cargando el álbum
                        }
                        else
                        {
                            try
                            {
                                exito = true;
                                string[] datosCancion = linea.Split(';');
                                if (datosCancion.Length == 3)
                                {
                                    byte bonus = Convert.ToByte(datosCancion[(int)CSV_Songs.IsBonus]);
                                    Song c = new Song(datosCancion[(int)CSV_Songs.Title], TimeSpan.FromSeconds(Convert.ToInt32(datosCancion[(int)CSV_Songs.TotalSeconds])), ref a, Convert.ToBoolean(bonus));
                                    a.AddSong(c, i);
                                }
                                else
                                {
                                    LongSong cl = new LongSong(datosCancion[(int)CSV_Songs.Title], a);
                                    int np = Convert.ToInt32(datosCancion[(int)CSV_Songs.TotalSeconds]);
                                    for (int j = 0; j < np; j++)
                                    {
                                        linea = lector.ReadLine();
                                        lineaC++;
                                        datosCancion = linea.Split(';');
                                        Song c = new Song(datosCancion[0], TimeSpan.FromSeconds(Convert.ToInt32(datosCancion[(int)CSV_Songs.TotalSeconds])), ref a);
                                        cl.AddPart(c);
                                    }
                                    a.AddSong(cl, i);
                                }
                            }
                            catch (FormatException)
                            {
                                SendErrorLoading(lineaC, fichero);
                                Environment.Exit(-1);
                            }
                        }
                    }
                    if (Collection.IsInCollection(a))
                    {
                        exito = false; //pues ya está repetido.
                        Log.Instance.PrintMessage("Álbum repetido -> " + a.Artist + " - " + a.Title, MessageType.Warning);
                    }

                    if (exito)
                        Collection.AddAlbum(ref a);


                    a.CanBeRemoved = true;
                    lineaC++;
                }
            }
            crono.Stop();
            Log.Instance.PrintMessage("Cargados " + Collection.Albums.Count + " álbumes correctamente", MessageType.Correct, crono, TimeType.Milliseconds);
            ReloadView();
        }
        public static void LoadCD(string fichero = "cd.json")
        {
            if (!File.Exists(fichero))
                return;

            using(StreamReader lector = new StreamReader(fichero))
            {
                string linea;
                while(!lector.EndOfStream)
                {
                    linea = lector.ReadLine();
                    CompactDisc cd = JsonConvert.DeserializeObject<CompactDisc>(linea);

                    cd.InstallAlbum();
                    Collection.AddCD(ref cd);
                    cd.AlbumData.CanBeRemoved = false;
                }
            }
        }
        private static void LoadPATHS()
        {
            Log.Instance.PrintMessage("Cargando PATHS", MessageType.Info);
            using(StreamReader entrada = new FileInfo("paths.txt").OpenText())
            {
                string linea = null;
                while(!entrada.EndOfStream)
                {
                    linea = entrada.ReadLine();
                    string[] datos = linea.Split(';');
                    List<AlbumData> listaAlbumes = Collection.SearchAlbum(datos[(int)CSV_PATHS_LYRICS.Album]);
                    if (listaAlbumes.Count != 0)
                    {
                        foreach (AlbumData album in listaAlbumes)
                        {
                            if(album.Artist == datos[(int)CSV_PATHS_LYRICS.Artist] && album.Title == datos[(int)CSV_PATHS_LYRICS.Album])
                            {
                                Song c = album.GetSong(datos[(int)CSV_PATHS_LYRICS.SongTitle]);
                                linea = entrada.ReadLine();
                                c.Path = linea;
                            }
                        }
                    }
                    else
                    {
                        linea = entrada.ReadLine();
                        continue;
                    }
                }
            }
        }
        public static void SavePATHS()
        {
            Log.Instance.PrintMessage("Guardando PATHS", MessageType.Info);
            Stopwatch crono = Stopwatch.StartNew();
            using(StreamWriter salida = new FileInfo("paths.txt").CreateText())
            {
                foreach (AlbumData album in Collection.Albums)
                {
                    if (string.IsNullOrEmpty(album.SoundFilesPath))
                        continue;
                    foreach (Song cancion in album.Songs)
                    {
                        if (!string.IsNullOrEmpty(cancion.Path))
                        {
                            salida.Write(cancion.SavePath());
                        }
                    }
                }
            }
            crono.Stop();
            Log.Instance.PrintMessage("Guardados los PATHS", MessageType.Correct, crono, TimeType.Milliseconds);
        }
        public static void SaveAlbums(string path, TipoGuardado tipoGuardado, bool json = false)
        {

            Stopwatch crono = Stopwatch.StartNew();
            FileInfo fich = new FileInfo(path);
            if (json)
            {
                using (StreamWriter salida = fich.CreateText())
                {
                    switch (tipoGuardado)
                    {
                        case TipoGuardado.Digital:
                            Log.Instance.PrintMessage(nameof(SaveAlbums) + " - Guardando la base de datos... (" + Program.Collection.Albums.Count + " discos)", MessageType.Info);
                            Log.Instance.PrintMessage("Nombre del fichero: " + path, MessageType.Info);
                            foreach (AlbumData a in Program.Collection.Albums)
                            {
                                JsonSerializer s = new JsonSerializer();
                                s.TypeNameHandling = TypeNameHandling.All;
                                salida.WriteLine(JsonConvert.SerializeObject(a));
                            }
                            break;
                        case TipoGuardado.CD:
                            Log.Instance.PrintMessage(nameof(SaveAlbums) + " - Guardando la base de datos... (" + Program.Collection.CDS.Count + " cds)", MessageType.Info);
                            Log.Instance.PrintMessage("Nombre del fichero: " + path, MessageType.Info);
                            foreach (CompactDisc compacto in Program.Collection.CDS)
                            {
                                salida.WriteLine(JsonConvert.SerializeObject(compacto));
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
            else
            {
                using (StreamWriter salida = fich.CreateText())
                {
                    switch (tipoGuardado)
                    {
                        case TipoGuardado.Digital:
                            Log.Instance.PrintMessage(nameof(SaveAlbums) + " - Guardando la base de datos... (" + Program.Collection.Albums.Count + " discos)", MessageType.Info);
                            Log.Instance.PrintMessage("Nombre del fichero: " + path, MessageType.Info);
                            foreach (AlbumData a in Collection.Albums)
                            {
                                if (!(a.Songs[0] == null)) //no puede ser un album con 0 canciones
                                {
                                    salida.WriteLine(a.Title + ";" + a.Artist + ";" + a.Year + ";" + a.NumberOfSongs + ";" + a.Genre.Id + ";" + a.CoverPath + ";"+a.IdSpotify + ";"+a.SoundFilesPath);
                                    for (int i = 0; i < a.NumberOfSongs; i++)
                                    {
                                        if (a.Songs[i] is LongSong longSong)
                                        {
                                            salida.WriteLine(longSong.Title + ";" + longSong.Parts.Count);//no tiene duracion y son 2 datos a guardar
                                            foreach (Song parte in longSong.Parts)
                                            {
                                                salida.WriteLine(parte.Title + ";" + (int)(parte.Length.TotalSeconds));
                                            }

                                        }
                                        else //titulo;400;0
                                            salida.WriteLine(a.Songs[i].Title + ";" + (int)a.Songs[i].Length.TotalSeconds + ";"+Convert.ToInt32(a.Songs[i].IsBonus));
                                    }
                                }
                                salida.WriteLine();
                            }
                            break;
                        case TipoGuardado.CD:
                            break;
                        case TipoGuardado.Vinilo:
                            break;
                        default:
                            break;
                    }

                }
            }
            crono.Stop();
            fich.Refresh();
            Log.Instance.PrintMessage(nameof(SaveAlbums) + "- Guardado", MessageType.Correct, crono, TimeType.Milliseconds);
            crono.Stop();
        }
        private static void LoadLyrics()
        {
            Log.Instance.PrintMessage("Cargando lyrics", MessageType.Info);
            Stopwatch crono = Stopwatch.StartNew();
            using (StreamReader entrada = new FileInfo("lyrics.txt").OpenText())
            {
                string linea = null;
                while (!entrada.EndOfStream)
                {
                    linea = entrada.ReadLine();
                    string[] datos = linea.Split(';');
                    AlbumData albumData = Collection.SearchAlbum(datos[(int)CSV_PATHS_LYRICS.Album])[0];
                    Song song = albumData.GetSong(datos[(int)CSV_PATHS_LYRICS.SongTitle]);
                    List<string> lyrics = new List<string>();
                    do
                    {
                        linea = entrada.ReadLine();
                        lyrics.Add(linea);
                    } while (linea != "---");
                    lyrics.Remove("---");
                    song.Lyrics = lyrics.ToArray();
                }
            }
            crono.Stop();
            Log.Instance.PrintMessage("Lyrics cargadas", MessageType.Correct, crono, TimeType.Milliseconds);
        }
        private static void SaveLyrics()
        {
            Log.Instance.PrintMessage("Guardando lyrics", MessageType.Info);
            Stopwatch crono = Stopwatch.StartNew();
            using (StreamWriter salida = new FileInfo("lyrics.txt").CreateText())
            {
                foreach (AlbumData album in Collection.Albums)
                {
                    foreach (Song cancion in album.Songs)
                    {
                        if (cancion.Lyrics != null && cancion.Lyrics.Length != 0)
                        {
                            salida.WriteLine(album.Artist + ";" + cancion.Title + ";" + album.Title);
                            foreach (string line in cancion.Lyrics)
                            {
                                salida.WriteLine(line);
                            }
                            salida.WriteLine("---");
                        }
                    }
                }
            }
            crono.Stop();
            Log.Instance.PrintMessage("Guardados las letras", MessageType.Correct, crono, TimeType.Milliseconds);
            FileInfo lyrics = new FileInfo("lyrics.txt");
            Log.Instance.PrintMessage("Tamaño del fichero: " + lyrics.Length/1024 + " kb", MessageType.Info);
        }
        static bool CheckForUpdates(out string verNueva)
        {
            HttpWebRequest GithubRequest = WebRequest.CreateHttp("https://api.github.com/repos/orestescm76/cassiopeia/releases");
            string contenido = string.Empty;
            GithubRequest.Accept = "text/html,application/vnd.github.v3+json";
            GithubRequest.UserAgent = ".NET Framework Test Agent"; //Si no lo pongo, 403.
            try
            {
                using (HttpWebResponse respuesta = (HttpWebResponse)GithubRequest.GetResponse())
                using (Stream flujo = respuesta.GetResponseStream())
                using (StreamReader lector = new StreamReader(flujo))
                {
                    while (!lector.EndOfStream)
                        contenido += lector.ReadLine();
                }
            }
            catch (WebException e)
            {
                Log.Instance.PrintMessage("Hubo un problema intentando localizar la nueva versión...", MessageType.Error);
                Log.Instance.PrintMessage("Respuesta del servidor: " + e.Response, MessageType.Info);
                verNueva = string.Empty;
                return false;
            }
            
            int indexVersion = contenido.IndexOf("tag_name");
            verNueva = contenido.Substring(indexVersion, 40);
            verNueva = verNueva.Split('\"')[2];
            if (verNueva != Version)
                return true;
            else
                return false;
        }

        [STAThread]
        static void Main(String[] args)
        {
            //Creación log.
            Log Log = Log.Instance;
            //prepara la aplicación para que ejecute formularios y demás.
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Config.CargarConfiguracion();
            LocalTexts = new ResXResourceSet(@"./idiomas/" + "original." + Config.Language + ".resx");
            //Parseo de argumentos...
            foreach (string Arg in args)
            {
                switch (Arg)
                {
                    case "-consola":
                        Console = true;
                        AllocConsole();
                        System.Console.Title = "Consola debug v" + Version;
                        System.Console.WriteLine("Log creado " + DateTime.Now);
                        Log.PrintMessage("Se ha iniciado la aplicación con el parámetro -consola", MessageType.Info);
                        break;
                    case "-noSpotify":
                        Spotify = false;
                        break;
                    case "-modoStream":
                    case "-streamMode":
                        ModoStream = true;
                        break;
                    case "-noActualizar":
                    case "-noUpdates":
                        ComprobarActualizaciones = false;
                        break;
                    case "-reproductor":
                    case "-player":
                        InicioReproductor = true;
                        break;
                    default:
                        break;
                }
            }

            //Cargar idiomas...
            DirectoryInfo cod = new DirectoryInfo("./idiomas");
            Program.idiomas = new String[cod.GetFiles().Length];
            int j = 0;
            foreach (var idioma in cod.GetFiles())
            {
                Program.NumIdiomas++;
                string id = idioma.Name.Replace(".resx", "");
                id = id.Replace("original.", "");
                Program.idiomas[j] = id;
                j++;
            }

            string versionNueva;
            if (CheckForUpdates(out versionNueva) && ComprobarActualizaciones)
            {
                Log.PrintMessage("Está disponible la actualización " + versionNueva, MessageType.Info);
                DialogResult act = MessageBox.Show(LocalTexts.GetString("actualizacion1") + Environment.NewLine + versionNueva + Environment.NewLine + LocalTexts.GetString("actualizacion2"), "", MessageBoxButtons.YesNo);
                if (act == DialogResult.Yes)
                    Process.Start("https://github.com/orestescm76/aplicacion-gestormusica/releases");
            }
            Collection = new Collection();
            SpotifyActivado = false;
            principal = new principal();

            if(Spotify)
            {
                if (!Config.LinkedWithSpotify)
                    _spotify = new Spotify(false);
                else
                {
                    _spotify = new Spotify(true);
                    SpotifyActivado = true;
                    principal.DesactivarVinculacion();
                }
            }
            else
            {
                SpotifyActivado = false;
                Log.PrintMessage("Se ha iniciado la aplicación con el parámetro -noSpotify, no habrá integración con Spotify", MessageType.Info);
                _spotify = null;
                principal.HayInternet(false);
            }

            if(!ModoStream)
            {
                Log.PrintMessage("Configurando géneros", MessageType.Info);
                for (int i = 0; i < idGeneros.Length; i++)
                {
                    if (idGeneros[i] == "")
                    {
                        Genres[i] = new Genre(idGeneros[i]);
                        Genres[i].Name = "-";
                    }
                    else
                    {
                        Genres[i] = new Genre(idGeneros[i]);
                        Genres[i].Name = LocalTexts.GetString("genero_" + Genres[i].Id);
                    }
                }
                if (args.Contains("-json"))
                    CargarAlbumes("discos.json");
                else
                {
                    if (File.Exists("discos.csv"))
                    {
                        LoadCSVAlbums("discos.csv");
                        LoadCD();
                    }
                    else
                    {
                        Log.PrintMessage("discos.csv no existe, se creará una base de datos vacía.", MessageType.Warning);
                    }
                }
                if (File.Exists("paths.txt"))
                    LoadPATHS();
                if (File.Exists("lyrics.txt"))
                    LoadLyrics();
            }
            //creo el Reproductor
            src.Forms.Reproductor.Instancia = new src.Forms.Reproductor();
            src.Forms.Reproductor.Instancia.RefrescarTextos();
            if (ModoStream) //enchufa la app sin nada, solo el spotify y el texto
            {
                Application.Run();
            }
            else if (!InicioReproductor) //tirale con el principal
                Application.Run(principal);
            else
            {
                Application.Run(src.Forms.Reproductor.Instancia);
            }
            if(_spotify != null && tareaRefrescoToken != null)
                tareaRefrescoToken.Abort();
            SavePATHS();
            SaveLyrics();
            Config.GuardarConfiguracion();

            if (File.Exists("./covers/np.jpg"))
                File.Delete("./covers/np.jpg");
            if (Console)
            {
                System.Console.WriteLine("Programa finalizado, presione una tecla para continuar...");
                System.Console.ReadKey();
            }
            Log.Instance.CloseLog();
        }

    }
}

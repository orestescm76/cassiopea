﻿using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Configuration;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
/* VERSION 1.5.2.82 RC2 CODENAME RAVEN
* Reproductor:
*  Reproduce en FLAC, MP3 y OGG
*  Soporta metadatos.
*  Soporta carátula MP3
*  Con tiempo actualizable, se puede saltar
*  Bitrate variable en OGG
*  Sincronizado entre Spotify y el Reproductor
* Spotify:
*  Ahora se puede vincular la app.
*  
*  Gestor:
*  Reproducir una cancion en local o Spotify desde la visualización
*  Guardado en CSV, menos espacio.
*  Drag & drop del visualizar álbum al reproductor
*  Nuevos idiomas, pero estarán vacios :-)
*  Arreglos con el ModoStream
*  Drag and Drop en el reproductor
*  Calcular los paths desde la visualización del álbum
*  Drag and drop en la lista de reproducción
*  Fusionado de canciones en una con partes
*/
namespace aplicacion_musica
{
    static class Programa
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        public static ResXResourceSet textosLocal;
        public static String[] idGeneros = {"clasica", "hardrock", "rockprog", "progmetal", "rockpsicodelico", "heavymetal", "blackmetal", "electronica", "postrock", "indierock",
            "stoner", "pop", "jazz", "disco", "vaporwave", "chiptune", "punk", "postpunk", "folk", "blues" ,"funk", "new wave", "rocksinfonico", "ska", "flamenquito", "house", "jazz fusion", ""}; //lista hardcoded que tendrá su respectiva traducción en las últimas líneas del fichero !!
        public static Coleccion miColeccion;
        public static Genero[] generos = new Genero[idGeneros.Length];
        private static Version ver = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string version = ver.ToString();
        public static string[] idiomas;
        public static Spotify _spotify;
        private static principal principal;
        public static string Idioma;
        public static bool ModoOscuro = false;
        public static readonly string CodeName = "Raven";
        private static ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
        public static bool SpotifyActivado = false;
        public static Configuration config;
        public static bool ModoReproductor = false;
        public static Thread tareaRefrescoToken;
        public static bool ModoStream = false;
        public static void Refresco()
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
        public static void cambiarIdioma(String idioma)
        {
            textosLocal = new ResXResourceSet(@"./idiomas/" + "original." + idioma + ".resx");
            Idioma = idioma;
            refrescarGeneros();
            refrescarVista();
        }
        public static void refrescarVista()
        {
            principal.Refrescar();
        }
        public static int findGenero(string g)
        {
            for (int i = 0; i < idGeneros.Length; i++)
            {
                if (g == idGeneros[i])
                    return i;
            }
            return -1;
        }
        public static int findGeneroTraducido(string g)
        {
            for (int i = 0; i < generos.Length; i++)
            {
                if (g == generos[i].traducido)
                    return i;
            }
            return -1;
        }
        public static void refrescarGeneros()
        {
            for (int i = 0; i < generos.Length-1; i++)
            {
                generos[i].traducido = textosLocal.GetString("genero_" + generos[i].Id);
            }
        }
        public static void cargarAlbumes(string fichero)
        {
            Log.Instance.ImprimirMensaje("Cargando álbumes almacenados en " + fichero, TipoMensaje.Info, "cargarAlbumes(string)");
            Stopwatch crono = Stopwatch.StartNew();
            using (StreamReader lector = new StreamReader(fichero))
            {
                string LineaJson = "";
                while (!lector.EndOfStream)
                {
                    LineaJson = lector.ReadLine();
                    Album a = JsonConvert.DeserializeObject<Album>(LineaJson);
                    a.RefrescarDuracion();
                    a.genero = generos[findGenero(a.genero.Id)];
                    a.numCanciones = (short)a.canciones.Count;
                    a.ConfigurarCanciones();
                    miColeccion.agregarAlbum(ref a);
                    a.LevantarBorrado();
                }
            }
            crono.Stop();
            Log.Instance.ImprimirMensaje("Cargados " + miColeccion.albumes.Count + " álbumes correctamente", TipoMensaje.Correcto, crono);
            refrescarVista();
        }
        public static void cargarAlbumesCSV(string fichero)
        {
            Log.Instance.ImprimirMensaje("Cargando álbumes CSV almacenados en " + fichero, TipoMensaje.Info, "cargarAlbumesLegacy(string)");
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
                        Log.Instance.ImprimirMensaje("Error cargando el álbum. Revise la línea " + lineaC + " del fichero " + fichero, TipoMensaje.Error);
                        MessageBox.Show("Error cargando el álbum. Revise la línea " + lineaC + " del fichero " + fichero, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(-1);
                    }
                    short nC = 0;
                    int gen = findGenero(datos[4]);
                    Genero g = Programa.generos[gen];
                    if (string.IsNullOrEmpty(datos[5])) datos[5] = string.Empty;
                    Album a = null;
                    try
                    {
                        nC = Convert.ToInt16(datos[3]);
                        a = new Album(g, datos[0], datos[1], Convert.ToInt16(datos[2]), Convert.ToInt16(datos[3]), datos[5]);
                    }
                    catch (FormatException e)
                    {
                        Log.Instance.ImprimirMensaje("Error cargando el álbum. Revise la línea " + lineaC + " del fichero " + fichero, TipoMensaje.Error);
                        MessageBox.Show("Error cargando el álbum. Revise la línea " + lineaC + " del fichero " + fichero, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(-1);
                    }
                    if (!string.IsNullOrEmpty(datos[6]))
                        a.SetSpotifyID(datos[6]);
                    if (!string.IsNullOrEmpty(datos[7]))
                        a.DirectorioSonido = datos[7];
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
                                    byte bonus = Convert.ToByte(datosCancion[2]);
                                    Cancion c = new Cancion(datosCancion[0], TimeSpan.FromSeconds(Convert.ToInt32(datosCancion[1])), ref a, Convert.ToBoolean(bonus));
                                    a.agregarCancion(c, i);
                                }
                                else
                                {
                                    CancionLarga cl = new CancionLarga(datosCancion[0], ref a);
                                    int np = Convert.ToInt32(datosCancion[1]);
                                    for (int j = 0; j < np; j++)
                                    {
                                        linea = lector.ReadLine();
                                        lineaC++;
                                        datosCancion = linea.Split(';');
                                        Cancion c = new Cancion(datosCancion[0], TimeSpan.FromSeconds(Convert.ToInt32(datosCancion[1])), ref a);
                                        cl.addParte(ref c);
                                    }
                                    a.agregarCancion(cl, i);
                                }
                            }
                            catch (FormatException e)
                            {
                                Log.Instance.ImprimirMensaje("Error cargando el álbum. Revise la línea " + lineaC + " del fichero " + fichero, TipoMensaje.Error);
                                MessageBox.Show("Error cargando el álbum. Revise la línea " + lineaC + " del fichero " + fichero, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Environment.Exit(-1);
                            }
                        }
                    }
                    if (miColeccion.estaEnColeccion(a))
                    {
                        exito = false; //pues ya está repetido.
                        Log.Instance.ImprimirMensaje("Álbum repetido -> " + a.artista + " - " + a.nombre, TipoMensaje.Advertencia);
                    }
                    if (exito)
                        miColeccion.agregarAlbum(ref a);
                    a.LevantarBorrado();
                    lineaC++;
                }
            }
            crono.Stop();
            Log.Instance.ImprimirMensaje("Cargados " + miColeccion.albumes.Count + " álbumes correctamente", TipoMensaje.Correcto, crono);
            refrescarVista();
        }
        public static void cargarCDS(string fichero = "cd.json")
        {
            if (!File.Exists(fichero))
                return;
            using(StreamReader lector = new StreamReader(fichero))
            {
                string linea;
                while(!lector.EndOfStream)
                {
                    linea = lector.ReadLine();
                    DiscoCompacto cd = JsonConvert.DeserializeObject<DiscoCompacto>(linea);
                    cd.InstallAlbum();
                    miColeccion.AgregarCD(ref cd);
                    cd.Album.ProtegerBorrado();
                }
            }
        }
        private static void CargarPATHS()
        {
            Log.Instance.ImprimirMensaje("Cargando PATHS", TipoMensaje.Info);
            using(StreamReader entrada = new FileInfo("paths.txt").OpenText())
            {
                string linea = null;
                while(!entrada.EndOfStream)
                {
                    linea = entrada.ReadLine();
                    string[] datos = linea.Split(';');
                    List<Album> listaAlbumes = miColeccion.buscarAlbum(datos[2]);
                    if (listaAlbumes.Count != 0)
                    {
                        foreach (Album album in listaAlbumes)
                        {
                            if(album.artista == datos[0] && album.nombre == datos[2])
                            {
                                Cancion c = album.canciones[album.buscarCancion(datos[1])];
                                linea = entrada.ReadLine();
                                c.PATH = linea;
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
        public static void GuardarPATHS()
        {
            Log.Instance.ImprimirMensaje("Guardando PATHS", TipoMensaje.Info);
            Stopwatch crono = Stopwatch.StartNew();
            using(StreamWriter salida = new FileInfo("paths.txt").CreateText())
            {
                foreach (Album album in miColeccion.albumes)
                {
                    if (string.IsNullOrEmpty(album.DirectorioSonido))
                        continue;
                    foreach (Cancion cancion in album.canciones)
                    {
                        if (!string.IsNullOrEmpty(cancion.PATH))
                        {
                            salida.Write(cancion.GuardarPATH());
                        }
                    }
                }
            }
            crono.Stop();
            Log.Instance.ImprimirMensaje("Guardados los PATHS", TipoMensaje.Correcto, crono);
        }
        public static void GuardarDiscos(string path, TipoGuardado tipoGuardado, bool json = false)
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
                            Log.Instance.ImprimirMensaje(nameof(GuardarDiscos) + " - Guardando la base de datos... (" + Programa.miColeccion.albumes.Count + " discos)", TipoMensaje.Info);
                            Log.Instance.ImprimirMensaje("Nombre del fichero: " + path, TipoMensaje.Info);
                            foreach (Album a in Programa.miColeccion.albumes)
                            {
                                JsonSerializer s = new JsonSerializer();
                                s.TypeNameHandling = TypeNameHandling.All;
                                salida.WriteLine(JsonConvert.SerializeObject(a));
                            }
                            break;
                        case TipoGuardado.CD:
                            Log.Instance.ImprimirMensaje(nameof(GuardarDiscos) + " - Guardando la base de datos... (" + Programa.miColeccion.cds.Count + " cds)", TipoMensaje.Info);
                            Log.Instance.ImprimirMensaje("Nombre del fichero: " + path, TipoMensaje.Info);
                            foreach (DiscoCompacto compacto in Programa.miColeccion.cds)
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
                            Log.Instance.ImprimirMensaje(nameof(GuardarDiscos) + " - Guardando la base de datos... (" + Programa.miColeccion.albumes.Count + " discos)", TipoMensaje.Info);
                            Log.Instance.ImprimirMensaje("Nombre del fichero: " + path, TipoMensaje.Info);
                            foreach (Album a in miColeccion.albumes)
                            {
                                if (!(a.canciones[0] == null)) //no puede ser un album con 0 canciones
                                {
                                    salida.WriteLine(a.nombre + ";" + a.artista + ";" + a.year + ";" + a.numCanciones + ";" + a.genero.Id + ";" + a.caratula + ";"+a.IdSpotify + ";"+a.DirectorioSonido);
                                    for (int i = 0; i < a.numCanciones; i++)
                                    {
                                        if (a.canciones[i] is CancionLarga cl)
                                        {
                                            salida.WriteLine(cl.titulo + ";" + cl.Partes.Count);//no tiene duracion y son 2 datos a guardar
                                            foreach (Cancion parte in cl.Partes)
                                            {
                                                salida.WriteLine(parte.titulo + ";" + parte.duracion.TotalSeconds);
                                            }

                                        }
                                        else //titulo;400;0
                                            salida.WriteLine(a.canciones[i].titulo + ";" + a.canciones[i].duracion.TotalSeconds + ";"+Convert.ToInt32(a.canciones[i].Bonus));
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
            Log.Instance.ImprimirMensaje(nameof(GuardarDiscos) + "- Guardado", TipoMensaje.Correcto, crono);
            Log.Instance.ImprimirMensaje("Tamaño: " + fich.Length + " bytes", TipoMensaje.Info);
        }
        static bool HayActualizacions(out string verNueva)
        {
            HttpWebRequest GithubRequest = WebRequest.CreateHttp("https://api.github.com/repos/orestescm76/aplicacion-gestormusica/releases");
            string contenido = string.Empty;
            GithubRequest.Accept = "text/html,application/vnd.github.v3+json";
            GithubRequest.UserAgent = ".NET Framework Test Client"; //Si no lo pongo, 403.
            List<string> Json = new List<string>();
            try
            {
                using (HttpWebResponse respuesta = (HttpWebResponse)GithubRequest.GetResponse())
                using (Stream flujo = respuesta.GetResponseStream())
                using (StreamReader lector = new StreamReader(flujo))
                {
                    //File.WriteAllText("github.json", lector.ReadToEnd());
                    while (!lector.EndOfStream)
                        contenido += lector.ReadLine();
                }
            }
            catch (WebException)
            {
                Log.Instance.ImprimirMensaje("Hubo un problema intentando localizar la nueva versión...", TipoMensaje.Error);
                verNueva = string.Empty;
                return false;
            }

            //contenido = File.ReadAllText("github.json");
            int indexVersion = contenido.IndexOf("tag_name");
            verNueva = contenido.Substring(indexVersion, 40);
            verNueva = verNueva.Split('v')[1].Split('\"')[0];
            if (verNueva != version)
                return true;
            else
                return false;
        }
        [STAThread]
        static void Main(String[] args)
        {
            //prepara la aplicación para que ejecute formularios y demás.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Idioma = ConfigurationManager.AppSettings["Idioma"];
            textosLocal = new ResXResourceSet(@"./idiomas/" + "original." + Idioma + ".resx");
            Log Log = Log.Instance;
            string versionNueva;
            if(HayActualizacions(out versionNueva))
            {
                Log.ImprimirMensaje("Está disponible la actualización " + versionNueva, TipoMensaje.Info);
                DialogResult act = MessageBox.Show(textosLocal.GetString("actualizacion1") + Environment.NewLine + versionNueva + Environment.NewLine + textosLocal.GetString("actualizacion2"), "", MessageBoxButtons.YesNo);
                if(act == DialogResult.Yes)
                    Process.Start("https://github.com/orestescm76/aplicacion-gestormusica/releases");
            }
            if(args.Contains("-consola"))
            {
                AllocConsole();
                Console.Title = "Consola debug v" + version;
                Console.WriteLine("Log creado " + DateTime.Now);
                Log.ImprimirMensaje("Se ha iniciado la aplicación con el parámetro -consola", TipoMensaje.Info);
            }
            miColeccion = new Coleccion();
            configFileMap.ExeConfigFilename = Environment.CurrentDirectory + "/aplicacion_musica.exe.config";
            config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            SpotifyActivado = false;
            principal = new principal();
            if(!args.Contains("-noSpotify"))
            {
                if (config.AppSettings.Settings["VinculadoConSpotify"].Value == "false")
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
                Log.ImprimirMensaje("Se ha iniciado la aplicación con el parámetro -noSpotify, no habrá integración con Spotify", TipoMensaje.Info);
                _spotify = null;
                principal.HayInternet(false);
            }

            if (args.Contains("-modoStream"))
            {
                ModoStream = true;
                Log.ImprimirMensaje("Iniciando modo Stream", TipoMensaje.Info);
            }

            Reproductor reproductor = Reproductor.Instancia;
            if(!ModoStream)
            {
                Log.ImprimirMensaje("Configurando géneros", TipoMensaje.Info);
                for (int i = 0; i < idGeneros.Length; i++)
                {
                    if (idGeneros[i] == "")
                    {
                        generos[i] = new Genero(idGeneros[i]);
                        generos[i].setTraduccion("-");
                    }
                    else
                    {
                        generos[i] = new Genero(idGeneros[i]);
                        generos[i].setTraduccion(textosLocal.GetString("genero_" + generos[i].Id));
                    }
                }
                if (args.Contains("-json"))
                    cargarAlbumes("discos.json");
                else
                {
                    if (File.Exists("discos.csv"))
                    {
                        cargarAlbumesCSV("discos.csv");
                        cargarCDS();
                    }
                    else
                    {
                        Log.ImprimirMensaje("discos.csv no existe, se creará una base de datos vacía.", TipoMensaje.Advertencia);
                    }
                }
                if (File.Exists("paths.txt"))
                    CargarPATHS();
            }
            //creo el Reproductor
            Reproductor.Instancia = new Reproductor();
            Reproductor.Instancia.RefrescarTextos();
            if (ModoStream) //enchufa la app sin nada, solo el spotify y el texto
            {
                Application.Run();
            }
            else if (!args.Contains("-reproductor")) //tirale con el principal
                Application.Run(principal);
            else
            {
                ModoReproductor = true;
                Application.Run(Reproductor.Instancia);
                //Reproductor.Instancia.Show();
            }
            if(_spotify != null && tareaRefrescoToken != null)
                tareaRefrescoToken.Abort();
            if(!ModoStream)
                GuardarPATHS();
            config.AppSettings.Settings["Idioma"].Value = Idioma;
            config.Save();

            if (File.Exists("./covers/np.jpg"))
            {
                try
                {
                    File.Delete("./covers/np.jpg");
                }
                catch (IOException)
                {
                    Log.ImprimirMensaje("No se ha podido eliminar el fichero np.jpg.", TipoMensaje.Advertencia);
                }
            }
            if (args.Contains("-consola"))
            {
                Console.WriteLine("Programa finalizado, presione una tecla para continuar...");
                Console.ReadKey();
            }
        }

    }
}

﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Cassiopeia
{
    public partial class editarAlbum : Form
    {
        private AlbumData albumAEditar;
        private string[] generosTraducidos = new string[Program.Genres.Length-1];
        public editarAlbum(ref AlbumData a)
        {
            InitializeComponent();
            Console.WriteLine("Editando canción");
            albumAEditar = a;
            textBoxArtista.Text = albumAEditar.Artist;
            textBoxAño.Text = albumAEditar.Year.ToString();
            textBoxTitulo.Text = albumAEditar.Title;
            labelRuta.Text = albumAEditar.CoverPath;
            labelDirectorioActual.Text = albumAEditar.SoundFilesPath;
            textBoxURISpotify.Text = albumAEditar.IdSpotify;
            vistaCanciones.View = View.List;
            ponerTextos();
            cargarVista();

        }
        private void ponerTextos()
        {
            Text = Program.LocalTexts.GetString("editando") + " " + albumAEditar.Artist + " - " + albumAEditar.Title;
            labelArtista.Text = Program.LocalTexts.GetString("artista");
            labelTitulo.Text = Program.LocalTexts.GetString("titulo");
            labelAño.Text = Program.LocalTexts.GetString("año");
            labelGeneros.Text = Program.LocalTexts.GetString("genero");
            labelCaratula.Text = Program.LocalTexts.GetString("caratula");
            labelDirectorio.Text = Program.LocalTexts.GetString("directorio");
            labelURISpotify.Text = Program.LocalTexts.GetString("uriSpotify");
            botonOkDoomer.Text = Program.LocalTexts.GetString("hecho");
            botonCancelar.Text = Program.LocalTexts.GetString("cancelar");
            botonCaratula.Text = Program.LocalTexts.GetString("buscar");
            buttonAñadirCancion.Text = Program.LocalTexts.GetString("añadir_cancion");
            buttonDirectorio.Text = Program.LocalTexts.GetString("buscarDirectorio");
            labelDirectorioActual.Text = albumAEditar.SoundFilesPath;
            for (int i = 0; i < generosTraducidos.Length; i++)
            {
                generosTraducidos[i] = Program.Genres[i].Name;
            }
            Array.Sort(generosTraducidos);
            comboBoxGeneros.Items.AddRange(generosTraducidos);
            int index = 0;
            for (int i = 0; i < generosTraducidos.Length; i++)
            {
                if (albumAEditar.Genre.Name == generosTraducidos[i])
                    index = i;
            }
            comboBoxGeneros.SelectedIndex = index;
            if(Config.Language == "el")
            {
                Font but = buttonAñadirCancion.Font;
                Font neo = new Font(but.FontFamily, 7);
                buttonAñadirCancion.Font = neo;
            }
        }
        private void cargarVista()
        {
            vistaCanciones.Items.Clear();
            ListViewItem[] items = new ListViewItem[albumAEditar.NumberOfSongs];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new ListViewItem(albumAEditar.Songs[i].Title);
            }
            vistaCanciones.Items.AddRange(items);
        }

        private void botonOkDoomer_Click(object sender, EventArgs e)
        {
            try//si está vacío pues guarda vacío
            {
                Log.Instance.PrintMessage("Intentando guardar", MessageType.Info);
                albumAEditar.Artist = textBoxArtista.Text;
                albumAEditar.Title = textBoxTitulo.Text;
                albumAEditar.Year = Convert.ToInt16(textBoxAño.Text);
                string gn = comboBoxGeneros.SelectedItem.ToString();
                Genre g = Program.Genres[Program.FindTranslatedGenre(gn)];
                albumAEditar.Genre = g;
                albumAEditar.CoverPath = labelRuta.Text;
                TimeSpan nuevaDuracion = new TimeSpan();
                albumAEditar.SoundFilesPath = labelDirectorioActual.Text;
                string[] uriSpotify = textBoxURISpotify.Text.Split(':');
                if(uriSpotify.Length == 3)
                    albumAEditar.IdSpotify = (uriSpotify[2]);
                else
                    albumAEditar.IdSpotify = (textBoxURISpotify.Text);
                foreach (Song c in albumAEditar.Songs)
                {
                    if(!c.IsBonus)
                        nuevaDuracion += c.Length;
                }
            }
            catch (NullReferenceException)
            {
                Log.Instance.PrintMessage("Algún campo está vacío", MessageType.Warning);
                MessageBox.Show(Program.LocalTexts.GetString("error_vacio1"));
            }

            catch (FormatException)
            {
                Log.Instance.PrintMessage("Formato incorrecto, no se guardará nada.", MessageType.Warning);
                MessageBox.Show(Program.LocalTexts.GetString("error_formato"));
                //throw;
            }
            catch (IndexOutOfRangeException)
            {
                Log.Instance.PrintMessage("Formato incorrecto, no se guardará nada.", MessageType.Warning);
                MessageBox.Show(Program.LocalTexts.GetString("error_formato"));
            }
            visualizarAlbum nuevo = new visualizarAlbum(ref albumAEditar);
            nuevo.Show();
            Program.ReloadView();
            Close();
            Program.ReloadView();
            Log.Instance.PrintMessage("Guardado sin problema", MessageType.Correct);
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            visualizarAlbum nuevo = new visualizarAlbum(ref albumAEditar);
            nuevo.Show();
            Close();
        }

        private void botonCaratula_Click(object sender, EventArgs e)
        {
            OpenFileDialog abrirImagen = new OpenFileDialog();
            abrirImagen.Filter = Program.LocalTexts.GetString("archivo") + " .jpg, .png|*.jpg;*.png;*.jpeg";
            abrirImagen.InitialDirectory = albumAEditar.SoundFilesPath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (abrirImagen.ShowDialog() == DialogResult.OK)
            {
                string fichero = abrirImagen.FileName;
                labelRuta.Text = fichero;
            }
        }

        private void vistaCanciones_MouseDoubleClick(object sender, MouseEventArgs e) //editar cancion
        {
            Log.Instance.PrintMessage("Editando canción", MessageType.Info);
            String text = vistaCanciones.SelectedItems[0].Text;
            Song cancionAEditar = albumAEditar.GetSong(text);
            agregarCancion editarCancion = new agregarCancion(ref cancionAEditar);
            editarCancion.ShowDialog();
            cargarVista();
            Log.Instance.PrintMessage("Guardado correctamente", MessageType.Correct);
        }

        private void buttonAñadirCancion_Click(object sender, EventArgs e)
        {
            agregarCancion AC = new agregarCancion(ref albumAEditar, -2);
            AC.ShowDialog();
            borrarVista();
            cargarVista();
        }
        private void borrarVista()
        {
            for (int i = 0; i < vistaCanciones.Items.Count; i++)
            {
                vistaCanciones.Clear();
            }
        }
        private void vistaCanciones_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                ListViewItem[] itemsborrar = new ListViewItem[vistaCanciones.SelectedItems.Count];
                int i = 0;
                foreach (ListViewItem item in vistaCanciones.SelectedItems)
                {
                    albumAEditar.RemoveSong(item.Text);
                    itemsborrar[i] = item;
                    i++;
                }
                foreach (var item in itemsborrar)
                {
                    vistaCanciones.Items.Remove(item);
                }
            }
        }

        private void buttonDirectorio_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialogCarpetaAlbum = new FolderBrowserDialog();
            dialogCarpetaAlbum.SelectedPath = Config.LastOpenedDirectory;
            DialogResult dr = dialogCarpetaAlbum.ShowDialog();
            if (dr == DialogResult.OK)
            {
                labelDirectorioActual.Text = Config.LastOpenedDirectory = dialogCarpetaAlbum.SelectedPath;
            }
        }
    }
}

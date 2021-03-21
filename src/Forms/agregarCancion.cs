﻿using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace Cassiopeia
{
    public partial class agregarCancion : Form
    {
        public string title;
        public int min, sec, np;
        private int cual;
        AlbumData album;
        Song cancion;
        LongSong cancionlarga;
        bool editar;
        bool larga;
        bool bonus;
        ToolTip ConsejoEsLarga;
        ToolTip ConsejoEsBonus;
        public agregarCancion(ref AlbumData a, int n) //caso normal
        {
            Log.Instance.PrintMessage("Creando canción", MessageType.Info);
            Stopwatch crono = Stopwatch.StartNew();
            InitializeComponent();
            album = a;
            cual = n;
            editar = false;
            textBoxNumPartes.Hide();
            labelNumPartes.Hide();
            cancionlarga = null;
            np = 0;
            ConsejoEsLarga = new ToolTip();
            ConsejoEsLarga.SetToolTip(esLarga, Program.LocalTexts.GetString("ayuda_larga"));
            ConsejoEsBonus = new ToolTip();
            ConsejoEsBonus.SetToolTip(checkBoxBonus, Program.LocalTexts.GetString("esBonusAyuda"));
            ponerTextos();
            crono.Stop();
            Log.Instance.PrintMessage("Cargado", MessageType.Correct, crono, TimeType.Milliseconds);
        }
        public agregarCancion(ref Song c) //editar
        {
            Log.Instance.PrintMessage("Editando canción", MessageType.Info);
            Stopwatch crono = Stopwatch.StartNew();
            InitializeComponent();
            cual = -1;
            esLarga.Hide();
            cancion = c;
            editar = true;
            cancionlarga = null;
            if(c is LongSong)
            {
                minTextBox.Enabled = false;
                secsTextBox.Enabled = false;
                checkBoxBonus.Enabled = false;
            }
            tituloTextBox.Text = c.Title;
            if (c.Length.TotalMinutes >= 60)
                minTextBox.Text = ((int)c.Length.TotalMinutes).ToString();
            else
                minTextBox.Text = c.Length.Minutes.ToString();
            secsTextBox.Text = c.Length.Seconds.ToString();
            if (c is LongSong)
            {
                minTextBox.Enabled = false;
                secsTextBox.Enabled = false;
            }
            esLarga.Hide();
            labelNumPartes.Hide();
            textBoxNumPartes.Hide();
            ConsejoEsBonus = new ToolTip();
            ConsejoEsBonus.SetToolTip(checkBoxBonus, Program.LocalTexts.GetString("esBonusAyuda"));
            if (c.IsBonus)
                checkBoxBonus.Checked = true;
            np = 0;
            ponerTextos();
            crono.Stop();
            Log.Instance.PrintMessage("Cargado", MessageType.Correct, crono, TimeType.Milliseconds);
        }
        public agregarCancion(ref AlbumData a, int n, bool l) //crear canción larga
        {
            Log.Instance.PrintMessage("Creando canción con partes", MessageType.Info);
            Stopwatch crono = Stopwatch.StartNew();
            InitializeComponent();
            larga = l;
            album = a;
            cual = n;
            editar = false;
            cancionlarga = null;
            ponerTextos();
            label2.Hide();
            labelMinutosSegundos.Hide();
            secsTextBox.Hide();
            minTextBox.Hide();
            esLarga.Hide();
            checkBoxBonus.Hide();
            crono.Stop();
            Log.Instance.PrintMessage("Cargado", MessageType.Correct, crono, TimeType.Milliseconds);
        }

        public agregarCancion(ref LongSong l, int n, ref AlbumData a) //crear parte de canción larga
        {
            Log.Instance.PrintMessage("Creando parte de canción larga", MessageType.Info);
            Stopwatch crono = Stopwatch.StartNew();
            InitializeComponent();
            cancionlarga = l;
            cual = n;
            editar = false;
            larga = true;
            album = a;
            textBoxNumPartes.Hide();
            labelNumPartes.Hide();
            esLarga.Hide();
            np = 0;
            checkBoxBonus.Hide();
            ponerTextos();
            crono.Stop();
            Log.Instance.PrintMessage("Cargado", MessageType.Correct, crono, TimeType.Milliseconds);
        }

        private void ponerTextos()
        {
            int cualdeVerdad = cual;
            if (cual == -2)
                cualdeVerdad = album.NumberOfSongs;

            if(editar)
            {
                Text = Program.LocalTexts.GetString("editando") + " " + cancion.Title;
                buttonOK.Text = Program.LocalTexts.GetString("hecho");
            } else
            {
                Text = Program.LocalTexts.GetString("añadir_cancion") + " " + (cualdeVerdad+1);
                buttonOK.Text = Program.LocalTexts.GetString("hecho");
            }

            if(cancionlarga != null)
            {
                Text = Program.LocalTexts.GetString("añadir_cancion") + " " + Utils.ConvertToRomanNumeral(cual);
            }

            buttonCancelar.Text = Program.LocalTexts.GetString("cancelar");
            labelTituloCancion.Text = Program.LocalTexts.GetString("introduce_cancion");
            labelMinutosSegundos.Text = Program.LocalTexts.GetString("min:sec");
            esLarga.Text = Program.LocalTexts.GetString("esLarga");
            labelNumPartes.Text = Program.LocalTexts.GetString("num_partes");
            checkBoxBonus.Text = Program.LocalTexts.GetString("esBonus");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(!larga && cancionlarga == null) //caso normal
                {
                    min = Convert.ToInt32(minTextBox.Text);
                    sec = Convert.ToInt32(secsTextBox.Text);
                    title = tituloTextBox.Text;
                    bonus = checkBoxBonus.Checked;
                    if (editar) //si edita
                    {
                        cancion.Title = title;
                        cancion.Length = new TimeSpan(0, min, sec);
                        cancion.IsBonus = bonus;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        Song c = new Song(title, new TimeSpan(0, min, sec), ref album, bonus);
                        album.AddSong(c);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                else if(larga && cancionlarga == null) //caso de que creemos una cancion larga, sin partes
                {
                    title = tituloTextBox.Text;
                    min = sec = 0;
                    np = Convert.ToInt32(textBoxNumPartes.Text);
                    LongSong longSong = new LongSong(title, album);

                    album.AddSong(longSong);
                    for (int i = 0; i < np; i++)
                    {
                        agregarCancion addParte = new agregarCancion(ref longSong, i + 1, ref album);
                        addParte.ShowDialog();
                        if (addParte.DialogResult == DialogResult.Cancel)
                            break;
                        else
                            DialogResult = DialogResult.OK;
                    }
                }
                else if(cancionlarga != null && larga == true)//parte de una cancion normal
                {
                    title = tituloTextBox.Text;
                    min = Convert.ToInt32(minTextBox.Text);
                    sec = Convert.ToInt32(secsTextBox.Text);
                    TimeSpan dur = new TimeSpan(0, min, sec);
                    np = 0;
                    Song p = new Song(title, dur, ref album);
                    cancionlarga.AddPart(p);
                    DialogResult = DialogResult.OK;
                }
                Dispose();
            }
            catch (NullReferenceException ex)
            {
                Log.Instance.PrintMessage(ex.Message, MessageType.Error);

                MessageBox.Show(Program.LocalTexts.GetString("error_vacio1"));

            }

            catch (FormatException ex)
            {
                Log.Instance.PrintMessage(ex.Message, MessageType.Error);
                MessageBox.Show(Program.LocalTexts.GetString("error_formato"));
                //throw;
            }


        }
        private void agregarCancion_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                min = Convert.ToInt32(minTextBox.Text);
                sec = Convert.ToInt32(secsTextBox.Text);
                title = tituloTextBox.Text;
                Song c = new Song(title, new TimeSpan(0, min, sec), ref album);
                album.AddSong(c, cual);
                Log.Instance.PrintMessage(title + " añadido correctamente", MessageType.Correct);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK);
                Log.Instance.PrintMessage(ex.Message, MessageType.Error);

            }
            Close();
        }

        private void tituloTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Tab))
            {
                minTextBox.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult =  DialogResult.Cancel;
            Log.Instance.PrintMessage("Cancelado", MessageType.Info);
            Close();
        }

        private void esLarga_Click(object sender, EventArgs e)
        {
            agregarCancion larga = new agregarCancion(ref album, cual, true);
            DialogResult = DialogResult.OK;
            larga.ShowDialog();
        }
    }
}

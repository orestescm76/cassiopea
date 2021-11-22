﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Cassiopeia.src.Forms
{
    public partial class About : Form
    {
        private bool bannerAntiguo = false;
        public About()
        {
            InitializeComponent();
            Text = Kernel.LocalTexts.GetString("acerca") + " " + Kernel.LocalTexts.GetString("titulo_ventana_principal") + " " + Kernel.Version;
            PonerTextos();
        }
        private void PonerTextos()
        {
            labelAcercaDe.Text = "";
            labelAcercaDe.AutoSize = true;
            string acercadeTexto = Kernel.Version + " Codename " + Kernel.CodeName;
            acercadeTexto += Environment.NewLine;
            acercadeTexto += Kernel.LocalTexts.GetString("desarrolladoPor") + " Orestes Colomina Monsalve" + Environment.NewLine +
                Kernel.LocalTexts.GetString("contacto") + Environment.NewLine + Environment.NewLine + Kernel.LocalTexts.GetString("agradecimientosA") + Environment.NewLine +
                Kernel.LocalTexts.GetString("agradecimiento1") + Environment.NewLine + "https://github.com/JohnnyCrazy/SpotifyAPI-NET" + Environment.NewLine +
                Kernel.LocalTexts.GetString("agradecimiento3") + Environment.NewLine +
                Kernel.LocalTexts.GetString("agradecimiento4") + Environment.NewLine;

            switch (Config.Language)
            {
                case "it":
                case "ca":
                    acercadeTexto += Kernel.LocalTexts.GetString("agradecimientoTraduccion");
                    break;
                case "el":
                    labelAcercaDe.Location = new Point(2, 186);
                    Font but = labelAcercaDe.Font;
                    Font neo = new Font(but.FontFamily, 10);
                    labelAcercaDe.Font = neo;
                    break;
                default:
                    break;
            }
            labelAcercaDe.Text = acercadeTexto;
            int posX = Width - labelAcercaDe.Size.Width;
            labelAcercaDe.Location = new Point(posX / 2, pictureBoxBanner.Size.Height + 1);
            labelBTC.Location = new Point((Width - labelBTC.Size.Width)/2, labelBTC.Location.Y);
        }
        private void cambiarBanner()
        {
            if (!bannerAntiguo)
                pictureBoxBanner.Image = Properties.Resources.banner1_6;
            else
                pictureBoxBanner.Image = Properties.Resources.banner1_5;
        }
        private void acercaDe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Close();
                Dispose();
            }
        }

        private void acercaDe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if ((e.Control & e.Alt) && e.KeyCode == Keys.F9)
            {
                bannerAntiguo = !bannerAntiguo;
                cambiarBanner();
            }
        }
    }
}

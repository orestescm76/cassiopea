﻿namespace Cassiopeia
{
    partial class agregarCancion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(agregarCancion));
            this.tituloTextBox = new System.Windows.Forms.TextBox();
            this.secsTextBox = new System.Windows.Forms.TextBox();
            this.minTextBox = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTituloCancion = new System.Windows.Forms.Label();
            this.labelMinutosSegundos = new System.Windows.Forms.Label();
            this.buttonCancelar = new System.Windows.Forms.Button();
            this.esLarga = new System.Windows.Forms.Button();
            this.labelNumPartes = new System.Windows.Forms.Label();
            this.textBoxNumPartes = new System.Windows.Forms.TextBox();
            this.checkBoxBonus = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tituloTextBox
            // 
            this.tituloTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tituloTextBox.Location = new System.Drawing.Point(12, 58);
            this.tituloTextBox.Name = "tituloTextBox";
            this.tituloTextBox.Size = new System.Drawing.Size(303, 25);
            this.tituloTextBox.TabIndex = 0;
            this.tituloTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tituloTextBox_KeyDown);
            // 
            // secsTextBox
            // 
            this.secsTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secsTextBox.Location = new System.Drawing.Point(190, 120);
            this.secsTextBox.Name = "secsTextBox";
            this.secsTextBox.Size = new System.Drawing.Size(125, 25);
            this.secsTextBox.TabIndex = 2;
            // 
            // minTextBox
            // 
            this.minTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minTextBox.Location = new System.Drawing.Point(12, 120);
            this.minTextBox.Name = "minTextBox";
            this.minTextBox.Size = new System.Drawing.Size(124, 25);
            this.minTextBox.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOK.Location = new System.Drawing.Point(12, 175);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(126, 32);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "añadir9";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(154, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 29);
            this.label2.TabIndex = 4;
            this.label2.Text = ":";
            // 
            // labelTituloCancion
            // 
            this.labelTituloCancion.AutoSize = true;
            this.labelTituloCancion.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTituloCancion.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelTituloCancion.Location = new System.Drawing.Point(9, 9);
            this.labelTituloCancion.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelTituloCancion.Name = "labelTituloCancion";
            this.labelTituloCancion.Padding = new System.Windows.Forms.Padding(5, 5, 15, 5);
            this.labelTituloCancion.Size = new System.Drawing.Size(129, 27);
            this.labelTituloCancion.TabIndex = 5;
            this.labelTituloCancion.Text = "introduceTitulo11";
            this.labelTituloCancion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMinutosSegundos
            // 
            this.labelMinutosSegundos.AutoSize = true;
            this.labelMinutosSegundos.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMinutosSegundos.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelMinutosSegundos.Location = new System.Drawing.Point(124, 86);
            this.labelMinutosSegundos.Name = "labelMinutosSegundos";
            this.labelMinutosSegundos.Padding = new System.Windows.Forms.Padding(10, 8, 10, 0);
            this.labelMinutosSegundos.Size = new System.Drawing.Size(70, 21);
            this.labelMinutosSegundos.TabIndex = 5;
            this.labelMinutosSegundos.Text = "mm:ss12";
            this.labelMinutosSegundos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonCancelar
            // 
            this.buttonCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancelar.Location = new System.Drawing.Point(190, 175);
            this.buttonCancelar.Name = "buttonCancelar";
            this.buttonCancelar.Size = new System.Drawing.Size(125, 32);
            this.buttonCancelar.TabIndex = 6;
            this.buttonCancelar.Text = "cancelar10";
            this.buttonCancelar.UseVisualStyleBackColor = true;
            this.buttonCancelar.Click += new System.EventHandler(this.button2_Click);
            // 
            // esLarga
            // 
            this.esLarga.Location = new System.Drawing.Point(226, 13);
            this.esLarga.Name = "esLarga";
            this.esLarga.Size = new System.Drawing.Size(89, 23);
            this.esLarga.TabIndex = 7;
            this.esLarga.Text = "eslarga?";
            this.esLarga.UseVisualStyleBackColor = true;
            this.esLarga.Click += new System.EventHandler(this.esLarga_Click);
            // 
            // labelNumPartes
            // 
            this.labelNumPartes.AutoSize = true;
            this.labelNumPartes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNumPartes.Location = new System.Drawing.Point(124, 94);
            this.labelNumPartes.Name = "labelNumPartes";
            this.labelNumPartes.Size = new System.Drawing.Size(65, 13);
            this.labelNumPartes.TabIndex = 8;
            this.labelNumPartes.Text = "num partes";
            // 
            // textBoxNumPartes
            // 
            this.textBoxNumPartes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNumPartes.Location = new System.Drawing.Point(115, 120);
            this.textBoxNumPartes.Name = "textBoxNumPartes";
            this.textBoxNumPartes.Size = new System.Drawing.Size(89, 25);
            this.textBoxNumPartes.TabIndex = 9;
            // 
            // checkBoxBonus
            // 
            this.checkBoxBonus.AutoSize = true;
            this.checkBoxBonus.Location = new System.Drawing.Point(13, 152);
            this.checkBoxBonus.Name = "checkBoxBonus";
            this.checkBoxBonus.Size = new System.Drawing.Size(55, 17);
            this.checkBoxBonus.TabIndex = 10;
            this.checkBoxBonus.Text = "bonus";
            this.checkBoxBonus.UseVisualStyleBackColor = true;
            // 
            // agregarCancion
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 219);
            this.Controls.Add(this.checkBoxBonus);
            this.Controls.Add(this.textBoxNumPartes);
            this.Controls.Add(this.labelNumPartes);
            this.Controls.Add(this.esLarga);
            this.Controls.Add(this.buttonCancelar);
            this.Controls.Add(this.labelTituloCancion);
            this.Controls.Add(this.labelMinutosSegundos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.minTextBox);
            this.Controls.Add(this.secsTextBox);
            this.Controls.Add(this.tituloTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "agregarCancion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "agregarCancion";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.agregarCancion_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tituloTextBox;
        private System.Windows.Forms.TextBox secsTextBox;
        private System.Windows.Forms.TextBox minTextBox;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelTituloCancion;
        private System.Windows.Forms.Label labelMinutosSegundos;
        private System.Windows.Forms.Button buttonCancelar;
        private System.Windows.Forms.Button esLarga;
        private System.Windows.Forms.Label labelNumPartes;
        private System.Windows.Forms.TextBox textBoxNumPartes;
        private System.Windows.Forms.CheckBox checkBoxBonus;
    }
}
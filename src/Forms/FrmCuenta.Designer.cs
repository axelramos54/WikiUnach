namespace WUNACH
{
    partial class FrmCuenta
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCuenta));
            this.pnlIngreso = new System.Windows.Forms.Panel();
            this.btnAdministrarTareas = new System.Windows.Forms.Button();
            this.btnregreso = new System.Windows.Forms.Button();
            this.btneditar = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cmbrol = new System.Windows.Forms.ComboBox();
            this.txtcorreo = new System.Windows.Forms.TextBox();
            this.txtnombre = new System.Windows.Forms.TextBox();
            this.txtmatricula = new System.Windows.Forms.TextBox();
            this.lblfechar = new System.Windows.Forms.Label();
            this.lblrol = new System.Windows.Forms.Label();
            this.lblcorreo = new System.Windows.Forms.Label();
            this.lblnombre = new System.Windows.Forms.Label();
            this.lblmatricula = new System.Windows.Forms.Label();
            this.lbldatosp = new System.Windows.Forms.Label();
            this.pnlTitulo = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblboblioteca = new System.Windows.Forms.Label();
            this.lblwikiestudiante = new System.Windows.Forms.Label();
            this.pnlIngreso.SuspendLayout();
            this.pnlTitulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlIngreso
            // 
            this.pnlIngreso.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlIngreso.Controls.Add(this.btnAdministrarTareas);
            this.pnlIngreso.Controls.Add(this.btnregreso);
            this.pnlIngreso.Controls.Add(this.btneditar);
            this.pnlIngreso.Controls.Add(this.textBox1);
            this.pnlIngreso.Controls.Add(this.cmbrol);
            this.pnlIngreso.Controls.Add(this.txtcorreo);
            this.pnlIngreso.Controls.Add(this.txtnombre);
            this.pnlIngreso.Controls.Add(this.txtmatricula);
            this.pnlIngreso.Controls.Add(this.lblfechar);
            this.pnlIngreso.Controls.Add(this.lblrol);
            this.pnlIngreso.Controls.Add(this.lblcorreo);
            this.pnlIngreso.Controls.Add(this.lblnombre);
            this.pnlIngreso.Controls.Add(this.lblmatricula);
            this.pnlIngreso.Controls.Add(this.lbldatosp);
            this.pnlIngreso.Location = new System.Drawing.Point(-13, 121);
            this.pnlIngreso.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlIngreso.Name = "pnlIngreso";
            this.pnlIngreso.Size = new System.Drawing.Size(1200, 546);
            this.pnlIngreso.TabIndex = 5;
            // 
            // btnAdministrarTareas
            // 
            this.btnAdministrarTareas.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnAdministrarTareas.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdministrarTareas.Location = new System.Drawing.Point(826, 402);
            this.btnAdministrarTareas.Name = "btnAdministrarTareas";
            this.btnAdministrarTareas.Size = new System.Drawing.Size(236, 65);
            this.btnAdministrarTareas.TabIndex = 15;
            this.btnAdministrarTareas.Text = "Administrar Tareas";
            this.btnAdministrarTareas.UseVisualStyleBackColor = false;
            // 
            // btnregreso
            // 
            this.btnregreso.BackColor = System.Drawing.Color.White;
            this.btnregreso.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnregreso.BackgroundImage")));
            this.btnregreso.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnregreso.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnregreso.Location = new System.Drawing.Point(237, 402);
            this.btnregreso.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnregreso.Name = "btnregreso";
            this.btnregreso.Size = new System.Drawing.Size(126, 65);
            this.btnregreso.TabIndex = 14;
            this.btnregreso.UseVisualStyleBackColor = false;
            this.btnregreso.Click += new System.EventHandler(this.btnregreso_Click);
            // 
            // btneditar
            // 
            this.btneditar.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btneditar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btneditar.Location = new System.Drawing.Point(34, 402);
            this.btneditar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btneditar.Name = "btneditar";
            this.btneditar.Size = new System.Drawing.Size(150, 65);
            this.btneditar.TabIndex = 11;
            this.btneditar.Text = "Editar ✏️";
            this.btneditar.UseVisualStyleBackColor = false;
            this.btneditar.Click += new System.EventHandler(this.BtnEditar_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(276, 311);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(228, 26);
            this.textBox1.TabIndex = 10;
            // 
            // cmbrol
            // 
            this.cmbrol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbrol.FormattingEnabled = true;
            this.cmbrol.Location = new System.Drawing.Point(124, 251);
            this.cmbrol.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbrol.Name = "cmbrol";
            this.cmbrol.Size = new System.Drawing.Size(202, 28);
            this.cmbrol.TabIndex = 9;
            // 
            // txtcorreo
            // 
            this.txtcorreo.Location = new System.Drawing.Point(160, 189);
            this.txtcorreo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtcorreo.Name = "txtcorreo";
            this.txtcorreo.Size = new System.Drawing.Size(343, 26);
            this.txtcorreo.TabIndex = 8;
            // 
            // txtnombre
            // 
            this.txtnombre.Location = new System.Drawing.Point(176, 129);
            this.txtnombre.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtnombre.Name = "txtnombre";
            this.txtnombre.Size = new System.Drawing.Size(328, 26);
            this.txtnombre.TabIndex = 7;
            // 
            // txtmatricula
            // 
            this.txtmatricula.Location = new System.Drawing.Point(186, 71);
            this.txtmatricula.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtmatricula.Name = "txtmatricula";
            this.txtmatricula.Size = new System.Drawing.Size(318, 26);
            this.txtmatricula.TabIndex = 6;
            // 
            // lblfechar
            // 
            this.lblfechar.AutoSize = true;
            this.lblfechar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfechar.Location = new System.Drawing.Point(62, 306);
            this.lblfechar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblfechar.Name = "lblfechar";
            this.lblfechar.Size = new System.Drawing.Size(216, 32);
            this.lblfechar.TabIndex = 5;
            this.lblfechar.Text = "Fecha De Registro:";
            // 
            // lblrol
            // 
            this.lblrol.AutoSize = true;
            this.lblrol.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblrol.Location = new System.Drawing.Point(62, 246);
            this.lblrol.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblrol.Name = "lblrol";
            this.lblrol.Size = new System.Drawing.Size(54, 32);
            this.lblrol.TabIndex = 4;
            this.lblrol.Text = "Rol:";
            // 
            // lblcorreo
            // 
            this.lblcorreo.AutoSize = true;
            this.lblcorreo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblcorreo.Location = new System.Drawing.Point(62, 185);
            this.lblcorreo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblcorreo.Name = "lblcorreo";
            this.lblcorreo.Size = new System.Drawing.Size(94, 32);
            this.lblcorreo.TabIndex = 3;
            this.lblcorreo.Text = "Correo:";
            // 
            // lblnombre
            // 
            this.lblnombre.AutoSize = true;
            this.lblnombre.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnombre.Location = new System.Drawing.Point(62, 125);
            this.lblnombre.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblnombre.Name = "lblnombre";
            this.lblnombre.Size = new System.Drawing.Size(109, 32);
            this.lblnombre.TabIndex = 2;
            this.lblnombre.Text = "Nombre:";
            // 
            // lblmatricula
            // 
            this.lblmatricula.AutoSize = true;
            this.lblmatricula.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblmatricula.Location = new System.Drawing.Point(62, 66);
            this.lblmatricula.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblmatricula.Name = "lblmatricula";
            this.lblmatricula.Size = new System.Drawing.Size(123, 32);
            this.lblmatricula.TabIndex = 1;
            this.lblmatricula.Text = "Matrícula:";
            // 
            // lbldatosp
            // 
            this.lbldatosp.AutoSize = true;
            this.lbldatosp.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbldatosp.Location = new System.Drawing.Point(56, 9);
            this.lbldatosp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbldatosp.Name = "lbldatosp";
            this.lbldatosp.Size = new System.Drawing.Size(312, 40);
            this.lbldatosp.TabIndex = 0;
            this.lbldatosp.Text = "Datos Del Usuario 👤";
            // 
            // pnlTitulo
            // 
            this.pnlTitulo.BackColor = System.Drawing.Color.MidnightBlue;
            this.pnlTitulo.Controls.Add(this.button1);
            this.pnlTitulo.Controls.Add(this.panel1);
            this.pnlTitulo.Controls.Add(this.lblboblioteca);
            this.pnlTitulo.Controls.Add(this.lblwikiestudiante);
            this.pnlTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlTitulo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTitulo.Name = "pnlTitulo";
            this.pnlTitulo.Size = new System.Drawing.Size(1175, 108);
            this.pnlTitulo.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.Location = new System.Drawing.Point(1058, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 75);
            this.button1.TabIndex = 21;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gold;
            this.panel1.Location = new System.Drawing.Point(0, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1174, 12);
            this.panel1.TabIndex = 20;
            // 
            // lblboblioteca
            // 
            this.lblboblioteca.AutoSize = true;
            this.lblboblioteca.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblboblioteca.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblboblioteca.Location = new System.Drawing.Point(22, 63);
            this.lblboblioteca.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblboblioteca.Name = "lblboblioteca";
            this.lblboblioteca.Size = new System.Drawing.Size(177, 28);
            this.lblboblioteca.TabIndex = 2;
            this.lblboblioteca.Text = "Biblioteca Digital";
            // 
            // lblwikiestudiante
            // 
            this.lblwikiestudiante.AutoSize = true;
            this.lblwikiestudiante.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblwikiestudiante.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblwikiestudiante.Location = new System.Drawing.Point(18, 14);
            this.lblwikiestudiante.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblwikiestudiante.Name = "lblwikiestudiante";
            this.lblwikiestudiante.Size = new System.Drawing.Size(202, 48);
            this.lblwikiestudiante.TabIndex = 1;
            this.lblwikiestudiante.Text = "WikiUnach";
            // 
            // FrmCuenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1175, 676);
            this.Controls.Add(this.pnlIngreso);
            this.Controls.Add(this.pnlTitulo);
            this.Name = "FrmCuenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmCuenta";
            this.pnlIngreso.ResumeLayout(false);
            this.pnlIngreso.PerformLayout();
            this.pnlTitulo.ResumeLayout(false);
            this.pnlTitulo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlIngreso;
        private System.Windows.Forms.Button btnAdministrarTareas;
        private System.Windows.Forms.Button btnregreso;
        private System.Windows.Forms.Button btneditar;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox cmbrol;
        private System.Windows.Forms.TextBox txtcorreo;
        private System.Windows.Forms.TextBox txtnombre;
        private System.Windows.Forms.TextBox txtmatricula;
        private System.Windows.Forms.Label lblfechar;
        private System.Windows.Forms.Label lblrol;
        private System.Windows.Forms.Label lblcorreo;
        private System.Windows.Forms.Label lblnombre;
        private System.Windows.Forms.Label lblmatricula;
        private System.Windows.Forms.Label lbldatosp;
        private System.Windows.Forms.Panel pnlTitulo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblboblioteca;
        private System.Windows.Forms.Label lblwikiestudiante;
    }
}
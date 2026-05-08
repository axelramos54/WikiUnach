namespace WUNACH
{
    partial class FrmCrearcuenta
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCrearcuenta));
            this.lblFacultad = new System.Windows.Forms.Label();
            this.lbldatosp = new System.Windows.Forms.Label();
            this.cmbFacultad = new System.Windows.Forms.ComboBox();
            this.btncrear = new System.Windows.Forms.Button();
            this.cmbrol = new System.Windows.Forms.ComboBox();
            this.txtcontraseña = new System.Windows.Forms.TextBox();
            this.txtcorreo = new System.Windows.Forms.TextBox();
            this.txtnombre = new System.Windows.Forms.TextBox();
            this.txtmatricula = new System.Windows.Forms.TextBox();
            this.lblrol = new System.Windows.Forms.Label();
            this.lblcontraseña = new System.Windows.Forms.Label();
            this.lblcorreo = new System.Windows.Forms.Label();
            this.lblnombre = new System.Windows.Forms.Label();
            this.lblmatricula = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblboblioteca = new System.Windows.Forms.Label();
            this.lblwikiestudiante = new System.Windows.Forms.Label();
            this.lblLicenciatura = new System.Windows.Forms.Label();
            this.cmbLicenciatura = new System.Windows.Forms.ComboBox();
            this.lblSemestre = new System.Windows.Forms.Label();
            this.cmbSemestre = new System.Windows.Forms.ComboBox();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFacultad
            // 
            this.lblFacultad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFacultad.AutoSize = true;
            this.lblFacultad.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFacultad.Location = new System.Drawing.Point(27, 344);
            this.lblFacultad.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFacultad.Name = "lblFacultad";
            this.lblFacultad.Size = new System.Drawing.Size(111, 32);
            this.lblFacultad.TabIndex = 45;
            this.lblFacultad.Text = "Facultad:";
            // 
            // lbldatosp
            // 
            this.lbldatosp.AutoSize = true;
            this.lbldatosp.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbldatosp.Location = new System.Drawing.Point(27, 107);
            this.lbldatosp.Name = "lbldatosp";
            this.lbldatosp.Size = new System.Drawing.Size(196, 40);
            this.lbldatosp.TabIndex = 44;
            this.lbldatosp.Text = "Crear Cuenta";
            // 
            // cmbFacultad
            // 
            this.cmbFacultad.FormattingEnabled = true;
            this.cmbFacultad.Location = new System.Drawing.Point(104, 346);
            this.cmbFacultad.Name = "cmbFacultad";
            this.cmbFacultad.Size = new System.Drawing.Size(188, 29);
            this.cmbFacultad.TabIndex = 43;
            // 
            // btncrear
            // 
            this.btncrear.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btncrear.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncrear.Location = new System.Drawing.Point(308, 478);
            this.btncrear.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btncrear.Name = "btncrear";
            this.btncrear.Size = new System.Drawing.Size(134, 42);
            this.btncrear.TabIndex = 42;
            this.btncrear.Text = "Crear Cuenta";
            this.btncrear.UseVisualStyleBackColor = false;
            this.btncrear.Click += new System.EventHandler(this.btncrear_Click);
            // 
            // cmbrol
            // 
            this.cmbrol.FormattingEnabled = true;
            this.cmbrol.Items.AddRange(new object[] {
            "MAESTRO",
            "ALUMNO"});
            this.cmbrol.Location = new System.Drawing.Point(74, 308);
            this.cmbrol.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbrol.Name = "cmbrol";
            this.cmbrol.Size = new System.Drawing.Size(180, 29);
            this.cmbrol.TabIndex = 41;
            // 
            // txtcontraseña
            // 
            this.txtcontraseña.Location = new System.Drawing.Point(131, 270);
            this.txtcontraseña.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtcontraseña.Name = "txtcontraseña";
            this.txtcontraseña.Size = new System.Drawing.Size(432, 27);
            this.txtcontraseña.TabIndex = 40;
            // 
            // txtcorreo
            // 
            this.txtcorreo.Location = new System.Drawing.Point(100, 232);
            this.txtcorreo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtcorreo.Name = "txtcorreo";
            this.txtcorreo.Size = new System.Drawing.Size(463, 27);
            this.txtcorreo.TabIndex = 39;
            // 
            // txtnombre
            // 
            this.txtnombre.Location = new System.Drawing.Point(184, 193);
            this.txtnombre.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtnombre.Name = "txtnombre";
            this.txtnombre.Size = new System.Drawing.Size(379, 27);
            this.txtnombre.TabIndex = 38;
            // 
            // txtmatricula
            // 
            this.txtmatricula.Location = new System.Drawing.Point(184, 154);
            this.txtmatricula.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtmatricula.Name = "txtmatricula";
            this.txtmatricula.Size = new System.Drawing.Size(379, 27);
            this.txtmatricula.TabIndex = 37;
            // 
            // lblrol
            // 
            this.lblrol.AutoSize = true;
            this.lblrol.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblrol.Location = new System.Drawing.Point(27, 306);
            this.lblrol.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblrol.Name = "lblrol";
            this.lblrol.Size = new System.Drawing.Size(54, 32);
            this.lblrol.TabIndex = 36;
            this.lblrol.Text = "Rol:";
            // 
            // lblcontraseña
            // 
            this.lblcontraseña.AutoSize = true;
            this.lblcontraseña.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblcontraseña.Location = new System.Drawing.Point(27, 267);
            this.lblcontraseña.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblcontraseña.Name = "lblcontraseña";
            this.lblcontraseña.Size = new System.Drawing.Size(144, 32);
            this.lblcontraseña.TabIndex = 35;
            this.lblcontraseña.Text = "Contraseña:";
            // 
            // lblcorreo
            // 
            this.lblcorreo.AutoSize = true;
            this.lblcorreo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblcorreo.Location = new System.Drawing.Point(27, 229);
            this.lblcorreo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblcorreo.Name = "lblcorreo";
            this.lblcorreo.Size = new System.Drawing.Size(94, 32);
            this.lblcorreo.TabIndex = 34;
            this.lblcorreo.Text = "Correo:";
            // 
            // lblnombre
            // 
            this.lblnombre.AutoSize = true;
            this.lblnombre.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnombre.Location = new System.Drawing.Point(27, 190);
            this.lblnombre.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblnombre.Name = "lblnombre";
            this.lblnombre.Size = new System.Drawing.Size(236, 32);
            this.lblnombre.TabIndex = 33;
            this.lblnombre.Text = "Nombre De Usuario:";
            // 
            // lblmatricula
            // 
            this.lblmatricula.AutoSize = true;
            this.lblmatricula.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblmatricula.Location = new System.Drawing.Point(27, 152);
            this.lblmatricula.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblmatricula.Name = "lblmatricula";
            this.lblmatricula.Size = new System.Drawing.Size(225, 32);
            this.lblmatricula.TabIndex = 32;
            this.lblmatricula.Text = "Matricula(si aplica):";
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.MidnightBlue;
            this.panelHeader.Controls.Add(this.button1);
            this.panelHeader.Controls.Add(this.panel1);
            this.panelHeader.Controls.Add(this.lblboblioteca);
            this.panelHeader.Controls.Add(this.lblwikiestudiante);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(852, 75);
            this.panelHeader.TabIndex = 31;
            // 
            // button1
            // 
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.Location = new System.Drawing.Point(714, 7);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(58, 49);
            this.button1.TabIndex = 25;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gold;
            this.panel1.Location = new System.Drawing.Point(0, 65);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(853, 10);
            this.panel1.TabIndex = 25;
            // 
            // lblboblioteca
            // 
            this.lblboblioteca.AutoSize = true;
            this.lblboblioteca.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblboblioteca.ForeColor = System.Drawing.Color.White;
            this.lblboblioteca.Location = new System.Drawing.Point(16, 39);
            this.lblboblioteca.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblboblioteca.Name = "lblboblioteca";
            this.lblboblioteca.Size = new System.Drawing.Size(177, 28);
            this.lblboblioteca.TabIndex = 1;
            this.lblboblioteca.Text = "Biblioteca Digital";
            // 
            // lblwikiestudiante
            // 
            this.lblwikiestudiante.AutoSize = true;
            this.lblwikiestudiante.BackColor = System.Drawing.Color.Transparent;
            this.lblwikiestudiante.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblwikiestudiante.ForeColor = System.Drawing.Color.White;
            this.lblwikiestudiante.Location = new System.Drawing.Point(11, 11);
            this.lblwikiestudiante.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblwikiestudiante.Name = "lblwikiestudiante";
            this.lblwikiestudiante.Size = new System.Drawing.Size(202, 48);
            this.lblwikiestudiante.TabIndex = 0;
            this.lblwikiestudiante.Text = "WikiUnach";
            // 
            // lblLicenciatura
            // 
            this.lblLicenciatura.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLicenciatura.AutoSize = true;
            this.lblLicenciatura.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicenciatura.Location = new System.Drawing.Point(27, 379);
            this.lblLicenciatura.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLicenciatura.Name = "lblLicenciatura";
            this.lblLicenciatura.Size = new System.Drawing.Size(151, 32);
            this.lblLicenciatura.TabIndex = 47;
            this.lblLicenciatura.Text = "Licenciatura:";
            // 
            // cmbLicenciatura
            // 
            this.cmbLicenciatura.FormattingEnabled = true;
            this.cmbLicenciatura.Location = new System.Drawing.Point(184, 381);
            this.cmbLicenciatura.Name = "cmbLicenciatura";
            this.cmbLicenciatura.Size = new System.Drawing.Size(188, 29);
            this.cmbLicenciatura.TabIndex = 46;
            // 
            // lblSemestre
            // 
            this.lblSemestre.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSemestre.AutoSize = true;
            this.lblSemestre.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSemestre.Location = new System.Drawing.Point(27, 414);
            this.lblSemestre.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSemestre.Name = "lblSemestre";
            this.lblSemestre.Size = new System.Drawing.Size(121, 32);
            this.lblSemestre.TabIndex = 49;
            this.lblSemestre.Text = "Semestre:";
            // 
            // cmbSemestre
            // 
            this.cmbSemestre.FormattingEnabled = true;
            this.cmbSemestre.Location = new System.Drawing.Point(155, 417);
            this.cmbSemestre.Name = "cmbSemestre";
            this.cmbSemestre.Size = new System.Drawing.Size(188, 29);
            this.cmbSemestre.TabIndex = 48;
            // 
            // FrmCrearcuenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 616);
            this.Controls.Add(this.lblSemestre);
            this.Controls.Add(this.cmbSemestre);
            this.Controls.Add(this.lblLicenciatura);
            this.Controls.Add(this.cmbLicenciatura);
            this.Controls.Add(this.lblFacultad);
            this.Controls.Add(this.lbldatosp);
            this.Controls.Add(this.cmbFacultad);
            this.Controls.Add(this.btncrear);
            this.Controls.Add(this.cmbrol);
            this.Controls.Add(this.txtcontraseña);
            this.Controls.Add(this.txtcorreo);
            this.Controls.Add(this.txtnombre);
            this.Controls.Add(this.txtmatricula);
            this.Controls.Add(this.lblrol);
            this.Controls.Add(this.lblcontraseña);
            this.Controls.Add(this.lblcorreo);
            this.Controls.Add(this.lblnombre);
            this.Controls.Add(this.lblmatricula);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Modern No. 20", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmCrearcuenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmCrearcuenta";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFacultad;
        private System.Windows.Forms.Label lbldatosp;
        private System.Windows.Forms.ComboBox cmbFacultad;
        private System.Windows.Forms.Button btncrear;
        private System.Windows.Forms.ComboBox cmbrol;
        private System.Windows.Forms.TextBox txtcontraseña;
        private System.Windows.Forms.TextBox txtcorreo;
        private System.Windows.Forms.TextBox txtnombre;
        private System.Windows.Forms.TextBox txtmatricula;
        private System.Windows.Forms.Label lblrol;
        private System.Windows.Forms.Label lblcontraseña;
        private System.Windows.Forms.Label lblcorreo;
        private System.Windows.Forms.Label lblnombre;
        private System.Windows.Forms.Label lblmatricula;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblboblioteca;
        private System.Windows.Forms.Label lblwikiestudiante;
        private System.Windows.Forms.Label lblLicenciatura;
        private System.Windows.Forms.ComboBox cmbLicenciatura;
        private System.Windows.Forms.Label lblSemestre;
        private System.Windows.Forms.ComboBox cmbSemestre;
    }
}
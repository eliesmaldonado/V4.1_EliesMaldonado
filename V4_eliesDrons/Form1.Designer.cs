namespace V4_eliesDrons
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelMap = new Panel();
            panelLateral = new Panel();
            btnCargarRuta = new Button();
            btnGuardarRuta = new Button();
            btnLimpiarRuta = new Button();
            btnEliminar = new Button();
            btnBajar = new Button();
            btnSubir = new Button();
            groupBoxFunciones = new GroupBox();
            btnAplicar = new Button();
            lblLongitud = new Label();
            lblLatitud = new Label();
            nudHeading = new NumericUpDown();
            nudAltitud = new NumericUpDown();
            label2 = new Label();
            label1 = new Label();
            cbFuncion = new ComboBox();
            waypointListBox = new ListBox();
            btnActualizarRuta = new Button();
            panelLateral.SuspendLayout();
            groupBoxFunciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudHeading).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAltitud).BeginInit();
            SuspendLayout();
            // 
            // panelMap
            // 
            panelMap.BackColor = SystemColors.ControlLight;
            panelMap.BorderStyle = BorderStyle.FixedSingle;
            panelMap.Location = new Point(208, 216);
            panelMap.Name = "panelMap";
            panelMap.Size = new Size(733, 420);
            panelMap.TabIndex = 0;
            // 
            // panelLateral
            // 
            panelLateral.BackColor = SystemColors.ControlLight;
            panelLateral.Controls.Add(btnActualizarRuta);
            panelLateral.Controls.Add(btnCargarRuta);
            panelLateral.Controls.Add(btnGuardarRuta);
            panelLateral.Controls.Add(btnLimpiarRuta);
            panelLateral.Controls.Add(btnEliminar);
            panelLateral.Controls.Add(btnBajar);
            panelLateral.Controls.Add(btnSubir);
            panelLateral.Controls.Add(groupBoxFunciones);
            panelLateral.Controls.Add(waypointListBox);
            panelLateral.Location = new Point(990, 12);
            panelLateral.Name = "panelLateral";
            panelLateral.Size = new Size(330, 724);
            panelLateral.TabIndex = 1;
            // 
            // btnCargarRuta
            // 
            btnCargarRuta.BackColor = Color.Blue;
            btnCargarRuta.ForeColor = Color.White;
            btnCargarRuta.Location = new Point(10, 690);
            btnCargarRuta.Name = "btnCargarRuta";
            btnCargarRuta.Size = new Size(310, 30);
            btnCargarRuta.TabIndex = 7;
            btnCargarRuta.Text = "Load Route";
            btnCargarRuta.UseVisualStyleBackColor = false;
            // 
            // btnGuardarRuta
            // 
            btnGuardarRuta.BackColor = Color.Green;
            btnGuardarRuta.ForeColor = Color.White;
            btnGuardarRuta.Location = new Point(10, 630);
            btnGuardarRuta.Name = "btnGuardarRuta";
            btnGuardarRuta.Size = new Size(310, 30);
            btnGuardarRuta.TabIndex = 6;
            btnGuardarRuta.Text = "Save Route";
            btnGuardarRuta.UseVisualStyleBackColor = false;
            // 
            // btnLimpiarRuta
            // 
            btnLimpiarRuta.BackColor = Color.FromArgb(192, 0, 0);
            btnLimpiarRuta.ForeColor = Color.White;
            btnLimpiarRuta.Location = new Point(10, 600);
            btnLimpiarRuta.Name = "btnLimpiarRuta";
            btnLimpiarRuta.Size = new Size(310, 30);
            btnLimpiarRuta.TabIndex = 5;
            btnLimpiarRuta.Text = "Delete Route";
            btnLimpiarRuta.UseVisualStyleBackColor = false;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.FromArgb(192, 0, 0);
            btnEliminar.FlatAppearance.BorderColor = Color.Black;
            btnEliminar.ForeColor = Color.White;
            btnEliminar.Location = new Point(225, 290);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(100, 30);
            btnEliminar.TabIndex = 4;
            btnEliminar.Text = "Delete";
            btnEliminar.UseVisualStyleBackColor = false;
            // 
            // btnBajar
            // 
            btnBajar.BackColor = Color.FromArgb(255, 128, 0);
            btnBajar.ForeColor = Color.White;
            btnBajar.Location = new Point(115, 290);
            btnBajar.Name = "btnBajar";
            btnBajar.Size = new Size(100, 30);
            btnBajar.TabIndex = 3;
            btnBajar.Text = "▼ Move Down";
            btnBajar.UseVisualStyleBackColor = false;
            // 
            // btnSubir
            // 
            btnSubir.BackColor = Color.FromArgb(255, 128, 0);
            btnSubir.ForeColor = Color.White;
            btnSubir.Location = new Point(5, 290);
            btnSubir.Name = "btnSubir";
            btnSubir.Size = new Size(100, 30);
            btnSubir.TabIndex = 2;
            btnSubir.Text = "▲ Move Up";
            btnSubir.UseVisualStyleBackColor = false;
            // 
            // groupBoxFunciones
            // 
            groupBoxFunciones.Controls.Add(btnAplicar);
            groupBoxFunciones.Controls.Add(lblLongitud);
            groupBoxFunciones.Controls.Add(lblLatitud);
            groupBoxFunciones.Controls.Add(nudHeading);
            groupBoxFunciones.Controls.Add(nudAltitud);
            groupBoxFunciones.Controls.Add(label2);
            groupBoxFunciones.Controls.Add(label1);
            groupBoxFunciones.Controls.Add(cbFuncion);
            groupBoxFunciones.Location = new Point(0, 337);
            groupBoxFunciones.Name = "groupBoxFunciones";
            groupBoxFunciones.Size = new Size(360, 251);
            groupBoxFunciones.TabIndex = 1;
            groupBoxFunciones.TabStop = false;
            groupBoxFunciones.Text = "Waypoint Function";
            // 
            // btnAplicar
            // 
            btnAplicar.BackColor = Color.SteelBlue;
            btnAplicar.ForeColor = SystemColors.Window;
            btnAplicar.Location = new Point(10, 210);
            btnAplicar.Name = "btnAplicar";
            btnAplicar.Size = new Size(290, 30);
            btnAplicar.TabIndex = 7;
            btnAplicar.Text = "Apply Changes";
            btnAplicar.UseVisualStyleBackColor = false;
            // 
            // lblLongitud
            // 
            lblLongitud.AutoSize = true;
            lblLongitud.Location = new Point(10, 185);
            lblLongitud.Name = "lblLongitud";
            lblLongitud.Size = new Size(52, 20);
            lblLongitud.TabIndex = 6;
            lblLongitud.Text = "Lon: --";
            // 
            // lblLatitud
            // 
            lblLatitud.AutoSize = true;
            lblLatitud.Location = new Point(10, 165);
            lblLatitud.Name = "lblLatitud";
            lblLatitud.Size = new Size(48, 20);
            lblLatitud.TabIndex = 5;
            lblLatitud.Text = "Lat: --";
            // 
            // nudHeading
            // 
            nudHeading.Location = new Point(10, 135);
            nudHeading.Maximum = new decimal(new int[] { 359, 0, 0, 0 });
            nudHeading.Name = "nudHeading";
            nudHeading.Size = new Size(290, 27);
            nudHeading.TabIndex = 4;
            // 
            // nudAltitud
            // 
            nudAltitud.Location = new Point(10, 85);
            nudAltitud.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            nudAltitud.Name = "nudAltitud";
            nudAltitud.Size = new Size(290, 27);
            nudAltitud.TabIndex = 3;
            nudAltitud.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 115);
            label2.Name = "label2";
            label2.Size = new Size(89, 20);
            label2.TabIndex = 2;
            label2.Text = "Heading (º):";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 65);
            label1.Name = "label1";
            label1.Size = new Size(92, 20);
            label1.TabIndex = 1;
            label1.Text = "Altitude (m):";
            // 
            // cbFuncion
            // 
            cbFuncion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFuncion.FormattingEnabled = true;
            cbFuncion.Location = new Point(10, 30);
            cbFuncion.Name = "cbFuncion";
            cbFuncion.Size = new Size(290, 28);
            cbFuncion.TabIndex = 0;
            // 
            // waypointListBox
            // 
            waypointListBox.Dock = DockStyle.Top;
            waypointListBox.FormattingEnabled = true;
            waypointListBox.Location = new Point(0, 0);
            waypointListBox.Name = "waypointListBox";
            waypointListBox.Size = new Size(330, 284);
            waypointListBox.TabIndex = 0;
            // 
            // btnActualizarRuta
            // 
            btnActualizarRuta.BackColor = Color.Indigo;
            btnActualizarRuta.ForeColor = Color.White;
            btnActualizarRuta.Location = new Point(10, 660);
            btnActualizarRuta.Name = "btnActualizarRuta";
            btnActualizarRuta.Size = new Size(310, 30);
            btnActualizarRuta.TabIndex = 8;
            btnActualizarRuta.Text = "Update Route";
            btnActualizarRuta.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1382, 753);
            Controls.Add(panelLateral);
            Controls.Add(panelMap);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Route Planer";
            panelLateral.ResumeLayout(false);
            groupBoxFunciones.ResumeLayout(false);
            groupBoxFunciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudHeading).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAltitud).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMap;
        private Panel panelLateral;
        private ListBox waypointListBox;
        private GroupBox groupBoxFunciones;
        private ComboBox cbFuncion;
        private Label label2;
        private Label label1;
        private NumericUpDown nudAltitud;
        private Label lblLatitud;
        private NumericUpDown nudHeading;
        private Button btnAplicar;
        private Label lblLongitud;
        private Button btnSubir;
        private Button btnBajar;
        private Button btnEliminar;
        private Button btnLimpiarRuta;
        private Button btnGuardarRuta;
        private Button btnCargarRuta;
        private Button btnActualizarRuta;
    }
}

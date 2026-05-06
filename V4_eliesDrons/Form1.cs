using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static GMap.NET.Entity.OpenStreetMapGraphHopperRouteEntity;

namespace V4_eliesDrons
{
    public partial class Form1 : Form
    {

        private GMapControl gmap;
        private GMapOverlay waypointsOverlay;
        private GMapOverlay routeOverlay;

        private List<Instruccion> instrucciones = new List<Instruccion>();
        private int instruccionCounter = 0;
        private Instruccion instruccionSeleccionada = null;

        private DroneAPIService droneAPIService;
        private string vueloActualCargado = null;

        

        
        public Form1()
        {
            InitializeComponent();
            InicializarMapa();

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                droneAPIService = new DroneAPIService("http://dronseetac.upc.edu:8104/api");
                MessageBox.Show("Conectado a Drone API ✓", "Conexión Exitosa");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error conectando a API: {ex.Message}", "Error");
                droneAPIService = null;
            }
        }


        //===== COSAS DEL MAPA =====
        private void InicializarMapa()
        {
            // Crear el control del mapa
            gmap = new GMapControl();
            gmap.Dock = DockStyle.Fill;
            panelMap.Controls.Add(gmap);

            InicializarControlesFunciones();

            // Configuración básica
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gmap.MapProvider = GMapProviders.GoogleSatelliteMap;

            // Posición inicial (tu ubicación: 41.27, 1.98)
            gmap.Position = new PointLatLng(41.27641, 1.98862);

            // Configuración de zoom
            gmap.MinZoom = 2;
            gmap.MaxZoom = 25;
            gmap.Zoom = 19;

            // Opciones visuales
            gmap.ShowTileGridLines = false;
            gmap.ShowCenter = false;
            gmap.IgnoreMarkerOnMouseWheel = true;
            gmap.CanDragMap = true;
            gmap.RetryLoadTile = 3;

            // Crear overlay para la ruta (NUEVO)
            routeOverlay = new GMapOverlay("route");
            gmap.Overlays.Add(routeOverlay);  // Agrégalo ANTES del overlay de waypoints
                                              // para que la ruta quede debajo

            waypointsOverlay = new GMapOverlay("waypoints");
            gmap.Overlays.Add(waypointsOverlay);

            gmap.MouseClick += Gmap_MouseClick;
        }

        private void Gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var posScreen = new Point(e.X, e.Y);
                var posGMap = gmap.FromLocalToLatLng(posScreen.X, posScreen.Y);

                int puntoId = (int)(DateTime.Now.Ticks % int.MaxValue);
                Punto punto = new Punto(puntoId, posGMap.Lat, posGMap.Lng);

                int instruccionId = (int)(DateTime.Now.Ticks % int.MaxValue);
                Instruccion instr = new Instruccion(instruccionId, punto);
                instrucciones.Add(instr);
                    
                AgregarMarcador(instr);
                ActualizarListaInstrucciones();
                ActualizarRuta();

            }
        }

        private void AgregarMarcador(Instruccion instr)
        {
            var posGMap = new PointLatLng(instr.Punto.Lat, instr.Punto.Long);

            var marker = new GMarkerGoogle(posGMap, GMarkerGoogleType.green)
            {
                ToolTipText = $"I{instr.VisualId}",  // Solo el ID
                Tag = instr.Id
            };
            marker.ToolTipMode = MarkerTooltipMode.Always;  // Siempre visible

            waypointsOverlay.Markers.Add(marker);
            gmap.Refresh();
        }


        private void ActualizarListaInstrucciones()
        {
            ListBox listBox = this.Controls.Find("waypointListBox", true).FirstOrDefault() as ListBox;

            if (listBox != null)
            {
                listBox.DataSource = null;
                listBox.DataSource = new List<Instruccion>(instrucciones);
                listBox.DisplayMember = "ToString";  // Vuelve a usar ToString()
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox.SelectedIndex >= 0)
            {
                instruccionSeleccionada = instrucciones[listBox.SelectedIndex];
                CargarDatosInstruccion(instruccionSeleccionada);
            }
        }



        private void InicializarControlesFunciones()
        {
            // Llenar ComboBox con las funciones disponibles
            ComboBox cbFuncion = this.Controls.Find("cbFuncion", true).FirstOrDefault() as ComboBox;
            if (cbFuncion != null)
            {
                cbFuncion.DataSource = Enum.GetValues(typeof(Directriz)).Cast<Directriz>().ToList();
            }

            // Evento cuando selecciona un waypoint en la lista
            ListBox listBox = this.Controls.Find("waypointListBox", true).FirstOrDefault() as ListBox;
            if (listBox != null)
            {
                listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            }

            // Evento del botón Aplicar
            Button btnAplicar = this.Controls.Find("btnAplicar", true).FirstOrDefault() as Button;
            if (btnAplicar != null)
            {
                btnAplicar.Click += BtnAplicar_Click;
            }

            Button btnSubir = this.Controls.Find("btnSubir", true).FirstOrDefault() as Button;
            if (btnSubir != null)
            {
                btnSubir.Click += BtnSubir_Click;
            }

            Button btnBajar = this.Controls.Find("btnBajar", true).FirstOrDefault() as Button;
            if (btnBajar != null)
            {
                btnBajar.Click += BtnBajar_Click;
            }

            Button btnEliminar = this.Controls.Find("btnEliminar", true).FirstOrDefault() as Button;
            if (btnEliminar != null)
            {
                btnEliminar.Click += BtnEliminar_Click;
            }

            Button btnLimpiarRuta = this.Controls.Find("btnLimpiarRuta", true).FirstOrDefault() as Button;
            if (btnLimpiarRuta != null)
            {
                btnLimpiarRuta.Click += BtnLimpiarRuta_Click;
            }

            // Evento del botón Guardar Ruta
            Button btnGuardarRuta = this.Controls.Find("btnGuardarRuta", true).FirstOrDefault() as Button;
            if (btnGuardarRuta != null)
            {
                btnGuardarRuta.Click += BtnGuardarRuta_Click;
            }
            // Evento del botón Cargar Ruta
            Button btnCargarRuta = this.Controls.Find("btnCargarRuta", true).FirstOrDefault() as Button;
            if (btnCargarRuta != null)
            {
                btnCargarRuta.Click += BtnCargarRuta_Click;
            }

            // Evento del botón Actualizar Ruta
            Button btnActualizarRuta = this.Controls.Find("btnActualizarRuta", true).FirstOrDefault() as Button;
            if (btnActualizarRuta != null)
            {
                btnActualizarRuta.Click += BtnActualizarRuta_Click;
            }
        }



        private void CargarDatosInstruccion(Instruccion instr)
        {
            ComboBox cbFuncion = this.Controls.Find("cbFuncion", true).FirstOrDefault() as ComboBox;
            if (cbFuncion != null)
            {
                if (Enum.TryParse<Directriz>(instr.Directriz, out Directriz dir))
                    cbFuncion.SelectedItem = dir;
            }

            NumericUpDown nudAltitud = this.Controls.Find("nudAltitud", true).FirstOrDefault() as NumericUpDown;
            if (nudAltitud != null)
            {
                nudAltitud.Value = (decimal)instr.Punto.Altitud;
            }

            NumericUpDown nudHeading = this.Controls.Find("nudHeading", true).FirstOrDefault() as NumericUpDown;
            if (nudHeading != null)
            {
                nudHeading.Value = (decimal)instr.Punto.Heading;
            }

            Label lblLatitud = this.Controls.Find("lblLatitud", true).FirstOrDefault() as Label;
            if (lblLatitud != null)
            {
                lblLatitud.Text = $"Lat: {instr.Punto.Lat}";
            }

            Label lblLongitud = this.Controls.Find("lblLongitud", true).FirstOrDefault() as Label;
            if (lblLongitud != null)
            {
                lblLongitud.Text = $"Lon: {instr.Punto.Long}";
            }
        }

        private void BtnAplicar_Click(object sender, EventArgs e)
        {
            if (instruccionSeleccionada == null)
            {
                MessageBox.Show("Selecciona una instrucción primero", "Error");
                return;
            }

            string idSeleccionado = instruccionSeleccionada.Id;  // ← CAMBIAR: string en lugar de int

            ComboBox cbFuncion = this.Controls.Find("cbFuncion", true).FirstOrDefault() as ComboBox;
            NumericUpDown nudAltitud = this.Controls.Find("nudAltitud", true).FirstOrDefault() as NumericUpDown;
            NumericUpDown nudHeading = this.Controls.Find("nudHeading", true).FirstOrDefault() as NumericUpDown;

            if (cbFuncion != null && cbFuncion.SelectedItem != null)
            {
                instruccionSeleccionada.Directriz = cbFuncion.SelectedItem.ToString();
            }

            if (nudAltitud != null)
            {
                instruccionSeleccionada.Punto.Altitud = (float)nudAltitud.Value;
            }

            if (nudHeading != null)
            {
                instruccionSeleccionada.Punto.Heading = (float)nudHeading.Value;
            }

            ActualizarListaInstrucciones();

            ListBox listBox = this.Controls.Find("waypointListBox", true).FirstOrDefault() as ListBox;
            if (listBox != null)
            {
                int nuevoIndice = instrucciones.FindIndex(i => i.Id == idSeleccionado);  // ← Ahora compara string con string
                if (nuevoIndice >= 0)
                {
                    listBox.SelectedIndex = nuevoIndice;
                }
            }

            MessageBox.Show("Cambios aplicados correctamente", "Éxito");
        }

        private void ActualizarRuta()
        {
            routeOverlay.Routes.Clear();

            if (instrucciones.Count >= 2)
            {
                var puntos = instrucciones.Select(i =>
                    new PointLatLng(i.Punto.Lat, i.Punto.Long)  // Ya son floats
                ).ToList();

                GMapRoute route = new GMapRoute(puntos, "ruta");
                route.Stroke = new System.Drawing.Pen(System.Drawing.Color.Blue, 2);
                route.Stroke.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                routeOverlay.Routes.Add(route);
            }

            gmap.Refresh();
        }

        private void BtnSubir_Click(object sender, EventArgs e)
        {
            if (instruccionSeleccionada == null)
            {
                MessageBox.Show("Selecciona una instrucción primero", "Error");
                return;
            }

            int indice = instrucciones.IndexOf(instruccionSeleccionada);

            if (indice > 0)
            {
                var temp = instrucciones[indice];
                instrucciones[indice] = instrucciones[indice - 1];
                instrucciones[indice - 1] = temp;

                ActualizarListaInstrucciones();
                ActualizarRuta();

                ListBox listBox = this.Controls.Find("waypointListBox", true).FirstOrDefault() as ListBox;
                if (listBox != null)
                {
                    listBox.SelectedIndex = indice - 1;
                }
            }
            else
            {
                MessageBox.Show("Esta instrucción ya está al inicio", "Aviso");
            }
        }


        private void BtnBajar_Click(object sender, EventArgs e)
        {
            if (instruccionSeleccionada == null)
            {
                MessageBox.Show("Selecciona una instrucción primero", "Error");
                return;
            }

            int indice = instrucciones.IndexOf(instruccionSeleccionada);

            if (indice < instrucciones.Count - 1)
            {
                var temp = instrucciones[indice];
                instrucciones[indice] = instrucciones[indice + 1];
                instrucciones[indice + 1] = temp;

                ActualizarListaInstrucciones();
                ActualizarRuta();

                ListBox listBox = this.Controls.Find("waypointListBox", true).FirstOrDefault() as ListBox;
                if (listBox != null)
                {
                    listBox.SelectedIndex = indice + 1;
                }
            }
            else
            {
                MessageBox.Show("Esta instrucción ya está al final", "Aviso");
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (instruccionSeleccionada == null)
            {
                MessageBox.Show("Selecciona una instrucción para eliminar", "Error");
                return;
            }

            DialogResult resultado = MessageBox.Show(
                $"¿Estás seguro de que quieres eliminar I{instruccionSeleccionada.VisualId}?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado == DialogResult.Yes)
            {
                instrucciones.Remove(instruccionSeleccionada);
                instruccionSeleccionada = null;

                ActualizarListaInstrucciones();
                ActualizarRuta();
                RefrescarMarcadores();
                // ← NO reinicia contador

                MessageBox.Show("Instrucción eliminada correctamente", "Éxito");
            }
        }
        private void RefrescarMarcadores()
        {
            waypointsOverlay.Markers.Clear();

            foreach (var instr in instrucciones)
            {
                AgregarMarcador(instr);
            }

            gmap.Refresh();
        }

        private void BtnLimpiarRuta_Click(object sender, EventArgs e)
        {
            if (instrucciones.Count == 0)
            {
                MessageBox.Show("No hay instrucciones para limpiar", "Aviso");
                return;
            }

            DialogResult resultado = MessageBox.Show(
                $"¿Estás seguro de que quieres eliminar todas las instrucciones ({instrucciones.Count})?\n\nEsta acción no se puede deshacer.",
                "Limpiar ruta completa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado == DialogResult.Yes)
            {
                instrucciones.Clear();
                instruccionSeleccionada = null;
                instruccionCounter = 0;
                vueloActualCargado = null;  // ← AGREGA ESTA LÍNEA

                Instruccion.ReiniciarContador();

                ActualizarListaInstrucciones();
                ActualizarRuta();
                RefrescarMarcadores();
                LimpiarControlesEdicion();

                MessageBox.Show("Ruta limpiada completamente", "Éxito");
            }
        }

        private async void BtnGuardarRuta_Click(object sender, EventArgs e)
        {
            if (instrucciones.Count == 0)
            {
                MessageBox.Show("No hay instrucciones para guardar", "Aviso");
                return;
            }

            if (droneAPIService == null)
            {
                MessageBox.Show("No hay conexión a la API", "Error");
                return;
            }

            // ← NUEVO: Pedir el nombre del vuelo
            NombVueloForm formNombre = new NombVueloForm();
            DialogResult resultadoNombre = formNombre.ShowDialog();

            if (resultadoNombre != DialogResult.OK)
            {
                return;  // Si cancela, no hacer nada
            }

            string nametag = formNombre.ObtenerNombre();

            if (string.IsNullOrWhiteSpace(nametag))
            {
                MessageBox.Show("El nombre del vuelo no puede estar vacío", "Error");
                return;
            }

            // Mostrar preview
            Vuelo vueloTemporal = new Vuelo();
            vueloTemporal.NameTag = nametag;
            vueloTemporal.Instrucciones = new List<Instruccion>(instrucciones);

            string previewGuardado = GenerarPreviewGuardado(vueloTemporal);

            PreviewForm previewForm = new PreviewForm(previewGuardado);
            DialogResult resultado = previewForm.ShowDialog();

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    Vuelo vuelo = new Vuelo();
                    vuelo.NameTag = nametag;
                    vuelo.Instrucciones = new List<Instruccion>(instrucciones);

                    await droneAPIService.GuardarRutaAsync(vuelo);
                    vueloActualCargado = vuelo.ID;

                    MessageBox.Show(
                        $"Ruta guardada en la API\nNombre: {vuelo.NameTag}\nID: {vuelo.ID}",
                        "Éxito"
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error al guardar");
                }
            }
        }

        private string GenerarPreviewGuardado(Vuelo vuelo)
        {
            string preview = "";
            preview += "═══════════════════════════════════════════════════════════\n";
            preview += "        PREVISUALIZACIÓN DE RUTA A GUARDAR\n";
            preview += "═══════════════════════════════════════════════════════════\n\n";

            preview += $"ID de Vuelo: {vuelo.ID}\n";
            preview += $"Fecha: {vuelo.Fecha:yyyy-MM-dd HH:mm:ss}\n";
            preview += $"Total de Instrucciones: {vuelo.Instrucciones.Count}\n";
            preview += "\n";
            preview += "═══════════════════════════════════════════════════════════\n\n";

            for (int i = 0; i < vuelo.Instrucciones.Count; i++)
            {
                var instr = vuelo.Instrucciones[i];

                preview += $"INSTRUCCIÓN I{instr.VisualId}\n";
                preview += "───────────────────────────────────────────────────────\n";
                preview += $"  Ubicación:\n";
                preview += $"    Latitud:   {instr.Punto.Lat,15:F6}\n";
                preview += $"    Longitud:  {instr.Punto.Long,15:F6}\n";
                preview += $"\n";
                preview += $"  Parámetros:\n";
                preview += $"    Altitud:   {instr.Punto.Altitud,15:F2} m\n";
                preview += $"    Heading:   {instr.Punto.Heading,15:F2}°\n";
                preview += $"\n";
                preview += $"  Función: {instr.Directriz}\n";
                preview += "\n";
            }

            preview += "═══════════════════════════════════════════════════════════\n";

            return preview;
        }

        // TODO: Descomentar cuando tu compañero termine la BD
        /*
        private void GuardarRutaEnBD(Vuelo vuelo)
        {
            try
            {
                // Aquí tu compañero agregará el código para guardar en MongoDB
                // Ejemplo:
                // MongoDBService.GuardarVuelo(vuelo);

                MessageBox.Show("Ruta guardada correctamente en la base de datos", "Éxito");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error");
            }
        }
        */

        private void LimpiarControlesEdicion()
        {
            ComboBox cbFuncion = this.Controls.Find("cbFuncion", true).FirstOrDefault() as ComboBox;
            if (cbFuncion != null)
            {
                cbFuncion.SelectedIndex = 0;
            }

            NumericUpDown nudAltitud = this.Controls.Find("nudAltitud", true).FirstOrDefault() as NumericUpDown;
            if (nudAltitud != null)
            {
                nudAltitud.Value = 5;
            }

            NumericUpDown nudHeading = this.Controls.Find("nudHeading", true).FirstOrDefault() as NumericUpDown;
            if (nudHeading != null)
            {
                nudHeading.Value = 0;
            }

            Label lblLatitud = this.Controls.Find("lblLatitud", true).FirstOrDefault() as Label;
            if (lblLatitud != null)
            {
                lblLatitud.Text = "Lat: --";
            }

            Label lblLongitud = this.Controls.Find("lblLongitud", true).FirstOrDefault() as Label;
            if (lblLongitud != null)
            {
                lblLongitud.Text = "Lon: --";
            }
        }
        private async void BtnCargarRuta_Click(object sender, EventArgs e)
        {
            if (droneAPIService == null)
            {
                MessageBox.Show("No hay conexión a la API", "Error");
                return;
            }

            try
            {
                var vuelos = await droneAPIService.ObtenerTodosVuelosAsync();

                if (vuelos.Count == 0)
                {
                    MessageBox.Show("No hay rutas guardadas", "Aviso");
                    return;
                }

                var vueloSeleccionado = SeleccionarVuelo(vuelos);
                if (vueloSeleccionado == null) return;

                Instruccion.ReiniciarContador();
                var vueloCompleto = await droneAPIService.CargarRutaAsync(vueloSeleccionado.ID);

                instrucciones.Clear();
                instruccionSeleccionada = null;
                instruccionCounter = 0;

                instrucciones = new List<Instruccion>(vueloCompleto.Instrucciones);
                vueloActualCargado = vueloSeleccionado.ID;

                ActualizarListaInstrucciones();
                ActualizarRuta();
                RefrescarMarcadores();

                MessageBox.Show(
                    $"Ruta cargada correctamente\n{vueloCompleto.Instrucciones.Count} instrucciones importadas",
                    "Éxito"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error al cargar");
            }
        }

        private Vuelo SeleccionarVuelo(List<Vuelo> vuelos)
        {
            using (Form form = new Form())
            {
                form.Text = "Seleccionar Ruta";
                form.Width = 500;
                form.Height = 350;
                form.StartPosition = FormStartPosition.CenterParent;

                ListBox listBox = new ListBox
                {
                    Dock = DockStyle.Fill,
                    DataSource = vuelos,
                    DisplayMember = "NameTag"
                };

                Button btnOK = new Button
                {
                    Text = "Aceptar",
                    Dock = DockStyle.Bottom,
                    Height = 30,
                    BackColor = System.Drawing.Color.Green,
                    ForeColor = System.Drawing.Color.White
                };

                Button btnCancel = new Button
                {
                    Text = "Cancelar",
                    Dock = DockStyle.Bottom,
                    Height = 30,
                    BackColor = System.Drawing.Color.Red,
                    ForeColor = System.Drawing.Color.White
                };

                btnOK.Click += (s, e) =>
                {
                    var vueloSeleccionado = (Vuelo)listBox.SelectedItem;
                    // ← MOSTRAR EL ID SELECCIONADO
                    MessageBox.Show($"Vuelo seleccionado:\nNombre: {vueloSeleccionado.NameTag}\nID: {vueloSeleccionado.ID}", "Debug");
                    form.DialogResult = DialogResult.OK;
                };

                btnCancel.Click += (s, e) => form.DialogResult = DialogResult.Cancel;

                form.Controls.Add(listBox);
                form.Controls.Add(btnOK);
                form.Controls.Add(btnCancel);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    return (Vuelo)listBox.SelectedItem;
                }
                return null;
            }
        }
        private async void BtnActualizarRuta_Click(object sender, EventArgs e)
        {
            if (instrucciones.Count == 0)
            {
                MessageBox.Show("No hay instrucciones para guardar", "Aviso");
                return;
            }

            if (vueloActualCargado == null)
            {
                MessageBox.Show("No hay una ruta cargada. Use 'Guardar Ruta' primero.", "Aviso");
                return;
            }

            if (droneAPIService == null)
            {
                MessageBox.Show("No hay conexión a la API", "Error");
                return;
            }

            DialogResult resultado = MessageBox.Show(
                $"¿Deseas actualizar la ruta?\nID: {vueloActualCargado}",
                "Actualizar Ruta",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    Vuelo vuelo = new Vuelo();
                    vuelo.ID = vueloActualCargado;
                    vuelo.Instrucciones = new List<Instruccion>(instrucciones);

                    await droneAPIService.ActualizarRutaAsync(vueloActualCargado, vuelo);

                    MessageBox.Show("Ruta actualizada correctamente", "Éxito");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error al actualizar");
                }
            }
        }

    }

}

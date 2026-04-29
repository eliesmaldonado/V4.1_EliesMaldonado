using GMap.NET;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace V4_eliesDrons
{
    public enum Directriz
    {
        None,
        TakePhoto,           // Hacer foto
        StartVideoRecording,  // Comenzar grabación de vídeo
        StopVideoRecording,   // Detener grabación de vídeo
        ChangeAltitude,       // Cambiar altitud
        ChangeHeading,        // Cambiar heading
        Hover,                // Mantener posición
        ReturnToHome,         // Volver a casa
        Wait
    }

    public class Punto
    {
        public int Id { get; set; }
        public float Lat { get; set; }          // ← float
        public float Long { get; set; }         // ← float
        public float Heading { get; set; }
        public float Altitud { get; set; }      // ← float

        public Punto()
        {
            Id = 0;
            Lat = 0;
            Long = 0;
            Heading = 0;
            Altitud = 50;
        }

        public Punto(int id, double lat, double lon)
        {
            Id = id;
            Lat = (float)lat;
            Long = (float)lon;
            Heading = 0;
            Altitud = 5;
        }
    }

    public class Instruccion
    {
        public int Id { get; set; }
        public int VisualId { get; set; }  // ← AGREGAR ESTO (nunca cambia)
        public Punto Punto { get; set; }
        public string Directriz { get; set; }

        private static int contador = 0;

        // Método para reiniciar el contador
        public static void ReiniciarContador()
        {
            contador = 0;
        }
        public Instruccion(int id, Punto punto)
        {
            Id = id;
            Punto = punto;
            Directriz = "None";
            VisualId = ++contador;  // Asignar un ID visual único (I1, I2, I3...)
        }

        public override string ToString()
        {
            return $"I{VisualId} - Lat: {Punto.Lat}, Lon: {Punto.Long} - {Directriz}";
        }
    }

    public class Vuelo
    {
        public string ID { get; set; }
        public List<Instruccion> Instrucciones { get; set; }
        public string Video { get; set; }
        public List<string> Fotos { get; set; }
        public DateTime Fecha { get; set; }

        public Vuelo()
        {
            ID = Guid.NewGuid().ToString();
            Instrucciones = new List<Instruccion>();
            Video = "";
            Fotos = new List<string>();
            Fecha = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Vuelo: {ID} - {Instrucciones.Count} instrucciones - {Fecha:yyyy-MM-dd HH:mm:ss}";
        }
    }

    public class VueloService
    {
        public static void GuardarRuta(Vuelo vuelo, string rutaArchivo)
        {
            try
            {
                var opciones = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true  // ← AGREGAR ESTO
                };

                string json = JsonSerializer.Serialize(vuelo, opciones);
                File.WriteAllText(rutaArchivo, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar: {ex.Message}");
            }
        }

        public static Vuelo CargarRuta(string rutaArchivo)
        {
            try
            {
                if (!File.Exists(rutaArchivo))
                    throw new FileNotFoundException($"Archivo no encontrado: {rutaArchivo}");

                var opciones = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // ← AGREGAR ESTO
                };

                string json = File.ReadAllText(rutaArchivo);
                Vuelo vuelo = JsonSerializer.Deserialize<Vuelo>(json, opciones);

                return vuelo ?? throw new Exception("No se pudo deserializar el archivo");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar: {ex.Message}");
            }
        }
    }
}


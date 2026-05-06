using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace V4_eliesDrons
{
    public enum Directriz
    {
        None,
        TakePhoto,
        StartVideoRecording,
        StopVideoRecording,
        ChangeAltitude,
        ChangeHeading,
        Hover,
        ReturnToHome,
        Wait
    }

    public class Punto
    {
        [JsonPropertyName("_id")]  // ← AGREGAR ESTO
        public string Id { get; set; }

        [JsonPropertyName("Latitud")]
        public float Lat { get; set; }

        [JsonPropertyName("Longitud")]
        public float Long { get; set; }

        [JsonPropertyName("Heading")]
        public float Heading { get; set; }

        [JsonPropertyName("Altitud")]
        public float Altitud { get; set; }

        public Punto()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Lat = 0;
            Long = 0;
            Heading = 0;
            Altitud = 5;
        }

        public Punto(int id, double lat, double lon)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Lat = (float)lat;
            Long = (float)lon;
            Heading = 0;
            Altitud = 5;
        }
    }

    public class Instruccion
    {
        [JsonPropertyName("_id")]  // ← AGREGAR ESTO
        public string Id { get; set; }

        [JsonPropertyName("ID_Vuelo")]
        public string ID_Vuelo { get; set; }

        public int VisualId { get; set; }

        [JsonPropertyName("Punto")]
        public Punto Punto { get; set; }

        [JsonPropertyName("directriz")]
        public string Directriz { get; set; }

        [JsonPropertyName("trail")]
        public int Trail { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("datetime")]
        public DateTime DateTime { get; set; }

        private static int contador = 0;

        public static void ReiniciarContador()
        {
            contador = 0;
        }

        public Instruccion()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Directriz = "None";
            Version = 1;
            DateTime = DateTime.Now;
            VisualId = ++contador;
        }

        public Instruccion(int id, Punto punto)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Punto = punto;
            Directriz = "None";
            Version = 1;
            DateTime = DateTime.Now;
            VisualId = ++contador;
        }

        public override string ToString()
        {
            return $"I{VisualId} - Lat: {Punto.Lat}, Lon: {Punto.Long} - {Directriz}";
        }
    }

    public class Vuelo
    {
        [JsonPropertyName("_id")]  // ← AGREGAR ESTO
        public string ID { get; set; }

        [JsonPropertyName("nametag")]
        public string NameTag { get; set; }

        [JsonPropertyName("numVersiones")]
        public int NumVersiones { get; set; }

        [JsonPropertyName("datetime")]
        public DateTime Fecha { get; set; }

        public List<Instruccion> Instrucciones { get; set; }
        public string Video { get; set; }
        public List<string> Fotos { get; set; }

        public Vuelo()
        {
            ID = ObjectId.GenerateNewId().ToString();
            NameTag = $"Ruta_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            NumVersiones = 1;
            Fecha = DateTime.Now;
            Instrucciones = new List<Instruccion>();
            Video = "";
            Fotos = new List<string>();
        }

        public override string ToString()
        {
            return $"{NameTag} - {Instrucciones.Count} instrucciones - {Fecha:yyyy-MM-dd HH:mm:ss}";
        }
    }

    public class DroneAPIService
    {
        private HttpClient _httpClient;
        private string _apiBaseUrl = "http://dronseetac.upc.edu:8104/api";

        public DroneAPIService(string apiBaseUrl = "http://dronseetac.upc.edu:8104/api")
        {
            _apiBaseUrl = apiBaseUrl;
            _httpClient = new HttpClient();
        }

        // ============ VUELOS ============

        public async Task<string> CrearVueloAsync(string nameTag)
        {
            try
            {
                var payload = new { nametag = nameTag };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/vuelo", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                {
                    if (doc.RootElement.TryGetProperty("_id", out var idProp))
                        return idProp.GetString();
                    else if (doc.RootElement.TryGetProperty("id", out var idProp2))
                        return idProp2.GetString();
                    else
                        throw new Exception($"No se encontró '_id' ni 'id' en la respuesta: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creando vuelo: {ex.Message}");
            }
        }

        public async Task<Vuelo> ObtenerVueloAsync(string vueloId)
        {
            try
            {
                if (string.IsNullOrEmpty(vueloId))
                    throw new Exception("El ID del vuelo no puede estar vacío");

                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/vuelo/{vueloId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new Exception($"Vuelo no encontrado. ID: {vueloId}");

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var vuelo = JsonSerializer.Deserialize<Vuelo>(responseContent, options);

                if (string.IsNullOrEmpty(vuelo.ID))
                    vuelo.ID = vueloId;

                return vuelo;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo vuelo: {ex.Message}");
            }
        }

        public async Task<List<Vuelo>> ObtenerTodosVuelosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/vuelo?page=1&limit=100");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var vuelos = new List<Vuelo>();

                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                {
                    JsonElement elementoParaProcesar = doc.RootElement;

                    if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                        doc.RootElement.TryGetProperty("data", out var dataProp))
                    {
                        elementoParaProcesar = dataProp;
                    }

                    foreach (var item in elementoParaProcesar.EnumerateArray())
                    {
                        var vuelo = JsonSerializer.Deserialize<Vuelo>(item.GetRawText(), options);

                        if (string.IsNullOrEmpty(vuelo.ID))
                        {
                            if (item.TryGetProperty("_id", out var idProp))
                                vuelo.ID = idProp.GetString();
                            else if (item.TryGetProperty("id", out var idProp2))
                                vuelo.ID = idProp2.GetString();
                        }

                        vuelos.Add(vuelo);
                    }
                }

                return vuelos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo vuelos: {ex.Message}");
            }
        }

        // ============ INSTRUCCIONES ============

        // BUG CORREGIDO: GET /instruccion/vuelo devuelve UNA sola instrucción.
        // Para obtener TODAS las de un vuelo, se usa GET /instruccion (paginado)
        // y se filtra por ID_Vuelo en el cliente.
        public async Task<List<Instruccion>> ObtenerInstruccionesVueloAsync(string idVuelo)
        {
            try
            {
                var todasInstrucciones = new List<Instruccion>();
                int page = 1;
                int limit = 100;
                bool hayMas = true;

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                while (hayMas)
                {
                    var response = await _httpClient.GetAsync(
                        $"{_apiBaseUrl}/instruccion?page={page}&limit={limit}");
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var instruccionesPagina = new List<Instruccion>();

                    using (JsonDocument doc = JsonDocument.Parse(responseContent))
                    {
                        JsonElement arrayElement = doc.RootElement;

                        // Algunos endpoints devuelven { data: [...] }
                        if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                            doc.RootElement.TryGetProperty("data", out var dataProp))
                        {
                            arrayElement = dataProp;
                        }

                        if (arrayElement.ValueKind != JsonValueKind.Array)
                            break;

                        foreach (var item in arrayElement.EnumerateArray())
                        {
                            var instr = JsonSerializer.Deserialize<Instruccion>(item.GetRawText(), options);
                            if (instr != null)
                                instruccionesPagina.Add(instr);
                        }
                    }

                    // Filtrar las que pertenecen a este vuelo
                    var delVuelo = instruccionesPagina
                        .Where(i => i.ID_Vuelo == idVuelo)
                        .ToList();

                    todasInstrucciones.AddRange(delVuelo);

                    // Si la página devolvió menos de 'limit', ya no hay más páginas
                    hayMas = instruccionesPagina.Count == limit;
                    page++;
                }

                // Ordenar por trail para recuperar el orden original
                return todasInstrucciones
                    .OrderBy(i => i.Trail)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo instrucciones: {ex.Message}");
            }
        }

        public async Task<bool> EliminarInstruccionAsync(string instruccionId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/instruccion/{instruccionId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error eliminando instrucción: {ex.Message}");
            }
        }

        // ============ GUARDAR/CARGAR RUTA ============

        // BUG CORREGIDO: Usar POST /instrucciones (bulk) en lugar de llamadas individuales
        public async Task GuardarRutaAsync(Vuelo vuelo)
        {
            try
            {
                string vueloId = await CrearVueloAsync(vuelo.NameTag);
                vuelo.ID = vueloId;

                if (vuelo.Instrucciones.Count == 0)
                    return;

                // Construir el array para POST /instrucciones
                // trail es OBLIGATORIO en bulk y debe empezar en 1, ser secuencial
                var payload = vuelo.Instrucciones.Select((instr, index) => new
                {
                    ID_Vuelo = vueloId,
                    trail = index + 1,
                    Punto = new
                    {
                        Latitud = instr.Punto.Lat,
                        Longitud = instr.Punto.Long,
                        Altitud = instr.Punto.Altitud,
                        Heading = instr.Punto.Heading
                    },
                    directriz = instr.Directriz
                }).ToList();

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/instrucciones", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error HTTP {response.StatusCode} al guardar instrucciones: {responseContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error guardando ruta: {ex.Message}");
            }
        }

        public async Task<Vuelo> CargarRutaAsync(string vueloId)
        {
            try
            {
                var vuelo = await ObtenerVueloAsync(vueloId);
                vuelo.Instrucciones = await ObtenerInstruccionesVueloAsync(vueloId);
                return vuelo;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error cargando ruta: {ex.Message}");
            }
        }

        // MEJORADO: Usar PUT /instrucciones (bulk update) en lugar de delete+recreate
        public async Task ActualizarRutaAsync(string vueloId, Vuelo vuelo)
        {
            try
            {
                // Obtener las instrucciones existentes para tener sus _id
                var instruccionesExistentes = await ObtenerInstruccionesVueloAsync(vueloId);

                if (instruccionesExistentes.Count > 0)
                {
                    // Si el número de instrucciones coincide, usar PUT /instrucciones (bulk update)
                    // que crea nuevas versiones sin borrar (inmutabilidad por versiones)
                    if (instruccionesExistentes.Count == vuelo.Instrucciones.Count)
                    {
                        var payload = instruccionesExistentes.Select((instrExistente, index) => new
                        {
                            _id = instrExistente.Id,
                            Punto = new
                            {
                                Latitud = vuelo.Instrucciones[index].Punto.Lat,
                                Longitud = vuelo.Instrucciones[index].Punto.Long,
                                Altitud = vuelo.Instrucciones[index].Punto.Altitud,
                                Heading = vuelo.Instrucciones[index].Punto.Heading
                            },
                            directriz = vuelo.Instrucciones[index].Directriz
                        }).ToList();

                        var json = JsonSerializer.Serialize(payload);
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        var response = await _httpClient.PutAsync($"{_apiBaseUrl}/instrucciones", content);

                        if (!response.IsSuccessStatusCode)
                        {
                            var err = await response.Content.ReadAsStringAsync();
                            throw new Exception($"Error HTTP {response.StatusCode}: {err}");
                        }
                    }
                    else
                    {
                        // Si cambió el número de instrucciones: eliminar todas y recrear
                        foreach (var instr in instruccionesExistentes)
                            await EliminarInstruccionAsync(instr.Id);

                        // Recrear con el nuevo conjunto
                        var payload = vuelo.Instrucciones.Select((instr, index) => new
                        {
                            ID_Vuelo = vueloId,
                            trail = index + 1,
                            Punto = new
                            {
                                Latitud = instr.Punto.Lat,
                                Longitud = instr.Punto.Long,
                                Altitud = instr.Punto.Altitud,
                                Heading = instr.Punto.Heading
                            },
                            directriz = instr.Directriz
                        }).ToList();

                        var json = JsonSerializer.Serialize(payload);
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync($"{_apiBaseUrl}/instrucciones", content);

                        if (!response.IsSuccessStatusCode)
                        {
                            var err = await response.Content.ReadAsStringAsync();
                            throw new Exception($"Error HTTP {response.StatusCode}: {err}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error actualizando ruta: {ex.Message}");
            }
        }
    }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ejercicio3
{
    public class Vivero
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("direccion")]
        public string Direccion { get; set; }

        [JsonPropertyName("plantas")]
        public List<Planta> Plantas { get; set; }
    }
}

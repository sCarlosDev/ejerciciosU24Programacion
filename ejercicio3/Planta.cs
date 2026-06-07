using System.Text.Json.Serialization;

namespace ejercicio3
{
    public class Planta
    {
        public int Id { get; set; }

        [JsonPropertyName("nombre_comun")]
        public string NombreComun { get; set; }

        [JsonPropertyName("nombre_cientifico")]
        public string NombreCientifico { get; set; }

        [JsonPropertyName("precio")]
        public double Precio { get; set; }

        [JsonPropertyName("stock")]
        public int Stock { get; set; }
    }
}

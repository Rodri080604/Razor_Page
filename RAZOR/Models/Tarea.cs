using System.Text.Json.Serialization;

namespace RAZOR.Models
{
    public class Tarea
    {
        public int idTarea { get; set; }
        public string nombreTarea { get; set; }

        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateTime fechaVencimiento { get; set; }

        public string estado { get; set; }
    }
}

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RAZOR.Models
{
    public class DateOnlyJsonConverter : JsonConverter<DateTime>
    {
        private readonly string formato = "dd/MM/yyyy"; 

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (DateTime.TryParseExact(value, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
            {
                return fecha;
            }
            return DateTime.Parse(value); 
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(formato));
        }
    }
}

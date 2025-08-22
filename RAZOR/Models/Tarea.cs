using System.ComponentModel.DataAnnotations;

namespace RAZOR.Models
{
    public class Tarea
    {
        public string idTarea { get; set; }

        [Required(ErrorMessage = "El nombre de la tarea es requerido")]
        public string nombreTarea { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es requerida")]
        public string fechaVencimiento { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public string estado { get; set; } // "Pendiente", "En curso", "Finalizado"
    }
}

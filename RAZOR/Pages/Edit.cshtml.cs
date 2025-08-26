using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RAZOR.Models;
using System.Text.Json;

namespace RAZOR.Pages
{
    public class EdithModel : PageModel
    {
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

        [BindProperty]
        public Tarea TareaActual { get; set; }

        public IActionResult OnGet(int id)
        {
            var tareas = LeerTareas();
            TareaActual = tareas.FirstOrDefault(t => t.idTarea == id);
            if (TareaActual == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var tareas = LeerTareas();
            var tareaExistente = tareas.FirstOrDefault(t => t.idTarea == TareaActual.idTarea);
            if (tareaExistente == null)
            {
                return NotFound();
            }

            
            tareaExistente.nombreTarea = TareaActual.nombreTarea;
            tareaExistente.fechaVencimiento = TareaActual.fechaVencimiento;
            


            GuardarTareas(tareas);

            return RedirectToPage("Index");
        }

        private List<Tarea> LeerTareas()
        {
            if (!System.IO.File.Exists(jsonFilePath))
            {
                return new List<Tarea>();
            }
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            return JsonSerializer.Deserialize<List<Tarea>>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Tarea>();
        }

        private void GuardarTareas(List<Tarea> tareas)
        {
            var jsonContent = JsonSerializer.Serialize(tareas, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(jsonFilePath, jsonContent);
        }
    }
}

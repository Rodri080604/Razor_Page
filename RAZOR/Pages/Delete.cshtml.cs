using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RAZOR.Models;
using System.Text.Json;

namespace RAZOR.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

        public Tarea TareaAEliminar { get; set; }

        public IActionResult OnGet(string id)
        {
            var tareas = LeerTareas();
            TareaAEliminar = tareas.FirstOrDefault(t => t.idTarea == id);
            if (TareaAEliminar == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost(string id)
        {
            var tareas = LeerTareas();
            var tarea = tareas.FirstOrDefault(t => t.idTarea == id);
            if (tarea != null)
            {
                tareas.Remove(tarea);
                GuardarTareas(tareas);
            }
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

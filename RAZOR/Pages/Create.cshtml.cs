using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RAZOR.Models;
using System.Text.Json;

namespace RAZOR.Pages
{
    public class CreateModel : PageModel
    {
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

        [BindProperty]
        public Tarea NuevaTarea { get; set; }

        public void OnGet()
        {
            // Vista vacía para el formulario
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var tareas = LeerTareas();
            NuevaTarea.idTarea = Guid.NewGuid().ToString().Substring(0, 8); // Generar ID único
            tareas.Add(NuevaTarea);
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
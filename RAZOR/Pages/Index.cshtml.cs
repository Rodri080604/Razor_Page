using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RAZOR.Models;

namespace RAZOR.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public List<Tarea> Tareas { get; set; } = new();
        public void OnGet()
        {
            // Ruta del archivo JSON
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);

                // Deserializar a Lista<Tarea>
                Tareas = JsonSerializer.Deserialize<List<Tarea>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
    }
}

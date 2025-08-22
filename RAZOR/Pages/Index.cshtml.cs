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
        public List<Tarea> Tareas { get; set; } = new();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 5;
        public int TotalTareas { get; set; }
        private readonly ILogger<IndexModel> _logger;
        private readonly string jsonFilePath;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");
        }

        public void OnGet(int pagina = 1, int tamanoPagina = 4)
        {
            try
            {
                var todasLasTareas = LeerTareas();

                // Filtrar tareas con estado "Pendiente" o "En curso"
                var tareasFiltradas = todasLasTareas
                    .Where(t => t.estado == "Pendiente" || t.estado == "En curso")
                    .ToList();

                TotalTareas = tareasFiltradas.Count;
                _logger.LogInformation($"Encontradas {TotalTareas} tareas con estado Pendiente o En curso");

                TamanoPagina = tamanoPagina > 0 ? tamanoPagina : 4;
                if (TamanoPagina > TotalTareas) TamanoPagina = TotalTareas;

                TotalPaginas = (int)Math.Ceiling(TotalTareas / (double)TamanoPagina);

                PaginaActual = pagina < 1 ? 1 : pagina;
                if (PaginaActual > TotalPaginas) PaginaActual = TotalPaginas;

                Tareas = tareasFiltradas
                    .Skip((PaginaActual - 1) * TamanoPagina)
                    .Take(TamanoPagina)
                    .ToList();

                _logger.LogInformation($"Cargadas {Tareas.Count} tareas para página {PaginaActual}, tamanoPagina {TamanoPagina}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en OnGet: {ex.Message}");
            }
        }



        protected List<Tarea> LeerTareas()
        {
            if (!System.IO.File.Exists(jsonFilePath))
            {
                return new List<Tarea>();
            }

            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            return JsonSerializer.Deserialize<List<Tarea>>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Tarea>();
        }

        protected void GuardarTareas(List<Tarea> tareas)
        {
            var jsonContent = JsonSerializer.Serialize(tareas, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(jsonFilePath, jsonContent);
        }
        public IActionResult OnPostCompletar(string id)
        {
            var tareas = LeerTareas();
            var tarea = tareas.FirstOrDefault(t => t.idTarea == id);
            if (tarea == null)
            {
                return NotFound();
            }
            if (tarea.estado == "Finalizado")
            {
                TempData["Error"] = "La tarea ya está finalizada y no se puede completar nuevamente.";
                return RedirectToPage("Index");
            }

            tarea.estado = "Finalizado";
            GuardarTareas(tareas);

            return RedirectToPage("Index");
        }
    }
}
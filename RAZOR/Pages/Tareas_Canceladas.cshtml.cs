using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RAZOR.Models;
using System.Text.Json;

namespace RAZOR.Pages
{
    public class Tareas_CanceladasModel : PageModel
    {
        public List<Tarea> Tareas { get; set; } = new();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 5;
        public int TotalTareas { get; set; }
        private readonly ILogger<Tareas_ConcluidasModel> _logger; 
        private readonly string jsonFilePath;

        public Tareas_CanceladasModel(ILogger<Tareas_ConcluidasModel> logger)
        {
            _logger = logger;
            jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");
        }

        public void OnGet(int pagina = 1, int tamanoPagina = 4)
        {
            try
            {
                var todasLasTareas = LeerTareas();

                
                var tareasFiltradas = todasLasTareas
                    .Where(t => t.estado == "Cancelado") 
                    .ToList();

                TotalTareas = tareasFiltradas.Count;
                _logger.LogInformation($"Encontradas {TotalTareas} tareas con estado Finalizado");

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



        public IActionResult OnPostReactivar(int id)
        {
            try
            {
                var tareas = LeerTareas();

                
                var tarea = tareas.FirstOrDefault(t => t.idTarea == id);
                if (tarea != null)
                {
                    tarea.estado = "Pendiente"; 
                    GuardarTareas(tareas);
                    TempData["Mensaje"] = $"La tarea '{tarea.nombreTarea}' fue reactivada correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se encontró la tarea a reactivar.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en OnPostReactivar: {ex.Message}");
                TempData["Error"] = "Ocurrió un error al intentar reactivar la tarea.";
            }

            
            return RedirectToPage(new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
        }


        private List<Tarea> LeerTareas()
        {
            if (!System.IO.File.Exists(jsonFilePath))
            {
                _logger.LogWarning("Archivo tareas.json no encontrado");
                return new List<Tarea>();
            }

            try
            {
                var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
                var tareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Tarea>();
                _logger.LogInformation($"Deserializadas {tareas.Count} tareas");
                return tareas;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al leer tareas.json: {ex.Message}");
                return new List<Tarea>();
            }
        }
        private void GuardarTareas(List<Tarea> tareas)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(tareas, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(jsonFilePath, jsonContent);
                _logger.LogInformation("Tareas guardadas correctamente en tareas.json");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar tareas.json: {ex.Message}");
            }
        }

    }
}

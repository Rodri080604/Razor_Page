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

        public void OnGet(int pagina = 1, int tamanoPagina = 5)
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

                TamanoPagina = tamanoPagina > 0 ? tamanoPagina : 5;
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
        /*------------ Completar o Finalizar tareas------------------------*/
        public IActionResult OnPostCompletar(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de tarea no válido en OnPostCompletar");
                TempData["Error"] = "ID de tarea inválido.";
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            var tareas = LeerTareas();
            var tarea = tareas.FirstOrDefault(t => t.idTarea == id);
            if (tarea == null)
            {
                _logger.LogWarning($"Tarea con ID {id} no encontrada en OnPostCompletar");
                TempData["Error"] = "Tarea no encontrada.";
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            if (tarea.estado == "Finalizado")
            {
                TempData["Error"] = "La tarea ya está finalizada y no se puede completar nuevamente.";
                _logger.LogWarning($"Intento de completar tarea ya finalizada con ID {id}");
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            tarea.estado = "Finalizado";
            GuardarTareas(tareas);
            TempData["Success"] = "Tarea completada con éxito.";
            _logger.LogInformation($"Tarea con ID {id} marcada como Finalizado");

            return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
        }
        /*------------ En curso tareas------------------------*/
        public IActionResult OnPostEnCurso(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de tarea no válido en OnPostEnCurso");
                TempData["Error"] = "ID de tarea inválido.";
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            var tareas = LeerTareas();
            var tarea = tareas.FirstOrDefault(t => t.idTarea == id);
            if (tarea == null)
            {
                _logger.LogWarning($"Tarea con ID {id} no encontrada en OnPostEnCurso");
                TempData["Error"] = "Tarea no encontrada.";
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            if (tarea.estado == "Finalizado" || tarea.estado == "Cancelado")
            {
                TempData["Error"] = "No se puede mover a 'En curso' una tarea finalizada o cancelada.";
                _logger.LogWarning($"Intento inválido de poner en curso tarea con ID {id}");
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            tarea.estado = "En curso";
            GuardarTareas(tareas);
            TempData["Success"] = "La tarea fue marcada como 'En curso'.";
            _logger.LogInformation($"Tarea con ID {id} marcada como En curso");

            return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
        }

        /*------------ Cancelar tareas------------------------*/
        public IActionResult OnPostCancelar(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de tarea no válido en OnPostCancelar");
                TempData["Error"] = "ID de tarea inválido.";
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            var tareas = LeerTareas();
            var tarea = tareas.FirstOrDefault(t => t.idTarea == id);
            if (tarea == null)
            {
                _logger.LogWarning($"Tarea con ID {id} no encontrada en OnPostCancelar");
                TempData["Error"] = "Tarea no encontrada.";
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            if (tarea.estado == "Finalizado")
            {
                TempData["Error"] = "No se puede cancelar una tarea que ya fue finalizada.";
                _logger.LogWarning($"Intento inválido de cancelar tarea finalizada con ID {id}");
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            if (tarea.estado == "Cancelado")
            {
                TempData["Error"] = "La tarea ya está cancelada.";
                _logger.LogWarning($"Intento de cancelar tarea ya cancelada con ID {id}");
                return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
            }

            tarea.estado = "Cancelado";
            GuardarTareas(tareas);
            TempData["Success"] = "La tarea fue marcada como Cancelada.";
            _logger.LogInformation($"Tarea con ID {id} marcada como Cancelada");

            return RedirectToPage("Index", new { pagina = PaginaActual, tamanoPagina = TamanoPagina });
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
                TempData["Error"] = "Error al guardar los cambios en el archivo.";
            }
        }
    }
}
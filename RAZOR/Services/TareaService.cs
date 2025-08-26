using System.Text.Json;
using RAZOR.Models;

namespace RAZOR.Services
{
    public class TareaService
    {
        private readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

        // Obtener todas las tareas
        public List<Tarea> GetTareas()
        {
            if (!File.Exists(filePath))
                return new List<Tarea>();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Tarea>>(json) ?? new List<Tarea>();
        }

        // Guardar cambios
        public void SaveTareas(List<Tarea> tareas)
        {
            var json = JsonSerializer.Serialize(tareas, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        // Crear nueva tarea
        public void AddTarea(Tarea nuevaTarea)
        {
            var tareas = GetTareas();
            nuevaTarea.idTarea = tareas.Count > 0 ? tareas.Max(t => t.idTarea) + 1 : 1;
            nuevaTarea.estado = "Pendiente"; // siempre inicia pendiente
            tareas.Add(nuevaTarea);
            SaveTareas(tareas);
        }

        // Editar tarea (nombre, fecha o estado)
        public void UpdateTarea(Tarea tareaActualizada)
        {
            var tareas = GetTareas();
            var tarea = tareas.FirstOrDefault(t => t.idTarea == tareaActualizada.idTarea);

            if (tarea != null)
            {
                tarea.nombreTarea = tareaActualizada.nombreTarea;
                tarea.fechaVencimiento = tareaActualizada.fechaVencimiento;
                tarea.estado = tareaActualizada.estado;
                SaveTareas(tareas);
            }
        }

        // Cambiar solo el estado de una tarea
        public void CambiarEstado(int id, string nuevoEstado)
        {
            var tareas = GetTareas();
            var tarea = tareas.FirstOrDefault(t => t.idTarea == id);

            if (tarea != null)
            {
                tarea.estado = nuevoEstado;
                SaveTareas(tareas);
            }
        }
    }
}

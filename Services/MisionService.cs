using System.ComponentModel.DataAnnotations;
using QuestLog.Models;
using QuestLog.Repositories;

namespace QuestLog.Services;

public class MisionService(IMisionRepository repository) : IMisionService
{
    private static Mision ValidarYCrear(Mision request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ValidationException("El nombre de la misión es obligatorio.");
        if (request.Dificultad is not ("Fácil" or "Media" or "Difícil"))
            throw new ValidationException("La dificultad debe ser Fácil, Media o Difícil.");
        if (request.RecompensaXP < 0)
            throw new ValidationException("La recompensa XP no puede ser negativa.");
        return new Mision
        {
            Nombre = request.Nombre.Trim(),
            Dificultad = request.Dificultad,
            RecompensaXP = request.RecompensaXP,
            Completada = request.Completada
        };
    }

    public List<Mision> ObtenerTodas() => repository.ObtenerTodos();

    public Mision? ObtenerPorId(int id) => repository.ObtenerPorId(id);

    public Mision Crear(Mision request)
    {
        var mision = ValidarYCrear(request);
        mision.Completada = false;
        return repository.Agregar(mision);
    }

    public void Actualizar(int id, Mision request)
    {
        if (repository.ObtenerPorId(id) is null)
            throw new KeyNotFoundException("La misión no existe.");
        var mision = ValidarYCrear(request);
        if (!repository.Actualizar(id, mision))
            throw new KeyNotFoundException("La misión no existe.");
    }

    public void Eliminar(int id)
    {
        if (!repository.Eliminar(id))
            throw new KeyNotFoundException("La misión no existe.");
    }
}

using QuestLog.Models;
namespace QuestLog.Repositories;

public interface IMisionRepository
{
    List<Mision> ObtenerTodos();
    Mision? ObtenerPorId(int Id);
    Mision Agregar(Mision mision);
    bool Actualizar(int id, Mision mision);
    bool Eliminar(int id);
}

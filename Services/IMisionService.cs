using QuestLog.Models;
namespace QuestLog.Services;

public interface IMisionService
{
    List<Mision> ObtenerTodas();
    Mision? ObtenerPorId(int id);
    Mision Crear(Mision request);
    void Actualizar(int id, Mision request);
    void Eliminar(int id);
}

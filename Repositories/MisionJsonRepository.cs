using System.Text.Json;
using QuestLog.Models;

namespace QuestLog.Repositories;

public sealed class MisionJsonRepository : IMisionRepository
{
    private readonly string _ruta;
    private readonly object _lock = new();
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public MisionJsonRepository(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _ruta = Path.GetFullPath(Path.Combine(environment.ContentRootPath,
            configuration["Persistencia:RutaArchivo"] ?? "Data/misiones.json"));
        Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);
    }

    private List<Mision> Leer()
    {
        if (!File.Exists(_ruta)) return new List<Mision>();
        // Un archivo corrupto no se interpreta como vacío para evitar pérdida de datos.
        return JsonSerializer.Deserialize<List<Mision>>(File.ReadAllText(_ruta), _options)
            ?? throw new InvalidDataException("El archivo de misiones no contiene una lista JSON.");
    }

    private void Guardar(List<Mision> misiones)
    {
        var temporal = _ruta + ".tmp";
        try
        {
            File.WriteAllText(temporal, JsonSerializer.Serialize(misiones, _options));
            File.Move(temporal, _ruta, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporal)) File.Delete(temporal);
        }
    }

    public List<Mision> ObtenerTodos()
    {
        lock (_lock) return Leer();
    }

    public Mision? ObtenerPorId(int id)
    {
        lock (_lock) return Leer().FirstOrDefault(m => m.Id == id);
    }

    public Mision Agregar(Mision mision)
    {
        lock (_lock)
        {
            var misiones = Leer();
            mision.Id = checked(misiones.Select(m => m.Id).DefaultIfEmpty(0).Max() + 1);
            misiones.Add(mision);
            Guardar(misiones);
            return mision;
        }
    }

    public bool Actualizar(int id, Mision mision)
    {
        lock (_lock)
        {
            var misiones = Leer();
            var index = misiones.FindIndex(m => m.Id == id);
            if (index < 0) return false;
            mision.Id = id;
            misiones[index] = mision;
            Guardar(misiones);
            return true;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_lock)
        {
            var misiones = Leer();
            if (misiones.RemoveAll(m => m.Id == id) == 0) return false;
            Guardar(misiones);
            return true;
        }
    }
}

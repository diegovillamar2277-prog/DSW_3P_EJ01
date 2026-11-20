using Microsoft.AspNetCore.Mvc;
using QuestLog.Models;
using QuestLog.Services;
using Microsoft.AspNetCore.Authorization;

namespace QuestLog.Controllers;

[ApiController]
[Route("api/Misiones")]
[Authorize]
public class MisionesController(IMisionService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Mision>> ObtenerTodas() => Ok(service.ObtenerTodas());

    [HttpGet("{id:int}")]
    public ActionResult<Mision> ObtenerPorId(int id)
    {
        var mision = service.ObtenerPorId(id);
        if (mision is null) return NotFound(new ProblemDetails
        {
            Status = 404,
            Title = "Misión no encontrada",
            Detail = "La misión no existe."
        });
        return Ok(mision);
    }

    [HttpPost]
    public ActionResult<Mision> Crear([FromBody] Mision request)
    {
        var mision = service.Crear(request);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = mision.Id }, mision);
    }

    [HttpPut("{id:int}")]
    public IActionResult Actualizar(int id, [FromBody] Mision request)
    {
        service.Actualizar(id, request);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        service.Eliminar(id);
        return NoContent();
    }
}

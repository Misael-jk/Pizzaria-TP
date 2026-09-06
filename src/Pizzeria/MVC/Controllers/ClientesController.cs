using Microsoft.AspNetCore.Mvc;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.MVC.Controllers;

[Route("api/clientes")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Cliente cliente)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var id = await _clienteService.RegistrarClienteAsync(cliente);
            return CreatedAtAction(nameof(Obtener), new { id }, new { IdCliente = id, Mensaje = "Cliente registrado." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var cliente = await _clienteService.ObtenerClienteAsync(id);
        return cliente is not null ? Ok(cliente) : NotFound("Cliente no encontrado.");
    }
}

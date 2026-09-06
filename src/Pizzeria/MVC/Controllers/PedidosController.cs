using Microsoft.AspNetCore.Mvc;
using Pizzeria.API.DTO;
using Pizzeria.Servicios.DTOs;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.MVC.Controllers;

[Route("api/pedidos")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPedidoDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Map API DTO to Service DTO
            var svcRequest = new CrearPedidoRequest
            {
                IdCliente = request.IdCliente,
                DireccionEntrega = request.DireccionEntrega,
                Detalles = request.Detalles.Select(d => new DetalleRequest { IdPizza = d.IdPizza, Cantidad = d.Cantidad }).ToList()
            };

            var id = await _pedidoService.CrearPedidoAsync(svcRequest);
            return CreatedAtAction(nameof(Obtener), new { id }, new { IdPedido = id, Mensaje = "Pedido recibido, comenzando preparación." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (Exception)
        {
            return Problem("Ocurrió un error interno al procesar el pedido.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var pedido = await _pedidoService.ObtenerPedidoAsync(id);
        return pedido is not null ? Ok(pedido) : NotFound("Pedido no encontrado.");
    }
}

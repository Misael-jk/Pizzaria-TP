// INSTRUCCIONES DE TRASLADO:
// Este controlador usa CrearClienteDto (src/Pizzeria/API/DTO/CrearClienteDto.cs) y la entidad Cliente.
// Pasos para mover:
// 1) Añade referencia al proyecto Servicios (dotnet add reference ../Servicios/Servicios.csproj).
// 2) Importa el namespace Pizzeria.API.DTO donde esté el DTO.
// 3) Ajusta el namespace del archivo según el nombre del proyecto receptor.
// 4) Registra IClienteService en Program.cs del proyecto receptor: builder.Services.AddScoped<IClienteService, ClienteService>();
// 5) Asegúrate de que el proyecto receptor tenga acceso a Pizzeria.Dominio (si es necesario) o que Cliente esté disponible vía Servicios.
// 6) Compila y prueba las rutas: POST /api/clientes y GET /api/clientes/{id}.

using Microsoft.AspNetCore.Mvc;
using Pizzeria.API.DTO;
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
    public async Task<IActionResult> Crear([FromBody] CrearClienteDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion
        };

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

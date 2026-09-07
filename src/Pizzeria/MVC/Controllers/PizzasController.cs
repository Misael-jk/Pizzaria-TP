// INSTRUCCIONES DE TRASLADO:
// Este controlador está en la carpeta src/Pizzeria/MVC/Controllers para facilitar su revisión.
// Si vas a moverlo a un proyecto MVC independiente o al proyecto API existente, sigue estos pasos:
// 1) Añade una referencia de proyecto al módulo Servicios desde el nuevo proyecto MVC:
//    <ProjectReference Include="..\..\Servicios\Servicios.csproj" />
//    (o desde la carpeta del proyecto: dotnet add reference ../Servicios/Servicios.csproj)
// 2) Asegurate de que el nuevo proyecto tenga acceso a las dependencias transitivas (Dominio, Persistencia) que ya están referenciadas por Servicios.
// 3) Ajusta el namespace del archivo si lo deseas (ej: Pizzeria.API.Controllers o Pizzeria.MVC.Controllers).
// 4) En Program.cs del proyecto receptor, registra los servicios necesarios en DI:
//    builder.Services.AddScoped<IPizzaService, PizzaService>();
//    (además de IPedidoService, IClienteService si usas otros controladores)
// 5) Mantén las mismas rutas y acciones; el controller solo depende de la interfaz IPizzaService.
// 6) Compila y prueba: dotnet build / dotnet run

using Microsoft.AspNetCore.Mvc;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.MVC.Controllers;

[Route("api/pizzas")]
[ApiController]
public class PizzasController : ControllerBase
{
    private readonly IPizzaService _pizzaService;

    public PizzasController(IPizzaService pizzaService)
    {
        _pizzaService = pizzaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDisponibles()
    {
        var pizzas = await _pizzaService.VerDisponiblesAsync();
        return Ok(pizzas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var pizza = await _pizzaService.ObtenerPizzaAsync(id);
        return pizza is not null ? Ok(pizza) : NotFound("Pizza no encontrada.");
    }
}

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

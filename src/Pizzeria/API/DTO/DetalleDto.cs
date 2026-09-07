using System.ComponentModel.DataAnnotations;

namespace Pizzeria.API.DTO;

public class DetalleDto
{
    public int IdPizza { get; set; }
    public int Cantidad { get; set; }
}

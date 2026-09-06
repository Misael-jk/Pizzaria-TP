using Pizzeria.API.Endpoints;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Persistencia.Repos;
using Pizzeria.Servicios.Interface;
using Pizzeria.Servicios.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Capa de Datos (Repositorios)
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();

// Capa de Negocio (Servicios)
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPizzaService, PizzaService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPizzaEndpoints();
app.MapClienteEndpoints();
app.MapPedidoEndpoints();

app.Run();
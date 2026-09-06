using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Persistencia.Repos;
using Pizzeria.Servicios.Interface;
using Pizzeria.Servicios.Service;
using Pizzeria.Servicios.Background;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

// Background queue and simulator
builder.Services.AddSingleton<IPedidoProcessingQueue, PedidoProcessingQueue>();
builder.Services.AddHostedService<PedidoStateSimulator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();

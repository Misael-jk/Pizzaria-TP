INSTRUCCIONES_PARA_TRASLADAR_CONTROLLERS.md

Estos archivos están listos para ser movidos manualmente a tu proyecto MVC.
Pasos recomendados:

1. Crea un nuevo proyecto MVC o reutiliza el proyecto API existente.
   - Nuevo proyecto (opcional): dotnet new mvc -n Pizzeria.MVC

2. En el proyecto receptor, añade referencias a los proyectos necesarios:
   - dotnet add <tu-proyec-mvc>.csproj reference ../Servicios/Servicios.csproj
   - Servicios ya referencia a Dominio y Persistencia, así que no necesitas agregar esas referencias manualmente salvo que el controller use entidades del dominio directamente.

3. Copia los archivos de src/Pizzeria/MVC/Controllers/* al folder Controllers/ de tu nuevo proyecto.

4. Copia los DTOs de API si los necesitas (src/Pizzeria/API/DTO/*.cs) al nuevo proyecto o añade un ProjectReference al proyecto API que los contenga.

5. En Program.cs del proyecto receptor, registra los servicios:
   builder.Services.AddScoped<IPedidoService, PedidoService>();
   builder.Services.AddScoped<IPizzaService, PizzaService>();
   builder.Services.AddScoped<IClienteService, ClienteService>();

6. Asegúrate de tener using correctos y namespaces alineados.

7. Compila y prueba: dotnet build && dotnet run

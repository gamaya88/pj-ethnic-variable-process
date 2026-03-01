// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PJ.Oti.ProcesaNuevosExpedientes.Service;
using PJ.Oti.ProcesaNuevosExpedientes.Service.Model.HelpDeskDb;
using System.IO;
using System.Threading.Tasks;

// Crea un host por defecto que maneja la configuración y el registro.
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        // Agrega tu archivo de configuración JSON.
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Obtiene la cadena de conexión de la configuración.
        var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");

        // Configura el DbContext con la cadena de conexión.
        services.AddDbContext<HelpDeskDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Registra la clase ProcesadorDeDatos como un servicio.
        services.AddScoped<ProcesadorDeDatos>();
    });

var host = builder.Build();

// Usando un scope para asegurar la correcta liberación de recursos.
using (var serviceScope = host.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var procesador = services.GetRequiredService<ProcesadorDeDatos>();

    // Carga la configuración personalizada de la aplicación.
    var configuracionApp = services.GetRequiredService<IConfiguration>()
                                   .GetSection("ConfiguracionApp")
                                   .Get<Configuracion>();

    // Llama al método para ejecutar la lógica de la aplicación.
    procesador.EjecutarProceso(configuracionApp.RutaExcel, configuracionApp.CorreosDestino);
}

await host.RunAsync();
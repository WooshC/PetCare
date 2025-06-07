// Services/ActualizacionEstadoService.cs
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetCare.Services
{
    public class ActualizacionEstadoService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public ActualizacionEstadoService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var solicitudService = scope.ServiceProvider.GetRequiredService<ISolicitudService>();
                    await solicitudService.ActualizarEstadosAutomaticos();
                }

                // Verificar cada minuto
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
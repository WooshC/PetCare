using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace PetCare.Services
{
    public class ChatHub : Hub
    {
        public async Task UnirseASolicitud(int solicitudId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, solicitudId.ToString());
        }

        public async Task EnviarMensaje(int solicitudId, string mensaje)
        {
            if (string.IsNullOrEmpty(mensaje))
                return;

            await Clients.Group(solicitudId.ToString())
                .SendAsync("ReceiveMessage", Context.User.Identity.Name, mensaje, DateTime.Now);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Limpiar grupos al desconectarse
            await base.OnDisconnectedAsync(exception);
        }
    }
} 
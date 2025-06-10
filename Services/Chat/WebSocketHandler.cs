using System.Net.WebSockets;
using System.Text;

namespace PetCare.Services.Chat
{
    public class WebSocketHandler
    {
        private static readonly List<WebSocket> _sockets = new();

        public async Task HandleAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _sockets.Add(webSocket);

                var buffer = new byte[1024 * 4];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var messageBytes = Encoding.UTF8.GetBytes(message);

                        foreach (var socket in _sockets.Where(s => s != webSocket && s.State == WebSocketState.Open))
                        {
                            await socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrado por el usuario", CancellationToken.None);
                        _sockets.Remove(webSocket);
                    }
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}

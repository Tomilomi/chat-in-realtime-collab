using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace chat_in_realtime.Hubs.Filters
{
    public class RateLimitingFilter : IHubFilter
    {
        private static readonly ConcurrentDictionary<string, DateTime> _lastActionTime = new();

        // Tiempo mínimo entre acciones (1 segundo)
        private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(1);

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var userId = invocationContext.Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                var now = DateTime.UtcNow;

                // Intentamos obtener el último tiempo de acción del usuario
                if (_lastActionTime.TryGetValue(userId, out var lastTime))
                {
                    if (now - lastTime < _cooldown)
                    {
                        // Si no pasó 1 segundo, rebotamos la petición directamente
                        throw new HubException("Estás enviando solicitudes muy rápido. Espera un momento.");
                    }
                }

                // Actualizamos el tiempo de la última acción
                _lastActionTime[userId] = now;
            }

            return await next(invocationContext);
        }
    }
}
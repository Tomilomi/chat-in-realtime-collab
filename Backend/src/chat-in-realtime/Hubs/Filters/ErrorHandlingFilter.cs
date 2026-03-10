using Application.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace chat_in_realtime.Hubs.Filters
{
    public class ErrorHandlingFilter : IHubFilter
    {
        private readonly ILogger<ErrorHandlingFilter> _logger;

        public ErrorHandlingFilter(ILogger<ErrorHandlingFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
        {
            ////////////
            /// ejecucion antes de la invocacion del metodo del hub

            var messageDto = invocationContext.HubMethodArguments.OfType<SendMessageDTO>().FirstOrDefault();

            //validar el mensaje
            if (messageDto != null)
            {
                var validator = invocationContext.ServiceProvider.GetService<IValidator<SendMessageDTO>>();

                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(messageDto);

                    if (!validationResult.IsValid)
                    {
                        var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                        throw new HubException($"Error de validación: {errors}");
                    }
                }
            }

            try
            {
                ////////////
                ///ejecucion del metodo del hub

                var result = await next(invocationContext);

                //si dió error, se asume que es un error de tipo IErrorOr
                if (result is IErrorOr errorOr && errorOr.IsError)
                {
                    var firstError = errorOr.Errors[0];
                    throw new HubException($"Error ({firstError.Code}): {firstError.Description}");
                }

                return result; //devolver resultado si todo salio bien
            }
            catch (HubException)
            {
                // Las HubExceptions ya las armamos nosotros, las dejamos pasar al cliente
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en el servidor.");
                throw new HubException("Error inesperado en el servidor.");
            }
        }
    }
}
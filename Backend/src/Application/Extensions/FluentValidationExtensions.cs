using ErrorOr;
using FluentValidation;

namespace chat_in_realtime.Extensions
{
    /// <summary>
    /// Metodo de extension para combinar FLuentValidation con ErrorOr,
    /// permitiendo agregar Errores custom en las reglas de validacion
    /// </summary>
    public static class FluentValidationExtensions
    {
        // Este método nos permite pasar un objeto Error de ErrorOr directamente a las reglas de validación
        public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, Error error)
        {
            if (error.Type != ErrorType.Validation)
            {
                throw new ArgumentException("El error proporcionado debe ser de tipo Validation.", nameof(error));
            }

            return rule
                .WithErrorCode(error.Code)
                .WithMessage(error.Description);
        }
    }
}
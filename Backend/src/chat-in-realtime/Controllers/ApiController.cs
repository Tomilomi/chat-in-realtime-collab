using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace chat_in_realtime.Controllers
{
    /// <summary>
    /// Clase base de controller que agrega métodos para mapear ErrorOr a respuestas HTTP estándar.
    /// </summary>
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// Método que mapea un resultado de tipo ErrorOr a una respuesta HTTP adecuada.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        protected IActionResult Problem(List<Error> errors)
        {
            if (errors.Count == 0) { return Problem(); }

            if (errors.All(error => error.Type == ErrorType.Validation)) { return ValidationProblem(errors); }

            var firstError = errors[0];
            var statusCode = firstError.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(statusCode: statusCode, title: firstError.Description);
        }

        /// <summary>
        /// Método que mapea un resultado tipo ErrorOr a una respuesta HTTP.
        /// Solo si todos los errores son de tipo Validation.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        private IActionResult ValidationProblem(List<Error> errors)
        {
            //clase de ASP.NET que trackea que campos tienen errores
            var modelStateDictionary = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelStateDictionary.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(modelStateDictionary);
        }
    }
}
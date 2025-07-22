using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorsController : ApiBaseController
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Exception? exception = exceptionHandlerFeature?.Error;

            if (exception != null)
            {
                // Registra la excepción con Serilog
                _logger.LogError(exception, "An unhandled exception occurred.");
            }
            else
            {
                // Registra un mensaje genérico si no hay excepción
                _logger.LogError("An error occurred, but no exception was captured.");
            }

            return Problem();
        }
    }
}

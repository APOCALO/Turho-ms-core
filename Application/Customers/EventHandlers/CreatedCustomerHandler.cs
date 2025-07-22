using Domain.Customers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.EventHandlers
{
    internal sealed class CreatedCustomerHandler : INotificationHandler<CreatedCustomerEvent>
    {
        private readonly ILogger<CreatedCustomerHandler> _logger;

        public CreatedCustomerHandler(ILogger<CreatedCustomerHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(CreatedCustomerEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {RequestName} with event: {@Notification}", typeof(CreatedCustomerEvent).Name, notification);

            return Task.CompletedTask;
        }
    }
}

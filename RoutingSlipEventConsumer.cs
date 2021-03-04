using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;

namespace MassTransitRoutingSlipMonitoring
{
    public class RoutingSlipEventConsumer :
        IConsumer<RoutingSlipFaulted>,
        IConsumer<RoutingSlipCompleted>
    {
        readonly ILogger<RoutingSlipEventConsumer> _logger;

        public RoutingSlipEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Log(LogLevel.Information, "Routing Slip Completed: {TrackingNumber}",
                    context.Message.TrackingNumber);

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Log(LogLevel.Information, "Routing Slip Faulted: {TrackingNumber} {ExceptionInfo}", 
                    context.Message.TrackingNumber,
                    context.Message.ActivityExceptions.FirstOrDefault());

            return Task.CompletedTask;
        }
    }
}

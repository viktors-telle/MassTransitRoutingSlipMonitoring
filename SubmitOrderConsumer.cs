using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.Definition;

namespace MassTransitRoutingSlipMonitoring
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            var routingSlipBuilder = new RoutingSlipBuilder(NewId.NextGuid());
        
            var activityName = nameof(SaveOrderActivity).Replace("Activity", string.Empty);
            var sanitizedName = KebabCaseEndpointNameFormatter.Instance.SanitizeName(activityName);

            routingSlipBuilder.AddActivity(
                activityName,
                new Uri($"queue:{sanitizedName}_execute"),
                new
                {
                    context.Message.OrderId
                }
            );
            
            var routingSlipAddress = new Uri($"rabbitmq://localhost/{KebabCaseEndpointNameFormatter.Instance.Consumer<RoutingSlipEventConsumer>()}");

            routingSlipBuilder.AddSubscription(
                routingSlipAddress,
                RoutingSlipEvents.Completed
            );
            
            var routingSlip = routingSlipBuilder.Build();
            await context.Execute(routingSlip);
            
            if (context.RequestId != null)
                await context.RespondAsync<OrderSubmissionAccepted>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId
                });
        }
    }
}
using System;

namespace MassTransitRoutingSlipMonitoring
{
    public interface OrderSubmissionRejected
    {
        Guid OrderId { get; }
        
        DateTime Timestamp { get; }
    }
}
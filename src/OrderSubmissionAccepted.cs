using System;

namespace MassTransitRoutingSlipMonitoring
{
    public interface OrderSubmissionAccepted
    {
        Guid OrderId { get; }
        
        DateTime Timestamp { get; }
    }
}
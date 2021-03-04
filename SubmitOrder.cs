using System;

namespace MassTransitRoutingSlipMonitoring
{
    public interface SubmitOrder
    {
        Guid OrderId { get; }
        
        DateTime Timestamp { get; }
    }
}
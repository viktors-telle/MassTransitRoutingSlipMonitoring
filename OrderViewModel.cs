using System;
using System.ComponentModel.DataAnnotations;

namespace MassTransitRoutingSlipMonitoring
{
    public class OrderViewModel
    {
        [Required] 
        public Guid Id { get; set; }
    }
}
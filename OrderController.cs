using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitRoutingSlipMonitoring
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;

        public OrderController(IRequestClient<SubmitOrder> submitOrderRequestClient)
        {
            _submitOrderRequestClient = submitOrderRequestClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (accepted, rejected) = await _submitOrderRequestClient
                .GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
                {
                    OrderId = model.Id,
                    InVar.Timestamp,
                });

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;

                return Accepted(response);
            }

            if (accepted.IsCompleted)
            {
                await accepted;

                return Problem("Order was not accepted");
            }
            else
            {
                var response = await rejected;

                return BadRequest(response.Message);
            }
        }
    }
}
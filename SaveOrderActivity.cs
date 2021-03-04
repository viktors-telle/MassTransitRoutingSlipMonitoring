using System;
using System.Threading.Tasks;
using MassTransit.Courier;

namespace MassTransitRoutingSlipMonitoring
{
    public class SaveOrderActivity : IExecuteActivity<SaveOrderActivityArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<SaveOrderActivityArguments> context)
        {
            await Task.Delay(500);
            
            return context.Completed();
        }
    }

    public interface SaveOrderActivityArguments
    {
        Guid OrderId { get; }
    }
}
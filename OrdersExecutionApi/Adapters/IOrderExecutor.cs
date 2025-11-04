using OrdersExecutionApi.Models;

namespace OrdersExecutionApi.Adapters;

internal interface IOrderExecutor
{
    ValueTask<Trade> ExecuteOrderAsync(Order order, CancellationToken cancellationToken);
}
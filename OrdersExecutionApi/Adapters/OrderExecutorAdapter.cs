using OrdersExecution;
using OrdersExecutionApi.Converters;
using OrdersExecutionApi.Models;
using OrdersExecutionApi.Services;

namespace OrdersExecutionApi.Adapters;

internal sealed class OrderExecutorAdapter : IOrderExecutor
{
    private readonly OrdersExecutionExercise.IOrderExecutor _orderExecutor;
    private readonly ITradeCache _tradeCache;
    private readonly ILogger<IOrderExecutor> _logger;
    public OrderExecutorAdapter(OrdersExecutionExercise.IOrderExecutor orderExecutor, ITradeCache tradeCache, ILogger<IOrderExecutor> logger)
    {
        _orderExecutor = orderExecutor;
        _tradeCache = tradeCache;
        _logger = logger;
    }
    public async ValueTask<Trade> ExecuteOrderAsync(Order order, CancellationToken cancellationToken)
    {
        if (_tradeCache.TryGetTrade(order, out var cachedTrade))
        {
            _logger.LogInformation("Retrieving {Trade} corresponding to {Order} from cache.", cachedTrade, order);
            return cachedTrade;
        }
        var libOrder = order.Convert();
        _logger.LogInformation("Requesting Trade corresponding to {Order} from external service...", order);
        var libTrade = await _orderExecutor.ExecuteOrderAsync(libOrder, cancellationToken).ConfigureAwait(false);
        var trade = libTrade.Convert();
        _tradeCache.Add(order, trade);
        _logger.LogInformation("Adding {Trade} to cache.", trade);
        return trade;
    }
}
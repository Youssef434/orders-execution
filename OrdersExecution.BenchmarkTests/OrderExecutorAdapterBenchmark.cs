using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using OrdersExecutionApi.Adapters;
using OrdersExecutionApi.Models;
using OrdersExecutionApi.Services;

namespace OrdersExecution.BenchmarkTests;

[MemoryDiagnoser]
public class OrderExecutorAdapterBenchmark
{
    private IOrderExecutor _executor;
    private Order _order;

    [GlobalSetup]
    public void Setup()
    {
        _order = new Order(Way.Buy, string.Empty, 0, 0, DateTime.Today);
        _executor = new OrderExecutorAdapter(
            new OrdersExecutionExercise.OrderExecutor(),
            new TradeCache(),
            new NullLogger<OrderExecutorAdapter>());
    }
    
    [Benchmark]
    public async Task ExecuteOrderTenThousandTimesWithValueTask()
    {
        for (var i = 0; i < 10_000; i += 1)
            _ = await _executor.ExecuteOrderAsync(_order, CancellationToken.None);
    }

    [Benchmark]
    public async Task ExecuteOrderThenThousandTimesWithTask()
    {
        for (var i = 0; i < 10_000; i += 1)
            _ = await ExecuteOrderAsync(_order).ConfigureAwait(false);
        return;
        async Task<Trade> ExecuteOrderAsync(Order order)
        {
            return await _executor.ExecuteOrderAsync(order, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
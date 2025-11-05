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
        await Parallel.ForEachAsync(Enumerable.Range(0, 10_000), async (_, _) =>
        {
            _ = await _executor.ExecuteOrderAsync(_order, CancellationToken.None).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    [Benchmark]
    public async Task ExecuteOrderThenThousandTimesWithTask()
    {
        await Parallel.ForEachAsync(Enumerable.Range(0, 10_000), async (_, _) =>
        {
            _ = await ExecuteOrderAsync(_order).ConfigureAwait(false);
        }).ConfigureAwait(false);
        return;
        async Task<Trade> ExecuteOrderAsync(Order order)
        {
            return await _executor.ExecuteOrderAsync(order, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
using OrdersExecution;
using OrdersExecutionApi.Adapters;
using OrdersExecutionApi.Models;
using OrdersExecutionApi.Services;
using static Microsoft.AspNetCore.Http.Results;
using static Microsoft.AspNetCore.Http.StatusCodes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITradeCache, TradeCache>();
builder.Services.AddSingleton<OrdersExecutionExercise.IOrderExecutor, OrdersExecutionExercise.OrderExecutor>();
builder.Services.AddSingleton<IOrderExecutor, OrderExecutorAdapter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/orders/execute", async (Order? order, IOrderExecutor orderExecutor, CancellationToken cancellationToken, ILogger<Program> logger) =>
{
    try
    {
        ArgumentNullException.ThrowIfNull(order);
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
        var trade = await orderExecutor.ExecuteOrderAsync(order, cancellationTokenSource.Token).ConfigureAwait(false);
        logger.LogInformation("Order {Order} executed successfully", order);
        return Ok(trade);
    }
    catch (ArgumentNullException e)
    {
        logger.LogError(e, "Invalid order");
        return BadRequest(e.Message);
    }
    catch (OperationCanceledException e)
    {
        logger.LogError("Execution cancelled for order {Order}", order);
        return Problem(detail: e.Message, statusCode: Status408RequestTimeout);
    }
    catch (Exception e)
    {
        logger.LogError(e, "Unexpected error for order {Order}", order);
        return Problem(detail: e.Message, statusCode: Status500InternalServerError);
    }
});

app.Run();
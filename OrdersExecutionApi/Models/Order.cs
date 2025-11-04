namespace OrdersExecutionApi.Models;

internal sealed record Order(
    Way Way, 
    string Instrument, 
    decimal Quantity, 
    decimal LimitPrice, 
    DateTime OrderDate);
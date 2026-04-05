using AggregationService.Application.Behaviors;
using AggregationService.Application.Interfaces;
using AggregationService.Infrastructure.Clients;
using AggregationService.Infrastructure.Handlers;
using AggregationService.Infrastructure.Middleware;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(LoggingBehavior<,>).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddScoped<IProductClient, ProductClient>();
builder.Services.AddScoped<IStockClient, StockClient>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient<IPricingClient, PricingClient>().AddTransientHttpErrorPolicy(policy =>
policy.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(100)));

var app = builder.Build();
app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

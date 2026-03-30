using Calcolb.API.Middleware;
using Calcolb.API.Pipeline;
using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Infrastructure.Persistence;
using Calcolb.Modules.Estimation.Infrastructure.Persistence.Repositories;
using Calcolb.Modules.Event.Application.Repositories;
using Calcolb.Modules.Event.Infrastructure.Persistence;
using Calcolb.Modules.Event.Infrastructure.Persistence.Repositories;
using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Modules.Expense.Infrastructure.Persistence;
using Calcolb.Modules.Expense.Infrastructure.Persistence.Repositories;
using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Application.Services;
using Calcolb.Modules.Shopping.Infrastructure.ExternalServices;
using Calcolb.Modules.Shopping.Infrastructure.Persistence;
using Calcolb.Modules.Shopping.Infrastructure.Persistence.Repositories;
using Calcolb.Modules.Transport.Application.Repositories;
using Calcolb.Modules.Transport.Application.Services;
using Calcolb.Modules.Transport.Infrastructure.ExternalServices;
using Calcolb.Modules.Transport.Infrastructure.Persistence;
using Calcolb.Modules.Transport.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

// ─── Serilog bootstrap ───────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Calcolb")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/calcolb-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Calcolb API başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // ─── Controllers & OpenAPI ───────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // ─── Mediator ────────────────────────────────────────────────────────────
    builder.Services.AddMediator();
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

    // ─── FluentValidation ────────────────────────────────────────────────────
    builder.Services.AddValidatorsFromAssemblyContaining<
        Calcolb.Modules.Event.Application.Commands.CreateEvent.CreateEventCommandValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<
        Calcolb.Modules.Transport.Application.Commands.CreateTransportPlan.CreateTransportPlanCommandValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<
        Calcolb.Modules.Shopping.Application.Commands.AddCartItem.AddCartItemCommandValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<
        Calcolb.Modules.Expense.Application.Commands.AddExpenseItem.AddExpenseItemCommandValidator>();

    // ─── Database Contexts ───────────────────────────────────────────────────
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

    builder.Services.AddDbContext<EventDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddDbContext<TransportDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddDbContext<ShoppingDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddDbContext<ExpenseDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddDbContext<EstimationDbContext>(options =>
        options.UseNpgsql(connectionString));

    // ─── Repositories ────────────────────────────────────────────────────────
    builder.Services.AddScoped<IEventRepository, EventRepository>();
    builder.Services.AddScoped<ITransportPlanRepository, TransportPlanRepository>();
    builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IExpensePlanRepository, ExpensePlanRepository>();
    builder.Services.AddScoped<IEstimationSessionRepository, EstimationSessionRepository>();

    // ─── External Services (stub implementations) ────────────────────────────
    builder.Services.AddScoped<IFuelPriceProvider, EpdkFuelPriceProvider>();
    builder.Services.AddScoped<IRouteCalculator, GoogleMapsRouteCalculator>();
    builder.Services.AddScoped<IPriceProvider, MarketFiyatiPriceProvider>();

    // ─── Build ───────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ─── Middleware pipeline ──────────────────────────────────────────────────
    app.UseMiddleware<ExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Calcolb API";
            options.Theme = ScalarTheme.Purple;
        });
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };
    });

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama başlatılamadı.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

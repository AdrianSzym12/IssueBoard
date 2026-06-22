using IssueBoard.Api.Middleware;
using IssueBoard.Application;
using IssueBoard.Infrastructure;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("IssueBoardWeb", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:5002",
                "http://localhost:5003")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("IssueBoardWeb");

app.MapControllers();

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        Application = "IssueBoard.Api",
        Status = "Running"
    });
});

app.Run();

public partial class Program;

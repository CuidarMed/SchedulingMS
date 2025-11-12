using Application.Converters;
using Application.Interfaces.IAppointment;
using Application.Interfaces.IAvailabilityBlock;
using Application.Interfaces.IDoctorAvailability;
using Application.Interfaces.IClinical;
using Application.Interfaces.IAuth;
using Application.Mappers;
using Application.Services;
using Application.Services.AppointmentService;
using Application.Services.AvailabilityBlockService;
using Application.Services.DoctorAvailabilityService;
using Application.Validators.AppointmentValidators;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Command;
using Infrastructure.Commands;
using Infrastructure.Handlers;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opt =>
    {
        // Devolver ProblemDetails estándar en validaciones
        opt.InvalidModelStateResponseFactory = context =>
            new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FluentValidation: valida DTOs/commands del proyecto Application
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Application"));

// ========== AvailabilityBlock ==========
builder.Services.AddScoped<ICreateAvailabilityBlockService, CreateAvailabilityBlockService>();
builder.Services.AddScoped<IUpdateAvailabilityBlockService, UpdateAvailabilityBlockService>();
builder.Services.AddScoped<ISearchAvailabilityBlockService, SearchAvailabilityBlockService>();
builder.Services.AddScoped<IAvailabilityBlockCommand, AvailabilityBlockCommand>();
builder.Services.AddScoped<IAvailabilityBlockQuery, AvailabilityBlockQuery>();
builder.Services.AddScoped<IAvailabilityBlockMapper, AvailabilityBlockMapper>();

// ========== Appointment ==========
builder.Services.AddScoped<ICreateAppointmentService, CreateAppointmentService>();
builder.Services.AddScoped<ISearchAppointmentService, SearchAppointmentService>();
builder.Services.AddScoped<IUpdateAppointmentService, UpdateAppointmentService>();
builder.Services.AddScoped<IAppointmentCommand, AppointmentCommand>();
builder.Services.AddScoped<IAppointmentQuery, AppointmentQuery>();
builder.Services.AddScoped<IAppointmentMapper, AppointmentMapper>();

// ========== DoctorAvailability ==========
builder.Services.AddScoped<IDoctorAvailabilityMapper, DoctorAvailabilityMapper>();
builder.Services.AddScoped<ICreateDoctorAvailabilityService, CreateDoctorAvailabilityService>();
builder.Services.AddScoped<IUpdateDoctorAvailabilityService, UpdateDoctorAvailabilityService>();
builder.Services.AddScoped<ISearchDoctorAvailabilityService, SearchDoctorAvailabilityService>();
builder.Services.AddScoped<IDoctorAvailabilityCommand, DoctorAvailabilityCommand>();
builder.Services.AddScoped<IDoctorAvailabilityQuery, DoctorAvailabilityQuery>();

// Obtenego la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("ConnectionString 'DefaultConnection' no configurada");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddCors(x => x.AddDefaultPolicy(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

// ========== JsonStringEnumConverter ==========
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
    });

// ========== HttpClient para AuthMS (obtener tokens de servicio) ==========
builder.Services.AddHttpClient("AuthMS", client =>
{
    var baseUrl = builder.Configuration["AuthMS:BaseUrl"] ?? "http://localhost:5093";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ========== HttpClient para ClinicalMS (con JWT) ==========
builder.Services.AddHttpClient("ClinicalMS", client =>
{
    var baseUrl = builder.Configuration["ClinicalMS:BaseUrl"] ?? "http://localhost:5073";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler(serviceProvider =>
{
    var tokenProvider = serviceProvider.GetRequiredService<IServiceTokenProvider>();
    var logger = serviceProvider.GetRequiredService<ILogger<JwtServiceClientHandler>>();
    return new JwtServiceClientHandler(tokenProvider, logger);
});

// ========== Servicios de comunicación entre microservicios ==========
builder.Services.AddSingleton<IServiceTokenProvider, ServiceTokenProvider>();
builder.Services.AddScoped<IClinicalService, ClinicalService>();

var app = builder.Build();

// ========== Aplicar migraciones ==========
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    const int maxRetries = 10;
    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Applying migrations... Attempt {Attempt} of {MaxRetries}", attempt, maxRetries);
            dbContext.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations on attempt {Attempt} of {MaxRetries}", attempt, maxRetries);
            if (attempt == maxRetries)
            {
                logger.LogCritical("Max migration attempts reached. Exiting application.");
                throw;
            }
            await Task.Delay(TimeSpan.FromSeconds(3)); // Wait before retrying
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

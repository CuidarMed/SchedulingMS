using Application.Converters;
using Application.Interfaces.IAppointment;
using Application.Interfaces.IAvailabilityBlock;
using Application.Interfaces.IDoctorAvailability;
using Application.Interfaces.IClinical;
using Application.Interfaces.IAuth;
using Application.Interfaces.IVideo;
using Application.Mappers;
using Application.Services;
using Application.Services.AppointmentService;
using Application.Services.AvailabilityBlockService;
using Application.Services.DoctorAvailabilityService;
using FluentValidation;
using Infrastructure.Command;
using Infrastructure.Commands;
using Infrastructure.Handlers;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Linq;


// BUILDER CONFIGURATION


var builder = WebApplication.CreateBuilder(args);

// -------------------- 1. Core Services: MVC, Swagger, FluentValidation --------------------

// Configuracion de Controllers, validación de ModelState y JSON Converters (consolidado)
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opt =>
    {
        opt.InvalidModelStateResponseFactory = context =>
            new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
    })
    .AddJsonOptions(options =>
    {
        // JSON Converters
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agrega los validadores definidos en el ensamblado 'Application'
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Application"));


// -------------------- 2. Database & Connection Configuration --------------------

// Se usa una sola vez la lectura y validación de la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// CORS
builder.Services.AddCors(x => x.AddDefaultPolicy(c =>
    c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));


// -------------------- 3. HttpClient & Inter-Service Communication --------------------

// Servicio Singleton para obtener tokens JWT (Necesario para el JwtServiceClientHandler)
builder.Services.AddSingleton<IServiceTokenProvider, ServiceTokenProvider>();

// HttpClient para AuthMS
builder.Services.AddHttpClient("AuthMS", client =>
{
    var baseUrl = builder.Configuration["AuthMS:BaseUrl"] ?? "http://localhost:5093";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// HttpClient para ClinicalMS (con JWT Handler)
builder.Services.AddHttpClient("ClinicalMS", client =>
{
    var baseUrl = builder.Configuration["ClinicalMS:BaseUrl"] ?? "http://localhost:5073";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
// Registra el Handler que inyectará el token JWT
.AddHttpMessageHandler(serviceProvider =>
{
    var tokenProvider = serviceProvider.GetRequiredService<IServiceTokenProvider>();
    var logger = serviceProvider.GetRequiredService<ILogger<JwtServiceClientHandler>>();
    return new JwtServiceClientHandler(tokenProvider, logger);
});

// Servicio que utiliza el HttpClient de ClinicalMS
builder.Services.AddScoped<IClinicalService, ClinicalService>();

// Video Service (Daily.co)
builder.Services.AddScoped<IVideoService, VideoService>();


// -------------------- 4. Domain Services: Application & Infrastructure DI --------------------

// AvailabilityBlock
builder.Services.AddScoped<ICreateAvailabilityBlockService, CreateAvailabilityBlockService>();
builder.Services.AddScoped<IUpdateAvailabilityBlockService, UpdateAvailabilityBlockService>();
builder.Services.AddScoped<ISearchAvailabilityBlockService, SearchAvailabilityBlockService>();
builder.Services.AddScoped<IAvailabilityBlockCommand, AvailabilityBlockCommand>();
builder.Services.AddScoped<IAvailabilityBlockQuery, AvailabilityBlockQuery>();
builder.Services.AddScoped<IAvailabilityBlockMapper, AvailabilityBlockMapper>();

// Appointment
builder.Services.AddScoped<ICreateAppointmentService, CreateAppointmentService>();
builder.Services.AddScoped<ISearchAppointmentService, SearchAppointmentService>();
builder.Services.AddScoped<IUpdateAppointmentService, UpdateAppointmentService>();
builder.Services.AddScoped<IAppointmentCommand, AppointmentCommand>();
builder.Services.AddScoped<IAppointmentQuery, AppointmentQuery>();
builder.Services.AddScoped<IAppointmentMapper, AppointmentMapper>();

// DoctorAvailability
builder.Services.AddScoped<IDoctorAvailabilityMapper, DoctorAvailabilityMapper>();
builder.Services.AddScoped<ICreateDoctorAvailabilityService, CreateDoctorAvailabilityService>();
builder.Services.AddScoped<IUpdateDoctorAvailabilityService, UpdateDoctorAvailabilityService>();
builder.Services.AddScoped<ISearchDoctorAvailabilityService, SearchDoctorAvailabilityService>();
builder.Services.AddScoped<IDoctorAvailabilityCommand, DoctorAvailabilityCommand>();
builder.Services.AddScoped<IDoctorAvailabilityQuery, DoctorAvailabilityQuery>();



// APP CONFIGURATION

var app = builder.Build();

// ========== Aplicar migraciones ==========
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        // Verificar si hay migraciones pendientes antes de aplicar
        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Aplicando {Count} migraciones pendientes...", pendingMigrations.Count);
            dbContext.Database.Migrate();
            logger.LogInformation("Migraciones aplicadas exitosamente");
        }
        else
        {
            logger.LogInformation("No hay migraciones pendientes");
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Error al aplicar migraciones. El servicio continuará iniciando.");
        // No lanzar la excepción para que el servicio pueda iniciar
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
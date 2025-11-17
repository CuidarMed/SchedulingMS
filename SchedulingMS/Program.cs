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
using FluentValidation;
using Infrastructure.Command;
using Infrastructure.Commands;
<<<<<<< HEAD
=======
using Infrastructure.Handlers;
>>>>>>> feature/develop-vol1
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Controllers & Swagger --------------------
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opt =>
    {
        opt.InvalidModelStateResponseFactory = context =>
            new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

<<<<<<< HEAD
// -------------------- FluentValidation --------------------
=======
// FluentValidation: valida DTOs/commands del proyecto Application
>>>>>>> feature/develop-vol1
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Application"));

// -------------------- Database Context --------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// -------------------- CORS --------------------
builder.Services.AddCors(x => x.AddDefaultPolicy(c =>
    c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

// -------------------- JSON Converters --------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
    });

// -------------------- Domain Services --------------------

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
<<<<<<< HEAD

// -------------------- App Configuration --------------------
=======

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

>>>>>>> feature/develop-vol1
var app = builder.Build();

// ========== Aplicar migraciones ==========
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

<<<<<<< HEAD
=======
// Configure the HTTP request pipeline.
>>>>>>> feature/develop-vol1
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

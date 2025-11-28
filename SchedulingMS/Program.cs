using Application.Converters;
using Application.Interfaces.IAppointment;
using Application.Interfaces.IAvailabilityBlock;
using Application.Interfaces.IDoctorAvailability;
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


// -------------------- App Configuration --------------------


// Obtenego la cadena de conexión

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




var app = builder.Build();

// ========== Aplicar migraciones ==========
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}


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
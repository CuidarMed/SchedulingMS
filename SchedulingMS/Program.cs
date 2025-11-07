using Application.Command.DoctorAvailability;
using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Infrastructure.Command;
using Infrastructure.Dependencias;   
using Infrastructure.Persistence;
using Infrastructure.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateDoctorAvailabilityHandler).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(UpdateDoctorAvailabilityHandler).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDoctorAvailabilityHandler).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateDoctorAvailabilityHandler).Assembly);

// MediatR: registra handlers del proyecto Application
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

// FluentValidation: valida DTOs/commands del proyecto Application
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Application"));

// Infrastructure: DbContext + repositorios (lee ConnectionStrings:Default)
builder.Services.AddInfrastructure(builder.Configuration);

// (Opcional) CORS para front local
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// Obtenego la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Si esta parte falla, es la causa del error.
    throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada en appsettings.json.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString) // Pasa la cadena LEÍDA aquí
);

// ========== QUERIES (lectura) - Infrastructure ==========
builder.Services.AddScoped<IAppointmentQuery, AppointmentQuery>();

// ========== COMMANDS (escritura) - Infrastructure ==========
builder.Services.AddScoped<IAppointmentCommand, AppointmentCommand>();

// ========== SERVICES - Doctors ==========
builder.Services.AddScoped<ICreateAppointmentService, CreateAppointmentService>();
builder.Services.AddScoped<ISearchAppointmentService, SearchAppointmentService>();
builder.Services.AddScoped<IUpdateAppointmentService, UpdateAppointmentService>();

// -------------------- App --------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("dev");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

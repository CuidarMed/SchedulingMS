using System.Reflection;
using FluentValidation;
using MediatR;
using Infrastructure.Dependencias;   
using Microsoft.AspNetCore.Mvc;
using Application.Command.DoctorAvailability;

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

// Interfaces 
builder.Services.AddTransient<Application.Interfaces.ICreateAvailabilityBlock, Application.Services.CreateAvailabilityBlock>();
builder.Services.AddTransient<Application.Interfaces.IUpdateAvailabilityBlock, Application.Services.UpdateAvailabilityBlock>();
builder.Services.AddTransient<Application.Interfaces.ISearchAvailabilityBlock, Application.Services.SearchAvailabilityBlock>();


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

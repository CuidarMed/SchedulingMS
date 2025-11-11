using Application.Command.DoctorAvailability;
using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Infrastructure.Command;
using Infrastructure.Dependencias;
using Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;
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

// Interfaces 
builder.Services.AddTransient<IAvailabilityBlockCommand, AvailabilityBlockCommand>();
builder.Services.AddTransient<IAvailabilityBlockQuery, AvailabilityBlockQuery>();
builder.Services.AddTransient<ICreateAvailabilityBlock, CreateAvailabilityBlock>();
builder.Services.AddTransient<IUpdateAvailabilityBlock, UpdateAvailabilityBlock>();
builder.Services.AddTransient<ISearchAvailabilityBlock, SearchAvailabilityBlock>();


builder.Services.AddCors(x => x.AddDefaultPolicy(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

var app = builder.Build();
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
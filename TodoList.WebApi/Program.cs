using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoList.Core.Interfaces;
using TodoList.Infrastructure.Data;
using TodoList.Infrastructure.Repositories;
using TodoList.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure DbContext to use in-memory database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TodoListDb"));

// Register Repositories
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Register Services
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "TodoList API", 
        Version = "v1",
        Description = "An API for managing Tasks."
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoList API V1");
        // Serve Swagger UI at the app's root (e.g. http://localhost:<port>/)
        c.RoutePrefix = string.Empty; 
    });
}

// app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

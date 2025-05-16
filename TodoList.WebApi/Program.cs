using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TodoList.Core.Interfaces; // Contains ITaskRepository, ITaskService, ITodoItemRepository, ITodoItemService
using TodoList.Infrastructure.Data; // Contains AppDbContext
using TodoList.Infrastructure.Repositories; // Contains TaskRepository, TodoItemRepository
using TodoList.Core.Services; // Contains TaskService, TodoItemService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure DbContext to use in-memory database
// For TodoItemRepository and TaskRepository
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
        Description = "An API for managing Todo items and Tasks."
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

// app.UseHttpsRedirection(); // HTTPS is good practice for production

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

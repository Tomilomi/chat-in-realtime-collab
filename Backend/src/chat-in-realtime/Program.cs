using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using chat_in_realtime.Hubs;
using Domain.Entities;
using chat_in_realtime.Handlers;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options
    => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// dependency injection
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPictureRepository, PictureRepository>();

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

// SignalR
//ruta del chat
app.MapHub<ChatHub>("/chathub");

// Reiniciar la base de datos al levantar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    // SEEDER PARA TESTEAR

    var picture1 = new Picture( [], Guid.Empty);
    var picture2 = new Picture( [], Guid.Empty);

    var user1 = User.Create("Marcos", "password123", picture1).Value;
    var user2 = User.Create("Laura", "password123", picture2).Value;

    db.Users.AddRange(user1, user2);  // las Pictures se guardan solas

    db.Messages.AddRange(
        new Message(user1, "Hola! alguien probó el nuevo update?"),
        new Message(user2, "Sí, está bastante bien!"),
        new Message(user1, "Me re gustó la nueva sidebar")
    );

    db.SaveChanges();
}

app.Run();
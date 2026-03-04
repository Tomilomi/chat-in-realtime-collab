using Infrastructure.Data;
using chat_in_realtime.Hubs;
using Domain.Entities;
using Application.Extensions;
using Infrastructure.Extensions;
using chat_in_realtime.Extensions;
using Domain.Enums;


var builder = WebApplication.CreateBuilder(args);

//add layer services
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseCors();

// JWT

app.UseAuthentication();
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

    var picture1 = new Picture([], Guid.Empty);
    var picture2 = new Picture([], Guid.Empty);

    var user1 = User.Create("Marcos", BCrypt.Net.BCrypt.HashPassword("password123"), picture1).Value;
    user1.ChangeRole(UserRole.Admin);
    var user2 = User.Create("Laura", BCrypt.Net.BCrypt.HashPassword("password123"), picture2).Value;

    db.Users.AddRange(user1, user2);  // las Pictures se guardan solas

    var messages = new List<Message>();
    for (int i = 1; i <= 50; i++)
    {
        var sender = i % 2 == 0 ? user1 : user2;
        var msg = new Message(sender, $"Mensaje de prueba número {i}");
        msg.ForceTimestamp(DateTime.UtcNow.AddMinutes(-50 + i));
        messages.Add(msg);
    }

    db.Messages.AddRange(messages);
    db.SaveChanges();
}

app.Run();
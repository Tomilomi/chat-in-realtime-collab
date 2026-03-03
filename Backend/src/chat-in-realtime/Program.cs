using Infrastructure.Data;
using chat_in_realtime.Hubs;
using Domain.Entities;
using Application.Extensions;
using Infrastructure.Extensions;
using chat_in_realtime.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
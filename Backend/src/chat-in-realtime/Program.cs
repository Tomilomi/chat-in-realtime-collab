using chat_in_realtime.Hubs;
using Application.Extensions;
using Infrastructure.Extensions;
using chat_in_realtime.Extensions;

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
app.UseStaticFiles();
app.UseCors();

// JWT

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR
//ruta del chat
app.MapHub<ChatHub>("/chathub");

// Reiniciar la base de datos al levantar
// seeder
Seeder.Initialize(app);

app.Run();
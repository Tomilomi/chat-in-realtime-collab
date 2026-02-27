using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace chat_in_realtime.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;

    public ChatHub(IMessageService messageService, IUserService userService)
    {
        _messageService = messageService;
        _userService = userService;
    }

    public async Task SendMessage(SendMessageDTO messageIn)
    {
        //identificar el usuario
        // NECESITA JWT
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        Guid userId = Guid.Parse(userIdString);

        //buscarlo en la bd
        //se puede mejorar el manejo de erorres
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new HubException("Usuario no encontrado.");
        }

        //entidad para la bd
        var newMessage = new Message(
            id: Guid.NewGuid(),
            sender: user,
            content: messageIn.Content,
            timestamp: DateTime.UtcNow
            );

        //guardar en bd
        await _messageService.SaveMessageAsync(newMessage);

        //dto para el hub
        var messageToBroadcast = new MessageReceivedDTO(
            Id: newMessage.Id,
            Content: newMessage.Content,
            Timestamp: newMessage.Timestamp,
            Sender: new UserSenderDTO
            (
                Id: user.Id,
                Username: user.Username
            )

        );
        //mensaje enviado al hub
        await Clients.All.SendAsync("ReceiveMessage", messageToBroadcast);
    }

    public async Task LoadMessages()
    {
        throw new NotImplementedException();
    }
}
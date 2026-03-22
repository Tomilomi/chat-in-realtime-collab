<div align="center">

# 💬 Chat in Real Time

A full-featured real-time chat backend with authentication, moderation tools, and live events.

[![.NET](https://img.shields.io/badge/.NET_8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet)
[![SignalR](https://img.shields.io/badge/SignalR-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Entity Framework](https://img.shields.io/badge/Entity_Framework_Core_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/ef/core/)
[![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)](https://jwt.io/)
[![BCrypt](https://img.shields.io/badge/BCrypt-00599C?style=for-the-badge&logo=letsencrypt&logoColor=white)](https://github.com/BcryptNet/bcrypt.net)

</div>

---

## ✨ Features

- 🔐 **JWT Authentication** — Secure login and registration with BCrypt password hashing
- ⚡ **Real-time messaging** via SignalR WebSockets
- 📜 **Paginated message history** — Load previous messages on scroll
- 🟢 **Connected users indicator** — See who's online in real time
- ✍️ **Typing indicator** — Know when someone is typing
- 🔔 **Join/leave notifications** — Get notified when users connect or disconnect
- 🛡️ **Role-based access control** — User, Moderator, and Admin roles
- 🚫 **Ban system** — Ban users with instant real-time kick
- 👢 **Kick system** — Moderators can kick users from the chat in real time
- 🗑️ **Message moderation** — Delete messages with real-time removal for all clients
- 🎭 **Avatar system** — Choose from predefined profile pictures served as static files
- 🔄 **Live profile updates** — Username and avatar changes reflect instantly in the chat
- 📋 **Clean Architecture** — Domain, Application, Infrastructure, and Presentation layers

---

## 🏗️ Architecture

This project follows **Clean Architecture** principles, separating concerns into four distinct layers:

```
Backend/src/
├── Domain/               # Enterprise business rules
│   ├── Entities/         # User, Message, Picture
│   ├── Enums/            # UserRole
│   └── Errors/           # DomainErrors
├── Application/          # Application business rules
│   ├── Interfaces/       # Repository and service contracts
│   ├── Services/         # Business logic
│   └── Common/           # DTOs
├── Infrastructure/       # Frameworks & drivers
│   ├── Data/             # AppDbContext (EF Core)
│   └── Repositories/     # Data access implementations
└── chat-in-realtime/     # Interface adapters
    ├── Controllers/      # HTTP endpoints
    ├── Hubs/             # ChatHub (SignalR)
    ├── Notifications/    # ChatNotificationService
    └── Extensions/       # DI, JWT, Swagger config
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### Database Setup (Fedora/Linux)

```bash
sudo dnf install postgresql postgresql-server
sudo postgresql-setup --initdb
sudo systemctl start postgresql && sudo systemctl enable postgresql

# Set password
sudo -u postgres psql
ALTER USER postgres PASSWORD 'yourpassword';
\q

# Edit pg_hba.conf: change 'ident' to 'md5' for localhost
sudo nano /var/lib/pgsql/data/pg_hba.conf
sudo systemctl restart postgresql
```

### Configuration

Edit `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=chat_db;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "chat-in-realtime",
    "Audience": "chat-in-realtime"
  }
}
```

### Run

```bash
cd Backend/src/chat-in-realtime
dotnet run
```

- API: `http://localhost:5135`
- Swagger UI: `http://localhost:5135/swagger`

> ⚠️ The database is dropped and re-seeded on every server start (development only).

---

## 🔒 Authentication

All protected endpoints require a JWT Bearer token:

```
Authorization: Bearer {token}
```

Obtain the token via `POST /api/auth/login`. Tokens expire after **8 hours**.

For SignalR connections, the token is passed as a query string parameter (handled automatically by the SignalR client library).

---

## 📡 API Reference

### Auth

| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Login and receive JWT |

### Users

| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/user/me` | Authenticated | Get own profile (id, username, role, pictureUrl) |
| GET | `/api/user/profiles` | Authenticated | Get all users' username and picture |
| GET | `/api/user` | Admin | Get all users with full data |
| PATCH | `/api/user/me` | Authenticated | Update own profile |
| PATCH | `/api/user/{id}` | Admin | Update any user's profile |
| POST | `/api/user/{id}/ban` | Admin | Ban a user (kicks if connected) |
| POST | `/api/user/{id}/unban` | Admin | Unban a user |
| POST | `/api/user/{id}/role` | Admin | Change a user's role |

### Messages

| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| DELETE | `/api/message/{id}` | Admin, Moderator | Delete a message |

### Pictures

| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/picture` | Public | Get all available avatars |

---

## ⚡ SignalR Hub — `/chathub`

### Client → Server

| Method | Access | Description |
|--------|--------|-------------|
| `SendMessage({ content })` | Authenticated | Send a message |
| `LoadMessages(page)` | Authenticated | Load paginated history (20/page) |
| `StartTyping()` | Authenticated | Notify others you're typing |
| `StopTyping()` | Authenticated | Notify others you stopped typing |
| `KickUser(userId)` | Admin, Moderator | Kick a connected user |

### Server → Client Events

| Event | Emitted To | Payload | Description |
|-------|-----------|---------|-------------|
| `ReceiveMessage` | All | `MessageReceivedDTO` | New message broadcast |
| `LoadMessages` | Caller | `MessageReceivedDTO[], page` | Paginated history |
| `MessageDeleted` | All | `messageId: guid` | A message was deleted |
| `UserConnected` | All | `username: string` | User joined |
| `UserDisconnected` | All | `username: string` | User left |
| `UpdateConnectedUsers` | All | `string[]` | Updated online list |
| `UserTyping` | Others | `username: string` | Someone is typing |
| `UserStoppedTyping` | Others | `username: string` | Someone stopped typing |
| `UserUpdated` | All | `{ userId, username?, pictureUrl? }` | Profile updated |
| `Kicked` | Target only | `reason: string` | User was kicked or banned |

---

## 🗂️ DTOs

```csharp
record RegisterRequestDTO(string Username, string Password);
record LoginRequestDTO(string Username, string Password);
record SendMessageDTO(string Content);
record MessageReceivedDTO(Guid Id, string Content, DateTime Timestamp, UserSenderDTO Sender);
record UserSenderDTO(Guid Id, string Username, string? PictureUrl);
record UserProfileDTO(string Username, string? PictureUrl);
record PictureDTO(Guid Id, string Url);
record UserUpdateRequestDTO(string? Username, string? Password, Guid? PictureId);
record ChangeRoleRequestDTO(UserRole Role);
```

---

## 📐 Domain Model

### UserRole

| Value | Int | Permissions |
|-------|-----|-------------|
| User | 0 | Send messages |
| Moderator | 1 | Delete messages, kick users |
| Admin | 2 | Full access: ban, unban, kick, delete messages, change roles |

---

## 📋 Business Rules

- Passwords are hashed with **BCrypt** on register and on update
- Usernames must be **unique** (validated at service level)
- Messages cannot be **empty or whitespace**
- **Rate limiting**: max 1 message per second per user
- Banned users **cannot connect** to the Hub
- Banning a connected user **kicks them in real time**
- Deleting a message **broadcasts** a `MessageDeleted` event to all clients
- Updating a profile **broadcasts** a `UserUpdated` event to all clients
- User IDs are **not exposed** to regular users — only Admins can see them
- Avatars are **shared** resources, not owned by any specific user

---

## 🔮 Roadmap

- [ ] Proper error responses for register/login
- [ ] Refresh token support
- [ ] Multiple chat rooms
- [ ] Private messages
- [ ] Message reactions

## 👥 Contributors

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/Tomilomi">
        <img src="https://github.com/Tomilomi.png" width="80px" alt="Tomilomi"/>
        <br />
        <sub><b>Tomilomi</b></sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/Mudo0">
        <img src="https://github.com/Mudo0.png" width="80px" alt="Mudo0"/>
        <br />
        <sub><b>Mudo0</b></sub>
      </a>
    </td>
  </tr>
</table>
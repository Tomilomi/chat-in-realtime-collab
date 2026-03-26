FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["chat-in-realtime/chat-in-realtime.csproj", "chat-in-realtime/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "chat-in-realtime/chat-in-realtime.csproj"

COPY . .

WORKDIR "/src/chat-in-realtime"
RUN dotnet build "chat-in-realtime.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "chat-in-realtime.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "chat-in-realtime.dll"]

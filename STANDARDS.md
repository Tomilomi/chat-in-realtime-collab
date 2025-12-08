# Guía de Estándares de Desarrollo .NET

**Proyecto:** Chat Realtime Collab  
**Versión:** 1.3

## 1. Introducción
Este documento unifica los criterios de desarrollo del equipo. Estas reglas son obligatorias para mantener la consistencia en el Backend.

---

## 2. Arquitectura y Estructura del Proyecto (Prioridad Alta)
Seguimos estrictamente los principios de **Clean Architecture**. Las dependencias fluyen hacia adentro; el Dominio no debe conocer nada del mundo exterior.

### 2.1 Organización de Capas
* **Domain:** El núcleo. Contiene Entidades y Excepciones de dominio. **No tiene dependencias**.
* **Application:** Casos de uso, Interfaces (Abstracciones), DTOs y Validaciones. Aquí vive la lógica de negocio.
* **Infrastructure:** Implementación de interfaces (Repositorios), Base de Datos (EF Core), Servicios externos (Email, Nube).
* **Api:** Punto de entrada. Contiene Controladores (`Controllers`) y configuración de inyección de dependencias (`Program.cs`).

---

## 3. Nomenclatura General (Prioridad Media)

### 3.1 Idioma
Todo el código debe escribirse estrictamente en **Inglés**.
* **Ejemplo:** `UserMessage` (Bien) vs `MensajeUsuario` (Mal).

### 3.2 Clases e Interfaces
* **PascalCase** (Primera letra mayúscula).
* Interfaces llevan prefijo `I`.
* **Ej:** `public class ChatService`, `public interface IChatRepository`.

### 3.3 Propiedades Públicas
* **PascalCase**. Todas las propiedades accesibles desde fuera inician con mayúscula.
```csharp
public string EmailAddress { get; set; } // Correcto
public string emailAddress { get; set; } // Incorrecto
```

### 3.4 Campos Privados (Private Fields)
* **_camelCase**. Inician con guion bajo `_` seguido de minúscula. Vital para inyección de dependencias.
```csharp
private readonly ILogger _logger;
```

### 3.5 Métodos
* **PascalCase** y verbos en inglés.
* **Asincronía:** Si es `async` o retorna `Task`, **debe** terminar en `Async`.
```csharp
public async Task<User> GetByIdAsync(int id) { ... }
```

---

## 4. Patrones de Diseño: Manejo de Errores (Result Pattern)
Utilizamos la librería **ErrorOr** para evitar excepciones y nulos.

### 4.1 Regla de Oro en Servicios (Application)
Los servicios **nunca** lanzan excepciones controladas. Siempre retornan `ErrorOr<T>`.

```csharp
public async Task<ErrorOr<User>> CreateUserAsync(UserDto dto)
{
    // Validación
    if (string.IsNullOrEmpty(dto.Email))
    {
        return Error.Validation("User.EmailRequired", "El email es obligatorio.");
    }

    // Lógica
    if (await _repo.ExistsAsync(dto.Email))
    {
        return Error.Conflict("User.Duplicate", "El usuario ya existe.");
    }

    // Éxito (Retorno implícito)
    return new User(dto.Name, dto.Email);
}
```

### 4.2 Tipos de Errores Permitidos
* `Error.NotFound` (404)
* `Error.Validation` (400)
* `Error.Conflict` (409)
* `Error.Unexpected` (500)

### 4.3 Consumo en Controladores (Api)
Los controladores no deciden la lógica, solo mapean la respuesta usando `.Match`.

```csharp
[HttpPost]
public async Task<IActionResult> Create(UserDto request)
{
    ErrorOr<User> result = await _service.CreateUserAsync(request);

    return result.Match(
        value => Ok(value),            // 200 OK
        errors => Problem(errors)      // Mapeo automático de errores
    );
}
```

---

## 5. Otras Buenas Prácticas de Sintaxis

### 5.1 Uso de "var"
Usar `var` solo cuando el tipo es evidente a simple vista.
```csharp
var users = new List<User>();    //  Bien
var data = await GetDataAsync(); //  Mal (¿Qué retorna data?)
```

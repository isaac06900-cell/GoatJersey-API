# Goat Jersey · API

API REST del **marketplace y colección de jerseys de fútbol**. Maneja el registro y login de usuarios (JWT) y la colección de jerseys de cada coleccionista.

Proyecto final · **Equipo Goat** · Grupo J-301 · Universidad Autónoma de Occidente (UAdeO).

Este repositorio es el **backend**. La app móvil está en [GoatJersey-App](https://github.com/isaac06900-cell/GoatJersey-App).

## Integrantes

| Nombre | GitHub |
|---|---|
| Isaac Jasiel Herrera Avila | [@isaac06900-cell](https://github.com/isaac06900-cell) |
| Andrew | [@Andrewclz](https://github.com/Andrewclz) |
| Alan | _(por agregar)_ |

## Tecnologías

ASP.NET Core (.NET 10) · Entity Framework Core · SQL Server 2022 · JWT (HS256, expira en 2 horas) · BCrypt · Swagger

## Endpoints

| Método | Ruta | Requiere token | Descripción |
|---|---|---|---|
| POST | `/api/auth/registro` | No | Crea una cuenta (`nombre`, `correo`, `password`) |
| POST | `/api/auth/login` | No | Devuelve el token JWT (`correo`, `password`) |
| GET | `/api/jerseys` | Sí | Lista mi colección |
| POST | `/api/jerseys` | Sí | Agrega un jersey |
| PUT | `/api/jerseys/{id}` | Sí | Edita un jersey |
| DELETE | `/api/jerseys/{id}` | Sí | Elimina un jersey |

Los errores regresan como `{ "error": "mensaje" }`. Roles: `Coleccionista` y `Admin`. El módulo de publicaciones está en desarrollo.

## Cómo correrlo

### 1. Base de datos (SQL Server en Docker)

```bash
docker run --platform linux/amd64 --name sqlserver-goat \
  -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<TU_PASSWORD_SA>" \
  -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

docker cp Database/01_crear_bd.sql sqlserver-goat:/tmp/01_crear_bd.sql
docker exec -u 0 sqlserver-goat chmod 644 /tmp/01_crear_bd.sql
docker exec sqlserver-goat /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "<TU_PASSWORD_SA>" -C -I -i /tmp/01_crear_bd.sql
```

`--platform linux/amd64` es necesario en Macs con Apple Silicon. El script crea la base `GoatJerseyDB` con las tablas `Usuarios`, `Jerseys` y `Publicaciones`.

### 2. Secretos (no se guardan en el repositorio)

```bash
cd GoatJersey_API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=GoatJerseyDB;User Id=sa;Password=<TU_PASSWORD_SA>;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Secret" "<cadena larga aleatoria, mínimo 32 caracteres>"
```

### 3. Arrancar la API

```bash
dotnet run --launch-profile http
```

Escucha en `http://0.0.0.0:5300`, accesible desde el teléfono por la IP de la computadora. La documentación interactiva está en `http://localhost:5300/swagger` (botón **Authorize** para pegar el token).

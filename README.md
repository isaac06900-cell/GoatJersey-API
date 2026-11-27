# Goat Jersey · API

API REST del **marketplace y colección de jerseys de fútbol**. Maneja el registro y login de usuarios (JWT), la colección de jerseys de cada coleccionista y la publicación de jerseys para venta o intercambio.

Proyecto final · **Equipo Goat** · Grupo J-301 · Programación de Dispositivos Móviles · Universidad Autónoma de Occidente (UAdeO), Unidad Regional Culiacán.

Este repositorio es el **backend**. La app móvil está en [GoatJersey-App](https://github.com/isaac06900-cell/GoatJersey-App).

## Integrantes

| Nombre | GitHub |
|---|---|
| Isaac Jasiel Herrera Ávila | [@isaac06900-cell](https://github.com/isaac06900-cell) |
| Andrew Neftalí Chaidez López | [@Andrewclz](https://github.com/Andrewclz) |
| Alan Jesús Aguilar Verdugo | [@alanjesusaguilarverdugo-lang](https://github.com/alanjesusaguilarverdugo-lang) |

## Tecnologías

ASP.NET Core (.NET 10) · Entity Framework Core · SQL Server 2022 · JWT (HS256, expira en 2 horas) · BCrypt · Swagger

## Endpoints

Todos los endpoints, salvo registro y login, exigen el encabezado `Authorization: Bearer <token>` y responden 401 sin él.

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/registro` | Crea una cuenta (`nombre`, `correo`, `password`) |
| POST | `/api/auth/login` | Devuelve el token JWT (`correo`, `password`) |
| GET | `/api/jerseys` | Lista mi colección |
| POST | `/api/jerseys` | Agrega un jersey a mi colección |
| PUT | `/api/jerseys/{id}` | Edita un jersey mío |
| DELETE | `/api/jerseys/{id}` | Elimina un jersey mío (sin publicación activa) |
| GET | `/api/publicaciones` | Explora las publicaciones activas de otros usuarios. Filtros opcionales por query: `equipo`, `temporada`, `talla`, `tipo`, `version` |
| GET | `/api/publicaciones/{id}` | Detalle de una publicación, con el jersey y el vendedor |
| POST | `/api/publicaciones` | Publica un jersey mío (`idJersey`, `tipo`, `precio`, `descripcion`) |
| PATCH | `/api/publicaciones/{id}/retirar` | Retira una publicación (su dueño o un Admin) |

Los errores regresan como `{ "error": "mensaje" }`, y los de validación del modelo como el detalle de cada campo. Códigos usados: 200, 201, 204, 400, 401, 403 y 404.

## Reglas de negocio

- Los usuarios nuevos siempre se registran con el rol `Coleccionista`; el rol `Admin` se asigna directamente en la base de datos.
- Un jersey solo puede tener **una publicación activa** a la vez.
- No se puede eliminar un jersey con una publicación activa: primero hay que retirarla.
- Solo el dueño puede editar, eliminar o publicar sus jerseys (si no, 403).
- El `tipo` de una publicación es `Venta`, `Intercambio` o `Ambos`; el `precio` es obligatorio y mayor que 0 cuando hay venta.
- Al publicar, el jersey pasa a `Publicado`; al retirar la publicación, regresa a `EnColeccion`.
- Una publicación la puede retirar su dueño o un usuario con rol `Admin` (moderación).

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

#### Alternativa en Windows (SQL Server Express, sin Docker)

Abre SSMS, conéctate a `.\SQLEXPRESS` y ejecuta completo `Database/01_crear_bd.sql`. Después, en vez de la cadena de Docker, usa esta:

```powershell
cd GoatJersey_API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=GoatJerseyDB;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet user-secrets set "Jwt:Secret" "<cadena larga aleatoria, mínimo 32 caracteres>"
```

Si el teléfono no conecta con la API, abre el puerto 5300 en el firewall (PowerShell como administrador):

```powershell
New-NetFirewallRule -DisplayName "GoatJersey API 5300" -Direction Inbound -Protocol TCP -LocalPort 5300 -Action Allow
```

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

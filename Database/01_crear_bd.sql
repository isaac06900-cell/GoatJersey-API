-- Goat Jersey · Base de datos GoatJerseyDB
IF DB_ID('GoatJerseyDB') IS NULL
    CREATE DATABASE GoatJerseyDB;
GO

USE GoatJerseyDB;
GO

IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
CREATE TABLE dbo.Usuarios (
    IdUsuario     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
    Nombre        NVARCHAR(100) NOT NULL,
    Correo        NVARCHAR(150) NOT NULL CONSTRAINT UQ_Usuarios_Correo UNIQUE,
    PasswordHash  NVARCHAR(200) NOT NULL,
    Rol           NVARCHAR(20)  NOT NULL CONSTRAINT DF_Usuarios_Rol DEFAULT 'Coleccionista',
    FechaRegistro DATETIME2     NOT NULL CONSTRAINT DF_Usuarios_Fecha DEFAULT SYSDATETIME(),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Coleccionista', 'Admin'))
);
GO

IF OBJECT_ID('dbo.Jerseys', 'U') IS NULL
CREATE TABLE dbo.Jerseys (
    IdJersey       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Jerseys PRIMARY KEY,
    IdUsuario      INT NOT NULL,
    Equipo         NVARCHAR(100) NOT NULL,
    Temporada      NVARCHAR(9)   NOT NULL,
    Tipo           NVARCHAR(15)  NOT NULL,
    Talla          NVARCHAR(5)   NOT NULL,
    Estado         NVARCHAR(15)  NOT NULL,
    Version        NVARCHAR(10)  NOT NULL,
    ValorEstimado  DECIMAL(10,2) NULL,
    FotoUrl        NVARCHAR(300) NULL,

    Disponibilidad NVARCHAR(15)  NOT NULL CONSTRAINT DF_Jerseys_Disp DEFAULT 'EnColeccion',
    FechaAlta      DATETIME2     NOT NULL CONSTRAINT DF_Jerseys_Fecha DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Jerseys_Usuarios FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios(IdUsuario),
    CONSTRAINT CK_Jerseys_Tipo   CHECK (Tipo   IN ('Local','Visitante','Tercero','Portero','Retro','Especial')),
    CONSTRAINT CK_Jerseys_Talla  CHECK (Talla  IN ('XS','S','M','L','XL','XXL')),
    CONSTRAINT CK_Jerseys_Estado CHECK (Estado IN ('Nuevo','Excelente','Bueno','Regular')),
    CONSTRAINT CK_Jerseys_Version CHECK (Version IN ('Original','Replica')),
    CONSTRAINT CK_Jerseys_Valor  CHECK (ValorEstimado IS NULL OR ValorEstimado >= 0),
    CONSTRAINT CK_Jerseys_Disp   CHECK (Disponibilidad IN ('EnColeccion','Publicado'))
);
GO

IF OBJECT_ID('dbo.Publicaciones', 'U') IS NULL
CREATE TABLE dbo.Publicaciones (
    IdPublicacion    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Publicaciones PRIMARY KEY,
    IdJersey         INT NOT NULL,
    Tipo             NVARCHAR(15)  NOT NULL,
    Precio           DECIMAL(10,2) NULL,
    Descripcion      NVARCHAR(500) NULL,
    Estatus          NVARCHAR(15)  NOT NULL CONSTRAINT DF_Publicaciones_Estatus DEFAULT 'Activa',
    FechaPublicacion DATETIME2     NOT NULL CONSTRAINT DF_Publicaciones_Fecha DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Publicaciones_Jerseys FOREIGN KEY (IdJersey) REFERENCES dbo.Jerseys(IdJersey) ON DELETE CASCADE,
    CONSTRAINT CK_Publicaciones_Tipo    CHECK (Tipo IN ('Venta','Intercambio','Ambos')),
    CONSTRAINT CK_Publicaciones_Estatus CHECK (Estatus IN ('Activa','Retirada')),
    CONSTRAINT CK_Publicaciones_Precio  CHECK (Tipo = 'Intercambio' OR (Precio IS NOT NULL AND Precio > 0))
);
GO

-- Solo una publicación Activa por jersey
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Publicaciones_JerseyActiva')
CREATE UNIQUE INDEX UX_Publicaciones_JerseyActiva
    ON dbo.Publicaciones(IdJersey) WHERE Estatus = 'Activa';

GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Jerseys_IdUsuario')
CREATE INDEX IX_Jerseys_IdUsuario ON dbo.Jerseys(IdUsuario);
GO

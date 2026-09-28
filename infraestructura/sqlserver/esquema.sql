SET NOCOUNT ON;
GO

IF OBJECT_ID(N'dbo.Ubicacion', N'U') IS NULL
CREATE TABLE dbo.Ubicacion (
    UbicacionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Ubicacion PRIMARY KEY,
    Longitud DECIMAL(19, 13) NULL,
    Latitud DECIMAL(19, 13) NULL,
    Ciudad VARCHAR(100) NULL,
    Estado VARCHAR(100) NULL,
    Pais VARCHAR(100) NULL
);
GO

IF OBJECT_ID(N'dbo.Acceso', N'U') IS NULL
CREATE TABLE dbo.Acceso (
    AccesoID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Acceso PRIMARY KEY,
    Correo VARCHAR(100) NOT NULL,
    ContrasenaHash VARCHAR(255) NOT NULL,
    EsAdmin BIT NOT NULL CONSTRAINT DF_Acceso_EsAdmin DEFAULT 0
);
GO

IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
CREATE TABLE dbo.Usuario (
    UsuarioID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    FechaRegistro DATETIME NULL CONSTRAINT DF_Usuario_FechaRegistro DEFAULT GETDATE(),
    Telefono VARCHAR(15) NULL,
    UbicacionID INT NULL,
    AccesoID INT NOT NULL
);
GO

IF OBJECT_ID(N'dbo.Mascota', N'U') IS NULL
CREATE TABLE dbo.Mascota (
    MascotaID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Mascota PRIMARY KEY,
    Nombre VARCHAR(45) NOT NULL,
    Especie VARCHAR(50) NOT NULL,
    Raza VARCHAR(100) NOT NULL,
    Edad VARCHAR(100) NOT NULL,
    Sexo VARCHAR(10) NOT NULL,
    Tamaño VARCHAR(10) NOT NULL,
    Descripcion VARCHAR(MAX) NULL
);
GO

IF OBJECT_ID(N'dbo.Adopcion', N'U') IS NULL
CREATE TABLE dbo.Adopcion (
    AdopcionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Adopcion PRIMARY KEY,
    FechaSolicitud DATETIME NULL CONSTRAINT DF_Adopcion_FechaSolicitud DEFAULT GETDATE(),
    Estado BIT NOT NULL CONSTRAINT DF_Adopcion_Estado DEFAULT 0,
    MascotaID INT NOT NULL,
    PublicadorID INT NOT NULL,
    UbicacionID INT NOT NULL
);
GO

IF OBJECT_ID(N'dbo.FotoUsuario', N'U') IS NULL
CREATE TABLE dbo.FotoUsuario (
    FotoUsuarioID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FotoUsuario PRIMARY KEY,
    UsuarioID INT NOT NULL CONSTRAINT UQ_FotoUsuario_Usuario UNIQUE,
    UrlFoto VARCHAR(255) NOT NULL
);
GO

IF OBJECT_ID(N'dbo.FotoMascota', N'U') IS NULL
CREATE TABLE dbo.FotoMascota (
    FotoID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FotoMascota PRIMARY KEY,
    MascotaID INT NOT NULL,
    UrlFoto VARCHAR(255) NOT NULL
);
GO

IF OBJECT_ID(N'dbo.VideoMascota', N'U') IS NULL
CREATE TABLE dbo.VideoMascota (
    VideoID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_VideoMascota PRIMARY KEY,
    MascotaID INT NOT NULL,
    UrlVideo VARCHAR(255) NOT NULL
);
GO

IF OBJECT_ID(N'dbo.Solicitud', N'U') IS NULL
CREATE TABLE dbo.Solicitud (
    SolicitudID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Solicitud PRIMARY KEY,
    AdoptanteID INT NOT NULL,
    AdopcionID INT NOT NULL
);
GO

IF OBJECT_ID(N'dbo.Chat', N'U') IS NULL
CREATE TABLE dbo.Chat (
    ChatID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Chat PRIMARY KEY,
    RemitenteID INT NOT NULL,
    DestinatarioID INT NOT NULL,
    Contenido NVARCHAR(MAX) NOT NULL,
    FechaEnvio DATETIME NOT NULL CONSTRAINT DF_Chat_FechaEnvio DEFAULT GETDATE()
);
GO

IF OBJECT_ID(N'dbo.Notificacion', N'U') IS NULL
CREATE TABLE dbo.Notificacion (
    NotificacionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Notificacion PRIMARY KEY,
    UsuarioID INT NOT NULL,
    Titulo VARCHAR(100) NOT NULL,
    Mensaje VARCHAR(MAX) NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Notificacion_FechaCreacion DEFAULT GETDATE(),
    Leida BIT NOT NULL CONSTRAINT DF_Notificacion_Leida DEFAULT 0,
    ReferenciaID INT NULL,
    ReferenciaTipo VARCHAR(50) NULL
);
GO

IF OBJECT_ID(N'dbo.FK_Usuario_Acceso', N'F') IS NULL
ALTER TABLE dbo.Usuario ADD CONSTRAINT FK_Usuario_Acceso
    FOREIGN KEY (AccesoID) REFERENCES dbo.Acceso (AccesoID);
GO

IF OBJECT_ID(N'dbo.FK_Usuario_Ubicacion', N'F') IS NULL
ALTER TABLE dbo.Usuario ADD CONSTRAINT FK_Usuario_Ubicacion
    FOREIGN KEY (UbicacionID) REFERENCES dbo.Ubicacion (UbicacionID) ON DELETE SET NULL;
GO

IF OBJECT_ID(N'dbo.FK_SolicitudAdopcion_Mascota', N'F') IS NULL
ALTER TABLE dbo.Adopcion ADD CONSTRAINT FK_SolicitudAdopcion_Mascota
    FOREIGN KEY (MascotaID) REFERENCES dbo.Mascota (MascotaID);
GO

IF OBJECT_ID(N'dbo.FK_SolicitudAdopcion_Publicador', N'F') IS NULL
ALTER TABLE dbo.Adopcion ADD CONSTRAINT FK_SolicitudAdopcion_Publicador
    FOREIGN KEY (PublicadorID) REFERENCES dbo.Usuario (UsuarioID);
GO

IF OBJECT_ID(N'dbo.FK_SolicitudAdopcion_Ubicacion', N'F') IS NULL
ALTER TABLE dbo.Adopcion ADD CONSTRAINT FK_SolicitudAdopcion_Ubicacion
    FOREIGN KEY (UbicacionID) REFERENCES dbo.Ubicacion (UbicacionID);
GO

IF OBJECT_ID(N'dbo.FK_FotoUsuario_Usuario', N'F') IS NULL
ALTER TABLE dbo.FotoUsuario ADD CONSTRAINT FK_FotoUsuario_Usuario
    FOREIGN KEY (UsuarioID) REFERENCES dbo.Usuario (UsuarioID) ON DELETE CASCADE;
GO

IF OBJECT_ID(N'dbo.FK_FotoMascota_Mascota', N'F') IS NULL
ALTER TABLE dbo.FotoMascota ADD CONSTRAINT FK_FotoMascota_Mascota
    FOREIGN KEY (MascotaID) REFERENCES dbo.Mascota (MascotaID) ON DELETE CASCADE;
GO

IF OBJECT_ID(N'dbo.FK_VideoMascota_Mascota', N'F') IS NULL
ALTER TABLE dbo.VideoMascota ADD CONSTRAINT FK_VideoMascota_Mascota
    FOREIGN KEY (MascotaID) REFERENCES dbo.Mascota (MascotaID) ON DELETE CASCADE;
GO

IF OBJECT_ID(N'dbo.FK_Solicitud_Adoptante', N'F') IS NULL
ALTER TABLE dbo.Solicitud ADD CONSTRAINT FK_Solicitud_Adoptante
    FOREIGN KEY (AdoptanteID) REFERENCES dbo.Usuario (UsuarioID);
GO

IF OBJECT_ID(N'dbo.FK_Solicitud_Adopcion', N'F') IS NULL
ALTER TABLE dbo.Solicitud ADD CONSTRAINT FK_Solicitud_Adopcion
    FOREIGN KEY (AdopcionID) REFERENCES dbo.Adopcion (AdopcionID) ON DELETE CASCADE;
GO

IF OBJECT_ID(N'dbo.FK_Chat_Usuario_Remitente', N'F') IS NULL
ALTER TABLE dbo.Chat ADD CONSTRAINT FK_Chat_Usuario_Remitente
    FOREIGN KEY (RemitenteID) REFERENCES dbo.Usuario (UsuarioID);
GO

IF OBJECT_ID(N'dbo.FK_Chat_Usuario_Destinatario', N'F') IS NULL
ALTER TABLE dbo.Chat ADD CONSTRAINT FK_Chat_Usuario_Destinatario
    FOREIGN KEY (DestinatarioID) REFERENCES dbo.Usuario (UsuarioID);
GO

IF OBJECT_ID(N'dbo.FK_Notificacion_Usuario', N'F') IS NULL
ALTER TABLE dbo.Notificacion ADD CONSTRAINT FK_Notificacion_Usuario
    FOREIGN KEY (UsuarioID) REFERENCES dbo.Usuario (UsuarioID) ON DELETE CASCADE;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Acceso_Correo')
CREATE UNIQUE INDEX UX_Acceso_Correo ON dbo.Acceso (Correo);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Solicitud_Adoptante_Adopcion')
CREATE UNIQUE INDEX UX_Solicitud_Adoptante_Adopcion ON dbo.Solicitud (AdoptanteID, AdopcionID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Adopcion_Publicador')
CREATE INDEX IX_Adopcion_Publicador ON dbo.Adopcion (PublicadorID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Chat_Participantes')
CREATE INDEX IX_Chat_Participantes ON dbo.Chat (RemitenteID, DestinatarioID, FechaEnvio);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Notificacion_Usuario')
CREATE INDEX IX_Notificacion_Usuario ON dbo.Notificacion (UsuarioID, FechaCreacion DESC);
GO

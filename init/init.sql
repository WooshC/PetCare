USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BDD_PETCARE')
BEGIN
    CREATE DATABASE BDD_PETCARE;
END
GO

USE BDD_PETCARE;
GO

CREATE TABLE Usuarios (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    ContrasenaHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    NombreCompleto VARCHAR(100) NOT NULL,
    Telefono VARCHAR(15) NOT NULL,
    Direccion VARCHAR(200),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    UltimoAcceso DATETIME,
    Activo BIT DEFAULT 1,
    Verificado BIT DEFAULT 0,
    CONSTRAINT UQ_Email UNIQUE (Email)
);
GO

-- Agregar columna Verificado si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'Verificado')
BEGIN
    ALTER TABLE Usuarios ADD Verificado BIT DEFAULT 0;
END
GO

CREATE TABLE Roles (
    RolID INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol VARCHAR(30) NOT NULL UNIQUE,
    Descripcion VARCHAR(200)
);
GO

CREATE TABLE UsuarioRoles (
    UsuarioRolID INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    RolID INT NOT NULL FOREIGN KEY REFERENCES Roles(RolID),
    FechaAsignacion DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_UsuarioRol UNIQUE (UsuarioID, RolID)
);
GO

-- Índices para mejorar el rendimiento
CREATE INDEX IX_UsuarioRoles_Usuario ON UsuarioRoles(UsuarioID);
CREATE INDEX IX_UsuarioRoles_Rol ON UsuarioRoles(RolID);
GO

-- Insertar roles básicos
INSERT INTO Roles (NombreRol, Descripcion) VALUES 
('Administrador', 'Acceso completo al sistema'),
('Cuidador', 'Personas que ofrecen servicios de cuidado de mascotas'),
('Cliente', 'Dueños de mascotas que buscan servicios');
GO

-- Insertar usuario administrador
INSERT INTO Usuarios (
    NombreUsuario, ContrasenaHash, Email, NombreCompleto, Telefono, Direccion, Verificado
) VALUES (
    'admin', 
    '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', -- Hash SHA256 de 'admin'
    'admin@petcare.com', 
    'Administrador del Sistema', 
    '0998887777',
    'Calle Principal 123',
    1
);
GO

-- Asignar rol de administrador
INSERT INTO UsuarioRoles (UsuarioID, RolID) VALUES (1, 1);
GO


USE BDD_PETCARE;
GO

-- Tabla para información específica de Clientes
CREATE TABLE Clientes (
    ClienteID INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    DocumentoIdentidad VARCHAR(20) NOT NULL,
    DocumentoIdentidadArchivo VARBINARY(MAX),
    DocumentoVerificado BIT DEFAULT 0,
    FechaVerificacion DATETIME,
    CONSTRAINT UQ_Cliente_Usuario UNIQUE (UsuarioID)
);
GO

-- Tabla para información específica de Cuidadores
CREATE TABLE Cuidadores (
    CuidadorID INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    DocumentoIdentidad VARCHAR(20) NOT NULL,
    DocumentoIdentidadArchivo VARBINARY(MAX),
    ComprobanteServiciosArchivo VARBINARY(MAX),
    TelefonoEmergencia VARCHAR(15) NOT NULL,
    Biografia TEXT,
    Experiencia TEXT,
    HorarioAtencion VARCHAR(100),
    TarifaPorHora DECIMAL(10,2),
    CalificacionPromedio DECIMAL(3,2) DEFAULT 0.0,
    DocumentoVerificado BIT DEFAULT 0,
    FechaVerificacion DATETIME,
    CONSTRAINT UQ_Cuidador_Usuario UNIQUE (UsuarioID)
);
GO

-- Tabla para documentos de verificación (opcional, puede usarse en lugar de campos en las tablas anteriores)
CREATE TABLE DocumentosVerificacion (
    DocumentoID INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    TipoDocumento VARCHAR(50) NOT NULL, -- 'Cedula', 'ComprobanteServicio', etc.
    Archivo VARBINARY(MAX) NOT NULL,
    FechaSubida DATETIME DEFAULT GETDATE(),
    Estado VARCHAR(20) DEFAULT 'Pendiente', -- 'Aprobado', 'Rechazado'
    Comentarios TEXT,
    FechaVerificacion DATETIME
);
GO

-- Tabla para calificaciones a cuidadores
CREATE TABLE Calificaciones (
    CalificacionID INT IDENTITY(1,1) PRIMARY KEY,
    CuidadorID INT NOT NULL FOREIGN KEY REFERENCES Cuidadores(CuidadorID),
    ClienteID INT NOT NULL FOREIGN KEY REFERENCES Clientes(ClienteID),
    Puntuacion INT NOT NULL CHECK (Puntuacion BETWEEN 1 AND 5),
    Comentario TEXT,
    FechaCalificacion DATETIME DEFAULT GETDATE()
);
GO

-- Crear tabla Solicitudes
CREATE TABLE Solicitudes (
    SolicitudID INT IDENTITY(1,1) PRIMARY KEY,
    ClienteID INT NOT NULL FOREIGN KEY REFERENCES Clientes(ClienteID),
    CuidadorID INT NOT NULL FOREIGN KEY REFERENCES Cuidadores(CuidadorID),
    FechaSolicitud DATETIME DEFAULT GETDATE(),
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    Estado VARCHAR(20) DEFAULT 'Pendiente',
    Descripcion TEXT,
    PrecioTotal DECIMAL(10,2)
);
GO

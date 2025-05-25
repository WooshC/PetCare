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
    CONSTRAINT UQ_Email UNIQUE (Email)
);
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
    NombreUsuario, ContrasenaHash, Email, NombreCompleto, Telefono, Direccion
) VALUES (
    'admin', 
    '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', -- Hash SHA256 de 'admin'
    'admin@petcare.com', 
    'Administrador del Sistema', 
    '0998887777',
    'Calle Principal 123'
);
GO

-- Asignar rol de administrador
INSERT INTO UsuarioRoles (UsuarioID, RolID) VALUES (1, 1);
GO
-- 2. Crear el login nuevamente
CREATE LOGIN PetCareApp WITH PASSWORD = 'AppPassword123!';
PRINT 'Login PetCareApp recreado.';
GO

-- 3. Crear el usuario en la base de datos BDD_PETCARE (no en master)
USE BDD_PETCARE;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'PetCareApp')
BEGIN
    CREATE USER PetCareApp FOR LOGIN PetCareApp;
    PRINT 'Usuario PetCareApp creado en BDD_PETCARE.';
END
ELSE
BEGIN
    PRINT 'El usuario ya existía en BDD_PETCARE.';
END
GO

-- 4. Asignar el rol db_owner (en BDD_PETCARE, no en master)
ALTER ROLE db_owner ADD MEMBER PetCareApp;
PRINT 'Rol db_owner asignado correctamente.';
GO

-- Crear tabla Usuarios
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

-- Crear tabla Roles
CREATE TABLE Roles (
    RolID INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol VARCHAR(30) NOT NULL UNIQUE,
    Descripcion VARCHAR(200)
);
GO

-- Crear tabla UsuarioRoles
CREATE TABLE UsuarioRoles (
    UsuarioRolID INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    RolID INT NOT NULL FOREIGN KEY REFERENCES Roles(RolID),
    FechaAsignacion DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_UsuarioRol UNIQUE (UsuarioID, RolID)
);
GO

-- Índices para UsuarioRoles
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

-- Crear tabla Clientes
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

-- Crear tabla Cuidadores
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

-- Crear tabla DocumentosVerificacion
CREATE TABLE DocumentosVerificacion (
    DocumentoID INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    TipoDocumento VARCHAR(50) NOT NULL,
    Archivo VARBINARY(MAX) NOT NULL,
    FechaSubida DATETIME DEFAULT GETDATE(),
    Estado VARCHAR(20) DEFAULT 'Pendiente',
    Comentarios TEXT,
    FechaVerificacion DATETIME
);
GO

CREATE TABLE Calificaciones (
    CalificacionID INT IDENTITY(1,1) PRIMARY KEY,
    CuidadorID INT NOT NULL FOREIGN KEY REFERENCES Cuidadores(CuidadorID),
    ClienteID INT NOT NULL FOREIGN KEY REFERENCES Clientes(ClienteID),
    Puntuacion INT NOT NULL CHECK (Puntuacion BETWEEN 1 AND 5),
    Comentario TEXT,
    FechaCalificacion DATETIME DEFAULT GETDATE()
);
GO
INSERT INTO Calificaciones (CuidadorID, ClienteID, Puntuacion, Comentario)
VALUES (1, 1, 5, 'Excelente servicio con mi mascota');
GO



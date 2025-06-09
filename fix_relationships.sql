USE BDD_PETCARE;
GO

-- Eliminar restricciones existentes si existen
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clientes_Usuarios')
BEGIN
    ALTER TABLE Clientes DROP CONSTRAINT FK_Clientes_Usuarios;
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cuidadores_Usuarios')
BEGIN
    ALTER TABLE Cuidadores DROP CONSTRAINT FK_Cuidadores_Usuarios;
END
GO

-- Recrear las restricciones con el comportamiento correcto
ALTER TABLE Clientes
ADD CONSTRAINT FK_Clientes_Usuarios
FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
ON DELETE NO ACTION;
GO

ALTER TABLE Cuidadores
ADD CONSTRAINT FK_Cuidadores_Usuarios
FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
ON DELETE NO ACTION;
GO

-- Asegurar que los índices únicos existan
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Cliente_Usuario' AND object_id = OBJECT_ID('Clientes'))
BEGIN
    CREATE UNIQUE INDEX UQ_Cliente_Usuario ON Clientes(UsuarioID);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Cuidador_Usuario' AND object_id = OBJECT_ID('Cuidadores'))
BEGIN
    CREATE UNIQUE INDEX UQ_Cuidador_Usuario ON Cuidadores(UsuarioID);
END
GO 
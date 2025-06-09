-- Verificar si la columna existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DocumentosVerificacion') AND name = 'CuidadorID')
BEGIN
    -- Agregar la columna CuidadorID
    ALTER TABLE DocumentosVerificacion ADD CuidadorID INT NULL;

    -- Agregar la restricción de clave foránea
    ALTER TABLE DocumentosVerificacion
    ADD CONSTRAINT FK_DocumentosVerificacion_Cuidadores
    FOREIGN KEY (CuidadorID) REFERENCES Cuidadores(CuidadorID);
END
GO 
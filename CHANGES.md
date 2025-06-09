# Registro de Cambios - PetCare

## Cambios en la Base de Datos

### 1. Estructura de la Base de Datos
```sql
-- Agregar columna Verificado a la tabla Usuarios
ALTER TABLE Usuarios ADD Verificado BIT DEFAULT 0;
GO

-- Actualizar el usuario administrador para que esté verificado
UPDATE Usuarios SET Verificado = 1 WHERE Email = 'admin@petcare.com';
GO
```

### 2. Usuarios y Permisos
```sql
-- Crear login
CREATE LOGIN PetCareApp WITH PASSWORD = 'AppPassword123!';
GO

-- Crear usuario en la base de datos
USE BDD_PETCARE;
GO
CREATE USER PetCareApp FOR LOGIN PetCareApp;
GO

-- Asignar rol db_owner
ALTER ROLE db_owner ADD MEMBER PetCareApp;
GO
```

## Cambios en la Configuración

### 1. appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BDD_PETCARE;User Id=PetCareApp;Password=AppPassword123!;TrustServerCertificate=True;",
    "PetCareConnection": "Server=localhost;Database=BDD_PETCARE;User Id=PetCareApp;Password=AppPassword123!;TrustServerCertificate=True;"
  }
}
```

## Estructura de Tablas

### 1. Usuarios
- UsuarioID (PK)
- NombreUsuario
- ContrasenaHash
- Email
- NombreCompleto
- Telefono
- Direccion
- FechaRegistro
- UltimoAcceso
- Activo
- Verificado (Nuevo)

### 2. Roles
- RolID (PK)
- NombreRol
- Descripcion

### 3. UsuarioRoles
- UsuarioRolID (PK)
- UsuarioID (FK)
- RolID (FK)
- FechaAsignacion

### 4. Clientes
- ClienteID (PK)
- UsuarioID (FK)
- DocumentoIdentidad
- DocumentoIdentidadArchivo
- DocumentoVerificado
- FechaVerificacion

### 5. Cuidadores
- CuidadorID (PK)
- UsuarioID (FK)
- DocumentoIdentidad
- DocumentoIdentidadArchivo
- ComprobanteServiciosArchivo
- TelefonoEmergencia
- Biografia
- Experiencia
- HorarioAtencion
- TarifaPorHora
- CalificacionPromedio
- DocumentoVerificado
- FechaVerificacion

### 6. DocumentosVerificacion
- DocumentoID (PK)
- UsuarioID (FK)
- TipoDocumento
- Archivo
- FechaSubida
- Estado
- Comentarios
- FechaVerificacion

### 7. Calificaciones
- CalificacionID (PK)
- CuidadorID (FK)
- ClienteID (FK)
- Puntuacion
- Comentario
- FechaCalificacion

## Notas Importantes

1. **Credenciales de Acceso**:
   - Usuario administrador: admin@petcare.com
   - Contraseña: admin
   - Hash SHA256: 8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918

2. **Conexión a la Base de Datos**:
   - Servidor: localhost
   - Base de datos: BDD_PETCARE
   - Usuario: PetCareApp
   - Contraseña: AppPassword123!

3. **Cambios Pendientes**:
   - [ ] Verificar que la columna 'Verificado' se haya agregado correctamente
   - [ ] Probar el inicio de sesión con las nuevas credenciales
   - [ ] Verificar los permisos del usuario PetCareApp

## Comandos Útiles

### Docker (si se necesita)
```powershell
docker-compose down -v
Remove-Item -Recurse -Force sqlserverdata
docker-compose up -d
```

### SQL Server
```sql
-- Verificar la existencia de la columna Verificado
SELECT * FROM sys.columns 
WHERE object_id = OBJECT_ID('Usuarios') 
AND name = 'Verificado';

-- Verificar los permisos del usuario
SELECT * FROM sys.database_principals 
WHERE name = 'PetCareApp';
``` 
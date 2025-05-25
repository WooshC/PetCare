# PetCare - Sistema de Gestión de Cuidado de Mascotas

Bienvenido a **PetCare**, una aplicación web construida con ASP.NET Core MVC, Entity Framework Core y SQL Server. Este sistema permite gestionar la autenticación de usuarios con roles predefinidos.

---

##  Instalación y Ejecución del Proyecto

### Clonar el repositorio

Puedes clonar el proyecto usando **GitHub Desktop** o por línea de comandos:

#### Instalar los paquetes NuGet
Abre el proyecto en Visual Studio 2022, abre la Consola del Administrador de Paquetes y ejecuta los siguientes comandos uno por uno:

powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer 

Install-Package Microsoft.EntityFrameworkCore.Tools

Install-Package Microsoft.AspNetCore.SignalR

Install-Package Microsoft.AspNetCore.SignalR.Client

##### Configurar y levantar la base de datos con Docker
Si tienes Docker instalado, abre el proyecto en Visual Studio Code o una terminal en la raíz del proyecto y ejecuta:
docker compose up
# En caso de que quieras eliminar contenedores anteriores o evitar conflictos de nombres:
docker compose down
#### Ejecutar el proyecto
Una vez levantado Docker y agregados los paquetes NuGet, vuelve a Visual Studio 2022 y:

Asegúrate de que la base de datos está conectada.

Ejecuta el proyecto (F5 o haz clic en "Iniciar").

Se abrirá el navegador en la ruta /Login.

# Credenciales de acceso
Puedes iniciar sesión con las siguientes credenciales predeterminadas:


Email:admin@petcare.com
Contraseña: admin
Estas credenciales corresponden al usuario Administrador creado automáticamente en la base de datos con datos semilla.

# Estructura del Proyecto
Controllers/ - Controlador MVC para login (LoginController)

Views/Login/ - Vistas Razor para autenticación y bienvenida

Models/ - Entidades: Usuario, Rol, UsuarioRol

Data/ApplicationDbContext.cs - Configuración de EF Core y datos iniciales

docker-compose.yml - Archivo de configuración para levantar SQL Server con Docker

# Requisitos
.NET 6 o superior

Visual Studio 2022

Docker Desktop

SQL Server (si no usas Docker)

Git

# Funcionalidades actuales
 Autenticación de usuarios con verificación de correo y contraseña

 Visualización de datos básicos del usuario tras el login

### Contribuciones
¡Las contribuciones son bienvenidas! Puedes hacer un fork del repositorio y enviar un pull request con mejoras, correcciones o nuevas funcionalidades.

# Licencia
Este proyecto está bajo licencia MIT. Puedes usarlo, modificarlo y distribuirlo libremente.

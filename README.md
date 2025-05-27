# ?? PetCare - Sistema de Gesti�n de Cuidado de Mascotas

Bienvenido a **PetCare**, una aplicaci�n web construida con ASP.NET Core MVC, Entity Framework Core y SQL Server. Este sistema permite gestionar la autenticaci�n de usuarios con roles predefinidos.

---

## ?? Instalaci�n y Ejecuci�n del Proyecto

### 1 Clonar el repositorio

Puedes clonar el proyecto usando **GitHub Desktop** o por l�nea de comandos:

```bash
git clone https://github.com/tu-usuario/petcare.git
cd petcare

2?? Instalar los paquetes NuGet
Abre el proyecto en Visual Studio 2022, abre la Consola del Administrador de Paquetes y ejecuta los siguientes comandos uno por uno:

powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.AspNetCore.SignalR
Install-Package Microsoft.AspNetCore.SignalR.Client


dotnet add package Microsoft.EntityFrameworkCore.Tools

3?? Configurar y levantar la base de datos con Docker
Si tienes Docker instalado, abre el proyecto en Visual Studio Code o una terminal en la ra�z del proyecto y ejecuta:


docker compose up
?? En caso de que quieras eliminar contenedores anteriores o evitar conflictos de nombres:

docker docker compose down -v

4?? Ejecutar el proyecto
Una vez levantado Docker y agregados los paquetes NuGet, vuelve a Visual Studio 2022 y:

Aseg�rate de que la base de datos est� conectada.

Ejecuta el proyecto (F5 o haz clic en "Iniciar").

Se abrir� el navegador en la ruta /Login.

? Credenciales de acceso
Puedes iniciar sesi�n con las siguientes credenciales predeterminadas:

text
Copiar
Editar
?? Email:    admin@petcare.com
?? Contrase�a: admin
Estas credenciales corresponden al usuario Administrador creado autom�ticamente en la base de datos con datos semilla.

?? Estructura del Proyecto
Controllers/ - Controlador MVC para login (LoginController)

Views/Login/ - Vistas Razor para autenticaci�n y bienvenida

Models/ - Entidades: Usuario, Rol, UsuarioRol

Data/ApplicationDbContext.cs - Configuraci�n de EF Core y datos iniciales

docker-compose.yml - Archivo de configuraci�n para levantar SQL Server con Docker

?? Requisitos
.NET 6 o superior

Visual Studio 2022

Docker Desktop

SQL Server (si no usas Docker)

Git

? Funcionalidades actuales
 Autenticaci�n de usuarios con verificaci�n de correo y contrase�a

 Visualizaci�n de datos b�sicos del usuario tras el login

????? Contribuciones
�Las contribuciones son bienvenidas! Puedes hacer un fork del repositorio y enviar un pull request con mejoras, correcciones o nuevas funcionalidades.

?? Licencia
Este proyecto est� bajo licencia MIT. Puedes usarlo, modificarlo y distribuirlo libremente.
# Restaurante - Auth Service (.NET)

Este repositorio aloja el **Microservicio de Autenticación** alternativo o complementario del sistema del Restaurante, construido con tecnología Microsoft. Utilizando C# y ASP.NET Core, este servicio ofrece robustez, tipado estático fuerte y un excelente rendimiento para la emisión y verificación de identidades mediante JWT.

## 🚀 Arquitectura y Patrones
- **Clean Architecture:** El código está claramente dividido en capas (Api, Application, Domain, Infrastructure) para garantizar la separación de responsabilidades y la facilidad de pruebas.
- **Inyección de Dependencias:** Aprovechamiento nativo del contenedor de inyección de .NET.
- **Seguridad:** Generación y validación de JSON Web Tokens (JWT) utilizando las librerías oficiales de Microsoft.

## 🛠 Stack Tecnológico
- **Lenguaje:** C#
- **Framework:** .NET (ASP.NET Core Web API)
- **Seguridad:** JWT (Microsoft.AspNetCore.Authentication.JwtBearer)
- **Documentación:** Swagger / OpenAPI
- **Despliegue:** Docker ready

## 📁 Estructura del Código Fuente
El proyecto principal se encuentra dentro de la solución `Restaurante.AuthService.sln`.
```
Restaurante-AuthService/
├── Restaurante.AuthService/
│   ├── Restaurante.AuthService.sln         # Archivo de la solución de Visual Studio
│   ├── Restaurante.AuthService.Api/        # Capa de Presentación (Controladores, Program.cs)
│   ├── Restaurante.AuthService.Application/# Capa de Aplicación (Casos de uso, interfaces lógicas)
│   ├── Restaurante.AuthService.Domain/     # Capa de Dominio (Entidades de negocio)
│   └── Restaurante.AuthService.Infrastructure/ # Capa de Infraestructura (Base de datos, servicios externos)
├── Dockerfile                              # Archivo para contenedorizar el servicio
└── README.md
```

## 📋 Requisitos para Desarrollo
- [.NET SDK](https://dotnet.microsoft.com/download) (versión correspondiente, típicamente .NET 6 o superior).
- Un IDE como Visual Studio 2022, Rider o Visual Studio Code (con la extensión de C#).

## ⚙️ Pasos de Instalación y Ejecución

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/IN6CM-GestorRestaurante/Restaurante-AuthService.git
   cd Restaurante-AuthService/Restaurante.AuthService
   ```

2. **Restaurar paquetes NuGet:**
   ```bash
   dotnet restore
   ```

3. **Configurar el archivo `appsettings.json`:**
   En la capa `Api`, asegúrate de configurar tu `appsettings.Development.json` con los secretos para JWT y cadenas de conexión si aplica:
   ```json
   {
     "Jwt": {
       "Key": "tu_clave_secreta_super_segura_aqui",
       "Issuer": "RestauranteIssuer",
       "Audience": "RestauranteAudience"
     }
   }
   ```

4. **Ejecutar el proyecto:**
   ```bash
   cd Restaurante.AuthService.Api
   dotnet run
   ```
   El servidor normalmente levantará en los puertos `http://localhost:5000` o `https://localhost:5001`.

## 📌 Documentación
Navega a la ruta de Swagger (por ejemplo: `https://localhost:5001/swagger`) en tu navegador para interactuar visualmente con los endpoints expuestos por la API.

# 🌐 **Turho-ms-core – Microservicio base de Turho**

![Plataforma](https://img.shields.io/badge/platform-.NET%209-blueviolet)
![Licencia](https://img.shields.io/badge/license-MIT-green)
![Arquitectura limpia](https://img.shields.io/badge/architecture-clean-blue)
![DDD](https://img.shields.io/badge/pattern-DDD-orange)
![MediatR](https://img.shields.io/badge/tool-MediatR-red)

> **Turho-ms-core** es el microservicio principal de la App **Turho**, encargado de la gestión de **reservas**. Permite **crear y consultar reservas** de forma escalable, siguiendo una arquitectura basada en **DDD, arquitectura limpia y MediatR**.  

---

## 🎯 **Visión general**

**Turho-ms-core** es el núcleo de la plataforma Turho. Este servicio expone la lógica principal relacionada con las **reservas de la aplicación**, garantizando alta disponibilidad, mantenibilidad y fácil integración con otros microservicios del ecosistema.

Está construido sobre:

- 🏗 **Arquitectura limpia** → Separación clara de capas para mayor mantenibilidad y escalabilidad.  
- 📦 **Diseño orientado al dominio (DDD)** → Organización de la lógica de negocio enfocada en el contexto de reservas.  
- 📡 **MediatR** → Comunicación desacoplada entre componentes mediante el patrón mediador.  

---

## 🛠 **Características principales**

- ✅ **Gestión completa de reservas**: creación, consulta y administración.  
- ✅ **Estructura modular y extensible** para agregar nuevas funcionalidades sin afectar la base.  
- ✅ **Optimizado para microservicios** → ligero y fácil de desplegar en contenedores.  
- ✅ **Mejores prácticas en .NET 9**, siguiendo patrones de diseño probados.  

---

## 🚀 **Primeros pasos**

### **Requisitos previos**

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)  
- Base de datos compatible (SQL Server, PostgreSQL, etc.)  
- [Docker](https://www.docker.com/products/docker-desktop) *(opcional para despliegues en contenedores)*  

### **Instalación**

1. **Clona el repositorio**  
   ```bash
   git clone https://github.com/TuOrg/Turho-ms-core.git
   cd Turho-ms-core
   ```

2. **Restaura dependencias**  
   ```bash
   dotnet restore
   ```

3. **Configura la base de datos**  
   Edita `appsettings.json` con la cadena de conexión correcta.  

4. **Ejecuta la aplicación**  
   ```bash
   dotnet run
   ```

---

## 📖 **Uso**

Este microservicio expone endpoints REST para gestionar reservas.  

- **Crear una reserva** → Envía un `POST` con los datos de la reserva.  
- **Consultar reservas** → Usa `GET` con filtros como fechas, usuario, estado, etc.  

La documentación de la API estará disponible mediante Swagger en:  
```
https://localhost:<puerto>/swagger
```

---

## 📂 **Estructura del proyecto**

```
📁 Turho-ms-core
├── 📁 Web.Api              # Capa de presentación con controladores y endpoints
├── 📁 Application          # Lógica de negocio, DTOs y handlers con MediatR
├── 📁 Domain               # Entidades, agregados y lógica central del dominio de reservas
└── 📁 Infrastructure       # Persistencia, repositorios y servicios externos
```

---

## 🧑‍🤝‍🧑 **Contribución**

¡Las contribuciones son bienvenidas! Para colaborar:  

1. Haz un fork del repositorio  
2. Crea una rama con tu feature (`feature/nueva-funcionalidad`)  
3. Haz commit de tus cambios  
4. Sube tu rama y abre un **Pull Request**  

Revisa nuestras [Directrices de contribución](CONTRIBUTING.md) antes de enviar cambios. 🙌  

---

## 📄 **Licencia**

Este proyecto está licenciado bajo **MIT**. Consulta [LICENSE](LICENSE) para más detalles.  

---

## 🌟 **Agradecimientos**

Gracias a la comunidad .NET y a todos los colaboradores que hacen posible la evolución de Turho. 🙏  

---

> **Hecho con ❤️ para Turho**

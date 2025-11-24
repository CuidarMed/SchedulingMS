# SchedulingMS
SchedulingMS es un microservicio del sistema CuidarMed encargado de **gestionar la disponibilidad de médicos y los horarios de atención  y los turnos de atención de los pacientes**. Permite crear, editar, eliminar y consultar horarios de manera centralizada, con soporte para múltiples médicos y días de la semana.

Este microservicio forma parte de un ecosistema de microservicios que incluye `DirectoryMS`, `AuthMS` y `ClinicalMS`.

---

## 📝 Características

- CRUD de disponibilidad de médicos (`Create`, `Read`, `Update`, `Delete`)  
- Gestión de horarios por día de la semana  
- Configuración de duración de cada turno  
- Integración con otros microservicios del sistema (Autenticación, Directorio, Historia Clínica)  
- Respuesta en formato JSON estandarizado para APIs REST  
- Aplicación de validaciones con FluentValidation  
- Manejo de grandes cargas de datos (imágenes, archivos) mediante configuración de Kestrel y FormOptions  

---

## ⚙️ Tecnologías

- **.NET 9 / ASP.NET Core**  
- **Entity Framework Core** para acceso a base de datos SQL Server  
- **Swagger/OpenAPI** para documentación de endpoints  
- **FluentValidation** para validaciones de modelos  
- **CORS** configurado para permitir acceso desde cualquier origen  
- **Localización** en español (`es-US`)  

---

## 🗄 Base de Datos

- **SQL Server** como sistema gestor de base de datos.  
- Tablas principales:

| Tabla | Descripción |
|-------|-------------|
| `DoctorAvailability` | Guarda los horarios disponibles de cada médico |
| `Appointment` | Registra y gestiona los turnos de los pacientes con los médicos |
| `AvailabilityBlock` | Registra los periodos en los que un médico no puede recibir turnos (vacaciones, licencias, reuniones, bloqueos de agenda)  |

---

## 🚀 Instalación

1. Clonar el repositorio:

```bash
git clone https://github.com/tu-usuario/CuidarMed-SchedulingMS.git
cd CuidarMed-SchedulingMS
```
2. Levantar el servicio con Docker desde la raíz del proyecto:
```bash
dotnet docker compose up --build
```
3. **Configurar la cadena de conexión en `appsettings.json` (si no usas Docker Compose con variables):**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SchedulingDB;User Id=sa;Password=TuPassword123!;"
  }
}
```
⚠️ Asegúrate de que el puerto, usuario y contraseña coincidan con tu contenedor Docker de SQL Server.

4. Aplicar migraciones (si es necesario):
```bash
dotnet ef database update
```
6. Ejecutar la aplicación:
```bash
dotnet run
```
8. Acceder a Swagger para explorar la API:
```bash
https://localhost:5001/swagger
```


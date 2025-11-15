# Comunicación entre Microservicios con JWT

## Configuración Implementada

### SchedulingMS → ClinicalMS (con JWT)

SchedulingMS ahora se comunica con ClinicalMS usando JWT para autenticación.

## Componentes Creados

1. **ServiceTokenProvider** (`Application/Services/ServiceTokenProvider.cs`)
   - Obtiene tokens JWT desde AuthMS usando credenciales de servicio
   - Cachea tokens para evitar llamadas innecesarias

2. **JwtServiceClientHandler** (`Infrastructure/Handlers/JwtServiceClientHandler.cs`)
   - Agrega automáticamente el token JWT a todas las peticiones HTTP a ClinicalMS

3. **ClinicalService** (`Application/Services/ClinicalService.cs`)
   - Servicio para comunicarse con ClinicalMS
   - Métodos: `HasEncounterForAppointmentAsync`, `GetEncounterByAppointmentIdAsync`

## Configuración Requerida

### 1. Crear Usuario de Servicio en AuthMS

Necesitás crear un usuario en AuthMS con las credenciales configuradas:

**Email:** `scheduling-service@cuidarmed.com`  
**Password:** `ServicePassword123!`  
**Role:** Puede ser `Patient` o `Doctor` (no afecta la funcionalidad)

**Cómo crearlo:**
1. Usar el endpoint de registro: `POST /api/v1/User`
2. O crear directamente en la base de datos

### 2. Configuración en appsettings.json

```json
{
  "AuthMS": {
    "BaseUrl": "http://localhost:5093"
  },
  "ClinicalMS": {
    "BaseUrl": "http://localhost:5073"
  },
  "ServiceAuth": {
    "Email": "scheduling-service@cuidarmed.com",
    "Password": "ServicePassword123!"
  }
}
```

### 3. Configuración en Docker

En `docker-compose.yml`:
```yaml
environment:
  AuthMS__BaseUrl: "http://authms_api:8080"
  ClinicalMS__BaseUrl: "http://clinicalms_api:8080"
  ServiceAuth__Email: "scheduling-service@cuidarmed.com"
  ServiceAuth__Password: "ServicePassword123!"
```

### 4. Clave JWT Compartida

**IMPORTANTE:** ClinicalMS debe usar la misma clave JWT que AuthMS para validar los tokens.

En `ClinicalMS/docker-compose.yml`:
```yaml
environment:
  JwtSettings__key: "dev-super-secret-key-change-me"  # Misma clave que AuthMS
```

## Flujo de Comunicación

```
1. SchedulingMS necesita llamar a ClinicalMS
   ↓
2. JwtServiceClientHandler intercepta la petición
   ↓
3. ServiceTokenProvider obtiene token de AuthMS
   (POST /api/v1/Auth/Login con credenciales de servicio)
   ↓
4. Token se agrega al header: Authorization: Bearer <token>
   ↓
5. ClinicalMS valida el token JWT
   ↓
6. ClinicalMS procesa la petición
```

## Ejemplo de Uso

En `UpdateAppointmentService.CancelAsync`:
```csharp
// Verificar si existe un encounter antes de cancelar
var hasEncounter = await _clinicalService.HasEncounterForAppointmentAsync(appointmentId);
if (hasEncounter)
{
    _logger.LogWarning("Appointment tiene encounter asociado");
}
```

## Endpoints de ClinicalMS Disponibles

- `GET /api/v1/Encounter?appointmentId={id}` - Buscar encounters por appointmentId
- `GET /api/v1/Encounter?patientId={id}&from={date}&to={date}` - Buscar por rango
- `POST /api/v1/Encounter?patientId={id}` - Crear encounter
- `GET /api/v1/Encounter/{id}` - Obtener encounter por ID

## Notas de Seguridad

⚠️ **Para Producción:**
- Usar claves JWT seguras y únicas
- Rotar credenciales de servicio periódicamente
- Considerar usar mTLS (mutual TLS) para comunicación entre servicios
- No exponer credenciales en código o repositorios públicos



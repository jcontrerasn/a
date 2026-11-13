# Arquitectura

El proyecto está organizado por capas dentro de la aplicación Web API:

- `Controllers`: recibe las solicitudes HTTP y devuelve respuestas HTTP.
- `Services`: contiene los casos de uso y la lógica de negocio.
- `Interfaces`: contiene los contratos de servicios y repositorios.
- `Dtos`: define los modelos de entrada y salida de la API.
- `Entities`: contiene las entidades del dominio.
- `Repositories`: abstrae el acceso a datos.

## Flujo de una solicitud

`Controller -> Service -> Repository -> Entity`

El ejemplo `TrackItem` usa `InMemoryTrackItemRepository` para poder probar la API sin base de datos. Cuando se configure persistencia real, se puede crear otra implementación de `ITrackItemRepository` y mantener el controller y el service sin cambios.

## Endpoints de ejemplo

- `GET /api/track-items`
- `GET /api/track-items/{id}`
- `POST /api/track-items`
- `GET /api/usuarios/cuenta/{cuenta}`
- `POST /api/usuarios/personal-trackit/buscar-por-filtro`
- `GET /api/tareas/involucrado/{usuario}`
- `GET /api/tareas/creadas-por/{usuario}`
- `GET /api/tareas/otras/{usuario}`
- `GET /api/tareas/{idTarea}`
- `POST /api/tareas`
- `PUT /api/tareas/{idTarea}`
- `DELETE /api/tareas/{idTarea}?usuario={usuario}`
- `GET /api/usuario-roles`
- `PUT /api/usuario-roles/{idUsuarioRol}`
- `PATCH /api/usuario-roles/{idUsuarioRol}/estado`
- `GET /api/roles`
- `GET /api/tareas/{idTarea}/historial`
- `POST /api/tareas/{idTarea}/anotaciones`

El endpoint de usuarios sigue el flujo:

`UsuariosController -> UsuarioService -> IUsuarioRepository/IUsuarioRolRepository -> SQL Server`

El usuario se obtiene mediante `pr_sm_BuscarUsuarioXCuenta` y se valida que tenga un rol activo en `TRACKIT_UsuarioRoles` y `TRACKIT_Roles`.

El endpoint de tareas consulta las tablas `TRACKIT_Tareas` y `TRACKIT_TareaInvolucrados`, y devuelve también todos los involucrados y el rol activo del creador.

Ejemplo de cuerpo para crear un elemento:

```json
{
  "title": "Revisar equipo",
  "description": "Validar el estado del equipo"
}
```

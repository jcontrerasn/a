# Arquitectura

El proyecto está organizado por capas dentro de la aplicación Web API:

- `Controllers`: recibe las solicitudes HTTP y devuelve respuestas HTTP.
- `Services`: contiene los casos de uso y la lógica de negocio.
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

Ejemplo de cuerpo para crear un elemento:

```json
{
  "title": "Revisar equipo",
  "description": "Validar el estado del equipo"
}
```

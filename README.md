# AdoptMe

Plataforma para publicar mascotas en adopción, encontrarlas por cercanía y coordinar la adopción mediante solicitudes, chat y notificaciones en tiempo real.

## Estructura

| Carpeta | Contenido |
| --- | --- |
| `server/` | API REST, servicios gRPC y Socket.IO en Node.js 22 |
| `desktop/` | Cliente de escritorio WPF en .NET 8 con MVVM |
| `mobile/` | Cliente Android en Java 17 con MVVM |
| `protos/` | Contratos gRPC compartidos por los tres proyectos |
| `infraestructura/` | Imágenes de nginx y SQL Server con el esquema de la base de datos |

## Arquitectura

```
Escritorio (WPF) ─┐                       ┌─ SQL Server
                  ├─ nginx :8080 ─ REST ──┤
Android ──────────┤        Socket.IO      ├─ Redis (índice geográfico)
                  └─ gRPC :50051 ─────────┘
                           servidor Node.js
```

- **Servidor.** Cada módulo de `server/src/modulos` separa repositorio, servicio, controlador, rutas y, cuando aplica, servicio gRPC. `contenedor.js` es la raíz de composición donde se inyectan las dependencias. Los errores de dominio (`ErrorAplicacion`) se traducen a códigos HTTP y gRPC en un solo lugar.
- **Escritorio.** `AdoptMe.Escritorio.Nucleo` contiene modelos, servicios y ViewModels sin dependencia de WPF, por eso se prueba de forma aislada. `AdoptMe.Escritorio` solo aporta vistas, estilos y servicios de plataforma.
- **Android.** Tiene cuatro capas:
  - `dominio`: modelos.
  - `datos`: repositorios REST, gRPC y de dispositivo detrás de interfaces.
  - `di`: `ContenedorDependencias` y la fábrica de ViewModels.
  - `ui`: pantallas agrupadas por funcionalidad.

  Los ViewModels solo conocen interfaces de repositorio.

## Puesta en marcha

### Servidor

Requiere Docker con Compose.

```bash
cp .env.example .env
docker compose up -d --build
```

Verifica que el servidor responda:

- Salud: `http://localhost:8080/salud`
- Documentación OpenAPI: `http://localhost:8080/api-docs`
- gRPC: `localhost:50051`

La cuenta de administrador se crea al arrancar con `ADMIN_EMAIL` y `ADMIN_PASSWORD` de `.env`. Cambia esos valores, `JWT_SECRET` y las contraseñas de base de datos antes de cualquier despliegue.

Pruebas:

```bash
cd server && npm ci && npm test
docker compose --profile pruebas run --rm pruebas
```

El primer comando ejecuta las pruebas unitarias; el segundo, las de integración contra SQL Server y Redis en contenedores.

### Escritorio

Requiere Windows y el SDK de .NET 8 o superior.

```bash
dotnet run --project desktop/src/AdoptMe.Escritorio
dotnet test desktop/AdoptMe.Escritorio.sln
```

Las direcciones del servidor están en `desktop/src/AdoptMe.Escritorio/appsettings.json`. Para incluir las pruebas de integración, define `ADOPTME_SERVIDOR=http://localhost:8080` antes de `dotnet test`.

### Android

Requiere JDK 17 y el SDK de Android 35. Crea `mobile/local.properties` con `sdk.dir` o define `ANDROID_HOME`.

```bash
cd mobile
./gradlew assembleDebug
./gradlew testDebugUnitTest lintDebug
```

La dirección del servidor se configura en `mobile/gradle.properties`:

- `adoptme.servidor.host` vale `10.0.2.2` por defecto, que es el equipo anfitrión visto desde el emulador. En un dispositivo físico usa la IP del equipo en la red local.
- `adoptme.servidor.puertoHttp` y `adoptme.servidor.puertoGrpc` definen los puertos.

Solo la variante `debug` permite tráfico HTTP sin cifrar.

## Contrato

- **REST.** Todo vive bajo `/api` y está documentado en `server/src/http/openapi.yaml`. Todas las rutas requieren `Authorization: Bearer <token>`, salvo `POST /api/acceso/iniciar-sesion` y `POST /api/usuarios`.
- **gRPC.** Los servicios están definidos en `protos/`:
  - `ServicioUbicacion`: adopciones cercanas.
  - `ServicioMultimedia`: subida de fotos y videos por flujo.
  - `ServicioNotificacion`: flujo de notificaciones.

  El token viaja en el metadato `authorization`.
- **Socket.IO.** El token se envía en `auth.token` del handshake. Hay tres eventos:
  - `enviar_mensaje`: el cliente lo emite con `{DestinatarioID, Contenido}`.
  - `nuevo_mensaje`: el servidor lo emite al remitente y al destinatario.
  - `error_mensaje`: el servidor avisa de un envío fallido.

Cualquier cambio en `protos/` regenera automáticamente el código de escritorio (Grpc.Tools) y de Android (protobuf-gradle-plugin) en la siguiente compilación.

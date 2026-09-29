# AdoptMe

A platform to list pets for adoption, find them by proximity and coordinate the adoption through requests, chat and real-time notifications.

## Structure

| Folder | Contents |
| --- | --- |
| `server/` | REST API, gRPC services and Socket.IO on Node.js 22 |
| `desktop/` | WPF desktop client on .NET 8 with MVVM |
| `mobile/` | Android client in Java 17 with MVVM |
| `protos/` | gRPC contracts shared by the three projects |
| `infraestructura/` | nginx and SQL Server images, including the database schema |

## Architecture

```
Desktop (WPF) ────┐                       ┌─ SQL Server
                  ├─ nginx :8080 ─ REST ──┤
Android ──────────┤        Socket.IO      ├─ Redis (geospatial index)
                  └─ gRPC :50051 ─────────┘
                            Node.js server
```

- **Server.** Each module in `server/src/modulos` separates repository, service, controller, routes and, where applicable, a gRPC service. `contenedor.js` is the composition root where dependencies are injected. Domain errors (`ErrorAplicacion`) are translated into HTTP and gRPC status codes in a single place.
- **Desktop.** `AdoptMe.Escritorio.Nucleo` holds models, services and ViewModels with no WPF dependency, so it is tested in isolation. `AdoptMe.Escritorio` only provides views, styles and platform services.
- **Android.** It has four layers:
  - `dominio`: models.
  - `datos`: REST, gRPC and device repositories behind interfaces.
  - `di`: `ContenedorDependencias` and the ViewModel factory.
  - `ui`: screens grouped by feature.

  ViewModels only know repository interfaces.

## Getting started

### Server

Requires Docker with Compose.

```bash
cp .env.example .env
docker compose up -d --build
```

Check that the server responds:

- Health: `http://localhost:8080/salud`
- OpenAPI documentation: `http://localhost:8080/api-docs`
- gRPC: `localhost:50051`

The administrator account is created at startup from `ADMIN_EMAIL` and `ADMIN_PASSWORD` in `.env`. Change those values, `JWT_SECRET` and the database passwords before any deployment.

Tests:

```bash
cd server && npm ci && npm test
docker compose --profile pruebas run --rm pruebas
```

The first command runs the unit tests; the second runs the integration tests against SQL Server and Redis in containers.

### Desktop

Requires Windows and the .NET 8 SDK or later.

```bash
dotnet run --project desktop/src/AdoptMe.Escritorio
dotnet test desktop/AdoptMe.Escritorio.sln
```

The server addresses are in `desktop/src/AdoptMe.Escritorio/appsettings.json`. To include the integration tests, set `ADOPTME_SERVIDOR=http://localhost:8080` before running `dotnet test`.

The user's location comes from the Windows location service (Wi-Fi or GPS). Location must be enabled under *Settings > Privacy & security > Location*, including the option that lets desktop apps access it. If it is unavailable, the client falls back to an approximate IP-based location and warns the user.

### Android

Requires JDK 17 and Android SDK 35. Create `mobile/local.properties` with `sdk.dir`, or set `ANDROID_HOME`.

```bash
cd mobile
./gradlew assembleDebug
./gradlew testDebugUnitTest lintDebug
```

The server address is configured in `mobile/gradle.properties`:

- `adoptme.servidor.host` defaults to `10.0.2.2`, which is how the emulator reaches the host machine. On a physical device, use the host's IP address on the local network.
- `adoptme.servidor.puertoHttp` and `adoptme.servidor.puertoGrpc` set the ports.

Only the `debug` build variant allows unencrypted HTTP traffic.

## Contract

- **REST.** Everything lives under `/api` and is documented in `server/src/http/openapi.yaml`. Every route requires `Authorization: Bearer <token>`, except `POST /api/acceso/iniciar-sesion` and `POST /api/usuarios`.
- **gRPC.** The services are defined in `protos/`:
  - `ServicioUbicacion`: nearby adoptions.
  - `ServicioMultimedia`: streamed photo and video uploads.
  - `ServicioNotificacion`: notification stream.

  The token is sent in the `authorization` metadata.
- **Socket.IO.** The token is sent in the handshake's `auth.token`. There are three events:
  - `enviar_mensaje`: emitted by the client with `{DestinatarioID, Contenido}`.
  - `nuevo_mensaje`: emitted by the server to both the sender and the recipient.
  - `error_mensaje`: emitted by the server when a message fails to send.

Any change in `protos/` automatically regenerates the desktop code (Grpc.Tools) and the Android code (protobuf-gradle-plugin) on the next build.

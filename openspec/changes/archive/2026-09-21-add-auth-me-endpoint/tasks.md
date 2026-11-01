## 1. Endpoint GET /api/auth/me

- [x] 1.1 Registrar `appGroup.MapGet("/me", GetCurrentUser)` em `UrlShortener.API/Endpoints/AuthEndpoints.cs` com `.WithName(nameof(GetCurrentUser))`, `.RequireAuthorization()` e `.Produces<GetUserResponse>()` / `401` / `404` / `500`
- [x] 1.2 Implementar handler `GetCurrentUser(ClaimsPrincipal user, IAuthService authService)` que lê `ClaimTypes.NameIdentifier`, retorna `401` se ausente/não numérico, chama `authService.GetUserById(id)` e retorna `404` se nulo ou `200` com `GetUserResponse`
- [x] 1.3 Adicionar `using System.Security.Claims;` ao `AuthEndpoints.cs` (se ausente) e compilar com `dotnet build UrlShortener.slnx`

## 2. Verificação

- [x] 2.1 Subir infra (`docker compose up -d`) e rodar a API em Development; obter access token via `POST /api/auth/access-token`
- [x] 2.2 Validar `GET /api/auth/me` com token `user` e `admin` retorna `200` com o próprio `GetUserResponse`; sem token retorna `401`; token inválido/expirado retorna `401`

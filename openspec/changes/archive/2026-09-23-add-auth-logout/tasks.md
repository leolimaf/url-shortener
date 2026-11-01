## 1. Application — revogação do refresh

- [x] 1.1 Adicionar `Task<bool> Logout(long userId)` a `IAuthService` (retorna falso quando usuário não existe, para logout idempotente).
- [x] 1.2 Implementar `Logout` em `AuthService`: buscar via `IUserRepository.GetUserById`, zerar `RefreshTokenHash` e `RefreshTokenExpiryTime`, persistir com `IUnitOfWork.SaveAsync`.

## 2. API — endpoint autenticado

- [x] 2.1 Mapear `POST /auth/logout` em `AuthEndpoints` com `RequireAuthorization()`, `Produces(204)` / `Produces(401)`, extraindo `userId` do claim `NameIdentifier` (padrão de `GetCurrentUser`: claim ausente/inválido → `401`).
- [x] 2.2 Retornar `204 No Content` em sucesso e em logout repetido (idempotente); `401` quando identidade inutilizável.

## 3. Config — expiração do access em 60 min

- [x] 3.1 Alterar `JwtSettings:ExpirationInMinutes` para `60` em `UrlShortener.API/appsettings.json` e `appsettings.Development.json`.
- [x] 3.2 Validar build da solução (`dotnet build UrlShortener.slnx`) e fluxo manual: login → `GET /auth/me` → logout (`204`) → refresh com token revogado (`401`) → logout repetido (`204`).

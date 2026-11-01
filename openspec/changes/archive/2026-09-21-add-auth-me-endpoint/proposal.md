## Why

Clientes autenticados hoje não têm como obter o próprio perfil sem conhecer seu `id` numérico; o único GET existente (`/api/auth/user/{id}`) exige role `admin`. Um endpoint `/me` permite que qualquer usuário autenticado recupere seus dados com o access token.

## What Changes

- Adicionar `GET /api/auth/me` no módulo Auth que retorna `GetUserResponse` do usuário autenticado.
- Requer autenticação JWT (qualquer role autenticada: `user` ou `admin`); sem token retorna `401`, token expirado/inválido retorna `401`.
- Resolver a identidade a partir do claim `NameIdentifier` do access token e reutilizar o fluxo existente `IAuthService.GetUserById`.
- Se o usuário do token não existir mais, retornar `404`.
- Sem mudanças em registro, login, refresh ou no schema do `GetUserResponse`.

## Capabilities

### New Capabilities

- `auth-me`: recuperação do perfil do usuário autenticado via `GET /api/auth/me` retornando `GetUserResponse`.

### Modified Capabilities

- Nenhuma (capabilidades existentes mantidas; sem specs prévias em `openspec/specs/`).

## Impact

- API: novo handler `GetCurrentUser` em `UrlShortener.API/Endpoints/AuthEndpoints.cs` sob o grupo `/api/auth`; nenhuma rota existente alterada.
- Application: nenhum DTO novo; reutiliza `GetUserResponse` e `IAuthService.GetUserById` (sem alteração de interface necessária).
- Auth/Autorização: depende da validação JWT já configurada em `UrlShortener.Infrastructure/DependencyInjection.cs` (`ValidateLifetime`, signing-key); segue o padrão `RequireAuthorization()` das rotas existentes.
- Risco baixo: leitura apenas, sem escrita, migração ou dependência nova.

## Why

O módulo Auth emite access token (JWT stateless) e refresh token (opaco, 7 dias) mas não oferece logout: após "sair" no front, o refresh continua válido no banco e pode gerar novos access tokens por até 7 dias. É preciso revogar o refresh no servidor e encurtar a janela residual do access token.

## What Changes

- Novo endpoint `POST /api/auth/logout` autenticado (`Authorization: Bearer`), que revoga o refresh token do usuário logado (limpa `RefreshTokenHash` e `RefreshTokenExpiryTime`).
- Logout idempotente: chamar duas vezes retorna sucesso (sem erro quando já não há refresh armazenado).
- Redução de `JwtSettings:ExpirationInMinutes` de `120` para `60` em `appsettings.json` e `appsettings.Development.json`.
- Contrato do front (controlado pelo solicitante): chamar `POST /logout` com Bearer válido e só descartar o storage local (local/session) após `204/200`; em `401` (access expirado), apenas descartar localmente.

## Capabilities

### New Capabilities

- `auth-logout`: logout autenticado com revogação do refresh token da sessão única do usuário.

### Modified Capabilities

- Nenhuma (o comportamento de `GET /api/auth/me`, `GET /api/auth/user/{id}`, `POST /auth/access-token` e `POST /auth/refresh-token` permanece inalterado; a troca de expiração é mudança de configuração, detalhada em design/tasks).

## Impact

- Afetados: `UrlShortener.API/Endpoints/AuthEndpoints.cs` (nova rota), `UrlShortener.Application/Abstractions/IAuthService.cs` + `Services/AuthService.cs` (novo método `Logout`), `UrlShortener.API/appsettings.json` e `appsettings.Development.json` (expiração 60 min).
- Sem migrations (reusa colunas existentes de `User`); sem blacklist de access token (janela residual de até 60 min documentada como limitação aceita); sem suporte a múltiplas sessões por usuário (modelo de sessão única mantido).
- Limitação conhecida: logout exige access token válido; com access expirado o middleware retorna `401` antes do handler, de modo que um refresh órfão só expira naturalmente (documentado, fora de escopo resolver agora).

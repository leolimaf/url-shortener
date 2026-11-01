## Context

O módulo Auth (`UrlShortener.API/Endpoints/AuthEndpoints.cs` + `UrlShortener.Application/Services/AuthService.cs`) emite dois tokens no login (`POST /auth/access-token`): access JWT stateless (assinatura + expiração, hoje 120 min, validado em `Infrastructure/DependencyInjection.cs` com `ValidateLifetime` + signing key) e refresh opaco (GUID, hash SHA256 em `User.RefreshTokenHash` + `RefreshTokenExpiryTime` de 7 dias, com rotação a cada `POST /auth/refresh-token`). Há uma única coluna de refresh por `User` (sessão única) e nenhum endpoint de logout. O solicitante controla o front e decidiu: sessão única, logout autenticado via Bearer, expiração do access em 60 min, revogação do refresh (sem blacklist).

## Goals / Non-Goals

**Goals:**
- Encerrar a sessão no servidor ao sair: refresh deixa de ser utilizável para gerar novos pares de tokens.
- Manter o padrão existente (handlers finos no endpoint, regra no `AuthService`, `IUserRepository` + `IUnitOfWork` para persistência).
- Reduzir a janela residual do access token de 120 para 60 min via configuração.

**Non-Goals:**
- Blacklist/revogação do access JWT (sem `jti`, sem cache por request).
- Múltiplas sessões por usuário / "sair deste dispositivo" (requereria nova tabela de sessões).
- Logout anônimo via `{ refreshToken }` no body (fora de escopo; documentado como limitação).
- Mudanças em login, refresh, registro ou busca de usuário.

## Decisions

- **Revogar só o refresh (limpar colunas) em vez de blacklist do access.** Alternativa C (Redis/HybridCache + `jti`) daria invalidação imediata do access, mas custa um check por request e complexidade contínua; com expiração em 60 min e front controlado, a janela residual é aceitável. Decisão alinhada ao padrão RFC 7009 adaptado: o único token revogável com estado hoje é o refresh.
- **Endpoint autenticado sem body (`POST /api/auth/logout`, `RequireAuthorization()`).** Alternativa (aceitar `{ refreshToken }` anonimamente, como `RefreshTokens` faz) cobriria o caso "access expirado", mas o solicitante escolheu Bearer-only pela simplicidade e o handler reaproveita o padrão de `GetCurrentUser` (claim `NameIdentifier` → `long.TryParse` → `401` se inválido). Sem DTO de request.
- **Resposta `204 No Content` + idempotência.** Se o usuário não existe mais ou o refresh já é null, ainda retorna sucesso em vez de 404/erro — evita falha em duplo-clique/retry e espelha semântica de "sessão encerrada". `401` para sem token/inválido/expirado vem do middleware JWT, sem lógica custom.
- **Reuso de `GetUserById` + `SaveAsync` no service.** Novo método `Logout(userId)` no `IAuthService`: busca via `IUserRepository.GetUserById`, se null retorna falso (endpoint mapeia para sucesso idempotente), senão zera as duas colunas e salva. Sem novo repositório, sem migration.
- **Expiração 60 min nos dois `appsettings`.** Troca só de valor em `UrlShortener.API/appsettings.json` e `appsettings.Development.json`; sem mudança de código de geração (usa `_jwtSettings.ExpirationInMinutes`).

## Risks / Trade-offs

- [Risco] Access emitido antes do logout segue válido até ~60 min → Mitigação: janela documentada e aceita; front descarta o access imediatamente, encurtando exposição prática; refresh revogado impede renovação.
- [Risco] Logout com access expirado retorna `401` e não revoga refresh órfão (válido até 7 dias) → Mitigação: documentado como limitação; fluxo normal (front chama logout com access válido) cobre o caso comum; resolver exigiria rota anônima com refresh no body (futuro).
- [Risco] Sessão única: login em outro dispositivo invalida o anterior; logout derruba todos → Mitigação: comportamento explicitado no spec; aceito pelo solicitante.
- [Risco] Front descartar storage antes de confirmar o logout deixa refresh válido no servidor → Mitigação: contrato "só limpar após 2xx" registrado no spec como obrigação do client.

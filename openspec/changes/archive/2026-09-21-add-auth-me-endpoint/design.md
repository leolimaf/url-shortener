## Context

O módulo Auth (`UrlShortener.API/Endpoints/AuthEndpoints.cs`, grupo `/api/auth` sob `/api`) hoje expõe `POST /user`, `GET /user/{id:long}` (restrito a role `admin`), `POST /access-token` e `POST /refresh-token`. O access token carrega `ClaimTypes.NameIdentifier` (= `User.Id`), `Email` e `Role` (gerado em `AuthService.GenerateTokens`), validado pelo JWT Bearer configurado em `UrlShortener.Infrastructure/DependencyInjection.cs` (`ValidateLifetime` + signing-key; issuer/audience não validados por decisão intencional). Não há forma de um usuário comum (`role=user`) ler o próprio perfil. O DTO de saída já existe e é reutilizado: `GetUserResponse(Id, FullName, BirthDate, Phone, Email, IsEmailConfirmed)` em `UrlShortener.Application/DTOs/Auth/Responses/`, populado por `IAuthService.GetUserById`.

## Goals / Non-Goals

**Goals:**

- Expor `GET /api/auth/me` autenticado que retorna `GetUserResponse` do dono do access token.
- Reutilizar `IAuthService.GetUserById` sem alterar interface/serviço, mantendo handlers thin conforme convenção do projeto.
- Seguir o padrão existente de respostas (`Produces`, `TypedResults`) e autorização JWT.

**Non-Goals:**

- Não criar novo DTO, novo método de serviço, cache, paginação ou edição de perfil.
- Não alterar validação JWT, roles existentes, rotas existentes ou `GetUserResponse`.
- Não expor lookup de outros usuários (isso continua restrito a `GET /user/{id}` admin).

## Decisions

- **Rota `GET /me` dentro do grupo `/auth` (resultado final `GET /api/auth/me`), registrada antes de `/user/{id:long}` por legibilidade.**
  Rationale: atende ao pedido literal ("/me no módulo Auth"); sem conflito de rota pois o segmento é distinto de `/user/{id:long}`. Alternativa considerada (`GET /api/auth/user/me` sob o prefixo `/user`) — rejeitada porque colidiria conceitualmente com o parâmetro `{id:long}` e exigiria ordenação cuidadosa; `/me` direto é inequívoco.
- **Autorização `RequireAuthorization()` sem exigência de role (qualquer JWT válido: `user` ou `admin`).**
  Rationale: o propósito é "usuário autenticado vê a si mesmo"; restringir a `admin` reproduziria a limitação atual. Alternativa (`RequireRole("user","admin")`) — desnecessária, pois qualquer identidade autenticada já satisfaz; manter simples segue o default do `AddAuthorization()`.
- **Extrair identidade via `ClaimsPrincipal` injetado no handler (`ClaimsPrincipal user`) lendo `ClaimTypes.NameIdentifier` + `long.TryParse`; ausência/parse falho → `401 Unauthorized`.**
  Rationale: Minimal API faz binding de `ClaimsPrincipal` automaticamente; espelha exatamente o claim emitido em `GenerateTokens`. Alternativa (`HttpContext.User`) — equivalente, porém mais verbosa; alternativa (buscar por claim `Email`) — mais frágil (email pode mudar; ID é PK estável).
- **Reutilizar `IAuthService.GetUserById(id)`; `null` → `404 NotFound`.**
  Rationale: zero duplicação de mapeamento `User → GetUserResponse`; cobre o caso "conta excluída após emissão do token". Alternativa (novo método `GetCurrentUser`) — rejeitada: seria um wrapper idêntico sem valor.
- **Contrato de respostas: `200 GetUserResponse`, `401` (sem token / inválido / claim ausente), `404` (usuário removido), `500`. Handler nomeado `GetCurrentUser` com `.WithName(nameof(GetCurrentUser))`.**
  Rationale: consistência com os `.Produces(...)` das rotas vizinhas; `401` para identidade inutilizável (não `400`, pois o problema é credencial, não sintaxe). Nota (MS Docs, Minimal APIs + OpenAPI): `TypedResults` já anota metadata OpenAPI automaticamente, mas mantemos `.Produces(...)` explícitos para seguir a convenção do `AuthEndpoints.cs` existente.

## Risks / Trade-offs

- [Token válido de usuário excluído retorna 404 até expirar] → Mitigação: comportamento documentado na spec; sem refresh possível após exclusão se o refresh hash for limpo junto (fora do escopo).
- [Claim `NameIdentifier` ausente em token antigo/customizado → 401 mesmo com assinatura válida] → Mitigação: todos os tokens gerados por `GenerateTokens` incluem o claim; validação de parse defensiva no handler.
- [`ValidateIssuer=false/ValidateAudience=false` herdado] → Mitigação: nenhuma mudança aqui por decisão intencional registrada em AGENTS.md; endpoint herda a mesma postura sem ampliar superfície.

# Proposal

## Why

A solucao hoje e somente API sem nenhuma interface para o usuario final. Uma SPA de pagina unica coloca o encurtador no centro, permite encurtar URLs com ou sem login e oferece entrada/saida de conta via modais, aumentando adocao sem mudar o backend.

## What Changes

- Nova pasta `FrontWeb/`: SPA Angular standalone (v22 LTS) + Angular Material, pt-BR, pagina unica `/` focada no encurtador.
- Header com logo SVG (icone + "ENCURTADOR"), alternador de tema claro/escuro, menu Entrar/Cadastrar via `MatDialog`; quando logado exibe primeiro nome + Sair (além do alternador de tema).
- Formulario de encurtamento (ReactiveForms): cola URL, valida formato, `POST /api/url-shortener`, exibe card de resultado com link curto + botao Copiar + `MatSnackBar` de feedback.
- Auth via modais na mesma pagina: login (email+senha) e cadastro (nome, sobrenome, nascimento?, telefone?, email, senha); tokens em `localStorage`, interceptor Bearer + refresh automatico; logout limpa local e tenta `POST /api/auth/logout` quando o endpoint existir (fallback silencioso).
- Tema claro/escuro via theming do Material com persistencia em `localStorage` e respeito a `prefers-color-scheme` como default.
- Footer apenas com copyright + ano atual centralizados.
- `FrontWeb` entra no `compose.yaml` como servico `urlshortener.web` (build multi-stage Node -> nginx, `:8081`), com proxy `/api/` para a API. Dev local usa `proxy.conf.json`. CORS do backend fica fora deste escopo (sera configurado depois).

## Capabilities

### New Capabilities

- `frontweb-spa`: SPA de pagina unica — encurtar URL (anonimo/autenticado), auth via modais, sessao/token/tema, layout header/hero/footer, integracao compose/nginx.

### Modified Capabilities

- (nenhuma — nenhum requisito de comportamento do backend muda)

## Impact

- Novo: `FrontWeb/` (app Angular, Dockerfile, nginx.conf, `proxy.conf.json`), servico `urlshortener.web` no `compose.yaml`.
- Backend: nenhum codigo alterado; apenas consumo dos endpoints existentes (`POST /api/url-shortener`, `POST /api/auth/user`, `POST /api/auth/access-token`, `POST /api/auth/refresh-token`, futuro `POST /api/auth/logout`).
- Premissas/limites: sem endpoint "me"/"meus links" (sem perfil e sem historico servidor); sem CORS neste change; logout servidor ainda inexistente (frontend com fallback local).

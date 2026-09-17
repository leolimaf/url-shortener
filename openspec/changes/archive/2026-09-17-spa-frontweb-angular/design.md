# Design

## Context

- Backend atual: Minimal API sem CORS configurado; `POST /api/url-shortener` aceita chamada anonima e vincula `UserId` quando ha JWT (`UrlShortenerService`); auth via `POST /api/auth/user`, `/access-token` (120 min) e `/refresh-token` (7 dias, rotativo); sem endpoint "me"/"meus links" e sem logout ainda. Ver `UrlShortener.API/Endpoints/*`, `UrlShortener.Application/Services/*`.
- Proposta (ver `proposal.md`): nova pasta `FrontWeb/`, pagina unica `/`, auth em modais, sem mudanca no backend. CORS fica fora deste change.
- Restricoes acordadas: Angular standalone v22 LTS + Material, pt-BR, router com rota unica desde o dia 1 (Opcao A), tokens em `localStorage`, `FrontWeb` no `compose.yaml`.

## Goals / Non-Goals

**Goals:**

- SPA de pagina unica com encurtador como hero, usavel anonimamente e com sessoes persistentes.
- Auth em dialogs sem sair da pagina, com refresh automatico e logout com fallback local.
- Base pronta para crescer (nova rota guardada futura) sem reescrita.
- Deploy local e em compose com `/api` funcionando sem configuracao manual.

**Non-Goals:**

- Configurar CORS no backend (sera feito depois, fora daqui).
- Endpoints novos ("me", "meus links", logout servidor) — o frontend apenas tolera/antecipa o logout.
- i18n multi-idioma, SSR, PWA, analytics, testes automatizados do frontend.

## Decisions

- **Angular 22 standalone + SSR desligado (`--ssr=false --routing=true --style=scss`).** Rationale: versao estavel atual (suporte ate jun/2028); standalone + signals + `inject()` + `OnPush` e o default moderno e reduz boilerplate para uma pagina. Alternativa (modulos NgModule / versao 20-21) rejeitada por gerar estrutura legada e janela de suporte menor.
- **Router desde o dia 1 com uma rota (`{ path: '', component: ShortenerPage }`).** Rationale: custo quase zero hoje, evita refactor quando "meus links" chegar; auth permanece em `MatDialog` (exigencia de mesma pagina), nao em rotas. Alternativa sem router rejeitada (economia minima, refactor certo).
- **Camadas `core / features / shared`:** `core/services` (`auth`, `url-shortener`, `token-storage`, `theme`), `core/interceptors/auth` (Bearer + refresh+retry), `core/layout` (header/footer), `features/shortener` (page/form/result), `features/auth` (login-dialog/register-dialog), `environments/*` com `apiUrl`. Rationale: segue convencoes Angular e separa singletons de UI; guards pastas criadas mas sem guard ativo (sem rota privada ainda). `proxy.conf.json` no dev espelha o nginx do compose.
- **Tokens em `localStorage` (`urlshortener.accessToken`, `urlshortener.refreshToken`) + tema em chave separada (`urlshortener.theme`).** Rationale: refresh de 7 dias implica expectativa de persistencia — memoria morre no F5 e `sessionStorage` morre ao fechar o browser; `localStorage` e o pragmatico para este perfil de risco (sem dados sensiveis alem do token, access curto, templates escapados por padrao). Cookie HttpOnly seria melhor contra XSS mas exige backend novo — registrado como evolucao. Logout: tenta `POST /api/auth/logout` se existir, ignora 404/501 e sempre limpa local.
- **Material 3 com dois temas (`light`/`dark` via classe no `document.body`), default `prefers-color-scheme`.** Componentes: `mat-toolbar` + `mat-menu` (header), `mat-card` + `mat-form-field` + `mat-error` (form/resultado), `mat-dialog` + tabs/link alternando login/cadastro, `mat-snack-bar` (copiado/erros), `mat-icon` + `mat-button`/`mat-icon-button` (tema, copiar). `MAT_DATE_LOCALE='pt-BR'`, `mat-datepicker` apenas se adotado no cadastro (nascimento pode ser input date nativo). Rationale: cobre todos os requisitos com um sistema de design so; dark/light sem duplicar assets.
- **Logo como asset unico `assets/branding/logo.svg` (icone elo + texto ENCURTADOR) desenhado com `currentColor`, + `favicon` derivado.** Rationale: atende "tratar em imagem" e ainda herda a cor do tema sem manter duas versoes; `<img>` no toolbar facilita troca futura.
- **Compose: `urlshortener.web` (multi-stage Node 22 build -> nginx alpine), `8081:80`, `depends_on: api`; nginx com `location /api/ { proxy_pass http://urlshortener.api:8080/api/; }` e fallback SPA (`try_files $uri $uri/ /index.html`). Frontend usa URL relativa por default (`apiUrl: '/api'`), com `environment.development.ts` apontando para o proxy local.** Rationale: elimina configuracao de URL pelo usuario e reduz dependencia do CORS dentro do compose; dev (`ng serve --proxy-config`) e prod tem o mesmo formato de chamada. Alternativa com `API_URL` build-arg rejeitada (rebuild por ambiente).
- **Erros mapeados no cliente:** `Uri.TryCreate`-like via `new URL()` + mensagens pt-BR; 400 do backend exibido traduzido, 401 dispara refresh uma vez, resto cai em snack de erro generico mantendo o input. Copiar usa `navigator.clipboard` com fallback `execCommand`.

## Risks / Trade-offs

- [XSS le `localStorage`] → Mitigacao: so tokens guardados, access de 120 min, refresh rotativo, sem `innerHTML`, limpeza total no logout; migracao para cookie HttpOnly documentada como evolucao.
- [Sem CORS, acesso direto browser->API em origem distinta falha] → Mitigacao: dev e compose usam proxy `/api` (mesma origem); CORS segue como pendencia externa.
- [Sem endpoint "me", nome exibido vem do JWT decodificado] → Mitigacao: decodificacao local apenas para exibicao (sem autoridade); quando "me" existir, trocar a fonte sem mudar layout.
- [Logout servidor ainda nao existe] → Mitigacao: `logout()` tenta o POST e degrada para limpeza local; nenhum erro visivel por 404.
- [Clipboard API pode ser negada (HTTP/permissao)] → Mitigacao: fallback legado + snack orientando copia manual.
- [Deriva de versao Angular/Node] → Mitigacao: pinnar `22.x` no `package.json` + Node 22 na imagem de build.

## Migration Plan

- Aditivo: nenhum arquivo existente e alterado exceto `compose.yaml` (novo servico). Rollback = remover servico `urlshortener.web` / parar container; API e dados inalterados.
- Ordem de deploy: subir `api` primeiro (health), depois `web`; validacao: abrir `:8081`, encurtar anonimo, login, encurtar logado, F5 (sessao mantida), alternar tema, copiar link.

## Open Questions

- Nenhuma que trave specs ou tasks. Acompanhar: formato final do `POST /api/auth/logout` (corpo/resposta) quando o backend o criar — o hook do frontend ja absorve qualquer formato via fallback.

# Tasks

## 1. Scaffold e base

- [x] 1.1 Criar projeto Angular 22 standalone em `FrontWeb/` (`ng new --standalone --routing --style=scss --ssr=false`) e verificar `ng version` e `npm start` sobem a pagina default
- [x] 1.2 Adicionar Angular Material + tema claro/escuro com persistencia e verificar alternancia aplica `light`/`dark` no `body` e sobrevive ao F5
- [x] 1.3 Configurar `app.routes.ts` com rota unica, `environments` (`apiUrl: '/api'`), `proxy.conf.json` e verificar `GET /api/url-shortener/xxx` proxia para a API local

## 2. Core (sessao, http, layout)

- [x] 2.1 Implementar `token-storage`, `auth.service` (login/cadastro/refresh/logout com fallback), `url-shortener.service` e verificar login persiste apos F5 e logout limpa tudo
- [x] 2.2 Implementar interceptor Bearer + refresh+retry e verificar chamada autenticada envia `Authorization` e 401 com refresh valido repete a chamada
- [x] 2.3 Implementar header (logo SVG + ENCURTADOR, toggle tema, menu Entrar/Cadastrar ou Nome/Sair) e footer (copyright + ano centralizado) e verificar estados logado/deslogado e temas

## 3. Encurtador e auth UI

- [x] 3.1 Implementar pagina/form/resultado do encurtador (validacao, `POST /api/url-shortener`, card + Copiar + snackbars pt-BR) e verificar encurtamento anonimo e autenticado exibem link curto copiavel
- [x] 3.2 Implementar dialogs de login e cadastro (ReactiveForms, erros inline pt-BR, conflito de email, sucesso) e verificar login invalido mostra "Credenciais invalidas." e cadastro duplo mostra conflito sem fechar
- [x] 3.3 Criar `assets/branding/logo.svg` (icone + ENCURTADOR em `currentColor`) + favicon e verificar renderizacao correta nos dois temas

## 4. Compose e verificacao final

- [x] 4.1 Adicionar `FrontWeb/Dockerfile` (Node 22 build + nginx), `nginx.conf` (proxy `/api/` + fallback SPA) e servico `urlshortener.web` (`8081:80`) no `compose.yaml` e verificar `docker compose up -d --build` serve a SPA e encurta via API do compose
- [x] 4.2 Verificacao ponta a ponta: abrir a pagina, encurtar anonimo, cadastrar/logar via modais, encurtar logado, F5 (sessao mantida), alternar tema, copiar link, Sair — e verificar cada comportamento sem erros no console

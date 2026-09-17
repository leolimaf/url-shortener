# Spec Delta

## Purpose

Oferecer uma pagina web unica em portugues que coloca o encurtador de URLs no centro e permite encurtar com ou sem conta, com entrada e cadastro via modais sem sair da pagina.

## ADDED Requirements

### Requirement: Pagina unica focada no encurtador

A pagina inicial SHALL apresentar o encurtador como conteudo principal, em uma unica rota, sem navegacao obrigatoria para encurtar uma URL.

#### Scenario: Acesso direto ao encurtador

- **WHEN** o usuario abre a raiz do frontend
- **THEN** o formulario de encurtamento esta visivel sem login previo e sem trocar de pagina

#### Scenario: Foco permanece na pagina apos auth

- **WHEN** o usuario conclui login ou cadastro no modal
- **THEN** o modal fecha e o usuario permanece na mesma pagina com o encurtador visivel

### Requirement: Encurtar URL anonima ou autenticada

O sistema SHALL enviar a URL longa para `POST /api/url-shortener` e, quando houver sessao valida, anexar o token de acesso para que o backend vincule o link ao usuario; sem sessao, a chamada SHALL funcionar sem token.

#### Scenario: Encurtamento anonimo

- **WHEN** um visitante sem sessao informa uma URL absoluta valida e aciona Encurtar
- **THEN** o sistema chama `POST /api/url-shortener` sem header de autorizacao e exibe o codigo/URL curta retornado

#### Scenario: Encurtamento autenticado

- **WHEN** um usuario logado informa uma URL valida e aciona Encurtar
- **THEN** a requisicao inclui o access token vigente e o resultado e exibido da mesma forma

### Requirement: Validacao e erros do encurtador

O sistema SHALL validar o formato da URL no cliente e propagar erros do backend de forma compreensivel em pt-BR, sem perder o valor digitado.

#### Scenario: URL com formato invalido

- **WHEN** o usuario informa texto que nao e URL absoluta e aciona Encurtar
- **THEN** o sistema exibe erro "Formato de URL invalido." e nenhuma requisicao e enviada

#### Scenario: Backend rejeita a URL

- **WHEN** o backend responde 400 (ex. `Invalid URL format.`)
- **THEN** o sistema exibe a mensagem de erro em pt-BR e mantem a URL digitada no campo

#### Scenario: Falha de rede ou 500

- **WHEN** a requisicao falha por rede ou erro interno
- **THEN** o sistema exibe erro generico em pt-BR e permite tentar novamente

### Requirement: Resultado e copia do link curto

Apos encurtar com sucesso o sistema SHALL exibir o link curto completo e oferecer copia em um clique com confirmacao visual.

#### Scenario: Exibicao do resultado

- **WHEN** o backend retorna `{ "code": "<codigo>" }`
- **THEN** o sistema exibe o link curto (base + codigo), a URL original e um botao Copiar

#### Scenario: Copiar link

- **WHEN** o usuario aciona Copiar
- **THEN** o link curto e copiado para a area de transferencia e uma confirmacao ("Link copiado!") e exibida

### Requirement: Header com marca, tema e menu de conta

O header SHALL exibir a logo (icone + "ENCURTADOR"), um alternador de tema claro/escuro e um menu de conta com Entrar e Cadastrar; quando logado, SHALL exibir o primeiro nome e a opcao Sair (mantendo o alternador de tema).

#### Scenario: Visitante ve opcoes de conta

- **WHEN** um visitante sem sessao abre a pagina
- **THEN** o header mostra logo, alternador de tema e menu com Entrar e Cadastrar

#### Scenario: Usuario logado ve nome e sair

- **WHEN** ha sessao valida
- **THEN** o header mostra logo, alternador de tema, o primeiro nome do usuario e a opcao Sair (sem exibir Entrar/Cadastrar)

#### Scenario: Alternar tema

- **WHEN** o usuario alterna claro/escuro
- **THEN** a pagina inteira aplica o tema imediatamente e a preferencia persiste entre recarregamentos, respeitando `prefers-color-scheme` apenas na primeira visita

### Requirement: Login via modal

O sistema SHALL oferecer login por email e senha em modal, validar campos, exibir "Credenciais invalidas." quando o backend rejeitar e fechar o modal com sessao ativa em caso de sucesso.

#### Scenario: Login com sucesso

- **WHEN** o usuario informa email valido e senha e o backend retorna access/refresh tokens
- **THEN** o modal fecha, a sessao e persistida e o header passa ao estado logado

#### Scenario: Login com credenciais invalidas

- **WHEN** o backend responde 400/nulo para as credenciais
- **THEN** o sistema exibe "Credenciais invalidas." no modal sem fecha-lo

#### Scenario: Validacao do formulario de login

- **WHEN** email tem formato invalido ou algum campo esta vazio
- **THEN** o botao de entrar permanece desabilitado ou erro inline e exibido, sem chamar a API

### Requirement: Cadastro via modal

O sistema SHALL oferecer cadastro em modal com nome, sobrenome, data de nascimento (opcional), telefone (opcional), email e senha; email duplicado SHALL resultar em mensagem de conflito sem fechar o modal.

#### Scenario: Cadastro com sucesso

- **WHEN** o usuario preenche os campos obrigatorios com email valido e o backend responde 201
- **THEN** o modal indica sucesso ("Conta criada! Faca login.") e oferece ir ao login

#### Scenario: Email ja cadastrado

- **WHEN** o backend sinaliza conflito (email existente)
- **THEN** o sistema exibe "Este email ja esta cadastrado." sem fechar o modal

#### Scenario: Validacao do cadastro

- **WHEN** campos obrigatorios estao vazios ou email/senha nao atendem ao formato minimo
- **THEN** erros inline em pt-BR sao exibidos e nenhuma requisicao e enviada

### Requirement: Sessao persistente com refresh e logout

O sistema SHALL persistir a sessao entre recarregamentos, anexar o access token vigente as chamadas, renovar via refresh token quando necessario e encerrar a sessao no logout limpando o armazenamento local.

#### Scenario: Sessao sobrevive ao recarregar

- **WHEN** o usuario recarrega ou reabre o navegador com tokens validos
- **THEN** o header permanece no estado logado sem novo login

#### Scenario: Refresh automatico

- **WHEN** uma chamada falha por access token expirado e ha refresh token valido
- **THEN** o sistema renova o par de tokens via `POST /api/auth/refresh-token`, repete a chamada original e so falha visivelmente se o refresh tambem falhar

#### Scenario: Logout

- **WHEN** o usuario aciona Sair
- **THEN** o sistema tenta encerrar no servidor quando o endpoint existir (ignorando indisponibilidade), limpa os tokens locais e volta o header ao estado de visitante

### Requirement: Footer minimalista

O footer SHALL exibir apenas copyright e ano atual centralizados.

#### Scenario: Exibicao do footer

- **WHEN** a pagina e renderizada em qualquer estado (logado ou nao)
- **THEN** o rodape mostra "[ano atual]" centralizado e nenhum outro conteudo navegacional

### Requirement: Interface em portugues

Toda a interface, validacoes e mensagens exibidas pelo frontend SHALL estar em pt-BR.

#### Scenario: Textos em pt-BR

- **WHEN** o usuario usa qualquer parte da pagina ou modal
- **THEN** rotulos, placeholders, erros e confirmacoes aparecem em portugues do Brasil

### Requirement: Servico web no compose

O frontend SHALL ser servido como servico `urlshortener.web` no `compose.yaml`, com chamadas a `/api/*` roteadas a API sem exigir configuracao manual de URL pelo usuario final.

#### Scenario: Compose sobe web + api

- **WHEN** `docker compose up -d` e executado
- **THEN** a SPA responde na porta publicada do servico web e encurtar/logar funciona contra a API do compose

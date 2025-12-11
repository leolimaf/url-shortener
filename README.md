# 🚀 UrlShortener - Encurtador de URLs

## 📚 Visão Geral
O UrlShortener converte URLs longas em versões mais curtas, limpas e fáceis de compartilhar. É ideal para uso em plataformas com limite de caracteres, para organizar links ou simplesmente para melhorar a experiência do usuário ao reduzir informações desnecessárias.

## ▶️ Execução do Projeto
Com o Docker Compose, você pode iniciar toda a aplicação. Certifique-se de ter o Docker instalado e execute na raiz da solução:

```bash
docker compose up -d
```

O ambiente é composto pelos seguintes serviços:
 - Banco de dados principal da aplicação
 - Cache para otimizar a resolução das URLs
 - Plataforma de monitoramento para visualizar logs estruturados
 - API responsável pelo encurtamento e redirecionamento das URLs
 - Interface Web amigável para interação com o usuário
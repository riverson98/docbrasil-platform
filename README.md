# DocBrasil Platform

> Plataforma full-stack para gestão de cooperativa/associação, em microsserviços .NET + Angular 17. Em produção em [appdocdobrasil.com.br](https://appdocdobrasil.com.br).

![Status](https://img.shields.io/badge/status-em%20produção-success)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![Angular](https://img.shields.io/badge/Angular-17-DD0031)
![License](https://img.shields.io/badge/license-MIT-blue)

## Stack

- **Backend:** C# .NET 8, Clean Architecture, DDD, JWT
- **Frontend:** Angular 17, Angular Material, RxJS
- **Banco:** MySQL 8
- **Infra:** Docker Compose, NGINX (reverse proxy + SSL), Let's Encrypt
- **CI/CD:** Azure DevOps Pipelines, Azure Key Vault

## Arquitetura

Microsserviços orquestrados via Docker Compose:

- `api-gateway` — entrada única
- `identity-service` — autenticação JWT, gestão de usuários
- `associados-service` — domínio principal
- `frontend` — SPA Angular
- `nginx` — reverse proxy + terminação SSL
- `mysql-domain` / `mysql-auth` — bancos isolados por serviço

## Como rodar localmente

```bash
git clone https://github.com/riverson98/docbrasil-platform.git
cd docbrasil-platform
cp .env.example .env       # preencha com seus valores
docker compose up -d
```

Acesse `http://localhost:8080`.

## Estrutura do projeto
backend/ 
DocAssociados/              # serviço principal 
DocAssociados.ApiGateway/   # gateway 
DocAssociados.Identity/     # autenticação 
frontend/doc-brasil/          # Angular 17 app 
nginx/                        # configs do reverse proxy 

## Autor

[Riverson Costa](https://github.com/riverson98) — [LinkedIn](https://linkedin.com/in/riverson-dev)

## Licença

MIT

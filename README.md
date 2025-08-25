# TaskHub — API Kanban minimalista com autenticação JWT

TaskHub é uma API em ASP.NET Core para gerenciamento de quadros Kanban, com suporte a autenticação JWT, versionamento de rotas e documentação via Swagger. O domínio contempla Boards, Columns, Cards, Labels e Comments, permitindo organizar tarefas por quadros e listas.

## ✨ Funcionalidades

- 🧭 API Minimal ASP.NET Core com versionamento de rotas (`/api/v1`)
- 🔐 Autenticação JWT (login/registro) e autorização por política (`AdminOnly`)
- 🧱 Domínio Kanban: Boards, Columns, Cards, Labels e Comments
- 🗃️ Persistência com Entity Framework Core (banco em memória para desenvolvimento)
- ✅ Validações com FluentValidation (auto-validation integrada)
- 🩹 Tratamento de erros padronizado via ProblemDetails
- 📚 Documentação interativa com Swagger/OpenAPI (ambiente de desenvolvimento)
- 🧪 Testes (inclui fluxo end-to-end com criação isolada de banco por teste)

## 🧰 Tecnologias e bibliotecas

- .NET 9 / ASP.NET Core (Minimal APIs)
- Entity Framework Core (InMemory para DEV/testes)
- Microsoft.IdentityModel.Tokens + JWT Bearer
- FluentValidation + AutoValidation
- Swagger (Swashbuckle)
- xUnit + FluentAssertions para testes

## 🚀 Como executar localmente

1) Pré-requisitos:
- .NET SDK 9 instalado

2) Executar a API:
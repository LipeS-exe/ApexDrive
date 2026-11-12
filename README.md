# ApexDrive API — Sistema de Revenda Automotiva

API RESTful desenvolvida em **C# (ASP.NET Core Web API)** com banco de dados **PostgreSQL** e **Entity Framework Core**. O projeto gerencia o catálogo de marcas, estoque de veículos, pacotes de opcionais, carteira de clientes e propostas de financiamento automotivo.

---

## Tecnologias e Arquitetura

* **Runtime & Linguagem:** .NET 10(C#)
* **Banco de Dados:** PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`)
* **ORM:** Entity Framework Core (Code-First & Migrations)
* **Padrões e Conceitos Aplicados:**
  * **Repository Pattern:** Isolamento total da camada de acesso a dados (`Interfaces` + `Implementations`).
  * **Injeção de Dependência (3 Lifetimes):**
    * `AddScoped`: Repositórios de domínio e `AppDbContext`.
    * `AddTransient`: Serviço de cálculo de parcelas e juros (`CalculadoraJurosService`).
    * `AddSingleton`: Contador global de acessos em memória (`ContadorAcessosService`).
  * **Programação Assíncrona:** Uso estrito de `async/await` (`Task<T>`, `ToListAsync`, `SaveChangesAsync`).
  * **Relacionamentos EF Core:**
    * **1:N (Um para Muitos):** `Marca -> Veiculos` e `Cliente -> Propostas`.
    * **N:N (Muitos para Muitos):** `Veiculo <-> Opcionais`.
* **Documentação Interativa:** Swagger / OpenAPI

---

## Divisão da Equipe e Módulos

O projeto foi dividido em 5 fatias verticais para desenvolvimento paralelo:

| Integrante             | Módulo                                 | Responsabilidade Principal                                                                                                   |
| :--------------------- | :-------------------------------------- | :--------------------------------------------------------------------------------------------------------------------------- |
| **Integrante 1** | **Marcas (`Marca`)**            | CRUD de Marcas, Relacionamento **1:N** com Veículos e configuração do `AppDbContext`.                             |
| **Integrante 2** | **Opcionais (`Opcional`)**      | CRUD de Acessórios, Relacionamento **N:N** com Veículos e serviço `AddSingleton` (`ContadorAcessosService`).    |
| **Integrante 3** | **Veículos (`Veiculo`)**       | CRUD de Veículos e endpoints de vinculação **N:N** (`Veiculo <-> Opcional`).                                      |
| **Integrante 4** | **Clientes (`Cliente`)**        | CRUD de Clientes, validação de CPF único e histórico **1:N** de Propostas do cliente.                              |
| **Integrante 5** | **Propostas (`PropostaVenda`)** | Registro e aprovação de propostas, baixa automática de estoque e serviço `AddTransient` (`CalculadoraJurosService`). |

---

# Diário de Bordo

## Dia 16 de Novembro

- Criação das  diarioBordo, models, service, controllers
- Criação Marca.cs(models)
- Container para rodar postgreSQL no pc de todos(Irei colocar comandos para todos conseguirem utilizar)
- Adicione a biblioteca do PostgreSQL (`Npgsql`)

## Dia 17 de Novembro

- Criação do arquivo `.env.example` na raiz do projeto como modelo de variáveis de ambiente para a equipe (mantendo o `.env` real protegido no `.gitignore`)
- Configuração do `docker-compose.yml` com a imagem `postgres:17`, mapeamento da porta `5432` e volume persistente (`apexdrive_pgdata`)
- Criação da pasta `Data/` e da classe `AppDbContext.cs` herdando de `DbContext` (Entity Framework Core) e mapeando a tabela `Marcas` (`DbSet<Marca>`)
- Configuração da `ConnectionStrings:DefaultConnection` no `appsettings.Development.json` e registro do `AppDbContext` no `Program.cs` usando `UseNpgsql`
- Instalação do pacote `Microsoft.EntityFrameworkCore.Design` e da ferramenta `dotnet-ef` para gerenciamento de Migrations
- Geração da primeira Migration (`CriacaoBancoDados`) e aplicação no banco PostgreSQL via Docker (`dotnet ef database update`), validando a criação da tabela `Marcas`
- Documentação no `Aviso.md` com o passo a passo e comandos para os demais integrantes subirem o banco na própria máquina



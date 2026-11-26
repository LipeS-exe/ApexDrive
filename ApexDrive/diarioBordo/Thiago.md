# Diário de Bordo - Thiago (Integrante 3 - Veículos)

## Dia 25 de Novembro

- Criação do `Veiculo.cs` (models) e do enum `StatusVeiculo.cs` (Disponivel, Reservado, Vendido)
- Criação do `VeiculoDto.cs` para receber os dados do POST e do PUT
- Criação do `IVeiculoRepository.cs` e do `VeiculoRepository.cs` (acesso ao banco com async/await)
- Criação do `IVeiculoService.cs` e do `VeiculoService.cs` (regras de negócio)
- Criação do `VeiculosController.cs` com o CRUD e os endpoints de vincular/desvincular opcional (N:N)
- No `AppDbContext.cs`: `DbSet<Veiculo>`, `DbSet<Opcional>`, relacionamento 1:N com Marca, N:N com Opcional (tabela `VeiculoOpcional`) e índice único na Placa
- No `Marca.cs`: liberei a linha `ICollection<Veiculo> Veiculos` que estava comentada
- No `Opcional.cs`: adicionei `ICollection<Veiculo> Veiculos` (lado do Opcional no N:N)
- No `Program.cs`: registro do Repository e do Service com `AddScoped` e configuração do JSON (`IgnoreCycles`)
- Criação do `Veiculos.http` com os testes de cada endpoint

---

# Material de estudo para a apresentação

## 1. O caminho de uma requisição

```
Cliente (Swagger) -> VeiculosController -> IVeiculoService / VeiculoService
                  -> IVeiculoRepository / VeiculoRepository -> AppDbContext -> PostgreSQL
```

| Camada | Arquivo | O que faz |
| --- | --- | --- |
| Cliente | Swagger ou `Veiculos.http` | Manda a requisição HTTP |
| Controller | `VeiculosController.cs` | Recebe o HTTP e devolve o código de status (200, 201, 204, 400, 404) |
| Service | `VeiculoService.cs` | Aplica as regras de negócio (placa única, preço, ano, marca existe) |
| Repository | `VeiculoRepository.cs` | Busca e grava no banco usando o `AppDbContext` |
| AppDbContext | `AppDbContext.cs` | É o EF Core: transforma o C# em SQL |
| Banco | PostgreSQL (Docker) | Guarda os dados |

A resposta faz o caminho de volta.

## 2. Injeção de dependência

No `Program.cs` eu registrei as minhas classes:

```csharp
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();
```

Quer dizer: "quando alguém pedir um `IVeiculoRepository`, entregue um `VeiculoRepository`".

Cada classe pede o que precisa no construtor:

- O `VeiculosController` pede um `IVeiculoService`
- O `VeiculoService` pede um `IVeiculoRepository`
- O `VeiculoRepository` pede o `AppDbContext`

Ninguém usa `new`. Quando chega uma requisição, o ASP.NET monta essa corrente sozinho, de trás pra frente.

**Por que `AddScoped`?** Cria um objeto por requisição. O `AppDbContext` também é Scoped, então o Repository e o Service acompanham ele.

**Por que usar interface?** O Controller conhece só o `IVeiculoService`, não a classe. Se um dia trocar a implementação (por exemplo, um repositório para SQL Server em vez de PostgreSQL), muda só a linha do `Program.cs`.

## 3. Os endpoints, passo a passo

### GET /api/veiculos (listar)
1. O Swagger manda o GET.
2. O roteamento cai no `GetVeiculos()` do `VeiculosController`.
3. O Controller chama `_service.ListarAsync()`.
4. O Service chama `_repository.ListarAsync()`.
5. O Repository faz `_context.Veiculos.Include(Marca).Include(Opcionais).ToListAsync()`. O EF gera o `SELECT` com `JOIN`.
6. Volta a lista e o Controller devolve **200 OK** com o JSON.

### GET /api/veiculos/{id} (buscar um)
1. O `id` da URL entra no parâmetro do `GetVeiculo(int id)`.
2. Controller -> Service -> Repository, que faz `FirstOrDefaultAsync(v => v.Id == id)`.
3. Achou: **200 OK**. Não achou (veio `null`): **404 Not Found**.

### POST /api/veiculos (cadastrar)
1. O JSON do corpo vira um `VeiculoDto` (desserialização).
2. O `[ApiController]` confere as validações do DTO (`[Required]`, `[StringLength]`). Se falhar, devolve **400** sozinho.
3. O Controller chama `_service.CriarAsync(dto)`.
4. O Service deixa a placa em maiúsculo e valida as regras. Se alguma falhar, lança `RegraDeNegocioException`.
5. Tudo certo: o Service monta o `Veiculo`, coloca `Status = Disponivel` e `DataCadastro = DateTime.UtcNow`.
6. O Repository faz `AddAsync` e depois `SaveChangesAsync`. Só no `SaveChanges` o `INSERT` vai pro banco.
7. O Controller devolve **201 Created** com o `CreatedAtAction`, que coloca o endereço do novo carro no cabeçalho `Location`.
8. Se veio a exceção: o `catch` devolve **400 Bad Request** com a mensagem.

### PUT /api/veiculos/{id} (atualizar)
1. O Service busca o carro pelo id. Não achou: **404**.
2. Valida as regras. A placa é conferida ignorando o próprio carro (`idIgnorado`), senão ele "conflitaria com ele mesmo".
3. Copia os campos do DTO para o carro que veio do banco.
4. `SaveChangesAsync`: o EF compara com o que tinha antes (change tracking) e faz o `UPDATE`.
5. Devolve **204 No Content**. Regra quebrada: **400**.

### DELETE /api/veiculos/{id} (excluir)
1. O Service busca o carro. Não achou: **404**.
2. O Repository faz `Remove` e depois `SaveChangesAsync`, que gera o `DELETE`.
3. Devolve **204 No Content**. As linhas dele na tabela `VeiculoOpcional` saem junto.

### POST /api/veiculos/{veiculoId}/opcionais/{opcionalId} (vincular N:N)
1. O Service busca o carro (com os opcionais dele) e o opcional.
2. Algum não existe: **404**.
3. O carro já tem esse opcional: **400** (RN04).
4. `veiculo.Opcionais.Add(opcional)` e `SaveChangesAsync`. O EF grava uma linha na tabela `VeiculoOpcional` (VeiculosId, OpcionaisId).
5. Devolve **204 No Content**.

O DELETE na mesma rota faz o contrário: `veiculo.Opcionais.Remove(opcional)` e o EF apaga a linha da tabela de ligação.

## 4. Relacionamentos

- **N:1 com Marca:** o `Veiculo` tem `MarcaId` (chave estrangeira) e `Marca` (navegação). A `Marca` tem `ICollection<Veiculo> Veiculos`. Uma marca, vários carros.
- **N:N com Opcional:** o `Veiculo` tem `ICollection<Opcional> Opcionais` e o `Opcional` tem `ICollection<Veiculo> Veiculos`. No banco não dá para ter "lista" dentro de uma linha, então o EF cria a tabela do meio `VeiculoOpcional`.
- **Include:** por padrão o EF não traz os dados relacionados. O `.Include(v => v.Marca)` manda ele fazer o `JOIN` e preencher a marca.
- **OnDelete Restrict:** não deixa apagar uma marca que ainda tem carros (RN03).

## 5. Regras de negócio (ficam no Service)

| Regra | Onde | Resposta |
| --- | --- | --- |
| Placa não pode repetir (RN01) | `ValidarRegrasAsync` + índice único no banco | 400 |
| Preço maior que zero (RN02) | `ValidarRegrasAsync` | 400 |
| Ano de fabricação até o ano que vem (RN02) | `ValidarRegrasAsync` | 400 |
| Ano do modelo igual ao de fabricação ou +1 | `ValidarRegrasAsync` | 400 |
| Marca precisa existir | `ValidarRegrasAsync` | 400 |
| Mesmo opcional duas vezes no carro (RN04) | `VincularOpcionalAsync` | 400 |

## 6. Perguntas que o professor pode fazer

**Por que DTO?** Para o cliente não mandar o que não deve, como `Id`, `Status` e `DataCadastro`. Quem define esses campos é a API.

**Por que async/await?** Enquanto espera o banco responder, a thread fica livre para atender outra requisição. O servidor aguenta mais gente ao mesmo tempo.

**O que acontece se esquecer o `SaveChangesAsync`?** O `Add` e o `Remove` só anotam a mudança. Sem o `SaveChanges`, nada vai pro banco, mesmo a API respondendo 201.

**Por que `DateTime.UtcNow`?** O PostgreSQL guarda em `timestamp with time zone` e o Npgsql exige data em UTC.

**O que é o `IgnoreCycles` no Program.cs?** O carro aponta para a marca e a marca aponta para os carros. Sem isso, o JSON entraria em loop infinito.

**Por que a placa é validada no Service e também tem índice único no banco?** O Service dá uma mensagem amigável (400). O índice é a garantia final, caso duas requisições cheguem ao mesmo tempo.

**Qual a diferença entre Service e Repository?** O Service decide **se pode** (regra). O Repository sabe **como** buscar e gravar (banco).

## 7. Comandos para rodar a minha parte

```bash
cd ApexDrive
docker compose up -d
dotnet ef migrations add AdicionaVeiculosEOpcionais
dotnet ef database update
dotnet run
```

A migration cria as tabelas `Veiculos`, `Opcionais` e `VeiculoOpcional`. Combinar com o João para ele **não** gerar outra migration criando a tabela `Opcionais`.

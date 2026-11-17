# !!!Aviso!!! 
Essa pasta como  o nome "diarioBordo" deve ser utilizada como o material de estudo para nós no dia da apresentação, então utilizem o modelo que foi feito por mim (Felipe), podem utilizar I.A para isso e cuidados parar não deixar o código exposto ou prejudicar o projeto **Utilizem I.A com Consciência** 

---

## 🐘 Como rodar o Banco de Dados (PostgreSQL + Docker) na sua máquina

**Pré-requisitos:** Ter o **Docker** (Docker Desktop no Windows/Mac ou Docker + Docker Compose no Linux) e o **.NET 10 SDK** instalados.

### 1. Atualize o projeto com a versão mais recente do Git
```bash
git pull
```

### 2. Crie o arquivo `.env` na raiz do projeto (`Projeto-IpexDrive/`)
Copie o conteúdo do arquivo `.env.example` para um novo arquivo chamado `.env` na raiz do projeto:

- **No Linux / Mac / Git Bash:**
  ```bash
  cp .env.example .env
  ```
- **No PowerShell (Windows):**
  ```powershell
  copy .env.example .env
  ```

### 3. Suba o container do PostgreSQL com Docker
Entre na pasta `ApexDrive/` e inicie o banco em segundo plano:
```bash
cd ApexDrive
docker compose up -d
```
*(Para conferir se o container subiu, rode `docker compose ps` — o status deve estar como `Up`).*

> **⚠️ Atenção (Caso dê erro de porta 5432 ocupada ou autenticação):**
> Se você já tiver outro PostgreSQL instalado direto no seu Windows/Linux, pare o serviço nativo do PostgreSQL para liberar a porta `5432` para o Docker (no Linux: `sudo systemctl disable --now postgresql`).

### 4. Instale a ferramenta do Entity Framework (Apenas na 1ª vez no seu PC)
```bash
dotnet tool install --global dotnet-ef
```

### 5. Restaure os pacotes e crie as tabelas no banco do Docker
Ainda dentro da pasta `ApexDrive/`, rode:
```bash
dotnet restore
dotnet ef database update
```
*(Quando aparecer `Done.` no final, sua tabela `Marcas` e as demais já estarão criadas no seu container!)*

### 6. Comandos úteis no dia a dia
- **Rodar a API:**
  ```bash
  dotnet run
  ```
- **Acessar o banco pelo terminal para ver as tabelas:**
  ```bash
  docker exec -it apexdrive_postgres psql -U apeXdrive_us0r -d ApexDrIve_dB
  ```
  *(Dentro do banco: use `\dt` para listar as tabelas, `SELECT * FROM "Marcas";` para consultar e `\q` para sair).*
- **Desligar o container do banco quando terminar de programar:**
  ```bash
  docker compose down
  ```
- **Regra importante sobre Migrations:**
  Não precisa rodar `dotnet ef migrations add` para o que já existe — basta rodar `dotnet ef database update` sempre que der `git pull`. Só rode `dotnet ef migrations add NomeDaMudanca` quando **você** criar ou alterar uma classe em `models/` ou no `AppDbContext.cs`.

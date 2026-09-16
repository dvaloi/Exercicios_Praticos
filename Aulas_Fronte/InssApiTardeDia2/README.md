# InssApiTardeDia2 — gabarito da tarde (M8)

Continua o `InssApiDia2` da manhã.

Visual Studio 2026 → abrir esta pasta → F5 → `/swagger`

Swagger **igual ao do aluno no fim do Dia 2**: GET/POST/PUT/PATCH/DELETE de ontem + `/{id}/pedidos` + GET pedidos paginado. Por trás:

- `Controllers` — só HTTP
- `Services` — regras (Ana, NUIT, transação)
- `Repositories` — `inss.db`
- `Middleware` — 500 amigável + `correlacao`
- `Program.cs` — `AddScoped` + `UseMiddleware`

Exibir → Saída: “Contribuinte criado {Nuit}”

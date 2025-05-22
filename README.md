# DevTrace

🚀 **DevTrace** é uma biblioteca para monitoramento de requisições em aplicações .NET.  
Ela registra as rotas acessadas, tempos de execução e códigos de status — exibindo tudo isso em um dashboard com diversos indicadores úteis.

---

## 📦 Instalação

Instale o pacote via NuGet (Ainda não criei o pacote):

```bash
dotnet add package DevTrace
```

---

## ⚙️ Configuração

No seu `Program.cs`, adicione o serviço e a UI:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços
builder.Services.AddDevTrace();

var app = builder.Build();

// Habilita o rastreamento
app.UseDevTrace(); // UI

app.Run();
```

---

## 🧠 O que o DevTrace faz?

- ✅ Registra todas as requisições recebidas e enviadas
- ✅ Captura tempo de resposta e status HTTP
- ✅ Exibe um dashboard acessível via `/devtrace`
- ✅ Interface amigável e de fácil análise
- ✅ Uso de IA para monitorar requisições problemáticas (Não implementado)

---

## 📍 Acesse o painel

Após iniciar sua aplicação, vá para:

```
https://localhost:{porta}/devtrace
```

---

## 📊 Em breve

- [ ] Filtros por status, métodos e endpoints
- [ ] Exportação de logs
- [ ] Armazenamento em banco
- [ ] Integração com algum modelo de IA para monitorar falhas

---

## 📃 Licença

MIT © 

---

## 🧪 Requisitos

- .NET 8 ou superior
- Estudando a disponibilização para para versões mais antigas.
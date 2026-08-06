// Wyciszamy ostrzeżenie o eksperymentalnym API (m.in. RemoveAllResilienceHandlers
// używane w AddOllamaResilienceHandlers oraz cache'owanie odpowiedzi czatu).
#pragma warning disable EXTEXP0001
using AiChatBuddy.Api.Models;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Wspólna konfiguracja Aspire: telemetria, health checks, service discovery i resilience.
builder.AddServiceDefaults();

// Redis jako IDistributedCache — wykorzystywany niżej przez UseDistributedCache() klienta czatu.
builder.AddRedisDistributedCache("cache");

// Provider bazy wektorowej wybierany na etapie kompilacji (UseAzure w Directory.Build.props) —
// pakiety Azure AI Search i SQLite wymagają niezgodnych wersji VectorData.Abstractions.
#if USE_AZURE
// Azure AI Search jako źródło RAG dla fragmentów incydentów
builder.AddAzureSearchClient("azure-search");
builder.Services
    .AddAzureSearchCollection("incidents-chunks")
    .AddOpenTelemetry();
builder
    .AddAzureChatCompletionsClient("foundry")
    .AddChatClient("gpt-5")
    .UseFunctionInvocation()
    .UseDistributedCache()
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = builder.Environment.IsDevelopment());
#else
// Lokalna baza wektorowa (SQLite) do wyszukiwania fragmentów incydentów w projekcie IngestionService
string sqlConnectionString = builder.Configuration.GetConnectionString("vector-store")
    ?? throw new InvalidOperationException("Vector store connection string is not configured");
builder.Services
    .AddSqliteCollection<string, VectorChunk>("incidents-chunks", sqlConnectionString);

// Klient modelu konwersacyjnego (Ollama, zasób "chat") z pipeline'em dekoratorów:
builder
    .AddOllamaApiClient("chat")
    .AddChatClient()
    // Pozwala modelowi samodzielnie wywoływać zarejestrowane narzędzia (function calling).
    .UseFunctionInvocation()
    // Cache'uje odpowiedzi w Redisie, żeby powtarzalne zapytania nie obciążały modelu.
    .UseDistributedCache()
    // Traces/metryki wywołań LLM; treści promptów logujemy tylko lokalnie (dane wrażliwe).
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = builder.Environment.IsDevelopment());
#endif

// Osobny klient Ollamy do generowania embeddingów — używany przy wyszukiwaniu wektorowym.
builder
    .AddOllamaApiClient("embedding")
    .AddEmbeddingGenerator()
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = builder.Environment.IsDevelopment());

// Add services to the container.
builder.Services.AddControllers();
// Nadpisuje domyślne timeouty HTTP na dłuższe — lokalne modele odpowiadają wolno.
builder.Services.AddOllamaResilienceHandlers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Endpointy /health i /alive (tylko w środowisku deweloperskim).
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Dokument OpenAPI + Swagger UI do ręcznego testowania API.
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
        options.DocumentPath = "/openapi/v1.json");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

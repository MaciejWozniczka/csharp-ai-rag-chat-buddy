using AiChatBuddy.IngestionService;
#if USE_AZURE
using AiChatBuddy.IngestionService.Extensions;
#endif
using Microsoft.Extensions.AI;

// Aplikacja hostowana bez serwera HTTP — jej jedynym zadaniem jest praca w tle (Worker).
var builder = Host.CreateApplicationBuilder(args);

// Wspólna konfiguracja Aspire: telemetria, health checks, service discovery i resilience.
builder.AddServiceDefaults();

// Generator embeddingów (Ollama, zasób "embedding") — używany przez chunker
// do wyliczania wektorów i oceny podobieństwa semantycznego fragmentów.
builder
    .AddOllamaApiClient("embedding")
    .AddEmbeddingGenerator()
    // Traces/metryki; treści dokumentów logujemy tylko lokalnie (dane wrażliwe).
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = builder.Environment.IsDevelopment());

// Provider bazy wektorowej wybierany na etapie kompilacji (UseAzure w Directory.Build.props) —
// pakiety Azure AI Search i SQLite wymagają niezgodnych wersji VectorData.Abstractions.
#if USE_AZURE
// Azure AI Search jako źródło RAG dla fragmentów incydentów (zasób "azure-search" z AppHosta).
builder.AddAzureSearchClient("azure-search");
builder.Services.AddAzureAiSearchVectorStore();
#else
// Lokalna baza wektorowa, z której czyta API (zasób "vector-store" z AppHosta).
var vectorStoreConnectionString = builder.Configuration.GetConnectionString("vector-store");
builder.Services.AddSqliteVectorStore(_ => vectorStoreConnectionString ?? throw new InvalidOperationException("Vector store connection string is not configured"));
#endif

// Rejestracja workera wykonującego pipeline ingestii.
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

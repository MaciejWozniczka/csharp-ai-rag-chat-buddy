// Punkt wejścia orkiestracji Aspire — definiuje wszystkie zasoby (kontenery, bazy, projekty)
// oraz zależności między nimi. Uruchomienie tego projektu podnosi całe środowisko lokalnie.

using Microsoft.Extensions.VectorData;

var builder = DistributedApplication.CreateBuilder(args);

var useAzure = true;

string vectorFunction = useAzure ? DistanceFunction.CosineSimilarity : DistanceFunction.CosineDistance;

// Redis dla cache'u odpowiedzi modelu; DbGate to UI do przeglądania kluczy.
var cache = builder.AddRedis("cache")
    .WithDbGate();

// Ollama w kontenerze; wolumen danych sprawia, że pobrane modele przetrwają restart.
var ollama = builder.AddOllama("ollama").WithDataVolume();
// Model konwersacyjny obsługujący czat.
var chatModel = ollama.AddModel("chat", "llama3.2");
// Lekki model embeddingów (384 wymiary — zgodnie z VectorChunk.VectorDimension).
var embeddings = ollama.AddModel("embedding", "all-minilm");

if (useAzure)
{
    // Tenant, w którym stoi Azure Search. Bez tego DefaultAzureCredential w projektach
    // bierze pierwsze konto z Visual Studio (inny tenant) i dostaje 401 invalid_token.
    var azureTenantId = builder.Configuration["Azure:TenantId"];

    // Parametry konfiguracyjne Azure Search (endpoint i resource group)
    var azureSearchEndpoint = builder.AddParameter("azureSearchEndpoint");
    var azureResourceGroup = builder.AddParameter("azureResourceGroup");
    // Azure AI Search jako narzędzie wyszukiwania semantycznego w dokumentach incydentów.

    var azureSearch = builder
        .AddAzureSearch("azure-search")
        .AsExisting(azureSearchEndpoint, azureResourceGroup);

    // API czatu. WithReference wstrzykuje connection stringi/adresy zasobów do konfiguracji,
    // WaitFor opóźnia start do momentu, gdy zależności są gotowe (modele pobrane, baza dostępna).
    builder.AddProject<Projects.AiChatBuddy_Api>("aichatbuddy-api")
        .WithReference(chatModel)
        .WithReference(embeddings)
        .WithReference(cache)
        .WithReference(azureSearch)
        .WithEnvironment("AZURE_TENANT_ID", azureTenantId)
        .WithEnvironment("useAzure", useAzure.ToString)
        .WaitFor(chatModel)
        .WaitFor(embeddings)
        .WaitFor(cache)
        .WaitFor(azureSearch);

    // Worker wgrywający dokumenty incydentów do bazy wektorowej — potrzebuje modelu
    // embeddingów do wyliczenia wektorów oraz samej bazy do zapisu.
    builder.AddProject<Projects.AiChatBuddy_IngestionService>("aichatbuddy-ingestionservice")
        .WithReference(embeddings)
        .WithReference(azureSearch)
        .WithEnvironment("vectorFunction", vectorFunction)
        .WithEnvironment("AZURE_TENANT_ID", azureTenantId)
        .WithEnvironment("useAzure", useAzure.ToString)
        .WaitFor(embeddings)
        .WaitFor(azureSearch);
}
else
{
    // Baza wektorowa na SQLite; WithSqliteWeb dodaje webowy podglądacz zawartości bazy.
    var vectorStore = builder
        .AddSqlite("vector-store")
        .WithSqliteWeb();

    // API czatu. WithReference wstrzykuje connection stringi/adresy zasobów do konfiguracji,
    // WaitFor opóźnia start do momentu, gdy zależności są gotowe (modele pobrane, baza dostępna).
    builder.AddProject<Projects.AiChatBuddy_Api>("aichatbuddy-api")
        .WithReference(chatModel)
        .WithReference(vectorStore)
        .WithReference(embeddings)
        .WithReference(cache)
        .WaitFor(chatModel)
        .WaitFor(vectorStore)
        .WaitFor(embeddings)
        .WaitFor(cache);

    // Worker wgrywający dokumenty incydentów do bazy wektorowej — potrzebuje modelu
    // embeddingów do wyliczenia wektorów oraz samej bazy do zapisu.
    builder.AddProject<Projects.AiChatBuddy_IngestionService>("aichatbuddy-ingestionservice")
        .WithReference(embeddings)
        .WithReference(vectorStore)
        .WithEnvironment("vectorFunction", vectorFunction)
        .WaitFor(embeddings)
        .WaitFor(vectorStore);
}

builder.Build().Run();

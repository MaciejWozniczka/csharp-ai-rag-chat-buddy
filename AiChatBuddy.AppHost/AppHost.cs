// Punkt wejścia orkiestracji Aspire — definiuje wszystkie zasoby (kontenery, bazy, projekty)
// oraz zależności między nimi. Uruchomienie tego projektu podnosi całe środowisko lokalnie.
var builder = DistributedApplication.CreateBuilder(args);

// Ollama w kontenerze; wolumen danych sprawia, że pobrane modele przetrwają restart.
var ollama = builder.AddOllama("ollama").WithDataVolume();
// Model konwersacyjny obsługujący czat.
var chatModel = ollama.AddModel("chat", "llama3.2");
// Lekki model embeddingów (384 wymiary — zgodnie z VectorChunk.VectorDimension).
var embeddings = ollama.AddModel("embedding", "all-minilm");

// Baza wektorowa na SQLite; WithSqliteWeb dodaje webowy podglądacz zawartości bazy.
var vectorStore = builder
    .AddSqlite("vector-store")
    .WithSqliteWeb();

// Redis dla cache'u odpowiedzi modelu; DbGate to UI do przeglądania kluczy.
var cache = builder.AddRedis("cache")
    .WithDbGate();

// Gotowy interfejs czatu do ręcznych testów modelu, wskazany bezpośrednio na Ollamę.
// Persistent lifetime = kontener nie jest usuwany po zatrzymaniu AppHosta.
builder.AddContainer("open-webui", "ghcr.io/open-webui/open-webui", "main")
    .WithHttpEndpoint(port: 3000, targetPort:8080, name: "http")
    .WithEnvironment("OLLAMA_BASE_URL", ollama.GetEndpoint("http"))
    .WithLifetime(ContainerLifetime.Persistent)
    .WaitFor(ollama);

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
    .WaitFor(embeddings)
    .WaitFor(vectorStore);

builder.Build().Run();

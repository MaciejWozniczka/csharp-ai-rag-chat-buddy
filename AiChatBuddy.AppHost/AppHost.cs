var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama").WithDataVolume();
var chatModel = ollama.AddModel("chat", "llama3.2");
var embeddingModel = ollama.AddModel("embedding", "all-minilm");

var vectorStore = builder
    .AddSqlite("vector-store")
    .WithSqliteWeb();

builder.AddContainer("open-webui", "ghcr.io/open-webui/open-webui", "main")
    .WithHttpEndpoint(port: 3000, targetPort:8080, name: "http")
    .WithEnvironment("OLLAMA_BASE_URL", ollama.GetEndpoint("http"))
    .WithLifetime(ContainerLifetime.Persistent)
    .WaitFor(ollama);

builder.AddProject<Projects.AiChatBuddy_Api>("aichatbuddy-api")
    .WithReference(chatModel)
    .WithReference(vectorStore)
    .WaitFor(chatModel)
    .WaitFor(vectorStore);

builder.AddProject<Projects.AiChatBuddy_IngestionService>("aichatbuddy-ingestionservice")
    .WithReference(embeddingModel)
    .WithReference(vectorStore)
    .WaitFor(embeddingModel)
    .WaitFor(vectorStore);

builder.Build().Run();

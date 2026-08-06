// Cały plik jest kompilowany tylko w wariancie Azure — bez pakietu konektora
// (patrz UseAzure w Directory.Build.props) użyte tu typy nie istnieją.
#if USE_AZURE
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

namespace AiChatBuddy.IngestionService.Extensions;

public static class AzureAiSearchExtensions
{
    public static IServiceCollection AddAzureAiSearchVectorStore(this IServiceCollection services)
    {
        services.AddSingleton<VectorStore>( sp =>
        {
            var indexClient = sp.GetRequiredService<SearchIndexClient>();
            return new AzureAISearchVectorStore(indexClient, new AzureAISearchVectorStoreOptions
            {
                EmbeddingGenerator = sp.GetRequiredService<IEmbeddingGenerator>()
            });
        });

        return services;
    }
}
#endif
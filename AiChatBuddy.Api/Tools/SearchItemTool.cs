using AiChatBuddy.Api.Models;
using Microsoft.Extensions.VectorData;
using System.ComponentModel;

namespace AiChatBuddy.Api.Tools;

/// <summary>
/// Narzędzie (function calling) udostępniane modelowi — pozwala mu wyszukiwać
/// fragmenty incydentów w bazie wektorowej zamiast opierać się na wiedzy własnej.
/// </summary>
public class SearchItemTool(VectorStoreCollection<string, VectorChunk> vectorCollection, ILogger<SearchItemTool> logger)
{
    // Atrybuty [Description] trafiają do schematu narzędzia wysyłanego do modelu —
    // to na ich podstawie model rozumie, kiedy i z jakim argumentem wywołać tę metodę.
    [Description("Searches for items based on the provided query.")]
    public async Task<IEnumerable<string>> SearchItemAsync([Description("The query to search for.")] string searchQuery, CancellationToken cancellationToken)
    {
        logger.LogInformation("Vector search for: {searchQuery}", searchQuery);

        // Zapytanie tekstowe jest automatycznie zamieniane na embedding, po czym kolekcja
        // zwraca 5 najbliższych chunków (wg odległości kosinusowej).
        var searchResults = vectorCollection
            .SearchAsync(searchQuery, top: 5, cancellationToken: cancellationToken);

        // Do modelu oddajemy wyłącznie treść fragmentów — bez metadanych i wyników podobieństwa.
        var chunks = await searchResults
            .Select(r => r.Record.Content)
            .ToListAsync(cancellationToken);

        // Wyjątki z tej metody nie przerywają żądania — FunctionInvokingChatClient przekazuje je
        // modelowi jako treść odpowiedzi narzędzia, więc brak tego logu oznacza błąd wyszukiwania.
        logger.LogInformation("Vector search returned {count} chunks", chunks.Count);

        return chunks;
    }
}
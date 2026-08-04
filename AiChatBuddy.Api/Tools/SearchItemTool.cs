using AiChatBuddy.Api.Models;
using Microsoft.Extensions.VectorData;
using System.ComponentModel;

namespace AiChatBuddy.Api.Tools;

/// <summary>
/// Narzędzie (function calling) udostępniane modelowi — pozwala mu wyszukiwać
/// fragmenty incydentów w bazie wektorowej zamiast opierać się na wiedzy własnej.
/// </summary>
public class SearchItemTool(VectorStoreCollection<string, VectorChunk> vectorCollection)
{
    // Atrybuty [Description] trafiają do schematu narzędzia wysyłanego do modelu —
    // to na ich podstawie model rozumie, kiedy i z jakim argumentem wywołać tę metodę.
    [Description("Searches for items based on the provided query.")]
    public async Task<IEnumerable<string>> SearchItemAsync([Description("The query to search for.")] string searchQuery, CancellationToken cancellationToken)
    {
        // Zapytanie tekstowe jest automatycznie zamieniane na embedding, po czym kolekcja
        // zwraca 5 najbliższych chunków (wg odległości kosinusowej).
        var searchResults = vectorCollection
            .SearchAsync(searchQuery, top: 5, cancellationToken: cancellationToken);

        // Do modelu oddajemy wyłącznie treść fragmentów — bez metadanych i wyników podobieństwa.
        return await searchResults
            .Select(r => r.Record.Content)
            .ToListAsync(cancellationToken);
    }
}
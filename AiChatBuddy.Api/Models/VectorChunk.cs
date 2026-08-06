using System.Text.Json.Serialization;
using Microsoft.Extensions.VectorData;

namespace AiChatBuddy.Api.Models;

/// <summary>
/// Pojedynczy fragment (chunk) dokumentu incydentu zapisany w bazie wektorowej.
/// Atrybuty opisują mapowanie właściwości na kolumny kolekcji wektorowej.
/// </summary>
public class VectorChunk
{
    // Liczba wymiarów wektora — musi odpowiadać modelowi embeddingów (all-minilm = 384)
    // oraz wartości ustawionej we VectorStoreWriter w IngestionService.
    public const int VectorDimension = 384;

    // Klucz główny rekordu w kolekcji.
    [JsonPropertyName("key")]
    [VectorStoreKey(StorageName = "key")]
    public required string Key { get; set; }

    // Treść fragmentu — to ona jest zwracana modelowi jako kontekst odpowiedzi.
    [JsonPropertyName("content")]
    [VectorStoreData(StorageName = "content")]
    public required string Content { get; set; }

    // Opcjonalny kontekst fragmentu (np. nagłówek sekcji, z której pochodzi).
    [JsonPropertyName("context")]
    [VectorStoreData(StorageName = "context")]
    public string? Context { get; set; }

    // Identyfikator dokumentu źródłowego, pozwala powiązać chunk z konkretnym incydentem.
    // Nazwa pola pisana małymi literami — tak tworzy je VectorStoreWriter w IngestionService,
    // a nazwy pól w Azure AI Search są rozróżniane pod względem wielkości znaków.
    [JsonPropertyName("documentid")]
    [VectorStoreData(StorageName = "documentid")]
    public required string DocumentId { get; set; }

    // Wektor embeddingu. Tylko do odczytu — wyliczany i zapisywany przez warstwę
    // vector store na podstawie właściwości Content, nie ustawiamy go ręcznie.
    // JsonPropertyName jest obowiązkowy: konektor Azure AI Search buduje listę pól
    // wektorowych w zapytaniu z nazw serializacji JSON, a nie z StorageName.
    [JsonPropertyName("embedding")]
    [VectorStoreVector(VectorDimension, StorageName = "embedding")]
    public float[]? Embedding { get; }
}
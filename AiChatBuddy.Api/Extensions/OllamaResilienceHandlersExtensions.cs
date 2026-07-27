using System.Diagnostics.CodeAnalysis;

namespace AiChatBuddy.Api.Extensions;

/// <summary>
/// Rejestruje domyślne polityki odporności dla klientów HTTP używanych do komunikacji z Ollamą.
/// </summary>
public static class OllamaResilienceHandlersExtensions
{
    /// <summary>
    /// Dodaje globalną konfigurację resilience handlerów dla wszystkich HttpClientów tworzonych w aplikacji.
    /// </summary>
    /// <param name="services">Kontener usług aplikacji.</param>
    /// <returns>Ten sam kontener usług, aby umożliwić dalsze łańcuchowanie rejestracji.</returns>
    [Experimental("EXTEXP0001")]
    public static IServiceCollection AddOllamaResilienceHandlers(this IServiceCollection services)
    {
        services.ConfigureHttpClientDefaults(httpClientBuilder =>
        {
            // Usuwamy wszystkie domyślne resilience handlery, aby uniknąć konfliktów z naszymi ustawieniami.
            httpClientBuilder.RemoveAllResilienceHandlers();

            httpClientBuilder.AddStandardResilienceHandler(config =>
            {
                // Pojedyncza próba może trwać dłużej, bo odpowiedzi modeli lokalnych bywają wolne.
                config.AttemptTimeout.Timeout = TimeSpan.FromMinutes(5);

                // Dłuższe okno próbkowania ogranicza przypadkowe otwieranie circuit breakera
                // przy chwilowych problemach, np. podczas startu modelu lub skoku obciążenia.
                config.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(10);

                // Cały request ma większy limit niż pojedyncza próba, aby uwzględnić retry
                // oraz narzut dodatkowych handlerów w pipeline HttpClienta.
                config.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(10);
            });
        });

        return services;
    }
}

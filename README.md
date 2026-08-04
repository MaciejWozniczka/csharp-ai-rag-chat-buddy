# AI Chat Buddy

AI Chat Buddy to projekt portfolio pokazujący, jak lokalny asystent AI może pomagać zespołom wyciągać wnioski z wcześniejszych incydentów produkcyjnych, zamiast odpowiadać na podstawie ogólnej wiedzy modelu.

W tej aplikacji asystent nie wymyśla odpowiedzi samodzielnie. Najpierw przeszukuje przygotowaną bazę wiedzy o incydentach, znajduje najbardziej trafne przypadki, a dopiero potem przygotowuje odpowiedź zawierającą diagnozę, rekomendowane działania i powiązane incydenty. Dzięki temu rozmowa opiera się na praktycznej wiedzy operacyjnej, a nie na ogólnikowych odpowiedziach AI.

## Co robi aplikacja

Projekt symuluje wewnętrznego asystenta wsparcia dla zespołów inżynierskich lub operacyjnych. Użytkownik może zadać pytanie takie jak:

- Dlaczego wyniki wyszukiwania nagle się pogorszyły?
- Co zwykle powoduje błędy uploadu plików po zmianie infrastruktury?
- Które wcześniejsze incydenty są podobne do awarii logowania?

Aplikacja następnie:

1. przyjmuje pytanie przez API,
2. przeszukuje bazę wiedzy o incydentach w poszukiwaniu najbardziej trafnych fragmentów,
3. przekazuje tylko te fragmenty do modelu językowego,
4. generuje uporządkowaną odpowiedź na podstawie historii incydentów.

To oznacza, że asystent działa bardziej jak przewodnik po firmowej wiedzy niż jak ogólny chatbot.

## Jak to działa w praktyce

Cały przepływ jest podzielony na dwie części.

### 1. Przygotowanie bazy wiedzy

Osobna usługa ingestii odczytuje dokumenty z incydentami zapisane jako pliki Markdown. Każdy dokument jest dzielony na mniejsze, sensowne fragmenty, a każdy fragment zostaje zamieniony na reprezentację liczbową opisującą jego znaczenie. Tak przygotowane dane trafiają do bazy wektorowej.

W praktyce oznacza to, że aplikacja zamienia opisane incydenty na format umożliwiający "wyszukiwanie po znaczeniu", a nie tylko po dokładnie pasujących słowach.

### 2. Odpowiadanie na pytania użytkownika

Gdy pojawia się pytanie, API wyszukuje fragmenty najbardziej zbliżone znaczeniowo do treści zapytania. Model czatu otrzymuje te fragmenty jako kontekst i ma odpowiadać wyłącznie na ich podstawie.

Dzięki temu odpowiedzi są zakotwiczone w danych projektu. Jeżeli zapisane incydenty nie zawierają wystarczających informacji, aplikacja ma zwrócić informację o braku danych, zamiast udawać, że zna odpowiedź.

## Co dzieje się w tle

Z perspektywy produktu aplikacja łączy cztery główne pomysły:

- lokalny model językowy do generowania odpowiedzi,
- warstwę wyszukiwania semantycznego do odnajdywania trafnej historii incydentów,
- pipeline ingestii dokumentów, który utrzymuje bazę wiedzy w gotowości,
- cache odpowiedzi, dzięki któremu powtarzalne pytania mogą być obsługiwane szybciej.

Dodatkowo projekt zawiera kontener Open WebUI podłączony do lokalnego modelu, co ułatwia ręczne testowanie i eksperymentowanie w trakcie developmentu.

## Dlaczego ten projekt jest ciekawy do portfolio

To nie jest po prostu "kolejny chatbot". Projekt pokazuje:

- projektowanie funkcji AI wokół konkretnego przypadku biznesowego,
- rozdzielenie przygotowania wiedzy od doświadczenia użytkownika po stronie czatu,
- ograniczanie halucynacji przez wymuszanie pracy modelu na odnalezionych danych,
- budowę małej aplikacji wielousługowej zamiast pojedynczego demonstracyjnego endpointu,
- pracę z lokalną infrastrukturą AI zamiast pełnego polegania na zewnętrznych usługach SaaS.

## Architektura w skrócie

Rozwiązanie składa się z czterech głównych części:

- `AiChatBuddy.AppHost`
  Uruchamia lokalne środowisko i spina wszystkie zasoby w całość.

- `AiChatBuddy.IngestionService`
  Odczytuje dokumenty z incydentami, dzieli je na fragmenty, tworzy embeddingi i zapisuje dane do bazy wektorowej.

- `AiChatBuddy.Api`
  Przyjmuje pytania użytkownika, wyszukuje trafne fragmenty incydentów i zwraca końcową odpowiedź AI.

- `AiChatBuddy.ServiceDefaults`
  Dostarcza wspólną konfigurację przekrojową dla usług, taką jak health checki, service discovery, resilience i telemetry.

Usługi wspierające to:

- Ollama jako lokalne zaplecze modeli AI,
- SQLite używane jako baza wektorowa,
- Redis używany do cache,
- Open WebUI do ręcznego testowania lokalnego modelu.

## Przykładowa ścieżka użytkownika

1. Raporty o incydentach trafiają do zbioru danych.
2. Usługa ingestii przetwarza je i aktualizuje przeszukiwalną bazę wiedzy.
3. Użytkownik wysyła pytanie do API czatu.
4. Aplikacja pobiera najbardziej trafne fragmenty incydentów.
5. Model przygotowuje odpowiedź wyłącznie na podstawie tych fragmentów.
6. Użytkownik otrzymuje zwięzłą odpowiedź z diagnozą, działaniami i powiązanymi incydentami.

## Charakter projektu

Ten projekt najlepiej rozumieć jako praktyczny proof of concept wewnętrznego "asystenta AI do wsparcia", który pomaga zespołom ponownie wykorzystywać wiedzę operacyjną już obecną w dokumentacji incydentów.

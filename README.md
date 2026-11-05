# ProductMessengerRabbitMQ

Niewielki projekt pokazujący obsługę tworzenia produktów z wykorzystaniem komunikacji asynchronicznej. API zapisuje produkt w SQLite i publikuje zdarzenie `ProductCreated` przez MassTransit do RabbitMQ. Osobny worker odbiera zdarzenie i zapisuje jego dane w logu.

## Jak to działa

Rozwiązanie składa się z trzech projektów:

- **ProductMessengerRabbitMQ.API** — endpoint HTTP do tworzenia produktów, zapis w SQLite i publikowanie zdarzeń.
- **ProductMessengerRabbitMQ.Contracts** — kontrakt zdarzenia współdzielony przez API i workera.
- **ProductMessengerRabbitMQ.Worker** — konsument zdarzeń `ProductCreated`.

API korzysta z outboxa MassTransit opartego na Entity Framework Core i SQLite. Zdarzenie jest zapisywane w ramach tego samego kontekstu bazy co produkt, a następnie przekazywane do RabbitMQ. Migracje bazy są uruchamiane przy starcie API.

## Wymagania

- Docker z Docker Compose — do uruchomienia całego rozwiązania.
- .NET 10 SDK — jeśli chcesz budować lub uruchamiać projekty poza Dockerem.

## Uruchomienie przez Docker Compose

W katalogu głównym repozytorium wykonaj:

```bash
docker compose up --build
```

Compose uruchomi RabbitMQ, API i workera. API będzie dostępne pod adresem `http://localhost:5070`, a panel RabbitMQ pod `http://localhost:15672`.

Dane dostępowe do panelu RabbitMQ w lokalnej konfiguracji:

- użytkownik: `user`
- hasło: `password`

Dokumentacja Swagger API jest dostępna pod `http://localhost:5070/swagger`.

Aby zatrzymać usługi:

```bash
docker compose down
```

Baza SQLite i dane RabbitMQ są przechowywane w wolumenach Dockera, więc pozostają po zatrzymaniu kontenerów. Usunięcie wolumenów (a wraz z nimi danych) wymaga polecenia:

```bash
docker compose down --volumes
```

## API

### Utworzenie produktu

`POST /api/products`

Przykładowe żądanie:

```json
{
  "name": "Klawiatura",
  "description": "Klawiatura mechaniczna",
  "price": 249.99
}
```

Przykład w PowerShell:

```powershell
$body = @{
    name = "Klawiatura"
    description = "Klawiatura mechaniczna"
    price = 249.99
} | ConvertTo-Json

Invoke-RestMethod `
    -Uri "http://localhost:5070/api/products" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

API nadaje produktowi identyfikator `Id` i zwraca utworzony obiekt. `Name` jest wymagane (od 1 do 50 znaków), `Price` musi mieścić się w zakresie od 1 do 100000. `Description` jest opcjonalne; jeśli zostanie podane, może mieć od 3 do 100 znaków.

Po zapisaniu produktu API publikuje zdarzenie `ProductCreated` z jego identyfikatorem, nazwą, opisem i ceną. Worker odbiera wiadomość i wypisuje te dane w logu. Dla błędów obsługi wiadomości skonfigurowano trzy ponowienia w odstępach pięciu sekund.

## Uruchomienie lokalne

Najpierw uruchom RabbitMQ:

```bash
docker compose up -d rabbitmq
```

Następnie, w osobnych terminalach, uruchom API i workera:

```bash
dotnet run --project ProductMessengerRabbitMQ.API
```

```bash
dotnet run --project ProductMessengerRabbitMQ.Worker
```

Przy konfiguracji domyślnej aplikacje łączą się z RabbitMQ pod `localhost` na standardowym porcie AMQP `5672`, używając danych `user` / `password`. API korzysta z lokalnej bazy SQLite wskazanej w `ProductMessengerRabbitMQ.API/appsettings.json`.

## Konfiguracja

Ustawienia RabbitMQ i bazy można nadpisywać standardową konfiguracją .NET, na przykład zmiennymi środowiskowymi:

| Zmienna | Znaczenie |
| --- | --- |
| `RabbitMQ__Host` | Host RabbitMQ |
| `RabbitMQ__User` | Użytkownik RabbitMQ |
| `RabbitMQ__Password` | Hasło RabbitMQ |
| `ConnectionStrings__Default` | Connection string SQLite (API) |

Wartości `user` / `password` w `docker-compose.yml` służą wyłącznie do lokalnego uruchomienia projektu. Nie należy używać ich jako danych dostępowych w środowisku produkcyjnym.

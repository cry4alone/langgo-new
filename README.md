# LanggoNew

ASP.NET Core 8.0 Web API для образовательной игровой платформы изучения языков.

## Особенности

- **Аутентификация и авторизация** - JWT токены, регистрация пользователей
- **Система словарей** - создание словарей, управление словами, импорт из файлов
- **Игровой процесс** - мультиплеерные игры в реальном времени через SignalR
- **Фоновые задачи** - обработка через Hangfire с Redis Storage
- **Модульная архитектура** - CQRS с MediatR, Vertical Slice Architecture

## Технологический стек

| Категория | Технологии |
|-----------|------------|
| Фреймворк | ASP.NET Core 8.0 |
| База данных | PostgreSQL 15 |
| Кэш/Очереди | Redis 7 |
| ORM | Entity Framework Core 8 |
| Архитектура | CQRS (MediatR), Minimal APIs, Vertical Slice |
| Валидация | FluentValidation |
| Маппинг | AutoMapper |
| Auth | JWT Bearer |
| Real-time | SignalR |
| Документация | Swagger/OpenAPI |
| Контейнеризация | Docker, Docker Compose |

## Структура проекта

```
LanggoNew/
├── Endpoints/              # Minimal API endpoints
├── Features/               # Вертикальные срезы по бизнес-функциональности
│   ├── Authentication/     # Регистрация, аутентификация
│   ├── Dictionaries/       # Управление словарями и словами
│   └── Games/              # Игровая логика (SignalR Hub)
├── Middleware/             # Кастомные middleware компоненты
├── Shared/                 # Общие компоненты
│   ├── Config/             # Настройки и опции
│   ├── Enum/               # Перечисления
│   ├── Exceptions/         # Обработка исключений
│   ├── Infrastructure/     # Сервисы и репозитории
│   └── Models/             # Общие модели
├── DependencyInjection.cs  # Расширения для DI контейнера
├── Program.cs              # Точка входа приложения
└── Dockerfile              # Docker конфигурация
```

## Быстрый старт

### Предварительные требования

- .NET 8.0 SDK
- Docker & Docker Compose
- IDE (Visual Studio 2022+, Rider или VS Code)

### Запуск через Docker Compose

```bash
docker-compose up --build
```

Сервисы будут доступны по адресам:
- **API:** http://localhost:8080
- **Swagger UI:** http://localhost:8080/swagger
- **PostgreSQL:** localhost:5432
- **Redis:** localhost:6379
- **Redis Insight:** http://localhost:5540

### Локальная разработка

1. Создайте файл `.env` в папке `LanggoNew/`:
```env
Jwt:Secret=your-super-secret-key-minimum-32-chars
LuckyPenny:LicenseKey=your-license-key
ConnectionStrings:Redis=localhost:6379
```

2. Запустите инфраструктуру:
```bash
docker-compose up postgres redis redisinsight
```

3. Запустите приложение:
```bash
cd LanggoNew
dotnet run
```

## Конфигурация

### Переменные окружения

| Переменная | Описание | Пример |
|------------|----------|--------|
| `Jwt:Secret` | Секретный ключ для JWT | Минимум 32 символа |
| `LuckyPenny:LicenseKey` | Лицензионный ключ MediatR/AutoMapper | Из лицензии |
| `ConnectionStrings:Redis` | Строка подключения к Redis | `localhost:6379` |

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=langgo;Username=admin;Password=langgo_admin"
  },
  "JwtSettings": {
    "Issuer": "LanggoApi",
    "Audience": "LanggoApiClient"
  },
  "GameTiming": {
    "RoundDurationSeconds": 30,
    "PauseBetweenRoundsSeconds": 5
  }
}
```

## API Endpoints

### Аутентификация
- `POST /api/auth/register` - Регистрация нового пользователя
- `POST /api/auth/authenticate` - Вход и получение JWT токена

### Словари
- `GET /api/dictionaries` - Получить список словарей
- `POST /api/dictionaries` - Создать новый словарь
- `POST /api/dictionaries/{id}/words` - Добавить слова в словарь
- `POST /api/dictionaries/import` - Импорт словаря из файла

### Игры
- `POST /api/games` - Создать новую игру
- `POST /api/games/{id}/join` - Присоединиться к игре
- `POST /api/games/{id}/start` - Начать игру
- `POST /api/games/{id}/leave` - Покинуть игру

### SignalR Hub
- `/game` - WebSocket хаб для игровых событий в реальном времени

## Авторизация

API использует JWT Bearer токены. Добавьте токен в заголовок запроса:

```
Authorization: Bearer <your-jwt-token>
```

Для SignalR подключений используйте query параметр:
```
/game?access_token=<your-jwt-token>
```

## Архитектурные решения

### Vertical Slice Architecture
Код организован по вертикальным срезам функциональности (Vertical Slices). Каждый срез инкапсулирует всю логику для конкретной бизнес-задачи — от API endpoint до обработки данных. Это обеспечивает:
- Высокую когезию внутри среза
- Слабую связанность между срезами
- Упрощённое тестирование и поддержку
- Возможность независимого развития функциональности

### CQRS с MediatR
Каждая операция представлена как команда или запрос с отдельным обработчиком, что естественно сочетается с вертикальными срезами.

### Минимальные API
Используются Minimal APIs с кастомным паттерном `IEndpoint` для регистрации роутов.

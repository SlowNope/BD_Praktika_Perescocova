# ProgrammingTasks

Настольное WPF-приложение для ведения базы задач по программированию.

## Запуск

Требуется Windows и .NET 8 SDK или Visual Studio 2022 с поддержкой WPF.

```powershell
dotnet restore ProgrammingTasks.sln
dotnet build ProgrammingTasks.sln
dotnet test ProgrammingTasks.sln
dotnet run --project src/ProgrammingTasks.Wpf
```

SQLite-файл `programming-tasks.db` создаётся при первом запуске. Автоматически добавляются демонстрационные задачи, теги, контесты и пользователи.

Для автономной публикации на Windows выполните `.\publish.ps1`, затем передайте содержимое папки `publish`. Подробный порядок показа описан в `DEMO_SCRIPT.md`.

Для создания ярлыка без Visual Studio выполните `.\create-shortcut.ps1`. Ярлык `ProgrammingTasks.lnk` появится в корне проекта; его можно скопировать на рабочий стол.

Диагностический лог приложения записывается в `logs/application.log` рядом с исполняемым файлом. В нём отмечены этапы авторизации, подключения SQLite, загрузки данных и открытия главного окна.

## Данные для демонстрации

- Администратор: `root`, пароль `Madmin1!`.
- Пользователь: `user1`, пароль отсутствует.

Администратор может управлять задачами, справочниками и пользователями. Пользователь `user1` может просматривать, фильтровать и экспортировать данные.

## Архитектура

- `ProgrammingTasks.Domain` — сущности и правила предметной области.
- `ProgrammingTasks.Application` — сервисы поиска и фильтрации.
- `ProgrammingTasks.Infrastructure` — SQLite, авторизация и репозитории.
- `ProgrammingTasks.Wpf` — интерфейс WPF.
- `ProgrammingTasks.Tests` — unit-тесты.

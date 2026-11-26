from docx import Document
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.shared import Pt

output = "Памятка где ничего не понятно.docx"
document = Document()
styles = document.styles
styles["Normal"].font.name = "Times New Roman"
styles["Normal"].font.size = Pt(11)
for name, size in (("Heading 1", 16), ("Heading 2", 14), ("Heading 3", 12)):
    styles[name].font.name = "Times New Roman"
    styles[name].font.size = Pt(size)
    styles[name].font.bold = True
    styles[name].font.color.rgb = None

def heading(text, level=1):
    document.add_heading(text, level=level)

def paragraph(text):
    p = document.add_paragraph(text)
    for run in p.runs:
        run.font.name = "Times New Roman"
        run.font.color.rgb = None

def bullets(items):
    for item in items:
        p = document.add_paragraph(item, style="List Bullet")
        for run in p.runs:
            run.font.name = "Times New Roman"
            run.font.color.rgb = None

title = document.add_paragraph()
title.alignment = WD_ALIGN_PARAGRAPH.CENTER
run = title.add_run("ПАМЯТКА\n«Где ничего не понятно»")
run.bold = True
run.font.name = "Times New Roman"
run.font.size = Pt(20)
run.font.color.rgb = None
paragraph("Практическое объяснение структуры решения ProgrammingTasks, назначения файлов и последовательности работы программы.")

heading("1. Как читать дерево решения")
paragraph("В Visual Studio файл ProgrammingTasks.sln является решением. Внутри него находятся проекты. Проект — это самостоятельная часть программы со своим файлом .csproj, исходным кодом и зависимостями.")
bullets(["src/ProgrammingTasks.Domain — предметная область: что такое задача, тег, контест и пользователь.", "src/ProgrammingTasks.Application — прикладные операции: поиск, фильтрация и правила работы с задачами.", "src/ProgrammingTasks.Infrastructure — работа с SQLite, PostgreSQL, авторизацией, файлами настроек и логами.", "src/ProgrammingTasks.Wpf — окна и визуальный интерфейс приложения.", "tests/ProgrammingTasks.Tests — автоматические проверки логики.", "README.md, REPORT.md и Word-файлы — документация.", "publish.ps1 и create-shortcut.ps1 — сборка автономного запуска."])

heading("2. Что происходит при запуске")
bullets(["App.xaml описывает ресурсы WPF, а App.xaml.cs запускает приложение.", "OnStartup включает диагностическое логирование и открывает LoginWindow.", "LoginWindow получает логин и пароль и вызывает AuthenticationService.", "После успешного входа создаётся MainWindow; режим интерфейса зависит от роли пользователя.", "SqliteTaskRepository создаёт таблицы и добавляет демонстрационные данные, если база пустая.", "MainWindow передаёт задачи в TaskService и отображает их в таблице."])

heading("3. Проект Domain")
heading("ProgrammingTask.cs", 2)
paragraph("Класс ProgrammingTask — модель одной задачи. Id — идентификатор; Title — название; Statement — условие; SolutionIdea — идея решения; PolygonUrl — ссылка; PreparedForCodeforces и PreparedForYandexContest — логические статусы; Difficulty — целое число от 1 до 10; Note — примечание; Tags и Contests — связанные справочные значения.")
heading("Contest.cs и Tag.cs", 2)
paragraph("Contest хранит название и год контеста. Tag хранит название темы. Связи с задачами сохраняются в таблицах TaskTags и TaskContests.")
heading("UserAccount.cs", 2)
paragraph("UserAccount описывает логин, хэш пароля и роль пользователя. Фактическая проверка выполняется инфраструктурным AuthenticationService.")
heading("ITaskRepository.cs", 2)
paragraph("Интерфейс задаёт общий контракт хранения: получить задачи, сохранить задачу и удалить задачу. Благодаря этому прикладная логика не зависит напрямую от SQLite или PostgreSQL.")
heading("TaskRules.cs", 2)
paragraph("ValidateDifficulty запрещает значения меньше 1 и больше 10. IsPreparedForCodeforces возвращает true, если заполнена Polygon-ссылка.")

heading("4. Проект Application")
heading("TaskFilter.cs", 2)
paragraph("TaskFilter содержит минимальную сложность, максимальную сложность и коллекцию обязательных тегов. Empty означает отсутствие ограничений.")
heading("TaskService.cs", 2)
paragraph("TaskService.Search получает список из репозитория и последовательно применяет фильтр сложности и тегов. TaskService.Save проверяет название, сложность и автоматически пересчитывает признак Codeforces перед записью.")

heading("5. Проект Infrastructure")
heading("SqliteTaskRepository.cs", 2)
paragraph("Создаёт локальную базу programming-tasks.db, таблицы и демонстрационные записи. Методы GetAll, Save и Delete выполняют чтение и изменение данных SQLite. LoadLinks восстанавливает теги и контесты.")
heading("PostgreSqlConnectionSettings.cs", 2)
paragraph("Хранит сервер, порт, базу данных, логин и пароль PostgreSQL. PostgreSqlSettingsStore сохраняет настройки в postgres-connection.json, чтобы их не вводить заново.")
heading("PostgreSqlMigrationService.cs", 2)
paragraph("Проверяет подключение, создаёт таблицы PostgreSQL и переносит задачи, теги, контесты и связи из локального хранилища.")
heading("PostgreSqlTaskRepository.cs", 2)
paragraph("Реализует тот же ITaskRepository для PostgreSQL. После переключения приложения операции просмотра, сохранения и удаления выполняются через PostgreSQL.")
heading("AuthenticationService.cs", 2)
paragraph("Создаёт системные учётные записи, хэширует пароль root с помощью SHA-256 и сравнивает введённые данные с базой.")
heading("ApplicationLogger.cs", 2)
paragraph("Записывает события с уровнем INFO или ERROR в logs/application.log. В лог попадают стадии запуска, авторизации, базы данных, CRUD и исключения.")

heading("6. Проект Wpf")
heading("App.xaml.cs", 2)
paragraph("Это точка входа WPF. Здесь задаётся режим завершения приложения, открывается авторизация и запускается главное окно. Ошибка запуска перехватывается и записывается в лог.")
heading("LoginWindow", 2)
paragraph("Содержит поля LoginText и PasswordText. Кнопка входа вызывает авторизацию. Escape закрывает окно без входа.")
heading("MainWindow", 2)
paragraph("Главное рабочее окно. MainTabs содержит вкладки. TasksGrid показывает строки TaskRow. DifficultyRangeText задаёт диапазон, TagsFilter — список тегов. Кнопки вызывают фильтрацию, отчёт, CSV, CRUD, справочники, пользователей и подключение PostgreSQL.")
heading("TaskEditorWindow", 2)
paragraph("Форма администратора. Поля TitleText, StatementText, IdeaText, DifficultyText, PolygonText, TagsText, ContestsText и NoteText собирают данные задачи. Перед сохранением проверяются обязательные поля и диапазон сложности.")
heading("TaskDetailsWindow", 2)
paragraph("Показывает полную карточку выбранной задачи без возможности изменения.")
heading("CatalogWindow и UsersWindow", 2)
paragraph("CatalogWindow управляет тегами и контестами. UsersWindow добавляет и удаляет менеджеров, но не позволяет удалить root.")
heading("PostgreSqlWindow", 2)
paragraph("Форма подключения PostgreSQL. После успешной проверки возвращает настройки в MainWindow, где выполняется миграция и переключение репозитория.")

heading("7. Что менять при типичных задачах")
bullets(["Изменить правило сложности — Domain/Services/TaskRules.cs и тесты.", "Изменить фильтрацию — Application/TaskService.cs и TaskFilter.cs.", "Изменить структуру SQLite — Infrastructure/SqliteTaskRepository.cs и init.sql.", "Изменить структуру PostgreSQL — PostgreSqlMigrationService.cs и PostgreSqlTaskRepository.cs.", "Изменить внешний вид главного окна — Wpf/MainWindow.xaml.", "Изменить обработку кнопок — Wpf/MainWindow.xaml.cs.", "Изменить окно редактирования — TaskEditorWindow.xaml и TaskEditorWindow.xaml.cs.", "Добавить диагностическую запись — вызвать ApplicationLogger.Info или Error.", "Изменить тесты — tests/ProgrammingTasks.Tests/TaskServiceTests.cs."])

heading("8. Как проверять изменения")
paragraph("После изменения кода остановите запущенное приложение, выполните dotnet build ProgrammingTasks.sln и dotnet test ProgrammingTasks.sln. Для проверки автономной версии выполните publish.ps1 и запустите ProgrammingTasks.lnk. Если приложение ведёт себя неожиданно, сначала откройте logs/application.log и найдите последнюю запись ERROR или последнюю успешно завершённую стадию.")

heading("9. Важное правило работы с кодом")
paragraph("Сначала определите слой, к которому относится изменение. Правило предметной области не должно прятаться в XAML, SQL не должен размещаться в окне, а интерфейс не должен напрямую реализовывать бизнес-правила. После изменения сохраняйте понятное имя, добавляйте краткий комментарий к нетривиальному участку и обновляйте соответствующий тест или раздел документации.")

document.save(output)
print(output)

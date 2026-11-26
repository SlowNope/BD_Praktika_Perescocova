from docx import Document
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.shared import Pt
from docx.oxml import OxmlElement
from docx.oxml.ns import qn

output = "Отчет по проекту ProgrammingTasks.docx"
document = Document()
styles = document.styles
styles["Normal"].font.name = "Arial"
styles["Normal"].font.size = Pt(11)

title = document.add_paragraph()
title.alignment = WD_ALIGN_PARAGRAPH.CENTER
run = title.add_run("ОТЧЁТ\nпо проекту ProgrammingTasks")
run.bold = True
run.font.size = Pt(20)
subtitle = document.add_paragraph()
subtitle.alignment = WD_ALIGN_PARAGRAPH.CENTER
subtitle.add_run("База задач по программированию с фильтрацией и отчётами").italic = True

def heading(text, level=1):
    document.add_heading(text, level=level)

def paragraph(text):
    document.add_paragraph(text)

def bullets(items):
    for item in items:
        document.add_paragraph(item, style="List Bullet")

heading("1. Назначение программы")
paragraph("ProgrammingTasks — настольное Windows-приложение для ведения каталога задач по программированию. Оно позволяет хранить условия задач, идеи решений, сложность, теги, контесты, Polygon-ссылки и признаки подготовки к Codeforces и Yandex Contest.")
paragraph("Программа предназначена для преподавателя, автора задач или организатора контестов: пользователь быстро находит нужные задачи, формирует выборки и сохраняет отчёты.")

heading("2. Основные возможности")
bullets(["Авторизация администратора root и обычных пользователей.", "Просмотр, добавление, редактирование и удаление задач.", "Фильтрация по диапазону сложности от 1 до 10 и нескольким тегам.", "Хранение связей задач с тегами и контестами.", "Просмотр полной карточки задачи: условие, идея, Polygon URL и примечание.", "Автоматическое определение готовности Codeforces по наличию Polygon URL.", "Управление тегами, контестами и менеджерами.", "Формирование статистического отчёта и экспорт текущей выборки в CSV.", "Автоматическое создание демонстрационных данных при первом запуске.", "Диагностическое логирование работы приложения."])

heading("3. Принцип работы")
paragraph("При запуске приложение открывает окно входа. AuthenticationService проверяет пользователя в SQLite. После успешной авторизации создаётся главное окно и интерфейс настраивается согласно роли.")
paragraph("SqliteTaskRepository создаёт необходимые таблицы. Если база пуста, добавляются демонстрационные задачи, теги и контесты. MainWindow получает задачи через TaskService и отображает их в DataGrid.")
paragraph("Пользователь вводит диапазон, например 3–7, и список тегов через запятую. TaskService.Search возвращает задачи, которые одновременно попадают в диапазон и содержат все указанные теги. Отчёт и CSV строятся по текущей выборке.")

heading("4. Роли и сценарии использования")
heading("4.1. Обычный пользователь", 2)
paragraph("Логин: user1. Пароль отсутствует. Пользователь просматривает задачи, применяет фильтры, открывает подробности, формирует отчёты и экспортирует CSV. Административная вкладка скрыта.")
heading("4.2. Администратор", 2)
paragraph("Логин: root. Пароль: Madmin1!. Администратор дополнительно управляет задачами, справочниками тегов и контестов, а также пользователями-менеджерами.")

heading("5. Стек технологий")
bullets(["C#.", ".NET 8.", "WPF для настольного интерфейса.", "SQLite и пакет Microsoft.Data.Sqlite.", "xUnit для модульного тестирования.", "MVVM-совместимое разделение слоёв; прикладная логика вынесена из хранения данных и интерфейса.", "Git для локального контроля версий."])

heading("6. Архитектура проекта")
bullets(["ProgrammingTasks.Domain — ProgrammingTask, Contest, Tag, UserAccount, интерфейс ITaskRepository и правила TaskRules.", "ProgrammingTasks.Application — TaskService и TaskFilter.", "ProgrammingTasks.Infrastructure — SQLite-репозиторий, авторизация, справочники, пользователи и ApplicationLogger.", "ProgrammingTasks.Wpf — окна входа, задач, редактирования, подробностей, справочников и пользователей.", "ProgrammingTasks.Tests — тесты фильтрации, диапазона сложности, автозаполнения Codeforces и авторизации."])

heading("7. Структура базы данных")
paragraph("Основная таблица ProgrammingTasks хранит поля задачи. Tags и Contests являются справочниками. TaskTags и TaskContests реализуют связи многие-ко-многим. Users хранит логины, хэши паролей и роли. Скрипт схемы находится в init.sql.")

heading("8. Безопасность и проверки ввода")
paragraph("Пароль root хранится в базе в виде SHA-256 хэша. Сложность проверяется на диапазон 1–10. Для сохранения задачи обязательны название, условие и идея решения. Для менеджера обязательны логин и непустой пароль. Системного пользователя root нельзя удалить.")

heading("9. Логирование")
paragraph("ApplicationLogger находится в Infrastructure. Лог записывается в logs/application.log рядом с исполняемым файлом. Записываются запуск, авторизация, операции SQLite, загрузка демонстрационных данных, CRUD и исключения. Это позволяет определить этап сбоя без отладчика.")

heading("10. Тестирование")
paragraph("Команды проверки: dotnet build ProgrammingTasks.sln и dotnet test ProgrammingTasks.sln. Текущий набор содержит 5 тестов, все проходят успешно.")

heading("11. Развёртывание и запуск")
paragraph("Для автономной Windows-версии выполните publish.ps1. Self-contained приложение создаётся в publish-current. Ярлык ProgrammingTasks.lnk запускает приложение без Visual Studio и без отдельной установки .NET.")

heading("12. Как продемонстрировать программу")
bullets(["Запустить ProgrammingTasks.lnk.", "Войти как user1 без пароля и показать таблицу задач.", "Ввести диапазон 3–7 и тег Графы, нажать Применить.", "Открыть задачу двойным щелчком.", "Сформировать отчёт и сохранить CSV.", "Сменить пользователя на root.", "Добавить или изменить задачу, открыть справочники и управление пользователями."])

heading("13. Файлы проекта")
bullets(["REPORT.md — краткое техническое описание.", "DEMO_SCRIPT.md — сценарий демонстрации.", "init.sql — схема SQLite.", "publish.ps1 — автономная публикация.", "create-shortcut.ps1 — создание ярлыка.", "logs/application.log — журнал работы приложения."])

document.save(output)
print(output)

using Homework2.Data;
using Homework2.Models;
using Homework2.Reporting;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string dbPath = "games.db";
string platformsCsv = Path.Combine(AppContext.BaseDirectory, "platforms.csv");
string gamesCsv = Path.Combine(AppContext.BaseDirectory, "games.csv");

var db = new DatabaseManager(dbPath);
db.InitializeDatabase(platformsCsv, gamesCsv);
Console.WriteLine();

string choice;
do
{
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║      УПРАВЛЕНИЕ ВИДЕОИГРАМИ          ║");
    Console.WriteLine("╠══════════════════════════════════════╣");
    Console.WriteLine("║ 1 — Показать все платформы           ║");
    Console.WriteLine("║ 2 — Показать все игры                ║");
    Console.WriteLine("║ 3 — Добавить игру                    ║");
    Console.WriteLine("║ 4 — Редактировать игру               ║");
    Console.WriteLine("║ 5 — Удалить игру                     ║");
    Console.WriteLine("║ 6 — Отчёты                           ║");
    Console.WriteLine("║ 7 — Экспорт в CSV (группа Б)         ║");
    Console.WriteLine("║ 0 — Выход                            ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.Write("Ваш выбор: ");
    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();

    switch (choice)
    {
        case "1": ShowPlatforms(db); break;
        case "2": ShowGames(db); break;
        case "3": AddGame(db); break;
        case "4": EditGame(db); break;
        case "5": DeleteGame(db); break;
        case "6": ReportsMenu(db); break;
        case "7": ExportCsv(db); break;
        case "0": Console.WriteLine("До свидания!"); break;
        default: Console.WriteLine("Неверный пункт."); break;
    }
    Console.WriteLine();
} while (choice != "0");

// ---------- Функции меню ----------
static void ShowPlatforms(DatabaseManager db)
{
    var platforms = db.GetAllPlatforms();
    Console.WriteLine("--- Все платформы ---");
    foreach (var p in platforms) Console.WriteLine("  " + p);
    Console.WriteLine($"Итого: {platforms.Count}");
}

static void ShowGames(DatabaseManager db)
{
    var games = db.GetAllGames();
    Console.WriteLine("--- Все игры ---");
    foreach (var g in games) Console.WriteLine("  " + g);
    Console.WriteLine($"Итого: {games.Count}");
}

static void AddGame(DatabaseManager db)
{
    Console.WriteLine("--- Добавление игры ---");
    var platforms = db.GetAllPlatforms();
    Console.WriteLine("Доступные платформы:");
    foreach (var p in platforms) Console.WriteLine("  " + p);

    Console.Write("ID платформы: ");
    if (!int.TryParse(Console.ReadLine(), out int pid)) { Console.WriteLine("Ошибка ввода."); return; }

    Console.Write("Название игры: ");
    string name = Console.ReadLine()?.Trim() ?? "";
    if (string.IsNullOrEmpty(name)) { Console.WriteLine("Имя не может быть пустым."); return; }

    Console.Write("Рейтинг (0-100): ");
    if (!int.TryParse(Console.ReadLine(), out int rating)) { Console.WriteLine("Ошибка ввода."); return; }

    try
    {
        db.AddGame(new Game(0, pid, name, rating));
        Console.WriteLine("Игра добавлена.");
    }
    catch (ArgumentException ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
}

static void EditGame(DatabaseManager db)
{
    Console.Write("ID игры для редактирования: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Ошибка ввода."); return; }
    var game = db.GetGameById(id);
    if (game == null) { Console.WriteLine("Игра не найдена."); return; }

    Console.WriteLine($"Текущие данные: {game}");
    Console.WriteLine("(Enter — оставить без изменений)");

    Console.Write($"Название [{game.Name}]: ");
    string input = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(input)) game.Name = input;

    Console.Write($"ID платформы [{game.PlatformId}]: ");
    input = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int newPid)) game.PlatformId = newPid;

    Console.Write($"Рейтинг [{game.Rating}]: ");
    input = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int newRating))
    {
        try { game.Rating = newRating; }
        catch (ArgumentException ex) { Console.WriteLine($"Ошибка: {ex.Message}"); return; }
    }

    db.UpdateGame(game);
    Console.WriteLine("Данные обновлены.");
}

static void DeleteGame(DatabaseManager db)
{
    Console.Write("ID игры для удаления: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;
    var game = db.GetGameById(id);
    if (game == null) { Console.WriteLine("Игра не найдена."); return; }
    Console.Write($"Удалить «{game.Name}»? (да/нет): ");
    if (Console.ReadLine()?.Trim().ToLower() == "да")
    {
        db.DeleteGame(id);
        Console.WriteLine("Игра удалена.");
    }
    else Console.WriteLine("Отмена.");
}

static void ReportsMenu(DatabaseManager db)
{
    string ch;
    do
    {
        Console.WriteLine("--- Отчёты ---");
        Console.WriteLine("1 — Игры по платформам");
        Console.WriteLine("2 — Количество игр на платформах");
        Console.WriteLine("3 — Средний рейтинг по платформам");
        Console.WriteLine("0 — Назад");
        Console.Write("Ваш выбор: ");
        ch = Console.ReadLine()?.Trim();
        switch (ch)
        {
            case "1": Report1(db); break;
            case "2": Report2(db); break;
            case "3": Report3(db); break;
            case "0": break;
            default: Console.WriteLine("Неверный пункт."); break;
        }
        Console.WriteLine();
    } while (ch != "0");
}

static void Report1(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT g.game_name, p.platform_name, g.rating
                 FROM game g
                 JOIN platform p ON g.platform_id = p.platform_id
                 ORDER BY g.game_name")
        .Title("Игры по платформам")
        .Header("Игра", "Платформа", "Рейтинг")
        .ColumnWidths(25, 15, 10)
        .Numbered()
        .Print();
}

static void Report2(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT p.platform_name, COUNT(*) AS cnt
                 FROM game g
                 JOIN platform p ON g.platform_id = p.platform_id
                 GROUP BY p.platform_name
                 ORDER BY p.platform_name")
        .Title("Количество игр на платформах")
        .Header("Платформа", "Кол-во игр")
        .ColumnWidths(20, 12)
        .Print();
}

static void Report3(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT p.platform_name,
                        ROUND(AVG(g.rating), 1) AS avg_rating
                 FROM game g
                 JOIN platform p ON g.platform_id = p.platform_id
                 GROUP BY p.platform_name
                 ORDER BY avg_rating DESC")
        .Title("Средний рейтинг по платформам")
        .Header("Платформа", "Средний рейтинг")
        .ColumnWidths(20, 18)
        .SaveToFile("avg_rating_report.txt")   // пример использования SaveToFile
        .Print();
}

static void ExportCsv(DatabaseManager db)
{
    string pPath = Path.Combine(AppContext.BaseDirectory, "platforms_export.csv");
    string gPath = Path.Combine(AppContext.BaseDirectory, "games_export.csv");
    db.ExportToCsv(pPath, gPath);
    Console.WriteLine($"Экспорт завершён:\n  {pPath}\n  {gPath}");
}
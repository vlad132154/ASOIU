namespace Homework3.Models;

/// <summary>
/// Игровая платформа (справочная таблица, сторона «один»)
/// </summary>
public class Platform
{
    /// <summary>
    /// Идентификатор платформы (первичный ключ)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название платформы
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Навигационное свойство: видеоигры этой платформы
    /// </summary>
    public ICollection<Game> Games { get; set; } = new List<Game>();
}

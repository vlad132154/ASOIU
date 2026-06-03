namespace Homework3.Models;

/// <summary>
/// Видеоигра (основная таблица, сторона «много»)
/// </summary>
public class Game
{
    /// <summary>
    /// Идентификатор игры (первичный ключ)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор платформы (внешний ключ)
    /// </summary>
    public int PlatformId { get; set; }

    /// <summary>
    /// Платформа (навигационное свойство)
    /// </summary>
    public Platform? Platform { get; set; }

    /// <summary>
    /// Название игры
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Оценка игроков (баллы из 100)
    /// </summary>
    public int Rating { get; set; }
}

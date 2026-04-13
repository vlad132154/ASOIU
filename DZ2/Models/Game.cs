using System;
using System.Collections.Generic;
using System.Text;

namespace Homework2.Models
{
    /// <summary>
    /// Видеоигра (основная таблица, сторона «много»)
    /// </summary>
    public class Game
    {
        /// <summary>Идентификатор игры</summary>
        public int Id { get; set; }

        /// <summary>Внешний ключ на платформу</summary>
        public int PlatformId { get; set; }

        /// <summary>Название игры</summary>
        public string Name { get; set; }

        private int _rating;

        /// <summary>
        /// Оценка игроков (баллы из 100, не может быть отрицательной)
        /// </summary>
        public int Rating
        {
            get => _rating;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Оценка не может быть отрицательной");
                _rating = value;
            }
        }

        public Game(int id, int platformId, string name, int rating)
        {
            Id = id;
            PlatformId = platformId;
            Name = name;
            Rating = rating;
        }

        public Game() : this(0, 0, "", 0) { }

        public override string ToString()
            => $"[{Id}] {Name}, платформа #{PlatformId}, рейтинг: {Rating}/100";
    }
}

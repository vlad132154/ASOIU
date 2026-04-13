using System;
using System.Collections.Generic;
using System.Text;

namespace Homework2.Models
{
    /// <summary>
    /// Игровая платформа (справочная таблица, сторона «один»)
    /// </summary>
    public class Platform
    {
        /// <summary>Идентификатор платформы</summary>
        public int Id { get; set; }

        /// <summary>Название платформы</summary>
        public string Name { get; set; }

        public Platform(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Platform() : this(0, "") { }

        public override string ToString() => $"[{Id}] {Name}";
    }
}

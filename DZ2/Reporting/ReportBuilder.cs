using System;
using System.Collections.Generic;
using System.Text;
using Homework2.Data;

namespace Homework2.Reporting
{
    public class ReportBuilder
    {
        private readonly DatabaseManager _db;
        private string _sql = "";
        private string _title = "";
        private string[] _headers = Array.Empty<string>();
        private int[] _widths = Array.Empty<int>();
        private bool _numbered = false;

        public ReportBuilder(DatabaseManager db) => _db = db;

        public ReportBuilder Query(string sql) { _sql = sql; return this; }
        public ReportBuilder Title(string title) { _title = title; return this; }
        public ReportBuilder Header(params string[] columns) { _headers = columns; return this; }
        public ReportBuilder ColumnWidths(params int[] widths) { _widths = widths; return this; }
        public ReportBuilder Numbered(bool numbered = true) { _numbered = numbered; return this; }

        public string Build()
        {
            var (columns, rows) = _db.ExecuteQuery(_sql);
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(_title))
                sb.AppendLine().AppendLine($"=== {_title} ===");

            string[] displayHeaders = _headers.Length > 0 ? _headers : columns;
            int colCount = displayHeaders.Length;

            int[] widths = _widths.Length >= colCount
                ? _widths
                : Enumerable.Repeat(20, colCount).ToArray();

            int numWidth = _numbered ? 5 : 0;

            // Шапка
            if (_numbered) sb.Append("№".PadRight(numWidth));
            for (int i = 0; i < colCount; i++)
                sb.Append(displayHeaders[i].PadRight(widths[i]));
            sb.AppendLine();

            // Разделитель
            int totalWidth = numWidth + widths.Sum();
            sb.AppendLine(new string('─', totalWidth));

            // Строки данных
            for (int r = 0; r < rows.Count; r++)
            {
                if (_numbered) sb.Append((r + 1).ToString().PadRight(numWidth));
                for (int c = 0; c < rows[r].Length && c < colCount; c++)
                    sb.Append(rows[r][c].PadRight(widths[c]));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void Print() => Console.Write(Build());

        public ReportBuilder SaveToFile(string path)
        {
            File.WriteAllText(path, Build());
            Console.WriteLine($"Отчёт сохранён в: {path}");
            return this;
        }
    }
}

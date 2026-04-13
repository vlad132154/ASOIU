using System;
using System.Collections.Generic;
using System.Text;
using Homework2.Models;
using Microsoft.Data.Sqlite;
namespace Homework2.Data
{
    

    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
        }

        public void InitializeDatabase(string platformsCsvPath, string gamesCsvPath)
        {
            CreateTables();
            if (GetAllPlatforms().Count == 0 && File.Exists(platformsCsvPath))
            {
                ImportPlatformsFromCsv(platformsCsvPath);
                Console.WriteLine($"[OK] Загружены платформы из {platformsCsvPath}");
            }
            if (GetAllGames().Count == 0 && File.Exists(gamesCsvPath))
            {
                ImportGamesFromCsv(gamesCsvPath);
                Console.WriteLine($"[OK] Загружены игры из {gamesCsvPath}");
            }
        }

        private void CreateTables()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS platform (
                platform_id INTEGER PRIMARY KEY AUTOINCREMENT,
                platform_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS game (
                game_id INTEGER PRIMARY KEY AUTOINCREMENT,
                platform_id INTEGER NOT NULL,
                game_name TEXT NOT NULL,
                rating INTEGER NOT NULL,
                FOREIGN KEY (platform_id) REFERENCES platform(platform_id)
            );";
            cmd.ExecuteNonQuery();
        }

        private void ImportPlatformsFromCsv(string path)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length < 2) continue;
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO platform (platform_id, platform_name) VALUES (@id, @name)";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@name", parts[1]);
                cmd.ExecuteNonQuery();
            }
        }

        private void ImportGamesFromCsv(string path)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length < 4) continue;
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO game (game_id, platform_id, game_name, rating)
                VALUES (@id, @platformId, @name, @rating)";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@platformId", int.Parse(parts[1]));
                cmd.Parameters.AddWithValue("@name", parts[2]);
                cmd.Parameters.AddWithValue("@rating", int.Parse(parts[3]));
                cmd.ExecuteNonQuery();
            }
        }

        public List<Platform> GetAllPlatforms()
        {
            var result = new List<Platform>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT platform_id, platform_name FROM platform ORDER BY platform_id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(new Platform(reader.GetInt32(0), reader.GetString(1)));
            return result;
        }

        public List<Game> GetAllGames()
        {
            var result = new List<Game>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT game_id, platform_id, game_name, rating FROM game ORDER BY game_id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(new Game(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3)));
            return result;
        }

        public Game? GetGameById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT game_id, platform_id, game_name, rating FROM game WHERE game_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return new Game(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3));
            return null;
        }

        public void AddGame(Game game)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO game (platform_id, game_name, rating) VALUES (@pid, @name, @rating)";
            cmd.Parameters.AddWithValue("@pid", game.PlatformId);
            cmd.Parameters.AddWithValue("@name", game.Name);
            cmd.Parameters.AddWithValue("@rating", game.Rating);
            cmd.ExecuteNonQuery();
        }

        public void UpdateGame(Game game)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            UPDATE game
            SET platform_id = @pid, game_name = @name, rating = @rating
            WHERE game_id = @id";
            cmd.Parameters.AddWithValue("@id", game.Id);
            cmd.Parameters.AddWithValue("@pid", game.PlatformId);
            cmd.Parameters.AddWithValue("@name", game.Name);
            cmd.Parameters.AddWithValue("@rating", game.Rating);
            cmd.ExecuteNonQuery();
        }

        public void DeleteGame(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM game WHERE game_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();

            string[] columns = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                columns[i] = reader.GetName(i);

            var rows = new List<string[]>();
            while (reader.Read())
            {
                string[] row = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                    row[i] = reader.GetValue(i)?.ToString() ?? "";
                rows.Add(row);
            }
            return (columns, rows);
        }

        // Экспорт для группы Б
        public void ExportToCsv(string platformsPath, string gamesPath)
        {
            var platforms = GetAllPlatforms();
            var lines = new List<string> { "platform_id;platform_name" };
            lines.AddRange(platforms.Select(p => $"{p.Id};{p.Name}"));
            File.WriteAllLines(platformsPath, lines);

            var games = GetAllGames();
            lines = new List<string> { "game_id;platform_id;game_name;rating" };
            lines.AddRange(games.Select(g => $"{g.Id};{g.PlatformId};{g.Name};{g.Rating}"));
            File.WriteAllLines(gamesPath, lines);
        }
    }
}

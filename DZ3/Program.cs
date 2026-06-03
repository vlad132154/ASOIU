using Homework3.Data;
using Homework3.Forms;
using Homework3.Models;

namespace Homework3;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        using (var context = new AppDbContext())
        {
            context.Database.EnsureCreated();

            if (!context.Platforms.Any())
            {
                var platforms = new[]
                {
                    new Platform { Name = "PC" },
                    new Platform { Name = "PlayStation 5" },
                    new Platform { Name = "Xbox Series X" },
                    new Platform { Name = "Nintendo Switch" }
                };
                context.Platforms.AddRange(platforms);
                context.SaveChanges();

                var games = new[]
                {
                    new Game { Name = "The Witcher 3", Rating = 95, PlatformId = platforms[0].Id },
                    new Game { Name = "Cyberpunk 2077", Rating = 85, PlatformId = platforms[0].Id },
                    new Game { Name = "God of War Ragnarok", Rating = 94, PlatformId = platforms[1].Id },
                    new Game { Name = "Spider-Man 2", Rating = 91, PlatformId = platforms[1].Id },
                    new Game { Name = "Horizon Forbidden West", Rating = 88, PlatformId = platforms[1].Id },
                    new Game { Name = "Halo Infinite", Rating = 80, PlatformId = platforms[2].Id },
                    new Game { Name = "Forza Motorsport", Rating = 84, PlatformId = platforms[2].Id },
                    new Game { Name = "Gears 5", Rating = 82, PlatformId = platforms[2].Id },
                    new Game { Name = "The Legend of Zelda: TOTK", Rating = 96, PlatformId = platforms[3].Id },
                    new Game { Name = "Super Mario Odyssey", Rating = 93, PlatformId = platforms[3].Id },
                    new Game { Name = "Metroid Dread", Rating = 89, PlatformId = platforms[3].Id },
                    new Game { Name = "Animal Crossing: NH", Rating = 90, PlatformId = platforms[3].Id }
                };
                context.Games.AddRange(games);
                context.SaveChanges();
            }
        }

        Application.Run(new MainForm());
    }
}

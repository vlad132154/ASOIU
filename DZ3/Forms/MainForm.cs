namespace Homework3.Forms;

/// <summary>
/// Главное окно приложения с навигацией между разделами
/// </summary>
public partial class MainForm : Form
{
    public MainForm()
    {
        Text = "Главное меню — ДЗ №3 (Вариант 12)";
        Size = new Size(400, 300);
        StartPosition = FormStartPosition.CenterScreen;

        var btnPlatforms = new Button { Text = "Игровые платформы", Dock = DockStyle.Top, Height = 60 };
        var btnGames = new Button { Text = "Видеоигры", Dock = DockStyle.Top, Height = 60 };
        var btnReport = new Button { Text = "Отчёт", Dock = DockStyle.Top, Height = 60 };

        btnPlatforms.Click += (s, e) => new PlatformForm().ShowDialog();
        btnGames.Click += (s, e) => new GameForm().ShowDialog();
        btnReport.Click += (s, e) => new ReportForm().ShowDialog();

        Controls.Add(btnReport);
        Controls.Add(btnGames);
        Controls.Add(btnPlatforms);
    }
}

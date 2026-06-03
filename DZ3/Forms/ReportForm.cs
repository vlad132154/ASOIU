using Homework3.Data;
using Microsoft.EntityFrameworkCore;

namespace Homework3.Forms;

/// <summary>
/// Форма отчёта с тремя разделами (LINQ-запросы)
/// </summary>
public partial class ReportForm : Form
{
    public ReportForm()
    {
        Text = "Отчёт";
        Size = new Size(800, 600);
        StartPosition = FormStartPosition.CenterParent;

        var tabControl = new TabControl { Dock = DockStyle.Fill };

        var tab1 = new TabPage("Раздел 1: Полный список");
        var dgv1 = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false
        };
        dgv1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название игры", DataPropertyName = "GameName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        dgv1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Платформа", DataPropertyName = "PlatformName", Width = 150 });
        dgv1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Оценка", DataPropertyName = "Rating", Width = 100 });
        tab1.Controls.Add(dgv1);

        var tab2 = new TabPage("Раздел 2: Количество по категориям");
        var dgv2 = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false
        };
        dgv2.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Платформа", DataPropertyName = "Platform", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        dgv2.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Количество", DataPropertyName = "Count", Width = 100 });
        tab2.Controls.Add(dgv2);

        var tab3 = new TabPage("Раздел 3: Средняя оценка по категориям");
        var dgv3 = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false
        };
        dgv3.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Платформа", DataPropertyName = "Platform", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        dgv3.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Средняя оценка", DataPropertyName = "AvgRating", Width = 150 });
        tab3.Controls.Add(dgv3);

        tabControl.TabPages.Add(tab1);
        tabControl.TabPages.Add(tab2);
        tabControl.TabPages.Add(tab3);
        Controls.Add(tabControl);

        LoadReport1(dgv1);
        LoadReport2(dgv2);
        LoadReport3(dgv3);
    }

    private void LoadReport1(DataGridView dgv)
    {
        using var context = new AppDbContext();
        var data = context.Games
            .Include(g => g.Platform)
            .OrderBy(g => g.Name)
            .Select(g => new
            {
                GameName = g.Name,
                PlatformName = g.Platform!.Name,
                g.Rating
            })
            .ToList();
        dgv.DataSource = data;
    }

    private void LoadReport2(DataGridView dgv)
    {
        using var context = new AppDbContext();
        var data = context.Games
            .GroupBy(g => g.Platform!.Name)
            .Select(g => new
            {
                Platform = g.Key,
                Count = g.Count()
            })
            .OrderBy(r => r.Platform)
            .ToList();
        dgv.DataSource = data;
    }

    private void LoadReport3(DataGridView dgv)
    {
        using var context = new AppDbContext();
        var data = context.Games
            .GroupBy(g => g.Platform!.Name)
            .Select(g => new
            {
                Platform = g.Key,
                AvgRating = g.Average(x => x.Rating)
            })
            .OrderByDescending(r => r.AvgRating)
            .ToList();
        dgv.DataSource = data;
    }
}

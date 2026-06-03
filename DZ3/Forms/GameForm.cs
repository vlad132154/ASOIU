using Homework3.Data;
using Microsoft.EntityFrameworkCore;
using Homework3.Models;

namespace Homework3.Forms;

/// <summary>
/// Форма для работы с основной таблицей видеоигр (CRUD)
/// </summary>
public partial class GameForm : Form
{
    private DataGridView dgv = null!;
    private Button btnAdd = null!, btnEdit = null!, btnDelete = null!, btnRefresh = null!;

    public GameForm()
    {
        Text = "Основная таблица: Видеоигры";
        Size = new Size(700, 450);
        StartPosition = FormStartPosition.CenterParent;

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false
        };
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Название", Width = 200 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PlatformName", HeaderText = "Платформа", Width = 150 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Rating", HeaderText = "Оценка", Width = 80 });

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
        btnAdd = new Button { Text = "Добавить", Left = 10, Top = 8 };
        btnEdit = new Button { Text = "Изменить", Left = 100, Top = 8 };
        btnDelete = new Button { Text = "Удалить", Left = 190, Top = 8 };
        btnRefresh = new Button { Text = "Обновить", Left = 280, Top = 8 };

        btnAdd.Click += BtnAdd_Click;
        btnEdit.Click += BtnEdit_Click;
        btnDelete.Click += BtnDelete_Click;
        btnRefresh.Click += (s, e) => LoadData();

        panel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });

        Controls.Add(dgv);
        Controls.Add(panel);

        LoadData();
    }

    private void LoadData()
    {
        using var context = new AppDbContext();
        var data = context.Games
            .Include(g => g.Platform)
            .OrderBy(g => g.Name)
            .Select(g => new
            {
                g.Id,
                g.Name,
                PlatformName = g.Platform!.Name,
                g.Rating
            })
            .ToList();
        dgv.DataSource = data;
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var dialog = new GameEditForm();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            using var context = new AppDbContext();
            context.Games.Add(dialog.Game);
            context.SaveChanges();
            LoadData();
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var row = dgv.SelectedRows[0];
        int id = (int)row.Cells["Id"].Value;

        using var context = new AppDbContext();
        var game = context.Games.Find(id);
        if (game == null) return;

        var dialog = new GameEditForm(game);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            game.Name = dialog.Game.Name;
            game.PlatformId = dialog.Game.PlatformId;
            game.Rating = dialog.Game.Rating;
            context.SaveChanges();
            LoadData();
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var row = dgv.SelectedRows[0];
        int id = (int)row.Cells["Id"].Value;
        string name = row.Cells["Name"].Value?.ToString() ?? "";

        if (MessageBox.Show($"Удалить игру '{name}'?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            using var context = new AppDbContext();
            var game = context.Games.Find(id);
            if (game != null)
            {
                context.Games.Remove(game);
                context.SaveChanges();
            }
            LoadData();
        }
    }
}

/// <summary>
/// Форма добавления/редактирования видеоигры
/// </summary>
public class GameEditForm : Form
{
    /// <summary>
    /// Редактируемая или новая видеоигра
    /// </summary>
    public Game Game { get; private set; }

    private TextBox txtName = null!;
    private ComboBox cmbPlatform = null!;
    private NumericUpDown numRating = null!;
    private Button btnOk = null!;

    public GameEditForm(Game? game = null)
    {
        Game = game ?? new Game();
        Text = game == null ? "Добавить игру" : "Редактировать игру";
        Size = new Size(400, 250);
        StartPosition = FormStartPosition.CenterParent;

        var lblName = new Label { Text = "Название:", Top = 20, Left = 20, Width = 100 };
        txtName = new TextBox { Text = Game.Name, Top = 20, Left = 130, Width = 200 };

        var lblPlatform = new Label { Text = "Платформа:", Top = 60, Left = 20, Width = 100 };
        cmbPlatform = new ComboBox { Top = 60, Left = 130, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Name", ValueMember = "Id" };

        var lblRating = new Label { Text = "Оценка:", Top = 100, Left = 20, Width = 100 };
        numRating = new NumericUpDown { Top = 100, Left = 130, Width = 200, Minimum = 0, Maximum = 100, Value = Math.Max(0, Game.Rating) };

        btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Top = 150, Left = 130, Width = 100 };
        btnOk.Click += BtnOk_Click;

        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblPlatform);
        Controls.Add(cmbPlatform);
        Controls.Add(lblRating);
        Controls.Add(numRating);
        Controls.Add(btnOk);

        LoadPlatforms();
    }

    private void LoadPlatforms()
    {
        using var context = new AppDbContext();
        cmbPlatform.DataSource = context.Platforms.OrderBy(p => p.Name).ToList();
        if (Game.PlatformId > 0)
            cmbPlatform.SelectedValue = Game.PlatformId;
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Название игры не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }

        int rating = (int)numRating.Value;
        if (rating < 0)
        {
            MessageBox.Show("Оценка не может быть отрицательной.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }

        Game.Name = txtName.Text.Trim();
        Game.PlatformId = (int)cmbPlatform.SelectedValue!;
        Game.Rating = rating;
    }
}

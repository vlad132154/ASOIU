using Homework3.Data;
using Homework3.Models;

namespace Homework3.Forms;

/// <summary>
/// Форма для работы со справочником игровых платформ (CRUD)
/// </summary>
public partial class PlatformForm : Form
{
    private DataGridView dgv = null!;
    private Button btnAdd = null!, btnEdit = null!, btnDelete = null!, btnRefresh = null!;

    public PlatformForm()
    {
        Text = "Справочник: Игровые платформы";
        Size = new Size(600, 400);
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
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Название", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

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
        dgv.DataSource = context.Platforms.OrderBy(p => p.Name).ToList();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var name = Prompt("Введите название платформы:");
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Название не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var context = new AppDbContext();
        context.Platforms.Add(new Platform { Name = name });
        context.SaveChanges();
        LoadData();
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var platform = (Platform)dgv.SelectedRows[0].DataBoundItem;
        var name = Prompt("Введите новое название:", platform.Name);
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Название не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var context = new AppDbContext();
        var p = context.Platforms.Find(platform.Id);
        if (p != null)
        {
            p.Name = name;
            context.SaveChanges();
        }
        LoadData();
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var platform = (Platform)dgv.SelectedRows[0].DataBoundItem;

        using var context = new AppDbContext();
        if (context.Games.Any(g => g.PlatformId == platform.Id))
        {
            MessageBox.Show("Невозможно удалить платформу, так как с ней связаны видеоигры.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (MessageBox.Show($"Удалить платформу '{platform.Name}'?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            var p = context.Platforms.Find(platform.Id);
            if (p != null)
            {
                context.Platforms.Remove(p);
                context.SaveChanges();
            }
            LoadData();
        }
    }

    private static string? Prompt(string text, string defaultValue = "")
    {
        using var form = new Form { Text = text, Size = new Size(400, 150), StartPosition = FormStartPosition.CenterParent };
        var txt = new TextBox { Text = defaultValue, Dock = DockStyle.Top };
        var btn = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom };
        form.Controls.Add(txt);
        form.Controls.Add(btn);
        form.AcceptButton = btn;
        return form.ShowDialog() == DialogResult.OK ? txt.Text : null;
    }
}

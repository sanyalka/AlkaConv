using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.Win32;

public class MainForm : Form
{
    private CheckBox chkMain;
    private CheckBox chkHigh;
    private CheckBox chkDel;
    private Button btnApply;
    private Button btnRemove;
    private Panel panelOptions;
    private Button btnMinimize;
    private Button btnClose;
    private Label lblTitle;

    private const string baseKey = @"Software\Classes\SystemFileAssociations\.pdf\Shell";

    public MainForm()
    {
        InitializeComponent();
        LoadState();
    }

    private void InitializeComponent()
    {
        // Настройка формы
        this.Text = "AlkaConv";
        this.Size = new Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(30, 30, 30);
        this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
        this.FormBorderStyle = FormBorderStyle.None;
        this.Padding = new Padding(1);

        // Иконка
        try
        {
            string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
            if (System.IO.File.Exists(iconPath))
                this.Icon = new Icon(iconPath);
            else
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }
        catch { }

        // Скругление формы
        this.Paint += (s, e) =>
        {
            GraphicsPath path = new GraphicsPath();
            int radius = 20;
            Rectangle rect = this.ClientRectangle;
            rect.Inflate(-1, -1);
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);
        };

        // Панель-карточка
        panelOptions = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(45, 45, 48),
            Padding = new Padding(20, 45, 20, 20)
        };
        panelOptions.Paint += (s, e) =>
        {
            GraphicsPath path = new GraphicsPath();
            int radius = 20;
            Rectangle rect = panelOptions.ClientRectangle;
            rect.Inflate(-1, -1);
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            panelOptions.Region = new Region(path);
        };
        this.Controls.Add(panelOptions);

        // ---------- ПЕРЕТАСКИВАНИЕ ОКНА ----------
        panelOptions.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                Control ctrl = panelOptions.GetChildAtPoint(e.Location);
                if (ctrl == null || ctrl == lblTitle)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(this.Handle, 0xA1, 0x2, 0);
                }
            }
        };

        // Заголовок
        lblTitle = new Label
        {
            Text = "AlkaConv",
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            Location = new Point(20, 10),
            AutoSize = true
        };
        panelOptions.Controls.Add(lblTitle);

        lblTitle.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, 0xA1, 0x2, 0);
            }
        };

        // ---------- КНОПКИ УПРАВЛЕНИЯ ОКНОМ ----------
        btnMinimize = new Button
        {
            Text = "─",
            Size = new Size(30, 30),
            Location = new Point(panelOptions.Width - 70, 5),
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 },
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 14f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            TabStop = false
        };
        btnMinimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 70);
        btnMinimize.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 50);
        btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
        panelOptions.Controls.Add(btnMinimize);

        btnClose = new Button
        {
            Text = "✕",
            Size = new Size(30, 30),
            Location = new Point(panelOptions.Width - 35, 5),
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 },
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            TabStop = false
        };
        btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
        btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(150, 30, 30);
        btnClose.Click += (s, e) => this.Close();
        panelOptions.Controls.Add(btnClose);

        panelOptions.Resize += (s, e) =>
        {
            btnMinimize.Location = new Point(panelOptions.Width - 70, 5);
            btnClose.Location = new Point(panelOptions.Width - 35, 5);
        };

        // ---------- ЧЕКБОКСЫ (с НАСТРОЙКОЙ читаемости) ----------
        chkMain = new CheckBox
        {
            Text = "Конвертация в JPG",
            Location = new Point(20, 50),
            AutoSize = true
        };
        SetupCheckBox(chkMain);
        panelOptions.Controls.Add(chkMain);

        chkHigh = new CheckBox
        {
            Text = "Конвертация в JPG в папке",
            Location = new Point(20, 100),
            AutoSize = true
        };
        SetupCheckBox(chkHigh);
        panelOptions.Controls.Add(chkHigh);

        chkDel = new CheckBox
        {
            Text = "Конвертация в JPG с удалением PDF",
            Location = new Point(20, 150),
            AutoSize = true
        };
        SetupCheckBox(chkDel);
        panelOptions.Controls.Add(chkDel);

        // ---------- КНОПКИ ПО ЦЕНТРУ ----------
        int buttonWidth = 140;
        int spacing = 20;
        int totalWidth = 2 * buttonWidth + spacing;
        int startX = (panelOptions.Width - totalWidth) / 2;

        btnApply = new Button
        {
            Text = "Сохранить",
            Size = new Size(buttonWidth, 40),
            Location = new Point(startX, 200),
            BackColor = Color.FromArgb(0, 122, 204),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 },
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnApply.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 150, 230);
        btnApply.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 100, 180);
        btnApply.Click += Apply;
        panelOptions.Controls.Add(btnApply);

        btnRemove = new Button
        {
            Text = "Очистить все",
            Size = new Size(buttonWidth, 40),
            Location = new Point(startX + buttonWidth + spacing, 200),
            BackColor = Color.FromArgb(60, 60, 70),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 },
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnRemove.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 90, 100);
        btnRemove.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 50);
        btnRemove.Click += Remove;
        panelOptions.Controls.Add(btnRemove);

        panelOptions.Resize += (s, e) =>
        {
            int newStartX = (panelOptions.Width - totalWidth) / 2;
            btnApply.Location = new Point(newStartX, 200);
            btnRemove.Location = new Point(newStartX + buttonWidth + spacing, 200);
        };
    }

    // ---------- МЕТОД НАСТРОЙКИ ЧЕКБОКСА (гарантирует читаемость) ----------
    private void SetupCheckBox(CheckBox chk)
    {
        chk.UseVisualStyleBackColor = false;          // отключаем системные цвета
        chk.FlatStyle = FlatStyle.Flat;               // плоский стиль
        chk.ForeColor = Color.White;                  // белый текст
        chk.Font = new Font("Segoe UI", 11f, FontStyle.Bold); // крупный жирный шрифт

        // Настройка внешнего вида квадратика галочки
        chk.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
        chk.FlatAppearance.BorderSize = 1;
        chk.FlatAppearance.CheckedBackColor = Color.FromArgb(0, 122, 204);
        chk.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 100, 180);
        chk.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 150, 230);
    }

    // ---------- РАБОТА С РЕЕСТРОМ ----------
    private void LoadState()
    {
        chkMain.Checked = Exists("ConvertToJPG");
        chkHigh.Checked = Exists("ConvertToJPG-High");
        chkDel.Checked = Exists("ConvertToJPG-Delete");
    }

    private bool Exists(string name)
    {
        using var k = Registry.CurrentUser.OpenSubKey($@"{baseKey}\{name}");
        return k != null;
    }

    private void Apply(object? sender, EventArgs e)
    {
        if (chkMain.Checked)
            Create("ConvertToJPG", "В JPG", $"\"{Application.ExecutablePath}\" \"%1\"");
        else
            Delete("ConvertToJPG");

        if (chkHigh.Checked)
            Create("ConvertToJPG-High", "В JPG в папке", $"\"{Application.ExecutablePath}\" -subfolder \"%1\"");
        else
            Delete("ConvertToJPG-High");

        if (chkDel.Checked)
            Create("ConvertToJPG-Delete", "В JPG с удалением PDF", $"\"{Application.ExecutablePath}\" -subfolder -delete \"%1\"");
        else
            Delete("ConvertToJPG-Delete");

        LoadState();
    }

    private void Remove(object? sender, EventArgs e)
    {
        Delete("ConvertToJPG");
        Delete("ConvertToJPG-High");
        Delete("ConvertToJPG-Delete");
        LoadState();
    }

    private void Create(string name, string text, string cmd)
    {
        using var k = Registry.CurrentUser.CreateSubKey($@"{baseKey}\{name}");
        k.SetValue("", text);
        k.SetValue("Icon", Application.ExecutablePath);

        using var c = Registry.CurrentUser.CreateSubKey($@"{baseKey}\{name}\command");
        c.SetValue("", cmd);
    }

    private void Delete(string name)
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree($@"{baseKey}\{name}", false);
        }
        catch { }
    }

    private static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}
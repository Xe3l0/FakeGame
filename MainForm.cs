using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FakeExe
{
    public class MainForm : Form
    {
        // Colors
        private readonly Color BG_DARK = Color.FromArgb(15, 16, 20);
        private readonly Color BG_CARD = Color.FromArgb(24, 26, 32);
        private readonly Color BG_INPUT = Color.FromArgb(32, 34, 42);
        private readonly Color ACCENT = Color.FromArgb(88, 101, 242);
        private readonly Color ACCENT_HOVER = Color.FromArgb(110, 122, 255);
        private readonly Color ACCENT_GREEN = Color.FromArgb(35, 165, 90);
        private readonly Color ACCENT_RED = Color.FromArgb(210, 60, 60);
        private readonly Color TEXT_PRIMARY = Color.FromArgb(240, 242, 255);
        private readonly Color TEXT_SECONDARY = Color.FromArgb(140, 145, 170);
        private readonly Color BORDER = Color.FromArgb(45, 48, 62);

        // Controls
        private RoundedPanel headerPanel;
        private Label titleLabel;
        private Label subtitleLabel;
        private RoundedPanel cardPanel;
        private Label clientIdLabel;
        private RoundedTextBox clientIdBox;
        private Label gameNameLabel;
        private RoundedTextBox gameNameBox;
        private Label detailsLabel;
        private RoundedTextBox detailsBox;
        private Label stateLabel;
        private RoundedTextBox stateBox;
        private Label imageKeyLabel;
        private RoundedTextBox imageKeyBox;
        private RoundedPanel statusPanel;
        private Label statusDot;
        private Label statusLabel;
        private Label timerLabel;
        private GlowButton startButton;
        private GlowButton stopButton;
        private Label footerLabel;

        // Logic
        private DiscordRPC _rpc;
        private System.Windows.Forms.Timer _timer;
        private int _seconds = 0;
        private bool _running = false;
        private NotifyIcon _trayIcon;
        private bool _realClose = false;

        public MainForm()
        {
            InitializeComponents();
            SetupTimer();
            SetupTray();
        }

        private void InitializeComponents()
        {
            // Form setup
            this.Text = "FakeExe";
            this.Size = new Size(480, 680);
            this.MinimumSize = new Size(480, 680);
            this.MaximumSize = new Size(480, 680);
            this.BackColor = BG_DARK;
            this.ForeColor = TEXT_PRIMARY;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);

            try
            {
                // Try to set a nice icon
                this.Icon = SystemIcons.Application;
            }
            catch { }

            int x = 24;
            int y = 24;
            int w = 432;

            // ── Header ──────────────────────────────────────────────
            headerPanel = new RoundedPanel(12);
            headerPanel.SetBounds(x, y, w, 80);
            headerPanel.BackColor = BG_CARD;
            headerPanel.BorderColor = BORDER;

            var logo = new Label();
            logo.Text = "🎮";
            logo.Font = new Font("Segoe UI Emoji", 22f);
            logo.SetBounds(16, 14, 50, 50);
            logo.BackColor = Color.Transparent;

            titleLabel = new Label();
            titleLabel.Text = "FakeExe";
            titleLabel.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
            titleLabel.ForeColor = TEXT_PRIMARY;
            titleLabel.SetBounds(74, 12, 200, 32);
            titleLabel.BackColor = Color.Transparent;

            subtitleLabel = new Label();
            subtitleLabel.Text = "Discord Rich Presence Spoofer";
            subtitleLabel.Font = new Font("Segoe UI", 9f);
            subtitleLabel.ForeColor = TEXT_SECONDARY;
            subtitleLabel.SetBounds(76, 44, 250, 20);
            subtitleLabel.BackColor = Color.Transparent;

            var versionLabel = new Label();
            versionLabel.Text = "v1.0";
            versionLabel.Font = new Font("Segoe UI", 8f);
            versionLabel.ForeColor = Color.FromArgb(80, 85, 110);
            versionLabel.SetBounds(370, 30, 40, 20);
            versionLabel.BackColor = Color.Transparent;

            headerPanel.Controls.AddRange(new Control[] { logo, titleLabel, subtitleLabel, versionLabel });
            this.Controls.Add(headerPanel);
            y += 96;

            // ── Card ────────────────────────────────────────────────
            cardPanel = new RoundedPanel(12);
            cardPanel.SetBounds(x, y, w, 370);
            cardPanel.BackColor = BG_CARD;
            cardPanel.BorderColor = BORDER;

            int cx = 16;
            int cy = 16;
            int cw = 400;

            // Client ID
            clientIdLabel = MakeLabel("Discord Application ID", cx, cy);
            cy += 22;
            clientIdBox = MakeTextBox(cx, cy, cw, "e.g. 1234567890123456789");
            clientIdBox.Text = "1234567890123456789";
            cy += 44;

            var clientIdHint = new Label();
            clientIdHint.Text = "ℹ  Get your own ID from discord.com/developers — or use the default for basic presence";
            clientIdHint.Font = new Font("Segoe UI", 7.5f);
            clientIdHint.ForeColor = Color.FromArgb(100, 105, 130);
            clientIdHint.SetBounds(cx, cy - 10, cw, 30);
            clientIdHint.BackColor = Color.Transparent;
            cy += 24;

            // Game Name
            gameNameLabel = MakeLabel("Game Name", cx, cy);
            cy += 22;
            gameNameBox = MakeTextBox(cx, cy, cw, "e.g. Minecraft, Valorant, GTA V...");
            cy += 44;

            // Details
            detailsLabel = MakeLabel("Details (Line 1)", cx, cy);
            cy += 22;
            detailsBox = MakeTextBox(cx, cy, cw, "e.g. In the main menu");
            cy += 44;

            // State
            stateLabel = MakeLabel("State (Line 2)", cx, cy);
            cy += 22;
            stateBox = MakeTextBox(cx, cy, cw, "e.g. Level 12 | Ranked Match");
            cy += 44;

            // Image Key
            imageKeyLabel = MakeLabel("Large Image Key (optional)", cx, cy);
            cy += 22;
            imageKeyBox = MakeTextBox(cx, cy, cw, "e.g. game_logo  (from your Discord app assets)");
            cy += 44;

            cardPanel.Controls.AddRange(new Control[] {
                clientIdLabel, clientIdBox, clientIdHint,
                gameNameLabel, gameNameBox,
                detailsLabel, detailsBox,
                stateLabel, stateBox,
                imageKeyLabel, imageKeyBox
            });

            this.Controls.Add(cardPanel);
            y += 386;

            // ── Status Panel ────────────────────────────────────────
            statusPanel = new RoundedPanel(10);
            statusPanel.SetBounds(x, y, w, 52);
            statusPanel.BackColor = BG_CARD;
            statusPanel.BorderColor = BORDER;

            statusDot = new Label();
            statusDot.Text = "●";
            statusDot.Font = new Font("Segoe UI", 11f);
            statusDot.ForeColor = Color.FromArgb(70, 75, 100);
            statusDot.SetBounds(14, 16, 20, 20);
            statusDot.BackColor = Color.Transparent;

            statusLabel = new Label();
            statusLabel.Text = "Not connected";
            statusLabel.Font = new Font("Segoe UI", 9f);
            statusLabel.ForeColor = TEXT_SECONDARY;
            statusLabel.SetBounds(36, 17, 220, 20);
            statusLabel.BackColor = Color.Transparent;

            timerLabel = new Label();
            timerLabel.Text = "00:00:00";
            timerLabel.Font = new Font("Consolas", 13f, FontStyle.Bold);
            timerLabel.ForeColor = Color.FromArgb(70, 75, 100);
            timerLabel.SetBounds(300, 14, 120, 24);
            timerLabel.BackColor = Color.Transparent;
            timerLabel.TextAlign = ContentAlignment.MiddleRight;

            statusPanel.Controls.AddRange(new Control[] { statusDot, statusLabel, timerLabel });
            this.Controls.Add(statusPanel);
            y += 68;

            // ── Buttons ─────────────────────────────────────────────
            startButton = new GlowButton();
            startButton.Text = "▶  Start Presence";
            startButton.SetBounds(x, y, 208, 46);
            startButton.NormalColor = ACCENT;
            startButton.HoverColor = ACCENT_HOVER;
            startButton.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            startButton.ForeColor = Color.White;
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);

            stopButton = new GlowButton();
            stopButton.Text = "■  Stop";
            stopButton.SetBounds(x + 224, y, 208, 46);
            stopButton.NormalColor = Color.FromArgb(50, 52, 68);
            stopButton.HoverColor = ACCENT_RED;
            stopButton.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            stopButton.ForeColor = TEXT_SECONDARY;
            stopButton.Enabled = false;
            stopButton.Click += StopButton_Click;
            this.Controls.Add(stopButton);
            y += 62;

            // ── Footer ──────────────────────────────────────────────
            footerLabel = new Label();
            footerLabel.Text = "github.com/YourUsername/FakeExe  •  Open Source";
            footerLabel.Font = new Font("Segoe UI", 8f);
            footerLabel.ForeColor = Color.FromArgb(60, 65, 85);
            footerLabel.SetBounds(x, y, w, 20);
            footerLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(footerLabel);
        }

        private Label MakeLabel(string text, int x, int y)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lbl.ForeColor = TEXT_SECONDARY;
            lbl.SetBounds(x, y, 400, 18);
            lbl.BackColor = Color.Transparent;
            return lbl;
        }

        private RoundedTextBox MakeTextBox(int x, int y, int w, string placeholder)
        {
            var tb = new RoundedTextBox();
            tb.SetBounds(x, y, w, 34);
            tb.BackColor = BG_INPUT;
            tb.ForeColor = TEXT_PRIMARY;
            tb.BorderColor = BORDER;
            tb.PlaceholderText = placeholder;
            tb.PlaceholderColor = Color.FromArgb(70, 75, 100);
            tb.Font = new Font("Segoe UI", 9.5f);
            return tb;
        }

        private void SetupTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000;
            _timer.Tick += (s, e) =>
            {
                _seconds++;
                int h = _seconds / 3600;
                int m = (_seconds % 3600) / 60;
                int sec = _seconds % 60;
                timerLabel.Text = $"{h:D2}:{m:D2}:{sec:D2}";
                timerLabel.ForeColor = ACCENT;
            };
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            string clientId = clientIdBox.Text.Trim();
            string gameName = gameNameBox.Text.Trim();
            string details = detailsBox.Text.Trim();
            string state = stateBox.Text.Trim();
            string imageKey = imageKeyBox.Text.Trim();

            if (string.IsNullOrEmpty(gameName))
            {
                ShowError("Please enter a game name!");
                gameNameBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(clientId))
                clientId = "1234567890123456789";

            _rpc = new DiscordRPC(clientId);

            SetStatus("Connecting to Discord...", Color.FromArgb(200, 170, 50));

            Thread connectThread = new Thread(() =>
            {
                bool connected = _rpc.Connect();
                this.Invoke((Action)(() =>
                {
                    if (connected)
                    {
                        _rpc.SetPresence(
                            gameName,
                            string.IsNullOrEmpty(details) ? $"Playing {gameName}" : details,
                            string.IsNullOrEmpty(state) ? "In Game" : state,
                            string.IsNullOrEmpty(imageKey) ? "game" : imageKey,
                            gameName
                        );

                        _running = true;
                        _seconds = 0;
                        _timer.Start();

                        SetStatus($"Playing {gameName}", Color.FromArgb(35, 165, 90));
                        startButton.Enabled = false;
                        stopButton.Enabled = true;
                        stopButton.ForeColor = Color.White;
                    }
                    else
                    {
                        _rpc = null;
                        SetStatus("Discord not found — is it running?", Color.FromArgb(210, 60, 60));
                    }
                }));
            });

            connectThread.IsBackground = true;
            connectThread.Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            _running = false;
            _seconds = 0;
            timerLabel.Text = "00:00:00";
            timerLabel.ForeColor = Color.FromArgb(70, 75, 100);

            _rpc?.Disconnect();
            _rpc = null;

            SetStatus("Not connected", Color.FromArgb(70, 75, 100));
            startButton.Enabled = true;
            stopButton.Enabled = false;
            stopButton.ForeColor = Color.FromArgb(140, 145, 170);
        }

        private void SetStatus(string text, Color dotColor)
        {
            statusLabel.Text = text;
            statusDot.ForeColor = dotColor;
        }

        private void ShowError(string msg)
        {
            MessageBox.Show(msg, "FakeExe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FakeExe", "settings.txt");

        private void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                var lines = new[]
                {
                    clientIdBox.Text,
                    gameNameBox.Text,
                    detailsBox.Text,
                    stateBox.Text,
                    imageKeyBox.Text
                };
                File.WriteAllLines(SettingsPath, lines);
            }
            catch { }
        }

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return;
                var lines = File.ReadAllLines(SettingsPath);
                if (lines.Length > 0 && !string.IsNullOrEmpty(lines[0])) clientIdBox.Text = lines[0];
                if (lines.Length > 1) gameNameBox.Text = lines[1];
                if (lines.Length > 2) detailsBox.Text = lines[2];
                if (lines.Length > 3) stateBox.Text = lines[3];
                if (lines.Length > 4) imageKeyBox.Text = lines[4];
            }
            catch { }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadSettings();
        }

        private void SetupTray()
        {
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "FakeExe";
            _trayIcon.Icon = SystemIcons.Application;
            _trayIcon.Visible = true;

            var menu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show");
            showItem.Click += (s, e) => ShowWindow();
            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => { _realClose = true; Application.Exit(); };

            menu.Items.Add(showItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exitItem);
            _trayIcon.ContextMenuStrip = menu;

            _trayIcon.DoubleClick += (s, e) => ShowWindow();
        }

        private void ShowWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_realClose)
            {
                e.Cancel = true;
                this.Hide();
                _trayIcon.ShowBalloonTip(1500, "FakeExe", "Still running in the background", ToolTipIcon.Info);
                return;
            }
            SaveSettings();
            _trayIcon.Visible = false;
            _rpc?.Disconnect();
            base.OnFormClosing(e);
        }
    }

    // ── Custom Controls ───────────────────────────────────────────────

    public class RoundedPanel : Panel
    {
        private int _radius;
        public Color BorderColor { get; set; } = Color.FromArgb(45, 48, 62);

        public RoundedPanel(int radius)
        {
            _radius = radius;
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(1, 1, Width - 2, Height - 2);
            using (var path = RoundRect(rect, _radius))
            using (var brush = new SolidBrush(BackColor))
            using (var pen = new Pen(BorderColor, 1))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        private GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class PlaceholderTextBox : TextBox
    {
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        private string _placeholder = "";
        public string PlaceholderText
        {
            get => _placeholder;
            set
            {
                _placeholder = value;
                if (IsHandleCreated)
                    SendMessage(Handle, EM_SETCUEBANNER, 1, value);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!string.IsNullOrEmpty(_placeholder))
                SendMessage(Handle, EM_SETCUEBANNER, 1, _placeholder);
        }
    }

    public class RoundedTextBox : Control
    {
        private PlaceholderTextBox _inner;
        public Color BorderColor { get; set; } = Color.FromArgb(45, 48, 62);

        public string PlaceholderText
        {
            get => _inner.PlaceholderText;
            set => _inner.PlaceholderText = value;
        }

        public Color PlaceholderColor { get; set; } = Color.Gray;

        public new string Text
        {
            get => _inner.Text;
            set => _inner.Text = value ?? "";
        }

        public RoundedTextBox()
        {
            this.DoubleBuffered = true;
            _inner = new PlaceholderTextBox();
            _inner.BorderStyle = BorderStyle.None;
            _inner.BackColor = Color.FromArgb(32, 34, 42);
            _inner.ForeColor = Color.FromArgb(240, 242, 255);

            _inner.GotFocus += (s, e) =>
            {
                BorderColor = Color.FromArgb(88, 101, 242);
                Invalidate();
            };

            _inner.LostFocus += (s, e) =>
            {
                BorderColor = Color.FromArgb(45, 48, 62);
                Invalidate();
            };

            this.Controls.Add(_inner);

            this.SizeChanged += (s, e) =>
            {
                _inner.SetBounds(10, (Height - _inner.Height) / 2, Width - 20, _inner.Height);
            };
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (_inner != null) _inner.Font = this.Font;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(1, 1, Width - 2, Height - 2);
            using (var path = RoundRect(rect, 7))
            using (var brush = new SolidBrush(BackColor))
            using (var pen = new Pen(BorderColor, 1.5f))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        private GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public new void Focus() => _inner.Focus();
    }

    public class GlowButton : Control
    {
        public Color NormalColor { get; set; } = Color.FromArgb(88, 101, 242);
        public Color HoverColor { get; set; } = Color.FromArgb(110, 122, 255);
        private bool _hover = false;
        private bool _pressed = false;

        public GlowButton()
        {
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            this.MouseEnter += (s, e) => { _hover = true; Invalidate(); };
            this.MouseLeave += (s, e) => { _hover = false; _pressed = false; Invalidate(); };
            this.MouseDown += (s, e) => { _pressed = true; Invalidate(); };
            this.MouseUp += (s, e) => { _pressed = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color bg = !Enabled ? Color.FromArgb(40, 42, 55) : _pressed ? NormalColor : _hover ? HoverColor : NormalColor;
            Color fg = !Enabled ? Color.FromArgb(70, 75, 100) : ForeColor;

            // Glow effect
            if (Enabled && _hover)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(30, HoverColor)))
                {
                    var glowRect = new Rectangle(-2, -2, Width + 4, Height + 4);
                    using (var path = RoundRect(glowRect, 12))
                        e.Graphics.FillPath(glowBrush, path);
                }
            }

            var rect = new Rectangle(1, 1, Width - 2, Height - 2);
            using (var path = RoundRect(rect, 10))
            using (var brush = new SolidBrush(bg))
            {
                e.Graphics.FillPath(brush, path);
            }

            using (var brush = new SolidBrush(fg))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(Text, Font, brush, new RectangleF(0, 0, Width, Height), sf);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        private GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

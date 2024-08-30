namespace Chess
{
    public partial class GameOverDialog : Form
    {
        private readonly Color DarkColor = Color.FromArgb(33, 32, 46);
        private readonly Color LightColor = Color.FromArgb(82, 79, 103);

        public GameOverDialog(string message)
        {
            InitializeComponent();

            GameOverLabel.Text = message;
            GameOverLabel.TextAlign = ContentAlignment.MiddleCenter;

            BackColor = DarkColor;
            RestartButton.Click += (sender, e) => DialogResult = DialogResult.Retry;
            RestartButton.BackColor = DarkColor;
            RestartButton.FlatStyle = FlatStyle.Flat;
            RestartButton.FlatAppearance.BorderSize = 1;
            RestartButton.FlatAppearance.BorderColor = LightColor;
            ExitButton.Click += (sender, e) => DialogResult = DialogResult.No;
            ExitButton.BackColor = DarkColor;
            ExitButton.FlatStyle = FlatStyle.Flat;
            ExitButton.FlatAppearance.BorderSize = 1;
            ExitButton.FlatAppearance.BorderColor = LightColor;
        }
    }
}
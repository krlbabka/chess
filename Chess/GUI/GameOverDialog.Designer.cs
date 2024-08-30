namespace Chess
{
    partial class GameOverDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            GameOverLabel = new Label();
            RestartButton = new Button();
            ExitButton = new Button();
            SuspendLayout();
            // 
            // GameOverLabel
            // 
            GameOverLabel.AutoSize = true;
            GameOverLabel.Font = new Font("Verdana", 14F, FontStyle.Bold);
            GameOverLabel.Location = new Point(130, 46);
            GameOverLabel.Name = "GameOverLabel";
            GameOverLabel.Size = new Size(161, 29);
            GameOverLabel.TabIndex = 0;
            GameOverLabel.Text = "Game Over";
            GameOverLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RestartButton
            // 
            RestartButton.Location = new Point(54, 124);
            RestartButton.Name = "RestartButton";
            RestartButton.Size = new Size(128, 53);
            RestartButton.TabIndex = 2;
            RestartButton.Text = "Restart";
            // 
            // ExitButton
            // 
            ExitButton.Location = new Point(226, 124);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new Size(128, 53);
            ExitButton.TabIndex = 4;
            ExitButton.Text = "Exit";
            // 
            // GameOverDialog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(41, 39, 35);
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(400, 200);
            ControlBox = false;
            Controls.Add(GameOverLabel);
            Controls.Add(RestartButton);
            Controls.Add(ExitButton);
            ForeColor = Color.White;
            Name = "GameOverDialog";
            RightToLeft = RightToLeft.Yes;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Game Over";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label GameOverLabel;
        private Button RestartButton;
        private Button ExitButton;
    }
}
namespace Chess
{
    partial class GameWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));
            GamePanel = new TableLayoutPanel();
            ParentGamePanel = new Panel();
            ParentGamePanel.SuspendLayout();
            SuspendLayout();
            // 
            // GamePanel
            // 
            resources.ApplyResources(GamePanel, "GamePanel");
            GamePanel.BackColor = Color.FromArgb(144, 140, 170);
            GamePanel.Name = "GamePanel";
            // 
            // ParentGamePanel
            // 
            resources.ApplyResources(ParentGamePanel, "ParentGamePanel");
            ParentGamePanel.Controls.Add(GamePanel);
            ParentGamePanel.Name = "ParentGamePanel";
            // 
            // GameWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 23, 36);
            Controls.Add(ParentGamePanel);
            ForeColor = Color.FromArgb(224, 222, 244);
            Name = "GameWindow";
            Load += GameWindow_Load;
            ParentGamePanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel GamePanel;
        private Panel ParentGamePanel;
    }
}

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
            ParentGamePanel = new TableLayoutPanel();
            label2 = new Label();
            FileLabels = new TableLayoutPanel();
            label1 = new Label();
            RankLabels = new TableLayoutPanel();
            GamePanel = new TableLayoutPanel();
            BlackTimerLabel = new Label();
            WhiteTimerLabel = new Label();
            DoneMovesTable = new FlowLayoutPanel();
            label3 = new Label();
            ParentGamePanel.SuspendLayout();
            SuspendLayout();
            // 
            // ParentGamePanel
            // 
            resources.ApplyResources(ParentGamePanel, "ParentGamePanel");
            ParentGamePanel.Controls.Add(label2, 1, 3);
            ParentGamePanel.Controls.Add(FileLabels, 1, 2);
            ParentGamePanel.Controls.Add(label1, 1, 0);
            ParentGamePanel.Controls.Add(RankLabels, 0, 1);
            ParentGamePanel.Controls.Add(GamePanel, 1, 1);
            ParentGamePanel.Controls.Add(BlackTimerLabel, 2, 0);
            ParentGamePanel.Controls.Add(WhiteTimerLabel, 2, 3);
            ParentGamePanel.Controls.Add(DoneMovesTable, 2, 1);
            ParentGamePanel.Name = "ParentGamePanel";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // FileLabels
            // 
            resources.ApplyResources(FileLabels, "FileLabels");
            FileLabels.Name = "FileLabels";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // RankLabels
            // 
            resources.ApplyResources(RankLabels, "RankLabels");
            RankLabels.Name = "RankLabels";
            // 
            // GamePanel
            // 
            resources.ApplyResources(GamePanel, "GamePanel");
            GamePanel.BackColor = Color.FromArgb(144, 140, 170);
            GamePanel.Name = "GamePanel";
            // 
            // BlackTimerLabel
            // 
            resources.ApplyResources(BlackTimerLabel, "BlackTimerLabel");
            BlackTimerLabel.Name = "BlackTimerLabel";
            // 
            // WhiteTimerLabel
            // 
            resources.ApplyResources(WhiteTimerLabel, "WhiteTimerLabel");
            WhiteTimerLabel.Name = "WhiteTimerLabel";
            // 
            // DoneMovesTable
            // 
            resources.ApplyResources(DoneMovesTable, "DoneMovesTable");
            DoneMovesTable.Name = "DoneMovesTable";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // GameWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 23, 36);
            Controls.Add(label3);
            Controls.Add(ParentGamePanel);
            ForeColor = Color.FromArgb(224, 222, 244);
            Name = "GameWindow";
            Load += GameWindowLoad;
            ParentGamePanel.ResumeLayout(false);
            ParentGamePanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel ParentGamePanel;
        private TableLayoutPanel GamePanel;
        private TableLayoutPanel RankLabels;
        private TableLayoutPanel FileLabels;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label BlackTimerLabel;
        private Label WhiteTimerLabel;
        private FlowLayoutPanel DoneMovesTable;
    }
}

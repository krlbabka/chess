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
            FileLabels = new TableLayoutPanel();
            RankLabels = new TableLayoutPanel();
            GamePanel = new TableLayoutPanel();
            BlackTimerLabel = new Label();
            WhiteTimerLabel = new Label();
            DoneMovesTable = new FlowLayoutPanel();
            WhitePlayerMaterial = new Label();
            BlackPlayerMaterial = new Label();
            _blackTakenPiecesPanel = new FlowLayoutPanel();
            _whiteTakenPiecesPanel = new FlowLayoutPanel();
            ParentGamePanel.SuspendLayout();
            SuspendLayout();
            // 
            // ParentGamePanel
            // 
            resources.ApplyResources(ParentGamePanel, "ParentGamePanel");
            ParentGamePanel.Controls.Add(FileLabels, 1, 2);
            ParentGamePanel.Controls.Add(RankLabels, 0, 1);
            ParentGamePanel.Controls.Add(GamePanel, 1, 1);
            ParentGamePanel.Controls.Add(BlackTimerLabel, 2, 0);
            ParentGamePanel.Controls.Add(WhiteTimerLabel, 2, 2);
            ParentGamePanel.Controls.Add(DoneMovesTable, 2, 1);
            ParentGamePanel.Controls.Add(WhitePlayerMaterial, 0, 3);
            ParentGamePanel.Controls.Add(BlackPlayerMaterial, 0, 0);
            ParentGamePanel.Controls.Add(_blackTakenPiecesPanel, 1, 3);
            ParentGamePanel.Controls.Add(_whiteTakenPiecesPanel, 0, 0);
            ParentGamePanel.Name = "ParentGamePanel";

            ParentGamePanel.SetRowSpan(WhiteTimerLabel, 2);
            // 
            // FileLabels
            // 
            resources.ApplyResources(FileLabels, "FileLabels");
            FileLabels.Name = "FileLabels";
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
            DoneMovesTable.BackColor = Color.FromArgb(38, 35, 58);
            DoneMovesTable.Name = "DoneMovesTable";
            // 
            // WhitePlayerMaterial
            // 
            resources.ApplyResources(WhitePlayerMaterial, "WhitePlayerMaterial");
            WhitePlayerMaterial.Name = "WhitePlayerMaterial";
            // 
            // BlackPlayerMaterial
            // 
            resources.ApplyResources(BlackPlayerMaterial, "BlackPlayerMaterial");
            BlackPlayerMaterial.Name = "BlackPlayerMaterial";
            // 
            // _blackTakenPiecesPanel
            // 
            resources.ApplyResources(_blackTakenPiecesPanel, "_blackTakenPiecesPanel");
            _blackTakenPiecesPanel.Name = "_blackTakenPiecesPanel";
            // 
            // _whiteTakenPiecesPanel
            // 
            resources.ApplyResources(_whiteTakenPiecesPanel, "_whiteTakenPiecesPanel");
            _whiteTakenPiecesPanel.Name = "_whiteTakenPiecesPanel";
            // 
            // GameWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 23, 36);
            Controls.Add(ParentGamePanel);
            ForeColor = Color.FromArgb(224, 222, 244);
            Name = "GameWindow";
            Load += GameWindowLoad;
            ParentGamePanel.ResumeLayout(false);
            ParentGamePanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel ParentGamePanel;
        private TableLayoutPanel GamePanel;
        private TableLayoutPanel RankLabels;
        private TableLayoutPanel FileLabels;
        private Label BlackPlayerMaterial;
        private Label WhitePlayerMaterial;
        private Label BlackTimerLabel;
        private Label WhiteTimerLabel;
        private FlowLayoutPanel DoneMovesTable;
        private FlowLayoutPanel _whiteTakenPiecesPanel;
        private FlowLayoutPanel _blackTakenPiecesPanel;
    }
}

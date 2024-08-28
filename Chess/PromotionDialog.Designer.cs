namespace Chess
{
    partial class PromotionDialog
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
            ButtonsLayout = new TableLayoutPanel();
            SuspendLayout();
            // 
            // ButtonsLayout
            // 
            ButtonsLayout.ColumnCount = 4;
            ButtonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ButtonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ButtonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ButtonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ButtonsLayout.Location = new Point(16, 12);
            ButtonsLayout.Margin = new Padding(4, 3, 4, 3);
            ButtonsLayout.Name = "ButtonsLayout";
            ButtonsLayout.RowCount = 1;
            ButtonsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            ButtonsLayout.Size = new Size(694, 108);
            ButtonsLayout.TabIndex = 0;
            // 
            // PromotionDialog
            // 
            AutoScaleDimensions = new SizeF(11F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(727, 132);
            ControlBox = false;
            Controls.Add(ButtonsLayout);
            Font = new Font("Verdana", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PromotionDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Form1";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel ButtonsLayout;
    }
}
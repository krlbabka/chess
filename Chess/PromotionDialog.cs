using Chess.Pieces;
using System.Windows.Forms;

namespace Chess
{
    public partial class PromotionDialog : Form
    {
        internal PieceType SelectedPieceType { get; private set; }
        bool isWhite;
        private readonly Color DarkColor = Color.FromArgb(33, 32, 46);
        private readonly Color LightColor = Color.FromArgb(82, 79, 103);

        public PromotionDialog(bool isWhite, Form owner)
        {
            InitializeComponent();
            InitializePromoteDialog(owner);
            CreatePromotionButton("Queen", PieceType.Queen, 0);
            CreatePromotionButton("Rook", PieceType.Rook, 1);
            CreatePromotionButton("Bishop", PieceType.Bishop, 2);
            CreatePromotionButton("Knight", PieceType.Knight, 3);

            this.isWhite = isWhite;
        }

        private void InitializePromoteDialog(Form owner)
        {
            ButtonsLayout.Dock = DockStyle.Fill; 
            FormBorderStyle = FormBorderStyle.None;
            Owner = owner;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(320, 80);
            BackColor = DarkColor;
        }

        private void CreatePromotionButton(string pieceName, PieceType pieceType, int column)
        {
            Button button = new Button
            {
                Tag = pieceType,
                Dock = DockStyle.Fill,
                Image = getPieceImage(pieceType),
                BackColor = LightColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0},
            };
            button.Click += PromotionButtonClick;
            ButtonsLayout.Controls.Add(button, column, 0);
        }

        private Image getPieceImage(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.Queen:
                    return Image.FromFile("Resources/" + (!isWhite ? "w_queen.png" : "b_queen.png"));
                case PieceType.Rook:
                    return Image.FromFile("Resources/" + (!isWhite ? "w_rook.png" : "b_rook.png"));
                case PieceType.Bishop:
                    return Image.FromFile("Resources/" + (!isWhite ? "w_bishop.png" : "b_bishop.png"));
                case PieceType.Knight:
                    return Image.FromFile("Resources/" + (!isWhite ? "w_knight.png" : "b_knight.png"));
                default:
                    return null;
            }
        }

        private void PromotionButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            SelectedPieceType = (PieceType)button.Tag;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
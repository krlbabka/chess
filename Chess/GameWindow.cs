using System.Diagnostics;

namespace Chess
{
    public partial class GameWindow : Form
    {
        public GameWindow()
        {
            InitializeComponent();
        }
        PictureBox[,] pictureBox;
        Button[,] boardButtons;

        private void GameWindow_Load(object sender, EventArgs e)
        {
            this.Size = new Size(920, 720);
            ParentGamePanel.Controls.Add(GamePanel);
            ParentGamePanel.Dock = DockStyle.None;
            ParentGamePanel.Top = 35;
            ParentGamePanel.Left = 35;
            ParentGamePanel.Size = new Size(600, 600);
            GamePanel.Dock = DockStyle.Fill;

            Board board = new Board();
            board.defaultPosition();
            board.printBoard();

            LoadChessboardBackground(board);

            this.FormClosing += (sender, e) => GameWindow_Closing();
        }

        private void GameWindow_Closing() 
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    if (boardButtons[row, col] != null)
                    {
                        boardButtons[row, col].Click -= (sender, e) =>
                        {
                            Debug.WriteLine($"{row} {col}");
                        };
                    }
                }
            }
            Application.Exit();
        }

        private void LoadChessboardBackground(Board board)
        {
            boardButtons = new Button[Board.BOARD_SIZE, Board.BOARD_SIZE];
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    int rowVar = row; 
                    int colVar = col;
                    boardButtons[rowVar, colVar] = new Button
                    {
                        BackColor = (rowVar + colVar) % 2 == 0 ? Color.White : Color.Black,
                        Dock = DockStyle.Fill,
                        FlatStyle = FlatStyle.Flat,
                        FlatAppearance = { BorderSize = 0 },
                        Margin = new Padding(0)
                    };

                    if (board.IsTileOccupied(new Vector(rowVar, colVar)))
                    {
                        boardButtons[rowVar, colVar].Enabled = true;
                        boardButtons[rowVar, colVar].Click += (sender, e) =>
                        {
                            Debug.WriteLine($"{rowVar} {colVar}");
                        };
                        Piece? piece = board.GetPieceAtPosition(new Vector(rowVar, colVar));
                        SetPieceImage(boardButtons[rowVar, colVar], piece.GetPieceImage(piece.IsWhite));
                        
                    }
                    else
                    {
                        boardButtons[rowVar, colVar].Enabled = false;
                    }
                    GamePanel.Controls.Add(boardButtons[rowVar, colVar], colVar, rowVar);
                }
            }
        }

        private void SetPieceImage(Button button, Image pieceImage)
        {
            button.Image = pieceImage;
            button.ImageAlign = ContentAlignment.MiddleCenter;
        }



    }
}

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
        Button[,] chessPieces;

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
                    if (chessPieces[row, col] != null)
                    {
                        chessPieces[row, col].Click -= (sender, e) =>
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
            pictureBox = new PictureBox[Board.BOARD_SIZE, Board.BOARD_SIZE];
            chessPieces = new Button[Board.BOARD_SIZE, Board.BOARD_SIZE];
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    int rowVar = row; 
                    int colVar = col;
                    pictureBox[rowVar, colVar] = new PictureBox
                    {
                        BackColor = (rowVar + colVar) % 2 == 0 ? Color.White : Color.Black,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(0)
                    };
                    GamePanel.Controls.Add(pictureBox[rowVar, colVar], colVar, rowVar);

                    if (board.TileOccupied(new Vector(rowVar, colVar)))
                    {
                        chessPieces[rowVar, colVar] = new Button
                        {
                            BackColor = Color.Red,
                            Dock = DockStyle.Bottom
                        };
                        chessPieces[rowVar, colVar].Click += (sender, e) =>
                        {
                            Debug.WriteLine($"{rowVar} {colVar}");
                        };
                        pictureBox[rowVar, colVar].Controls.Add(chessPieces[rowVar, colVar]);
                    }
                }
            }
        }
    }
}

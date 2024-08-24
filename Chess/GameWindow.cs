using System.Diagnostics;
using System.Drawing;

namespace Chess
{
    public partial class GameWindow : Form
    {

        Button[,]? boardButtons;
        Board board;
        ChessLogic chessLogic;
        Vector ClickedPosition;
        Vector LastMove;

        public GameWindow()
        {
            InitializeComponent();
            board = new Board();
            board.defaultPosition();
            SetupChessboard(board);
            chessLogic = new ChessLogic(board);
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            this.Size = new Size(920, 720);
            ParentGamePanel.Controls.Add(GamePanel);
            ParentGamePanel.Dock = DockStyle.None;
            ParentGamePanel.Top = 35;
            ParentGamePanel.Left = 35;
            ParentGamePanel.Size = new Size(600, 600);
            GamePanel.Dock = DockStyle.Fill;
            UpdateChessboard();

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

        private void SetupChessboard(Board board)
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
                        FlatAppearance = {  BorderSize = 0,
                                            MouseDownBackColor = (rowVar + colVar) % 2 == 0 ? Color.White : Color.Black,
                                            MouseOverBackColor = (rowVar + colVar) % 2 == 0 ? Color.White : Color.Black},
                        Margin = new Padding(0),
                    };
                }
            }
            updateGUI();
        }

        private void UpdateChessboard()
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    Vector position = new Vector(row, col);
                    Button button = boardButtons[row, col];

                    button.Click -= Button_Click;
                    if (board.IsTileOccupied(position))
                    {
                        if (board.BoardGrid[position.X, position.Y].OccupyingPiece!.IsWhite == chessLogic.IsWhiteTurn())
                        {
                            button.Click += Button_Click;
                        }
                        else
                        {
                            LegalMoveResetAction(position.X, position.Y);
                        }
                    }
                    else
                    {
                        LegalMoveResetAction(position.X, position.Y);
                    }
                    GamePanel.Controls.Add(button, position.Y, position.X);
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int rowVar = GamePanel.GetRow(button);
            int colVar = GamePanel.GetColumn(button);
            ClickedPosition = new Vector(rowVar, colVar);
            board.FindLegalTiles(board.BoardGrid[rowVar, colVar], board.BoardGrid[rowVar, colVar].OccupyingPiece);
            updateGUI();
            updateActions();
        }

        private void Move_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int rowVar = GamePanel.GetRow(button);
            int colVar = GamePanel.GetColumn(button);

            MovePiece(ClickedPosition, board.BoardGrid[rowVar, colVar].Position);
        }

        private void LegalMoveResetAction(int rowVar, int colVar)
        {
            boardButtons[rowVar, colVar].Click += (sender, e) =>
            {
                board.ResetLegalTiles();
                updateGUI();
                updateActions();
            };
        }

        private void updateGUI()
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    Vector position = new Vector(row, col);
                    Button button = boardButtons[row, col];
                    
                    if (board.BoardGrid[position.X, position.Y].IsOccupied)
                    {
                        Piece piece = board.GetPieceAtPosition(position)!;
                        SetPieceImage(button, piece.GetPieceImage(piece.IsWhite));
                    }
                    else
                    {
                        SetPieceImage(button, null);
                    }
                    if (board.BoardGrid[position.X, position.Y].LegalMove)
                    {
                        button.BackColor = Color.Green;
                        button.FlatAppearance.MouseDownBackColor = Color.Green;
                        button.FlatAppearance.MouseOverBackColor = Color.Green;
                    }
                    else
                    {
                        button.BackColor = (position.X + position.Y) % 2 == 0 ? Color.White : Color.Black;
                        button.FlatAppearance.MouseDownBackColor = (position.X + position.Y) % 2 == 0 ? Color.White : Color.Black;
                        button.FlatAppearance.MouseOverBackColor = (position.X + position.Y) % 2 == 0 ? Color.White : Color.Black;
                    }
                    if (LastMove != null && position.IsEqual(LastMove))
                    {
                        button.BackColor = Color.Yellow;
                    }
                }
            }
        }

        private void updateActions() 
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    int rowVar = row;
                    int colVar = col;
                    if (board.BoardGrid[rowVar, colVar].LegalMove)
                    {
                        boardButtons[rowVar, colVar].Click -= Button_Click;
                        boardButtons[rowVar, colVar].Click += Move_Click;
                    }
                    else
                    {
                        boardButtons[rowVar, colVar].Click -= Move_Click;
                    }
                }
            }
        }


        private void SwitchTurn()
        {
            chessLogic.SwitchTurn();
        }

        private void MovePiece(Vector Current, Vector New)
        {
            board.MovePiece(Current, New);
            LastMove = New;
            SwitchTurn();
            UpdateChessboard();
            updateGUI();
            updateActions();
        }

        private void SetPieceImage(Button button, Image pieceImage)
        {
            button.Image = pieceImage;
            button.ImageAlign = ContentAlignment.MiddleCenter;
        }
    }
}

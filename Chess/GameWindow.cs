using Chess.HelperClasses;
using Chess.Pieces;

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
            board = new Board();
            chessLogic = new ChessLogic(board);
            InitializeComponent();
            board.defaultPosition();
            SetupChessboard();
            SetupCoordinateLabels();
        }

        private void GameWindowLoad(object sender, EventArgs e)
        {
            Size = new Size(920, 720);
            ParentGamePanel.Controls.Add(GamePanel);
            ParentGamePanel.Dock = DockStyle.None;
            ParentGamePanel.Top = 35;
            ParentGamePanel.Left = 35;
            ParentGamePanel.Size = new Size(600, 600);
            GamePanel.Dock = DockStyle.Fill;
            UpdateChessboard();

            FormClosing += (sender, e) => GameWindowClosing();
        }

        private void GameWindowClosing()
        {
            Application.Exit();
        }
        private void SetupCoordinateLabels()
        {
        }

        private void SetupChessboard()
        {
            boardButtons = new Button[Board.BOARD_SIZE, Board.BOARD_SIZE];
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    Vector currentPosition = new(row, col);
                    Color tileColor = (currentPosition.X + currentPosition.Y) % 2 == 0 ? Color.White : Color.Black;
                    boardButtons[currentPosition.X, currentPosition.Y] = new Button
                    {
                        BackColor = tileColor,
                        Dock = DockStyle.Fill,
                        FlatStyle = FlatStyle.Flat,
                        FlatAppearance = { BorderSize = 0, MouseDownBackColor = tileColor, MouseOverBackColor = tileColor },
                        Margin = new Padding(0),
                    };
                }
            }
            UpdateChessboardGUI();
        }

        private void UpdateChessboard()
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    Vector position = new Vector(row, col);
                    Button button = boardButtons[row, col];

                    button.Click -= ButtonClick;
                    if (board.IsTileOccupied(position))
                    {
                        if (board.GetPieceAt(position)!.IsWhite == chessLogic.IsWhiteTurn())
                        {
                            button.Click += ButtonClick;
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

        private void ButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int rowVar = GamePanel.GetRow(button);
            int colVar = GamePanel.GetColumn(button);
            ClickedPosition = new Vector(rowVar, colVar);
            chessLogic.FindLegalTiles(board, board.BoardGrid[rowVar, colVar], board.GetPieceAt(ClickedPosition));
            UpdateChessboardGUI();
            UpdateButtonActions();
        }

        private void MoveAction(object sender, EventArgs e)
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
                board.ResetLegalMoves();
                UpdateChessboardGUI();
                UpdateButtonActions();
            };
        }

        private void UpdateChessboardGUI()
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    Vector position = new(row, col);
                    Button button = boardButtons[row, col];

                    if (board.BoardGrid[position.X, position.Y].IsOccupied)
                    {
                        Piece piece = board.GetPieceAt(position)!;
                        SetPieceImage(button, piece.GetPieceImage(piece.IsWhite));
                    }
                    else
                    {
                        SetPieceImage(button, null);
                    }

                    // Button highlights
                    if (board.BoardGrid[position.X, position.Y].LegalMove)
                    {
                        SetButtonColor(button, Color.Green);
                    }
                    else
                    {
                        Color btnColor = (position.X + position.Y) % 2 == 0 ? Color.White : Color.Black;
                        SetButtonColor(button, btnColor);
                    }
                    if (LastMove != null && position.IsEqual(LastMove))
                    {
                        SetButtonColor(button, Color.BlueViolet);
                    }
                }
            }
            /*
            bool threatCheck = chessLogic.IsCheck(board, chessLogic.IsWhiteTurn()) ||
                chessLogic.IsMate(board, !chessLogic.IsWhiteTurn()) ||
                chessLogic.IsStalemate(board, chessLogic.IsWhiteTurn());
            if (threatCheck)
            {
                Vector kingPosition = chessLogic.FindKingPosition(board, chessLogic.IsWhiteTurn());
                Button button = boardButtons[kingPosition.X, kingPosition.Y];
                SetButtonColor(button, Color.Red);
            }
            */
        }

        private void UpdateButtonActions()
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    int rowVar = row;
                    int colVar = col;
                    if (board.BoardGrid[rowVar, colVar].LegalMove)
                    {
                        boardButtons[rowVar, colVar].Click -= ButtonClick;
                        boardButtons[rowVar, colVar].Click += MoveAction;
                    }
                    else
                    {
                        boardButtons[rowVar, colVar].Click -= MoveAction;
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
            if (chessLogic.CanMove(Current, New))
            {
                chessLogic.MovePiece(board, Current, New);
                LastMove = New;
                Update();
                SwitchTurn();
            }
            Update();
            //CheckGameOver();
        }

        private void CheckGameOver()
        {
            if (chessLogic.IsMate(board, chessLogic.IsWhiteTurn()))
            {
                GameOver("Mate");
            }
            if (chessLogic.IsStalemate(board, chessLogic.IsWhiteTurn()))
            {
                GameOver("Stalemate");
            }
            if (chessLogic.IsDraw())
            {
                GameOver($"Draw");
            }
        }

        private void Update()
        {
            UpdateChessboard();
            UpdateChessboardGUI();
            UpdateButtonActions();
        }

        private static void SetPieceImage(Button b, Image pieceImage)
        {
            b.Image = pieceImage;
            b.ImageAlign = ContentAlignment.MiddleCenter;
        }

        private static void SetButtonColor(Button b, Color color)
        {
            b.BackColor = color;
            b.FlatAppearance.MouseDownBackColor = color;
            b.FlatAppearance.MouseOverBackColor = color;
        }

        private void GameOver(string message)
        {
            MessageBox.Show(message);
            Application.Exit();
        }
    }
}

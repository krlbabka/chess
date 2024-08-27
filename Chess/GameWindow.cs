using System.Diagnostics;

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
                        FlatAppearance = {  BorderSize = 0, MouseDownBackColor = tileColor, MouseOverBackColor = tileColor},
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
                        if (board.GetPieceAtPosition(position)!.IsWhite == chessLogic.IsWhiteTurn())
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
            board.FindLegalTiles(board.BoardGrid[rowVar, colVar], board.GetPieceAtPosition(ClickedPosition));
            UpdateChessboardGUI();
            UpdateButtonActions();
        }

        private void MoveClick(object sender, EventArgs e)
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
                        Piece piece = board.GetPieceAtPosition(position)!;
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
            if (chessLogic.IsCheck(board, chessLogic.IsWhiteTurn()))
            {
                Vector kingPosition = board.FindKingPosition(board, chessLogic.IsWhiteTurn());
                Button button = boardButtons[kingPosition.X, kingPosition.Y];
                SetButtonColor(button, Color.Red);
            }
            if(chessLogic.IsMate(board, chessLogic.IsWhiteTurn()))
            {
                Vector kingPosition = board.FindKingPosition(board, !chessLogic.IsWhiteTurn());
                Button button = boardButtons[kingPosition.X, kingPosition.Y];
                SetButtonColor(button, Color.Red);
            }
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
                        boardButtons[rowVar, colVar].Click += MoveClick;
                    }
                    else
                    {
                        boardButtons[rowVar, colVar].Click -= MoveClick;
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
                if (chessLogic.IsMate(board, chessLogic.IsWhiteTurn()))
                {
                    GameOver("Mate");
                    return;
                }
                SwitchTurn();
            }
            Update();
            if (chessLogic.IsStalemate())
            {
                GameOver("Stalemate");
                return;
            }
            if (chessLogic.IsDraw())
            {
                Update();
                GameOver($"Draw");
                return;
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
        }
    }
}

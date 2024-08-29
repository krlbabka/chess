using Chess.HelperClasses;
using Chess.Logic;
using Chess.Pieces;

namespace Chess
{
    public partial class GameWindow : Form
    {

        private Button[,]? _boardButtons;
        private Board _board;
        private ChessLogic _chessLogic;
        private Vector _ClickedPosition;
        private Vector _LastMove;
        private ChessTimer _whiteTimer;
        private ChessTimer _blackTimer;



        private readonly Color DarkColor = Color.FromArgb(38, 35, 58);
        private readonly Color LightColor = Color.FromArgb(82, 79, 103);
        private readonly Color LastMoveColor = Color.FromArgb(196, 167, 231);
        private readonly Color LegalMoveColor = Color.FromArgb(156, 207, 216);
        private readonly Color KingInDangerColor = Color.FromArgb(235, 111, 146);

        public GameWindow()
        {
            _board = new Board();
            _chessLogic = new ChessLogic(_board);
            _chessLogic.OnPromotion += HandlePromotionDialog;
            InitializeComponent();
            _board.DefaultPosition();
            SetupChessboard();
            SetupCoordinateLabels();
            _whiteTimer = new ChessTimer(5, 0, WhiteTimerLabel);
            _blackTimer = new ChessTimer(5, 0, BlackTimerLabel);
            _whiteTimer.OnGameOver += () => GameOver("Black wins on time");
            _blackTimer.OnGameOver += () => GameOver("White wins on time");
        }

        private void GameWindowLoad(object sender, EventArgs e)
        {
            DoneMovesTable.FlowDirection = FlowDirection.TopDown;
            DoneMovesTable.WrapContents = false;
            DoneMovesTable.Dock = DockStyle.Fill;
            DoneMovesTable.AutoScroll = true;
            DoneMovesTable.BackColor = DarkColor;

            Size = new Size(920, 720);
            ParentGamePanel.Controls.Add(GamePanel);
            ParentGamePanel.Dock = DockStyle.None;
            ParentGamePanel.Top = 35;
            ParentGamePanel.Left = 35;
            ParentGamePanel.Size = new Size(700, 620);

            GamePanel.Dock = DockStyle.Fill;
            GamePanel.Size = new Size(600, 600);
            GamePanel.Margin = new Padding(0, 0, 0, 0);
            UpdateChessboard();

            FormClosing += (sender, e) => GameWindowClosing();
        }

        private void GameWindowClosing()
        {
            Application.Exit();
        }

        private void SetupCoordinateLabels()
        {
            Label[] rowLabels = new Label[Board.BOARD_SIZE];
            Label[] colLabels = new Label[Board.BOARD_SIZE];

            RankLabels.Size = new Size(20, GamePanel.Height);
            RankLabels.Dock = DockStyle.Fill;
            RankLabels.Margin = new Padding(0, 0, 0, 0);

            FileLabels.Size = new Size(GamePanel.Width, 20);
            FileLabels.Dock = DockStyle.Top;
            FileLabels.Margin = new Padding(0, 0, 0, 0);


            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                rowLabels[i] = new Label
                {
                    Text = (8 - i).ToString(),
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Verdana", 10, FontStyle.Bold),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                    Margin = new Padding(0, 0, 0, 0)
                };
                RankLabels.Controls.Add(rowLabels[i], 0, i);
            }

            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                colLabels[i] = new Label
                {
                    Text = ((char)('a' + i)).ToString(),
                    TextAlign = ContentAlignment.TopCenter,
                    Font = new Font("Verdana", 10, FontStyle.Bold),
                    Margin = new Padding(0, 0, 0, 0),
                    Top = 0
                };
                FileLabels.Controls.Add(colLabels[i], i, 0);
            }
        }

        private void SetupChessboard()
        {
            _boardButtons = new Button[Board.BOARD_SIZE, Board.BOARD_SIZE];
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    Vector currentPosition = new(row, col);
                    Color tileColor = (currentPosition.X + currentPosition.Y) % 2 == 0 ? LightColor : DarkColor;
                    _boardButtons[currentPosition.X, currentPosition.Y] = new Button
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

        private void TurnButtonsOff() 
        {
            foreach (Button button in _boardButtons) 
            {
                button.Enabled = false;
            }
        }

        private void UpdateChessboard()
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    Vector position = new Vector(row, col);
                    Button button = _boardButtons[row, col];

                    button.Click -= ButtonClick;
                    button.Click -= LegalMoveResetAction;
                    if (_board.IsTileOccupied(position))
                    {
                        if (_board.GetPieceAt(position)!.IsWhite == _chessLogic.IsWhiteTurn)
                        {
                            button.Click += ButtonClick;
                        }
                        else
                        {
                            button.Click += LegalMoveResetAction;
                        }
                    }
                    else
                    {
                        button.Click += LegalMoveResetAction;
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
            _ClickedPosition = new Vector(rowVar, colVar);
            _chessLogic.FindLegalTiles(_board, _board.BoardGrid[rowVar, colVar], _board.GetPieceAt(_ClickedPosition));
            UpdateChessboardGUI();
            UpdateButtonActions();
        }

        private void MoveAction(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int rowVar = GamePanel.GetRow(button);
            int colVar = GamePanel.GetColumn(button);

            MovePiece(_ClickedPosition, _board.BoardGrid[rowVar, colVar].Position);
        }

        private void LegalMoveResetAction(object? sender, EventArgs e)
        {
            _board.ResetLegalMoves();
            UpdateChessboardGUI();
            UpdateButtonActions();
        }

        private void UpdateChessboardGUI()
        {
            for (int row = 0; row < Board.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Board.BOARD_SIZE; col++)
                {
                    Vector position = new(row, col);
                    Button button = _boardButtons[row, col];
                    if (_board.BoardGrid[position.X, position.Y].IsOccupied)
                    {
                        Piece piece = _board.GetPieceAt(position)!;
                        SetPieceImage(button, piece.PieceImage);
                    }
                    else
                    {
                        SetPieceImage(button, null);
                    }
                    if (_LastMove != null && position.IsEqual(_LastMove))
                    {
                        SetButtonColor(button, LastMoveColor);
                        continue;
                    }
                    if (_board.BoardGrid[position.X, position.Y].LegalMove)
                    {
                        SetButtonColor(button, LegalMoveColor);
                        continue;
                    }
                    Color btnColor = (position.X + position.Y) % 2 == 0 ? LightColor : DarkColor;
                    SetButtonColor(button, btnColor);
                }
            }
            
            bool check = _chessLogic.IsCheck(_board);
            bool stalemate = _chessLogic.IsStalemate(_board);
            if (check || stalemate)
            {
                Vector kingPosition = _chessLogic.FindKingPosition(_board);
                Button button = _boardButtons[kingPosition.X, kingPosition.Y];
                SetButtonColor(button, KingInDangerColor);
            }
            UpdateMaterialDifferenceGUI();
        }

        private void UpdateMaterialDifferenceGUI()
        {
            int materialDiff = _chessLogic.GetMaterialDifference();
            if (materialDiff > 0)
            {
                BlackPlayerMaterial.Text = materialDiff.ToString();
                WhitePlayerMaterial.Text = " ";

            }
            else if (materialDiff < 0)
            {
                BlackPlayerMaterial.Text = " ";
                WhitePlayerMaterial.Text = (-materialDiff).ToString();
            }
            else
            {
                BlackPlayerMaterial.Text = " ";
                WhitePlayerMaterial.Text = " ";
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
                    if (_board.BoardGrid[rowVar, colVar].LegalMove)
                    {
                        _boardButtons[rowVar, colVar].Click -= ButtonClick;
                        _boardButtons[rowVar, colVar].Click += MoveAction;
                    }
                    else
                    {
                        _boardButtons[rowVar, colVar].Click -= MoveAction;
                    }
                }
            }
        }

        private void SwitchTurn()
        {
            _chessLogic.SwitchTurn();
            HandleTimers();
        }

        private void MovePiece(Vector Current, Vector New)
        {
            if (_chessLogic.CanMove(Current, New))
            {
                _chessLogic.MovePiece(_board, Current, New);
                _LastMove = New;

                string NotationLabel = _chessLogic.GetLastMoveNotation().ToString();
                DoneMovesTable.Controls.Add(new Label { Text = NotationLabel, ForeColor = Color.White });
                
                SwitchTurn();
            }
            Update();
            CheckGameOver();
        }

        private void HandleTimers() 
        {
            if (_chessLogic.IsWhiteTurn)
            {
                _blackTimer.Stop();
                _whiteTimer.Start();
            }
            else
            {
                _whiteTimer.Stop();
                _blackTimer.Start();
            }
        }
        private void StopTimers()
        {
            _whiteTimer.Stop();
            _blackTimer.Stop();
        }

        private void CheckGameOver()
        {
            if (_chessLogic.IsMate(_board, _chessLogic.IsWhiteTurn))
            {
                DoneMovesTable.Controls[DoneMovesTable.Controls.Count -1].Text += "#";
                TurnButtonsOff();
                StopTimers();
                GameOver("Mate");
            }
            if (_chessLogic.IsStalemate(_board))
            {
                TurnButtonsOff();
                StopTimers();
                GameOver("Stalemate");
            }
            if (_chessLogic.IsDraw())
            {
                TurnButtonsOff();
                StopTimers();
                GameOver($"Draw");
            }
        }

        private void HandlePromotionDialog(bool isWhite, Action<PieceType> onActionSuccess)
        {
            using (PromotionDialog dialog = new PromotionDialog(isWhite, this))
            {
                dialog.StartPosition = FormStartPosition.Manual;

                int vertical = isWhite ? Location.Y + (Height - dialog.Height) / 8 + 30 : Location.Y;
                dialog.Location = new Point(Location.X + (Width - dialog.Width) / 2, vertical);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    onActionSuccess?.Invoke(dialog.SelectedPieceType);
                }
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

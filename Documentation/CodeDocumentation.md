# Code documentation

## Project Hierarchy

- [root folder](#classes-in-the-root-folder)
  - Classes for tile and board representation and GUI handling classes.
- [HelperClasses](#helper-classes)
  - Classes that used throughout the project for ease of use.
- [Logic](#logic)
  - Classes that handle core chess logic
- [Pieces](#pieces)
  - Piece classes and piece strategy classes based on the strategy design pattern.
- Resources
  - Contains the images used.

### Classes in the root folder

#### Tile.cs

Represents one tile on the chessboard.

Properties:

- Vector `Position` - X,Y coordinates on the board
- bool `IsOccupied` - whether a Piece is on this tile
- bool `LegalMove` - whether a seleceted Piece can move to this tile
- MoveType `MoveType` - if it's a special move
- Piece `OccupyingPiece` - the piece that is currently on the Tile (nullable)

Methods:

- `CreatePiece` - creates a new piece instance from the PieceFactory
- `RemoveCurrentPiece` - removes the piece from the tile
- `SetNewPiece` - adds the piece to the tile

#### Board.cs

Represents the Board as an 8x8 Tile grid.

Properties:

- Tile[,] `BoardGrid` - the board representation

Methods:

- `GetPieceAt` - return the Piece on the respective Tile (overrired to take either a `Vector` or a `Tile` as a parameter)
- `IsTileOccupied` - return a bool whether a piece is on the respective Tile (overrired to take either a `Vector` or a `Tile` as a parameter)
- `GetTile` - returns a Tile on the input vectors position
- `GetBoardCopy` - return a new Tile[,] copy of the current chessboard state
- `SetDefaultBoardPosition`
- `ResetLegalMoves` - sets all Tiles' isLegalMove property to false
- `WithinBounds` - whether the position is on the board
- `AreEnemies` - takes two pieces and returns true if they are a different color

#### GameWindow.cs

Handles the GUI updates and the communication between chess logic and GUI.

Properties:

- `_boardButtons` - the GUI chessboard 2d grid of buttons
- `Vector` _ClickedPosition - last clicked position on the chessboard
- `Vector` _LastMove - the after move position of the last move that was made
- `_board` - `Board` instance
- `_chessLogic` - `ChessLogic`  instance
- `_whiteTimer`, `_blackTimer` - `ChessTimer` instances
- color properties

Methods:

- `GameWindowLoad` - traditional Form Loader, setups GUI, uses other `Setup —` methods for respective GUI elements
- `UpdateChessboardGUI` - iterated over the button grid and updates GUI elements to match the game state
- `UpdateChessboard` - iterates over the button grid and updates events
- `ButtonClick`, `MoveAction`, `LegalMoveResetAction` - Event for click, movement and event reset respectively
- `SwitchTurn`, `MovePiece` - logic calling methods
- `HandleTimers`, `CheckGameOver`, `UpdateTakenPiecesPanels` - post move update methods
- `HandlePromotionDialog`, `HandleGameOverDialog` - Dialog calling methods
- `SetPieceImage`, `SetButtonColor` - ease of use helper methods

#### PromotionDialog.cs

Form for a pop up dialog on pawn promotion;

#### GameOverDialog.cs

Form for a pop up dialog on game end;

### Helper Classes

Very simple classes just to make the rest of the code more readable and for ease of use.

#### ChessTimer.cs

Simple timer class for the player countdown during game.
Methods:

- `Start`,
- `Stop`

#### Move.cs

Used for storing moves.\
Contains MoveType enum for special moves.\
Properties:

- Vectors `from` and `to`,
- Piece `movedPiece`,
- booleans for special move checks.

#### Vector.cs

Simple class for positional ease of use on a grid.\
Properties:

- integers `X` and `Y` as the position

Methods:

- overriden addition/equal check methods.

### Logic

#### ChessLogic.cs

Handles delegeting logic between GameStateHandler and MovementHandler and their communication via events. These are more explained the the respective classes. Separately handles moves being converted to the algebraic notation.

- `_board` - `Board` instance
- `_gameStateHandler` - `GameStateHandler` instance
- `_movementHandler` - `MovementHandler` instance
- `_whiteTurn` - bool check for the current turn
- `_lastMove` - last move stored as a `Move`

Methods using MovementHandler methods:

- `SortTakenPieces`
- `FindLegalTiles`
- `CanMove`
- `MovePiece`
- `GetMaterialDifference`
- `HandlePromotion`

Methods using GameStateHandler methods:

- `FindKingPosition`
- `IsCheck`
- `IsMate`
- `IsStalemate`
- `IsDraw`

Other methods:

- `PotentialCheckAfterMove` - simulates a move and looks if there is a check after the simulated move
- `GetLastMoveNotation` - converts last move into notation

#### GameStateHandler.cs

Properties:

- `_boardStateCounts` - a dictionary of board states saved as strings and the count of their repetitions
- `_fiftyMoveCounter` - int for the fifty move rule checking

Methods:

- `GetPossibleMoves` - invokes an event to call the MovementHandler's GetPossibleMoves method
- `PotentialCheck` - invokes an event to call ChessLogic's PotentialCheckAfterMove method
- `IsCheck` - checks if the king is in direct sight of any enemy piece
  - `FindKingPosition` - looks for the king piece
  - `IsKingUnderAttack` - iterates possible legal moves of enemy pieces and checks if the king is in danger
- `IsMate`, `IsStalemate` - check if any piece can move
  - `IsMateOrStalemate`, `NoMoveLeft`, `CanMoveWithoutCheck` helper functions for shared logic of mate and stalemate - iterates over pieces and tries to find a move
  - `IsDraw` - returns if any draw condition has been met
    - `Repetition` - checks if 3 game states have repeated (3fold repetition)
    - `FiftyMoveRule` - if there was a piece taken or a pawn pushed in the last 50 moves
    - `IsInsufficientMaterial` - There's not enough pieces on the board to get a mate/stalemate
  - `GenerateBoardString`, `SaveBoardState` - Generates/Saves the current state of the board in a string representation for faster difference checking

#### MovementHandler.cs

Properties:

- `_moves` - list of done moves
- `_lastMove` - last move stored as a `Move`
- `_boardStateCounts` - a dictionary of board states saved as strings and the count of their repetitions
- `_fiftyMoveCounter` - int for the fifty move rule checking
- `_materialDifference` - difference in material based on taken pieces/promotions..
- `_whiteTakenPieces`, `_blackTakenPieces` - lists of pieces taken

Methods:

- `SortTakenPieces` - sort the pieces that were taken by value
- `CanMove` - checks if the move would be valid
- `MovePiece` - Moves the piece and handles all logic with Promotion/Castling pieces moving/changing
  - `UpdateMaterialDifference`, `UpdateDrawVariables`, `UpdateMoves`, `HandleTakenPieces`, `UpdateTilesOnMove` - updates the relevant values/lists after a move is done
  - `HandleSpecialMoveTypes`
    - `HandleEnPassant`, `HandleCastling`, `HandleCastlingRookMovement`, `HandlePromotion` - helper methods for piece movement
- `GetPossibleMoves` - for a given piece and position looks at all the positions the piece can move and returns these positions as a list
- `FindLegalTiles` - checks if the move would be valid, and if so sets the Tile at such a position as a legal move
  - `TileNotValid`, `PotentialCastleCheck` - helper methods

### Pieces

Implemented under strategy design pattern, in this case that means every PieceStragety handles the piece's move validation.

#### Piece.cs

Parent abstract class for all Pieces. Has an IMovable in the constructor, meaning the respective piece's strategy class.\
Contains `PieceType` enum for piece types.

Properties:

- `PieceType` type - type of piece based on the enum
- bool `isWhite` - true if the white is piece
- char `Notation` - The short piece acronym used in [algebraic notation](https://en.wikipedia.org/wiki/Algebraic_notation_(chess)).
- int `MaterialValue` - the value of the piece, using the standard values (desc. 9, 5, 3, 3, 1)
- bool `IsPromotedPawn` - check for correctly displaying taken pieces (when you have a promoted pawn you should "take" a pawn not the promoted piece)

#### IMovable.cs

Interface for all pieces' Strategy classes.\

Methods:

- `CanMove` - Each strategy class implements it's own version for move validation in this method. Gets two Vectors as parameters and return a boolean.

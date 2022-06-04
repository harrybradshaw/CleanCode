using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class TileGame
    {
        private List<GameElement> NewElements { get; set; }
        private Scoreboard Scoreboard { get; set; } = new Scoreboard
        {
            Value = 0,
        };
        private IntcodeComputer IntcodeComputer { get; set; }
        private readonly GameElementFactory _gameElementFactory;
        private TileType[,] _gameBoard;
        private int BallX { get; set; }
        private int PaddleX { get; set; }

        public TileGame(string inputString)
        {
            _gameElementFactory = new GameElementFactory();
            IntcodeComputer = new IntcodeComputer(inputString, new IntcodeIoHandler(new long[]{0}))
            {
                PauseOnOutput = false,
                AwaitInput = true,
            };
        }

        public void RunGame()
        {
            while (IntcodeComputer.State != IntCodeStates.Halted)
            {
                IntcodeComputer.RunUntilAwaitingInput();
                SingleRender();
                var input = CalculateInputFromPosition();
                IntcodeComputer.IntcodeIoHandler.InputList.Add(input);
                if (IntcodeComputer.State == IntCodeStates.AwaitingInput)
                {
                    IntcodeComputer.State = IntCodeStates.Running;
                }
            }
            RenderFooter();
        }

        private void SingleRender()
        {
            if (CanRenderGameState())
            {
                GenerateGameElements();
                if (_gameBoard == null)
                {
                    InitialiseGameBoard();
                }
                ApplyElementsToGameBoard();
                //RenderGameBoard();
                //RenderFooter();
            }
        }

        private int CalculateInputFromPosition()
        {
            return Math.Sign(BallX - PaddleX);
        }
        private void InitialiseGameBoard()
        {
            var xMax = NewElements.Max(t => t.X);
            var yMax = NewElements.Max(t => t.Y);
            _gameBoard = new TileType[xMax + 1, yMax + 1];
        }

        private void RenderFooter()
        {
            Console.WriteLine($"Score: {Scoreboard.Value}, Ball: {BallX}, Paddle: {PaddleX}");
        }

        private bool CanRenderGameState()
        {
            return IntcodeComputer.IntcodeIoHandler.OutputList.Count > 0 &&
                   IntcodeComputer.IntcodeIoHandler.OutputList.Count % 3 == 0;
        }

        private void GenerateGameElements()
        {
            if (CanRenderGameState())
            {
                NewElements = IntcodeComputer.IntcodeIoHandler.OutputList
                    .SplitIntoChunks(3)
                    .Select(c => _gameElementFactory.Create(c))
                    .ToList();
                IntcodeComputer.IntcodeIoHandler.OutputList.Clear();
            }
        }

        private void ApplyElementsToGameBoard()
        {
            NewElements.ForEach(e =>
            {
                if (e.ElementIsScoreboard)
                {
                    Scoreboard = new Scoreboard
                    {
                        Value = e.Value,
                    };
                    return;
                }

                var t = new Tile
                {
                    TileType = (TileType) e.Value,
                    X = e.X,
                    Y = e.Y,
                };
                _gameBoard[t.X, t.Y] = t.TileType;
                switch (t.TileType)
                {
                    case TileType.Paddle:
                        PaddleX = t.X;
                        break;
                    case TileType.Ball:
                        BallX = t.X;
                        break;
                }
            });
        }

        private void RenderGameBoard()
        {
            for (var y = 0; y < _gameBoard.GetLength(1); y++)
            {
                var line = "";
                for (var x = 0; x < _gameBoard.GetLength(0); x++)
                {
                    line += RenderTileType(_gameBoard[x, y]);
                }
                Console.WriteLine(line);
            }
        }

        private static char RenderTileType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Ball:
                    return 'O';
                case TileType.Block:
                    return (char) 9608;
                case TileType.Paddle:
                    return '-';
                case TileType.Wall:
                    return '|';
                default:
                case TileType.Empty:
                    return ' ';
            }
        }
    }
}
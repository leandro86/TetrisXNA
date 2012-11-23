using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TetrisXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TetrisGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private Shape _currentShape;
        private Shape _nextShape;
        private Board _board;

        private Texture2D _shapesTexture;
        private Texture2D _backgroundTexture;
        private Texture2D _levelsTexture;
        private Texture2D _gameOverTexture;
        private Texture2D _youWonTexture;
        private SpriteFont _defaultFont;

        private float _autoFallTimer;
        private float _keyboardLeftRightTimer;
        private float _keyboardDownTimer;
        private float _levelBannerTimer;
        private float _removingLinesTimer;
        private float _levelTimer;

        private float _autoFallInterval = InitialAutoFallInterval;
        
        private const int ShapeInitialPositionX = 3;
        private const int ShapeInitialPositionY = 0;
        private const int LevelsTime = 1;
        private const int BlockSize = 27;
        private const float InitialAutoFallInterval = 0.85F;

        private bool _removingLines;
        private bool _drawFilledLines;
        private int _removedLinesEffectRepeat = 4;
        
        private enum GameState
        {
            InProgress,
            AboutToStart,      
            GameOver,
            GameComplete
        }
        private GameState _currentGameState;
        
        private int _currentScore;
        private int _currentLevel;
        private Block[] _filledLinesBlocks;

        public TetrisGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 519;
            _graphics.PreferredBackBufferHeight = 554;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _board = new Board();
            NewGame(1);           

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _shapesTexture = Content.Load<Texture2D>("Shapes");
            _backgroundTexture = Content.Load<Texture2D>("Background");
            _youWonTexture = Content.Load<Texture2D>("YouWon");
            _levelsTexture = Content.Load<Texture2D>("Levels");
            _gameOverTexture = Content.Load<Texture2D>("GameOver");

            _defaultFont = Content.Load<SpriteFont>("Verdana");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GetInput(elapsed, Keyboard.GetState());

            if (_currentGameState == GameState.AboutToStart)
            {
                _levelBannerTimer += elapsed;
                if (_levelBannerTimer >= 3.0F)
                {
                    _currentGameState = GameState.InProgress;
                    _levelBannerTimer = 0;
                }
            }
            else if (_currentGameState == GameState.InProgress)
            {
                if (IsLevelComplete(elapsed))
                {
                    _currentLevel++;
                    _autoFallInterval -= 0.19F;
                    if (_currentLevel > 5)
                    {
                        _currentLevel--;
                        _currentGameState = GameState.GameComplete;
                    }
                    else
                    {
                        NewGame(_currentLevel);
                    }
                }
                else
                {
                    AutoFallShape(elapsed);
                    if (_removingLines)
                    {
                        DoLinesFilledEffect(elapsed);
                    }
                }
            }

            base.Update(gameTime);
        }

        private bool IsLevelComplete(float elapsed)
        {
            _levelTimer += elapsed;

            if (_levelTimer >= LevelsTime * 60)
            {
                _levelTimer = 0;
                return true;
            }

            return false;
        }

        private void AutoFallShape(float elapsed)
        {
            _autoFallTimer += elapsed;

            if (_autoFallTimer >= _autoFallInterval)
            {
                MoveShapeDown();

                _autoFallTimer = 0;
            }
        }

        private void MoveShapeDown()
        {
            _currentShape.Y++;
            if (_board.IsGroundCollision(_currentShape))
            {
                _currentShape.Y--;
                _board.Put(_currentShape);

                _filledLinesBlocks = _board.GetFilledLinesBlocks();
                if (_filledLinesBlocks.Length != 0)
                {
                    _removingLines = true;

                    int filledLines = _filledLinesBlocks.Length / 10;
                    int scoreMultiplier = filledLines * 10;
                    _currentScore += scoreMultiplier * filledLines;
                }

                if (_board.IsGameOver())
                {
                    _currentGameState = GameState.GameOver;
                    return;
                }

                _currentShape = _nextShape;
                _nextShape = GetRandomShape();
            }
        }

        private void DoLinesFilledEffect(float elapsed)
        {
            _removingLinesTimer += elapsed;
            if (_removingLinesTimer >= 0.05F)
            {                    
                _removingLinesTimer = 0;
                _drawFilledLines = !_drawFilledLines;
                _removedLinesEffectRepeat--;
            }

            if (_removedLinesEffectRepeat == 0)
            {
                _removingLines = false;
                _removedLinesEffectRepeat = 4;
                _board.RemoveFilledLines();
            }
        }

        private void GetInput(float elapsed, KeyboardState keyboardState)
        {
            if (_currentGameState == GameState.GameOver || _currentGameState == GameState.GameComplete)
            {
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    _currentGameState = GameState.AboutToStart;
                    _currentLevel = 1;
                    _autoFallInterval = InitialAutoFallInterval;
                    _currentScore = 0;

                    NewGame(_currentLevel);
                }
            }
            else if (_currentGameState != GameState.AboutToStart)
            {
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    _keyboardLeftRightTimer += elapsed;
                    if (_keyboardLeftRightTimer > 0.15F)
                    {
                        _currentShape.X++;
                        if (_board.IsSidesCollision(_currentShape))
                        {
                            _currentShape.X--;
                        }

                        _keyboardLeftRightTimer = 0;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Left))
                {
                    _keyboardLeftRightTimer += elapsed;
                    if (_keyboardLeftRightTimer > 0.15F)
                    {
                        _currentShape.X--;
                        if (_board.IsSidesCollision(_currentShape))
                        {
                            _currentShape.X++;
                        }

                        _keyboardLeftRightTimer = 0;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Down) && !_removingLines)
                {
                    _keyboardDownTimer += elapsed;
                    if (_keyboardDownTimer > 0.04F)
                    {
                        MoveShapeDown();

                        _keyboardDownTimer = 0;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Up))
                {
                    _keyboardLeftRightTimer += elapsed;
                    if (_keyboardLeftRightTimer > 0.15F)
                    {
                        if (_board.CanRotateShape(_currentShape))
                        {
                            _currentShape.Rotate();
                        }

                        _keyboardLeftRightTimer = 0;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Space))
                {
                    _keyboardLeftRightTimer += elapsed;
                    if (_keyboardLeftRightTimer > 0.15F)
                    {
                        ShapeType currentShape = _currentShape.ShapeType;
                        currentShape += 1;
                        currentShape = (ShapeType)((int)currentShape % 7);

                        _currentShape.ChangeActualShape(currentShape);

                        _keyboardLeftRightTimer = 0;
                    }
                }
                else
                {
                    _keyboardLeftRightTimer = 0.9F;
                    _keyboardDownTimer = 0.9F;
                }
            }
        }

        public void NewGame(int level)
        {
            _currentLevel = level;
            _autoFallTimer = 0;
            _board.Clear();           

            _currentShape = GetRandomShape();
            _nextShape = GetRandomShape();

            _currentGameState = GameState.AboutToStart;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            
            DrawBackground();
            DrawText();
            if (_currentGameState == GameState.AboutToStart)
            {
                DrawCurrentLevelBanner(gameTime);
            }
            else if (_currentGameState == GameState.GameComplete)
            {
                DrawGameCompleteBanner();
            }
            else
            {
                if (_removingLines)
                {
                    DrawLinesFilledEffect();
                }
                else
                {
                    DrawBoard();
                    DrawFallingShape();
                    if (_currentGameState == GameState.GameOver)
                    {
                        DrawGameOverTexture();                        
                    }
                }
                DrawNextShape();
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGameCompleteBanner()
        {
            _spriteBatch.Draw(_youWonTexture, new Vector2(6, 210), Color.White);
        }

        private void DrawCurrentLevelBanner(GameTime gameTime)
        {
            _spriteBatch.Draw(_levelsTexture, new Vector2(6, 210), GetCurrentLevelImageRectangle(), Color.White);
        }

        private void DrawBackground()
        {
            _spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), Color.White);
        }

        private void DrawText()
        {
            _spriteBatch.DrawString(_defaultFont, "Level:", new Vector2(283, 125), Color.CornflowerBlue);
            _spriteBatch.DrawString(_defaultFont, _currentLevel.ToString(), new Vector2(430 , 125), Color.CornflowerBlue);

            _spriteBatch.DrawString(_defaultFont, "Score:", new Vector2(283, 175), Color.CornflowerBlue);
            _spriteBatch.DrawString(_defaultFont, _currentScore.ToString(), new Vector2(430, 175), Color.CornflowerBlue);
        }

        private void DrawGameOverTexture()
        {
            _spriteBatch.Draw(_gameOverTexture, new Vector2(6, 150), Color.White);
        }

        private void DrawNextShape()
        {
            // some calculations to draw the "next shape" sprite right in the center of the background
            int shapeWidth = _nextShape.Width * BlockSize;
            int shapeHeight = _nextShape.Height * BlockSize;
            float x = 283 + (231 / 2 - shapeWidth / 2);
            float y = 7 + (98 / 2 - shapeHeight / 2);

            foreach (Point point in _nextShape)
            {
                _spriteBatch.Draw(_shapesTexture,
                                  new Vector2(x + point.X * BlockSize, y + point.Y * BlockSize),
                                  GetShapeImageRectangle(_nextShape.ShapeType),
                                  Color.White);
            }            
        }

        private void DrawLinesFilledEffect()
        {
            foreach (Block block in _board)
            {
                if (_removingLines & !_drawFilledLines)
                {
                    if (_filledLinesBlocks.Contains(block))
                    {
                        continue;
                    }
                }

                _spriteBatch.Draw(_shapesTexture,
                                  new Vector2(6 + (block.Point.X) * BlockSize, 7 + (block.Point.Y) * BlockSize),
                                  GetShapeImageRectangle(block.ShapeType),
                                  Color.White);
            }            
        }

        private void DrawBoard()
        {
            foreach (Block block in _board)
            {
                Rectangle shapeImageRectangle = _currentGameState == GameState.GameOver
                                                    ? new Rectangle(7 * BlockSize, 0, BlockSize, BlockSize)
                                                    : GetShapeImageRectangle(block.ShapeType);

                _spriteBatch.Draw(_shapesTexture,
                                  new Vector2(6 + (block.Point.X) * BlockSize, 7 + (block.Point.Y) * BlockSize),
                                  shapeImageRectangle,
                                  Color.White);
            }
        }

        private void DrawFallingShape()
        {
            if (_currentGameState == GameState.GameOver)
            {
                return;
            }
            
            foreach (Point point in _currentShape)
            {
                _spriteBatch.Draw(_shapesTexture,
                                  new Vector2(6 + (point.X + _currentShape.X) * BlockSize, 7 + (point.Y + _currentShape.Y) * BlockSize),
                                  GetShapeImageRectangle(_currentShape.ShapeType),
                                  Color.White);
            }
        }

        private Rectangle GetShapeImageRectangle(ShapeType shapeType)
        {            
            return new Rectangle((int)shapeType * BlockSize, 0, BlockSize, BlockSize);
        }

        private Rectangle GetCurrentLevelImageRectangle()
        {
            return new Rectangle((_currentLevel - 1) * 270, 0, 270, 102);
        }

        private Shape GetRandomShape()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            ShapeType randomShapeType = (ShapeType)random.Next(0, 7);

            return new Shape(randomShapeType) {X = ShapeInitialPositionX, Y = ShapeInitialPositionY};
        }
    }
}

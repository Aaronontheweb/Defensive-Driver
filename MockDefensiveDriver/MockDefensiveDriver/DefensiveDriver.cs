using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using MockDefensiveDriver.Entities;
using MockDefensiveDriver.Entities.Cars;

namespace MockDefensiveDriver
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DefensiveDriver : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        #region Entities

        private ScrollingBackground _background;
        private World _world;
        private IList<Car> Cars = new List<Car>();
        private PcCar _pc;
        private Vector2 firstTouchPoint;
        private ScoreBoard _scoreBoard;

        #endregion

        #region Game Content

        private Texture2D _backGroundTexture;
        private Texture2D _whitecarTexture;
        private Texture2D _redcarTexture;
        private Texture2D _bluecarTexture;
        private SoundEffect _explosionSoundEffect;
        private SpriteFont _font;

        #endregion

        #region Game Flags

        private bool _isPcTouched = false;
        private bool _isGameOver = false;

        #endregion

        public DefensiveDriver()
        {
            _graphics = new GraphicsDeviceManager(this)
                            {
                                SupportedOrientations = DisplayOrientation.Portrait,
                                PreferredBackBufferHeight = 800,
                                PreferredBackBufferWidth = 480
                            };
            Content.RootDirectory = "Content";

            _background = new ScrollingBackground();

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
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
            _backGroundTexture = Content.Load<Texture2D>("Content\\Images\\road");
            _whitecarTexture = Content.Load<Texture2D>("Content\\Images\\whitecar");
            _redcarTexture = Content.Load<Texture2D>("Content\\Images\\redcar");
            _bluecarTexture = Content.Load<Texture2D>("Content\\Images\\bluecar");
            World.Explosions = new List<Texture2D>
                              {
                                  Content.Load<Texture2D>("Content\\Images\\explosion_1"),
                                  Content.Load<Texture2D>("Content\\Images\\explosion_2"),
                                  Content.Load<Texture2D>("Content\\Images\\explosion_3"),
                                  Content.Load<Texture2D>("Content\\Images\\explosion_4")
                              };
            _explosionSoundEffect = Content.Load<SoundEffect>("Content\\Audio\\Explosion");
            _font = Content.Load<SpriteFont>("Content\\Fonts\\GameText");
            _background.LoadContent(GraphicsDevice, _backGroundTexture);
            _world = new World(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            _scoreBoard = new ScoreBoard("Your Score: ", ref _world.Bounds, _font);
            
            _pc = new PcCar(_redcarTexture);
            _world.InitializePcCar(_pc);
            _pc.LoadNpcCars(_world, Cars, 17, new Texture2D[] { _whitecarTexture, _bluecarTexture });

            TouchPanel.EnabledGestures = GestureType.FreeDrag;
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // The time since Update was called last.
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(!_isGameOver)
            {
                _background.Update(elapsed * 50);
                HandleTouchInput();

                foreach (var car in Cars)
                {
                    car.Update(gameTime, ref _world.Bounds);
                }
                _pc.Update(gameTime, ref _world.Bounds);
                _scoreBoard.Update(gameTime);

                //Check for NPC collisions against each other
                var collidedCars = _world.CheckNpcCollisions(new List<Car>(Cars));
                foreach(var car in collidedCars)
                {
                    _explosionSoundEffect.Play(1.0f, 0.0f, 0.0f);
                    car.Explode();
                }

                //Check for player collision with NPCs
                Car collidedCar;
                if (_world.CheckNpcHardCollision(Cars, _pc.HardCollisionBoundary, out collidedCar))
                {
                    _explosionSoundEffect.Play(1.0f, 0.0f, 0.0f);
                    _isGameOver = true;
                    collidedCar.Explode();
                    _pc.Explode();
                }
            }

            
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Processes the physics behind user touch input
        /// </summary>
        public void HandleTouchInput()
        {
            var touches = TouchPanel.GetState();

            foreach (var touch in touches)
            {
                switch (touch.State)
                {
                    case TouchLocationState.Pressed:
                        if (_pc.TouchArea.Contains(new Point((int) touch.Position.X, (int) touch.Position.Y)))
                        {
                            _isPcTouched = true;
                            firstTouchPoint = touch.Position;
                        }
                        else
                        {
                            _isPcTouched = false;
                        }

                        break;
                    case TouchLocationState.Moved:
                        if (_isPcTouched)
                        {
                            _pc.Velocity += touch.Position - firstTouchPoint;
                            firstTouchPoint = touch.Position;
                        }

                        break;
                    case TouchLocationState.Released:
                        break;
                }
            }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _background.Draw(_spriteBatch);
            foreach(var car in Cars)
            {
                car.Draw(_spriteBatch);
            }
            _pc.Draw(_spriteBatch);
            _scoreBoard.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

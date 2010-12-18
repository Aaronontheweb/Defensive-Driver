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

        private const int MaxSpawnAttempts = 5;
        private const int MinimumSafeSpawnDistance = 25;
        private Texture2D _backGroundTexture;
        private Texture2D _whitecarTexture;
        private Texture2D _redcarTexture;
        private Texture2D _bluecarTexture;
        public static IList<Texture2D> Explosions;
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
            Explosions = new List<Texture2D>
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
            InitializePcCar(_pc);
            LoadNpcCars(17, new Texture2D[] { _whitecarTexture, _bluecarTexture });

            TouchPanel.EnabledGestures = GestureType.FreeDrag;
        }

        private void LoadNpcCars(int quantity, Texture2D[] textures)
        {
            Cars.Clear(); //Delete any existing cars in operation
            var rand = new Random();
            var laneNum = 0;

            for(var i = 0; i < quantity; i++)
            {
                var spawnAttempts = 0;
                var textureId = rand.Next(0, textures.Count()-1);
                
                var newCar = new Car(textures[textureId]);
                InitializeNpcCar(newCar, laneNum, ref rand);
                OptimizeSpawnPosition(newCar, laneNum, ref rand, ref spawnAttempts);

                //If the car did not hit its number of maximum spawn attempts, assume it safe-spawned and spawn it
                if(spawnAttempts <= MaxSpawnAttempts)
                {
                    Cars.Add(newCar);
                    laneNum++;
                    if (laneNum >= _world.Lanes.Count())
                        laneNum = 0;
                }
                
            }
        }

        private void InitializePcCar(Car car)
        {
            var laneNum = _world.Lanes.Count()/2;
            car.Center = new Vector2((_world.Lanes[laneNum].LaneBox.X + (_world.Lanes[laneNum].LaneBox.Width - car.Width)/2f), _world.Bounds.Height - car.Height);
            car.Velocity = new Vector2(0f, -10f);
        }

        private void InitializeNpcCar(Car car, int laneNum, ref Random rand)
        {
            var yPos = rand.Next(0, _world.Bounds.Height - car.Height);
            var velocity = (rand.Next(-45, 45) + 0.5f);
            car.Center = new Vector2((_world.Lanes[laneNum].LaneBox.X + (_world.Lanes[laneNum].LaneBox.Width - car.Width)/2f), yPos);
            car.Velocity = new Vector2(0f, velocity);
        }

        /// <summary>
        /// Recursively looks for an optimal place to spawn an NPC car where it won't collide with any other NPCs
        /// </summary>
        /// <param name="car">The car we intend to spawn</param>
        /// <param name="laneNum">the lane on the freeway the car belongs to</param>
        /// <param name="rand">a random number generator we pass by reference</param>
        /// <param name="attemptCount">the number of attempts it has taken us to spawn this car</param>
        private void OptimizeSpawnPosition(Car car, int laneNum, ref Random rand, ref int attemptCount)
        {
            Car collisionCar;

            //Create a spawning cusion for each NPC character so nothing spawns on top of the other
            var npcCollisionBoundary = car.CollisionBoundary;
            npcCollisionBoundary.Inflate(MinimumSafeSpawnDistance, MinimumSafeSpawnDistance);


            //Give the PC a much larger spawning cushion to begin the game
            var pcCollisionBoundary = car.CollisionBoundary;
            pcCollisionBoundary.Inflate(MinimumSafeSpawnDistance*3, MinimumSafeSpawnDistance*3);

            if (
                (CheckNpcCollision(npcCollisionBoundary, out collisionCar) ||
                _pc.CollisionBoundary.Intersects(pcCollisionBoundary))
                && attemptCount <= MaxSpawnAttempts
                )
            {

                /* All of this code needs to execute whenever an NPC car has:
                 *  1. Spawned on top of another NPC
                 *  2. Spawned on top of the PC
                 *  3. And the maxmimum number of spawn attempts has not been reached.
                */

                attemptCount++;
                InitializeNpcCar(car, laneNum, ref rand);
                OptimizeSpawnPosition(car, laneNum, ref rand, ref attemptCount);
            }
        }

        /// <summary>
        /// Checks a given area against all NPCs in order to find a collision - event terminates on the first collision in case there's more than one.
        /// </summary>
        /// <param name="toCheck">the area to check for a collision</param>
        /// <param name="collided">a reference to the car that collided with the given rectangle</param>
        /// <returns>true or false</returns>
        private bool CheckNpcCollision(Rectangle toCheck, out Car collided)
        {
            foreach (var car in Cars.Where(car => car.CollisionBoundary.Intersects(toCheck) && car.IsColliding == false))
            {
                collided = car;
                return true;
            }

            collided = null;
            return false;
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

                //Check for player collision with NPCs
                Car collidedCar;
                if (CheckNpcCollision(_pc.CollisionBoundary, out collidedCar))
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

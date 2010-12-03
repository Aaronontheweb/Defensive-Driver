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

namespace MockDefensiveDriver
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MockDefensiveDriver : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        #region Entities

        private ScrollingBackground _background;
        private World _world;

        #endregion 

        #region GameSprites

        private Texture2D _backGroundTexture;
        private Texture2D _whitecarTexture;
        private Texture2D _redcarTexture;

        #endregion 

        public MockDefensiveDriver()
        {
            _graphics = new GraphicsDeviceManager(this)
                            {
                                SupportedOrientations = DisplayOrientation.Portrait,
                                PreferredBackBufferHeight = 800,
                                PreferredBackBufferWidth = 480
                           };
            Content.RootDirectory = "Content";
            _background = new ScrollingBackground();

            _world = new World();

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
            _world.LoadContent(GraphicsDevice, new Texture2D[]{_whitecarTexture, _redcarTexture});
            _background.LoadContent(GraphicsDevice, _backGroundTexture);
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

            _background.Update(elapsed * 50);
            _world.Update();

            base.Update(gameTime);
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
            _world.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

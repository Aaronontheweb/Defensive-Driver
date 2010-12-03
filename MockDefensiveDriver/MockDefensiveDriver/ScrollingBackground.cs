using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MockDefensiveDriver
{
    public class ScrollingBackground
    {

        #region Private members

        private Vector2 _origin, _screenPosition, _texturesize;
        private Texture2D _background;
        private int _screenHeight, _screenWidth;

        #endregion 

        public void LoadContent(GraphicsDevice device, Texture2D background)
        {
            _background = background;
            _screenHeight = device.Viewport.Height;
            _screenWidth = device.Viewport.Width;

            //Set origin of the background to the center of the top edge
            _origin = new Vector2(_background.Width / 2f, 0);

            //Set screen position to center of screen
            _screenPosition = new Vector2(_screenWidth / 2f, 0);

            //Offset to begin drawing the second texture, when necessary
            _texturesize = new Vector2(0, _background.Height);
        }

        public void Update(float deltaY)
        {
            _screenPosition.Y += deltaY;
            _screenPosition.Y = _screenPosition.Y % _background.Height;
        }

        public void Draw( SpriteBatch batch )
        {
            // Draw the texture, if it is still onscreen.
            if(_screenPosition.Y < _screenHeight)
            {
                batch.Draw(_background, _screenPosition, null, Color.White, 0f, _origin, 1f, SpriteEffects.None, 1f);
            }

            //Draw the texture a second time if the first part is beginning to trail off-screen
            batch.Draw(_background, _screenPosition - _texturesize, null, Color.White, 0f, _origin, 1f, SpriteEffects.None, 1f);
        }
    }
}

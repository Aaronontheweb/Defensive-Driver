using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MockDefensiveDriver.Entities
{
    public class ScoreBoard
    {
        public Vector2 Center;
        public const int Width = 200;
        public const int Height = 50;
        public double ElapsedTime { get; private set; }
        public string Message { get; private set; }

        private SpriteFont _font;

        public ScoreBoard(string message, ref Rectangle bounds, SpriteFont font)
        {
            Message = message;
            Center = new Vector2((bounds.Width)/2, Height);
            _font = font;
        }

        public void Update(GameTime time)
        {
            ElapsedTime = time.TotalGameTime.TotalMilliseconds/1000;
            
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(_font, this.Message + String.Format("{0:n}",ElapsedTime),Center,Color.Black, 0f, new Vector2(Width/2, Height/2), Vector2.One, SpriteEffects.None, 0);
        }
    }
}

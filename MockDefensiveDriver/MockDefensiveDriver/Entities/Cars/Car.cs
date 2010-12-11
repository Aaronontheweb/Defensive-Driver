using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MockDefensiveDriver.Entities.Cars
{

    public class Car
    {
        public bool IsColliding { get; set; }
        public Rectangle CollisionBoundary
        {
            get
            {
                var boundingBox =  new Rectangle(
                    (int)(Center.X - _texture.Width / 2 * Scale),
                    (int)(Center.Y - _texture.Height / 2 * Scale),
                    (int)(_texture.Width * Scale),
                    (int)(_texture.Height * Scale));

                //Make it easier to touch the car
                return boundingBox;
            }
        }

        public Rectangle TouchArea
        {
            get
            {
                var boundingBox = new Rectangle(
                    (int)(Center.X - _texture.Width / 2 * Scale),
                    (int)(Center.Y - _texture.Height / 2 * Scale),
                    (int)(_texture.Width * Scale),
                    (int)(_texture.Height * Scale));

                //Make it easier to touch the car
                boundingBox.Inflate(BoxPadding, BoxPadding);
                return boundingBox;
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2 Center;
        public Vector2 Velocity;

        public float Scale { get { return scale; } set { scale = MathHelper.Clamp(value, MinScale, MaxScale); } }

        // the minimum and maximum scale values for the sprite
        public const int BoxPadding = 15;
        public const float MinScale = .5f;
        public const float MaxScale = 2f;

        // this is the percentage of velocity lost each second as
        // the sprite moves around.
        public const float Friction = .9f;

        #region protected members

        protected float scale = 1f;
        protected Texture2D _texture;
        protected int _explosion_sequence = 0;

        #endregion

        public Car(Texture2D texture)
        {
            IsColliding = false;
            _texture = texture;
            Width = _texture.Width;
            Height = _texture.Height;
        }

        public void Explode()
        {
            IsColliding = true;
            Velocity = Vector2.Zero;
        }

        public virtual void Update(GameTime time, ref Rectangle bounds)
        {
            Center += Velocity * (float)time.ElapsedGameTime.TotalSeconds;

            Velocity *= 1f - (Friction*(float) time.ElapsedGameTime.TotalSeconds);

            // calculate the scaled width and height for the method
            ManageBounds(bounds);
        }

        virtual protected void ManageBounds(Rectangle bounds)
        {
            if (IsColliding) return;

            var halfWidth = (Width * Scale) / 2f;
            var halfHeight = (Height * Scale) / 2f;

            if(Center.X < bounds.Left + halfWidth)
            {
                Center.X = bounds.Left + halfWidth;
            }

            if(Center.X > bounds.Right - halfWidth)
            {
                Center.X = bounds.Right - halfWidth;
            }

            if(Center.Y < bounds.Top - halfHeight)
            {
                Center.Y = bounds.Bottom + halfHeight;
            }

            if(Center.Y > bounds.Bottom + halfHeight)
            {
                Center.Y = bounds.Top - halfHeight;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_texture, Center, null, Color.White, 0, new Vector2(Width / 2, Height /2), Scale, SpriteEffects.None, 0);
            if(IsColliding)
            {
                var explosionTexture = MockDefensiveDriver._explosions[_explosion_sequence];
                batch.Draw(explosionTexture, Center, null, Color.White, 0, new Vector2(Width / 2, Height / 2), Scale, SpriteEffects.None, 0);
                if(_explosion_sequence + 1 == MockDefensiveDriver._explosions.Count)
                    _explosion_sequence = 0;
                else
                    _explosion_sequence++;
            }

        }

    }
}

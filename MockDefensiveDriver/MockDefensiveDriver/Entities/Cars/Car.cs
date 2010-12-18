using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MockDefensiveDriver.Entities.Cars
{

    public class Car
    {

        //Contants used by the AI to determine whether or not there is a collision that it needs to avoid
        protected const int CollisionDetectionDistanceX = 25;
        protected const int CollisionDetectionDistanceY = 35;

        public bool IsColliding { get; set; }
        public bool NeedsRespawn { get; set; }

        public Rectangle HardCollisionBoundary
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

        public virtual Rectangle SoftCollisionBoundary
        {
            get
            {
                var boundingBox = new Rectangle(
                    (int)(Center.X - _texture.Width / 2 * Scale),
                    (int)(Center.Y - _texture.Height / 2 * Scale),
                    (int)(_texture.Width * Scale),
                    (int)(_texture.Height * Scale));

                boundingBox.Inflate(CollisionDetectionDistanceX, CollisionDetectionDistanceY);

                //Make it easier to touch the car);
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
        public const int BoxPadding = 65;
        public const float MinScale = .5f;
        public const float MaxScale = 2f;

        //Controls the iterations between explosion animation effects
        public static readonly TimeSpan ExplosionDelay = new TimeSpan(0,0,0,0,150);

        //Controls how long the explosion lasts before the NPC respawns.
        public static readonly TimeSpan ExplosionDuration = new TimeSpan(0,0,0,750);

        // this is the percentage of velocity lost each second as
        // the sprite moves around.
        public const float Friction = .9f;

        #region protected members

        protected float scale = 1f;
        protected Texture2D _texture;
        protected int _explosion_sequence = 0;
        protected DateTime _last_explosion;
        protected DateTime _explosion_start;
        protected Random _rand;

        #endregion

        public Car(Texture2D texture)
        {
            IsColliding = false;
            _texture = texture;
            Width = _texture.Width;
            Height = _texture.Height;
            _rand = new Random();
        }

        public void Explode()
        {
            IsColliding = true;
            _explosion_start = DateTime.Now;
            _explosion_sequence = _rand.Next(0, World.Explosions.Count-1);
        }

        public virtual void Update(GameTime time, ref Rectangle bounds)
        {
            if(!IsColliding)
            {
                Center += Velocity * (float)time.ElapsedGameTime.TotalSeconds;

                // calculate the scaled width and height for the method
                ManageBounds(bounds); 
            }
            else
            {
                if(DateTime.Now - _explosion_start >= ExplosionDuration) //If the explosion has met its duration, respawn.
                {
                    IsColliding = false;
                    NeedsRespawn = true;
                }
            }
            
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
                var explosionTexture = World.Explosions[_explosion_sequence];
                batch.Draw(explosionTexture, Center, null, Color.White, 0, new Vector2(Width / 2, Height / 2), Scale, SpriteEffects.None, 0);
                if(_last_explosion == DateTime.MinValue || (DateTime.Now -_last_explosion >= ExplosionDelay))
                {
                    _last_explosion = DateTime.Now;
                    if (_explosion_sequence + 1 == World.Explosions.Count)
                        _explosion_sequence = 0;
                    else
                        _explosion_sequence++;
                }
                
            }

        }

    }
}

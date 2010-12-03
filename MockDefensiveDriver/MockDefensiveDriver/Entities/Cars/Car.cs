using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MockDefensiveDriver.Entities.Cars
{

    public class Car
    {
        public bool Collided { get; set; }
        public bool IsColliding { get; set; }
        public Rectangle CarBoundary { get; private set; }
        public Lane CurrentLane { get; set; }

        #region private members

        protected Vector2 _origin, _position;
        protected Texture2D _texture;
        protected float _yPos;
        protected float _velocity;
        protected int _duration = 0; //duration in seconds for bounce movements

        #endregion

        public Car(Lane lane)
        {
            Collided = false;
            IsColliding = false;
            CurrentLane = lane;
            var rand = new Random();
            _velocity = (float) (rand.NextDouble() * 3d);
        }

        public void LoadContent(float yPos, Texture2D texture)
        {
            _origin = new Vector2(CurrentLane.LaneBox.X + (CurrentLane.LaneBox.Width / 2), yPos);
            _position = new Vector2(_origin.X, _origin.Y);
            _texture = texture;
            CarBoundary = new Rectangle((int)_origin.X , (int)_origin.Y, texture.Width, texture.Height);
            
        }

        public void Update()
        {
            _position.Y += _velocity;
            //Bounding functions
            if (_position.Y < ((-1) * _texture.Height)) _position.Y = CurrentLane.LaneBox.Height;
            if(_position.Y > (CurrentLane.LaneBox.Height + _texture.Height)) _position.Y = (-1) * _texture.Height;

        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_texture, _position, null, Color.White);
        }

    }
}

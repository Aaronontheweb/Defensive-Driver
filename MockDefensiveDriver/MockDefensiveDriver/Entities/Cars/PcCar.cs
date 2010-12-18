using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MockDefensiveDriver.Entities.Cars
{
    public class PcCar : Car
    {
        public PcCar(Texture2D texture) : base(texture)
        {
        }

        public override Rectangle SoftCollisionBoundary
        {
            get
            {
                var boundingBox = new Rectangle(
                    (int)(Center.X - _texture.Width / 2 * Scale),
                    (int)(Center.Y - _texture.Height / 2 * Scale),
                    (int)(_texture.Width * Scale),
                    (int)(_texture.Height * Scale));

                boundingBox.Inflate(CollisionDetectionDistanceX*3, CollisionDetectionDistanceY*3);

                //Make it easier to touch the car);
                return boundingBox;
            }
        }

        public override void Update(GameTime time, ref Rectangle bounds)
        {
            Center += Velocity * (float)time.ElapsedGameTime.TotalSeconds;

            Velocity *= 1f - (Friction * (float)time.ElapsedGameTime.TotalSeconds);

            // calculate the scaled width and height for the method
            ManageBounds(bounds);
        }

        protected override void ManageBounds(Microsoft.Xna.Framework.Rectangle bounds)
        {
            var halfWidth = (Width * Scale) / 2f;
            var halfHeight = (Height * Scale) / 2f;

            if (Center.X < bounds.Left + halfWidth)
            {
                Center.X = bounds.Left + halfWidth;
            }

            if (Center.X > bounds.Right - halfWidth)
            {
                Center.X = bounds.Right - halfWidth;
            }

            if (Center.Y < bounds.Top + halfHeight)
            {
                Center.Y = bounds.Top + halfHeight;
            }

            if (Center.Y > bounds.Bottom - halfHeight)
            {
                Center.Y = bounds.Bottom - halfHeight;
            }
        }

        public void LoadNpcCars(World world, IList<Car> list, int quantity, Texture2D[] textures)
        {
            list.Clear(); //Delete any existing cars in operation
            var rand = new Random();
            var laneNum = 0;

            for(var i = 0; i < quantity; i++)
            {
                var spawnAttempts = 0;
                var textureId = rand.Next(0, textures.Count()-1);
                
                var newCar = new Car(textures[textureId]);
                world.InitializeNpcCar(newCar, laneNum, ref rand);
                world.OptimizeSpawnPosition(this, list, newCar, laneNum, ref rand, ref spawnAttempts);

                //If the car did not hit its number of maximum spawn attempts, assume it safe-spawned and spawn it
                if(spawnAttempts <= World.MaxSpawnAttempts)
                {
                    list.Add(newCar);
                    laneNum++;
                    if (laneNum >= world.Lanes.Count())
                        laneNum = 0;
                }
                
            }
        }
    }
}

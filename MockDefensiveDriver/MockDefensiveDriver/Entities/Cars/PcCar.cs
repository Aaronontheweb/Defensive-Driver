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
    }
}

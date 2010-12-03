using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockDefensiveDriver.Entities.Cars
{
    public class PCCar : Car
    {
        public PCCar(Lane lane) : base(lane)
        {
            var rand = new Random();
            _velocity = (float)(-1) * (float)(rand.NextDouble() * 3d);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MockDefensiveDriver.Entities.Cars;

namespace MockDefensiveDriver.Entities
{
    

    public class World
    {
        #region private members

        private const int GrassWidth = 24;
        private const int LaneWidth = 73;
        private const int LineWidth = 10;

        private int _laneCount;
        private Lane[] _lanes;
        private Car[] _cars;

        #endregion 

        #region public members

        #endregion

        #region public methods

        public void LoadContent(GraphicsDevice device, Texture2D[] cars)
        {
            _lanes = new Lane[5];
            _lanes[0] = new Lane{ BoundaryType = LaneBoundary.LeftBoundary, IsBoundary = true, LaneBox = new Rectangle(GrassWidth, 0, LaneWidth, device.Viewport.Height)};
            _lanes[1] = new Lane { BoundaryType = LaneBoundary.NoBoundary, IsBoundary = false, LaneBox = new Rectangle(_lanes[0].LaneBox.X + LineWidth + LaneWidth, 0, LaneWidth, device.Viewport.Height) };
            _lanes[2] = new Lane { BoundaryType = LaneBoundary.NoBoundary, IsBoundary = false, LaneBox = new Rectangle(_lanes[1].LaneBox.X + LineWidth + LaneWidth, 0, LaneWidth, device.Viewport.Height) };
            _lanes[3] = new Lane { BoundaryType = LaneBoundary.NoBoundary, IsBoundary = false, LaneBox = new Rectangle(_lanes[2].LaneBox.X + LineWidth + LaneWidth, 0, LaneWidth, device.Viewport.Height) };
            _lanes[4] = new Lane { BoundaryType = LaneBoundary.RightBoundary, IsBoundary = true, LaneBox = new Rectangle(_lanes[3].LaneBox.X + LineWidth + LaneWidth, 0, LaneWidth, device.Viewport.Height) };
            _cars = new Car[7];
            _cars[0] = new Car(_lanes[0]);
            _cars[0].LoadContent(450f, cars[0]);
            _cars[1] = new Car(_lanes[1]);
            _cars[1].LoadContent(120f, cars[0]);
            _cars[2] = new Car(_lanes[2]);
            _cars[2].LoadContent(320f, cars[0]);
            _cars[3] = new Car(_lanes[3]);
            _cars[3].LoadContent(320f, cars[0]);
            _cars[4] = new Car(_lanes[4]);
            _cars[4].LoadContent(131f, cars[0]);
            _cars[5] = new Car(_lanes[4]);
            _cars[5].LoadContent(615f, cars[0]);
            _cars[6] = new PCCar(_lanes[3]);
            _cars[6].LoadContent(device.Viewport.Height - cars[1].Height - 5, cars[1]);
        }

        public void Update()
        {
            foreach(var car in _cars)
            {
                car.Update();
            }
        }

        public void Draw(SpriteBatch batch)
        {
            foreach(var car in _cars)
            {
                car.Draw(batch);
            }
        }

        #endregion 
    }
}

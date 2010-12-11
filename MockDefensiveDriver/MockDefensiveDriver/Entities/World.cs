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

        private const int GrassWidth = 35;
        private const int LaneWidth = 76;
        private const int LineWidth = 13;

        private int _laneCount;
        private Car[] _cars;

        public Rectangle Bounds;

        #endregion 

        #region public members

        public Lane[] Lanes { get; private set; }

        #endregion

        #region public methods

        public World(Rectangle bounds)
        {
            Bounds = bounds;
            Lanes = new Lane[5];
            Lanes[0] = new Lane { BoundaryType = LaneBoundary.LeftBoundary, IsBoundary = true, LaneBox = new Rectangle(GrassWidth, 0, LaneWidth, bounds.Height) };
            Lanes[1] = new Lane { BoundaryType = LaneBoundary.NoBoundary, IsBoundary = false, LaneBox = new Rectangle(Lanes[0].LaneBox.X + LineWidth + LaneWidth, 0, LaneWidth, bounds.Height) };
            Lanes[2] = new Lane { BoundaryType = LaneBoundary.NoBoundary, IsBoundary = false, LaneBox = new Rectangle(Lanes[1].LaneBox.X + LineWidth + LaneWidth, 0, LaneWidth, bounds.Height) };
            Lanes[3] = new Lane { BoundaryType = LaneBoundary.NoBoundary, IsBoundary = false, LaneBox = new Rectangle(Lanes[2].LaneBox.X + LineWidth + LaneWidth, 0, LaneWidth, bounds.Height) };
            Lanes[4] = new Lane { BoundaryType = LaneBoundary.RightBoundary, IsBoundary = true, LaneBox = new Rectangle(Lanes[3].LaneBox.X + LineWidth + LaneWidth, 0, LaneWidth, bounds.Height) };
            
        }

        #endregion 
    }
}

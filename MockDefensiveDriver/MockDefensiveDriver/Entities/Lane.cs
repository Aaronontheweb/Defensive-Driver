using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MockDefensiveDriver.Entities
{
    public enum LaneBoundary
    {
        LeftBoundary,
        NoBoundary,
        RightBoundary
    } ;

    public struct Lane
    {
        public int LaneId { get; set; }
        public bool IsBoundary { get; set; }
        public LaneBoundary BoundaryType { get; set; }
        public Rectangle LaneBox { get; set; }
    }
}

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



        public const int MaxSpawnAttempts = 5;
        public const int MinimumSafeSpawnDistance = 25;
        public static IList<Texture2D> Explosions;

        #region spawning functions

        public void InitializePcCar(Car car)
        {
            var laneNum = Lanes.Count() / 2;
            car.Center = new Vector2((Lanes[laneNum].LaneBox.X + (Lanes[laneNum].LaneBox.Width - car.Width) / 2f), Bounds.Height - car.Height);
            car.Velocity = new Vector2(0f, -10f);
        }

        public void InitializeNpcCar(Car car, int laneNum, ref Random rand)
        {
            var yPos = rand.Next(0, Bounds.Height - car.Height);
            var velocity = (rand.Next(-45, 45) + 0.5f);
            car.Center = new Vector2((Lanes[laneNum].LaneBox.X + (Lanes[laneNum].LaneBox.Width - car.Width) / 2f), yPos);
            car.Velocity = new Vector2(0f, velocity);
        }

        /// <summary>
        /// Recursively looks for an optimal place to spawn an NPC car where it won't collide with any other NPCs
        /// </summary>
        /// <param name="pcCar"></param>
        /// <param name="list"></param>
        /// <param name="car">The car we intend to spawn</param>
        /// <param name="laneNum">the lane on the freeway the car belongs to</param>
        /// <param name="rand">a random number generator we pass by reference</param>
        /// <param name="attemptCount">the number of attempts it has taken us to spawn this car</param>
        public void OptimizeSpawnPosition(PcCar pcCar, IList<Car> list, Car car, int laneNum, ref Random rand, ref int attemptCount)
        {
            Car collisionCar;

            if ((CheckNpcHardCollision(list, car.SoftCollisionBoundary, out collisionCar)
                    || pcCar.SoftCollisionBoundary.Intersects(car.HardCollisionBoundary)
                ) && attemptCount <= MaxSpawnAttempts)
            {

                /* All of this code needs to execute whenever an NPC car has:
                 *  1. Spawned on top of another NPC
                 *  2. Spawned on top of the PC
                 *  3. And the maxmimum number of spawn attempts has not been reached.
                */

                attemptCount++;
                InitializeNpcCar(car, laneNum, ref rand);
                OptimizeSpawnPosition(pcCar, list, car, laneNum, ref rand, ref attemptCount);
            }
        }

        #endregion

        #region Collision detection functions

        public void CheckNpcSoftCollision(IList<Car> cars, PcCar pc)
        {

        }

        /// <summary>
        /// Checks a given area against all NPCs in order to find a collision - event terminates on the first collision in case there's more than one.
        /// </summary>
        /// <param name="cars"></param>
        /// <param name="toCheck">the area to check for a collision</param>
        /// <param name="collided">a reference to the car that collided with the given rectangle</param>
        /// <returns>true or false</returns>
        public bool CheckNpcHardCollision(IList<Car> cars, Rectangle toCheck, out Car collided)
        {
            foreach (var car in cars.Where(car => car.HardCollisionBoundary.Intersects(toCheck) && car.IsColliding == false))
            {
                collided = car;
                return true;
            }

            collided = null;
            return false;
        }

        /// <summary>
        /// Returns a list of all of the NPCs that have collided with eachother
        /// </summary>
        /// <param name="cars"></param>
        /// <returns></returns>
        public IList<Car> CheckNpcCollisions(IList<Car> cars)
        {
            var collidedCars = new List<Car>();

            for (var i = 0; i < cars.Count; i++)
            {
                for (var j = i + 1; j < cars.Count; j++)
                {
                    if (cars[i].HardCollisionBoundary.Intersects(cars[j].HardCollisionBoundary)) //Collision detected
                    {
                        collidedCars.Add(cars[i]);
                        collidedCars.Add(cars[j]);
                        cars.Remove(cars[i]);
                        cars.Remove(cars[j]);
                    }
                }
            }

            return collidedCars;
        }


        #endregion
    }
}

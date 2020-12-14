using System;
using System.Collections.Generic;
using static CurvedTopologyExperiment.Geometry.GeometryHelper;

namespace CurvedTopologyExperiment.Geometry
{
    //GeometryHelper contains all of the methods which make this useful? 
    /// <summary>
    ///  given a direction in degrees (from North) return the Compass Point on an element compass rose.
    /// </summary>
    public class TestCompassRose //ICollection, IEnumerable
    {
        internal protected const int DefaultSize = 8;
        internal protected const float DefaultStep = 360f / DefaultSize;
        internal protected const float DefaultHalfStep = DefaultSize / 2.0f;

        /// <summary>
        /// 
        /// </summary>
        public class Direction
        {
            internal readonly float Start;
            internal readonly float End;
            internal readonly float Centre;
            internal readonly float HalfStep;
            internal readonly CompassDirection CompassDirection;
            public string Name => CompassDirection.ToString();

            internal Direction(CompassDirection compassDirection, float centre, float halfStep) => (CompassDirection, Centre, Start, End, HalfStep) = (compassDirection, centre, Centre - halfStep, Centre + halfStep, halfStep);

            public bool Contains(float degree) => Start < degree && End > degree;

            //Could be extensions methods for CompassDirection
            public Direction RotateRight() => new Direction((CompassDirection)((Start + 90) % 360), Centre, HalfStep);

            public Direction RotateLeft() => new Direction((CompassDirection)((Start + 270) % 360), Centre, HalfStep);

            //Step 22.5 should work with these and they should be added to the enum
            static readonly string[] CardinalDirections = {
              "N", "NNE", "NE", "ENE", 
              "E", "ESE", "SE", "SSE",
              "S", "SSW", "SW", "WSW", 
              "W", "WNW", "NW", "NNW",
              "N"
            };

            private string FormatBearing(double bearing)
            {
                if (bearing < 0 && bearing > -180)
                {
                    // Normalize to [0,360]
                    bearing = 360.0 + bearing;
                }
                if (bearing > 360 || bearing < -180)
                {
                    return "Unknown";
                }

               
                string cardinal = CardinalDirections[(int)Math.Floor(((bearing + 11.25) % 360) / 22.5)];
                return cardinal + " (" + bearing + " deg)";
            }

        }

        /// <summary>
        /// The default points on the compass
        /// </summary>
        public static IEnumerable<CompassDirection> DefaultPoints => ((CompassDirection[])Enum.GetValues(typeof(CompassDirection)));//.Concat(CompassDirection.N);

        /// <summary>
        /// An 8 point compass
        /// </summary>
        public static TestCompassRose CompassRose8 = new TestCompassRose(DefaultSize, DefaultHalfStep);

        /// <summary>
        /// 
        /// </summary>
        protected readonly List<Direction> _directions = new List<Direction>();

        /// <summary>
        /// Points in the compass
        /// </summary>
        public int Points => _directions.Count;

        /// <summary>
        /// Get the <see cref="Direction"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Direction this[int index] => _directions[index];

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Direction> Directions => _directions;

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="size"><see cref="DefaultSize"/></param>
        /// <param name="halfStep"><see cref="DefaultHalfStep"/></param>
        /// <param name="points"><see cref="DefaultPoints"/></param>
        public TestCompassRose(int size = DefaultSize, float halfStep = DefaultHalfStep, IEnumerable<CompassDirection> points = null)
        {
            int k = 0;
            points = points ?? DefaultPoints;
            var pointEnumerator = points.GetEnumerator();
            pointEnumerator.MoveNext();
            for (float i = 0; i <= 360; i += 360.0f / size, ++k)
            {
                Direction d = new Direction(pointEnumerator.Current, i, halfStep);
                _directions.Add(d);
                pointEnumerator.MoveNext();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="heading"></param>
        /// <returns></returns>
        public CompassDirection GetCompassDirection(float heading)
        {
            while (heading < 0)
            {
                heading += 360;
            }
            while (heading > 360)
            {
                heading -= 360;
            }
            foreach (var d in _directions)
            {
                if (d.Contains(heading))
                {
                    return d.CompassDirection;
                }
            }
            return CompassDirection.N;
        }
    }
}

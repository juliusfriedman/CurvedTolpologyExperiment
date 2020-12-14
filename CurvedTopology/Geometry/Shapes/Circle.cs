namespace CurvedTopologyExperiment.Geometry.Shapes
{
    /// <summary>
    /// This class provides operations for handling the usage of Circles and Arcs in Geometries.
    /// </summary>
    public class Circle : Shape
    {
        /// <summary>
        /// Given 2 points defining an arc on the circle, interpolates the circle into a collection of
        /// points that provide connected chords that approximate the arc based on the tolerance value.
        /// The tolerance value specifies the maximum distance between a chord and the circle.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Coordinate> LinearizeCircle(
            NetTopologySuite.Geometries.Coordinate p1, NetTopologySuite.Geometries.Coordinate p2, NetTopologySuite.Geometries.Coordinate p3, double tolerance)
        {
            Circle c = new Circle(p1, p2, p3);
            return c.LinearizeArc(p1, p2, p1, tolerance);
        }

        /// <summary>
        /// Given 2 points defining an arc on the circle, interpolates the circle into a collection of
        /// points that provide connected chords that approximate the arc based on the tolerance value.
        /// This method uses a tolerence value of 1/100 of the length of the radius.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <returns></returns>
        public static System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Coordinate> LinearizeCircle(
            double x1, double y1, double x2, double y2, double x3, double y3)
        {
            NetTopologySuite.Geometries.Coordinate p1 = new (x1, y1);
            NetTopologySuite.Geometries.Coordinate p2 = new (x2, y2);
            NetTopologySuite.Geometries.Coordinate p3 = new (x3, y3);
            Circle c = new Circle(p1, p2, p3);
            double tolerence = 0.01 * c.Radius;
            return c.LinearizeArc(p1, p2, p1, tolerence);
        }

        /// <summary>
        /// Given 2 points defining an arc on the circle, interpolates the circle into a collection of
        /// points that provide connected chords that approximate the arc based on the tolerance value.
        /// The tolerance value specifies the maximum distance between a chord and the circle.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="tolerence">tolerence maximum distance between the center of the chord and the outer edge</param>
        /// <returns></returns>
        public static System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Coordinate> LinearizeCircle(
            double x1, double y1, double x2, double y2, double x3, double y3, double tolerence)
        {
            NetTopologySuite.Geometries.Coordinate p1 = new(x1, y1);
            NetTopologySuite.Geometries.Coordinate p2 = new(x2, y2);
            NetTopologySuite.Geometries.Coordinate p3 = new(x3, y3);
            Circle c = new Circle(p1, p2, p3);
            return c.LinearizeArc(p1, p2, p1, tolerence);
        }

        /// <summary>
        /// Given 2 points defining an arc on the circle, interpolates the circle into a collection of
        /// points that provide connected chords that approximate the arc based on the tolerance value.
        /// This method uses a tolerence value of 1/100 of the length of the radius.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <returns>an ordered list of <see cref="NetTopologySuite.Geometries.Coordinate"/> representing a series of chords approximating the</returns>
        public static System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Coordinate> LinearizeArc(
            double x1, double y1, double x2, double y2, double x3, double y3)
        {
            NetTopologySuite.Geometries.Coordinate p1 = new (x1, y1);
            NetTopologySuite.Geometries.Coordinate p2 = new (x2, y2);
            NetTopologySuite.Geometries.Coordinate p3 = new (x3, y3);
            Circle c = new Circle(p1, p2, p3);
            double tolerence = 0.01 * c.Radius;
            return c.LinearizeArc(p1, p2, p3, tolerence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Coordinate> LinearizeArc(
            double x1, double y1, double x2, double y2, double x3, double y3, double tolerance)
        {
            NetTopologySuite.Geometries.Coordinate p1 = new(x1, y1);
            NetTopologySuite.Geometries.Coordinate p2 = new(x2, y2);
            NetTopologySuite.Geometries.Coordinate p3 = new(x3, y3);
            Circle c = new Circle(p1, p2, p3);
            return c.LinearizeArc(p1, p2, p3, tolerance);
        }

        /// <summary>
        /// A <see cref="Circle"/> with a zero <see cref="Shape.Radii"/>
        /// </summary>
        public new static readonly Circle Empty = new(0f);

        /// <summary>
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="manifold"></param>
        public Circle(NetTopologySuite.Geometries.GeometryFactoryEx factory, Manifold manifold)
            : base(factory, manifold) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="manifold"></param>
        /// <param name="components"></param>
        public Circle(NetTopologySuite.Geometries.GeometryFactoryEx factory, Manifold manifold, System.Numerics.Vector4 components)
            : base(factory, manifold, components) => Add(Factory.CreatePoint());

        /// <summary>
        /// Empty circle at 0,0
        /// </summary>
        /// <param name="radius"></param>
        public Circle(float radius) : this(GeometryHelper.GeometryFactory, default, default) => Radii = (float)radius;

        /// <summary>
        /// Create a circle with a defined center and radius
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public Circle(NetTopologySuite.Geometries.Coordinate center, double radius) : this((float)radius) => Add(Factory.CreatePoint(center));

        /// <summary>
        /// Create a circle using the x/y coordinates for the center.
        /// </summary>
        /// <param name="xCenter"></param>
        /// <param name="yCenter"></param>
        /// <param name="radius"></param>
        public Circle(double xCenter, double yCenter, double radius) : this(new NetTopologySuite.Geometries.Coordinate(xCenter, yCenter), radius) { }

        /// <summary>
        /// Creates a circle based on bounding box.It is possible for the user of this class to pass
        /// bounds to this method that do not represent a square. If this is the case, we must force the
        /// bounding rectangle to be a square. To this end, we check the box and set the side of the box
        /// to the larger dimension of the rectangle
        /// </summary>
        /// <param name="xLeft"></param>
        /// <param name="yUpper"></param>
        /// <param name="xRight"></param>
        /// <param name="yLower"></param>
        public Circle(double xLeft, double yUpper, double xRight, double yLower)
            : this(System.Math.Min(System.Math.Abs(xRight - xLeft), System.Math.Abs(yLower - yUpper)), xLeft, yUpper, xRight, yLower) { }

        /// <summary>
        /// Supports <see cref="Circle"/> construction
        /// </summary>
        /// <param name="side"></param>
        /// <param name="xLeft"></param>
        /// <param name="yUpper"></param>
        /// <param name="xRight"></param>
        /// <param name="yLower"></param>
        protected Circle(double side, double xLeft, double yUpper, double xRight, double yLower)
            : this(new(System.Math.Min(xRight, xLeft) + side / 2, System.Math.Min(yUpper, yLower) + side / 2), side / 2) { }

        /// <summary>
        // Defines the circle based on three points.All three points must be on on the circumference of
        // the circle, and hence, the 3 points cannot be have any pair equal, and cannot form a line.
        // Therefore, each point given is one radius measure from the circle's center.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public Circle(NetTopologySuite.Geometries.Coordinate p1, NetTopologySuite.Geometries.Coordinate p2, NetTopologySuite.Geometries.Coordinate p3)
            : this(new(), 0)
        {
            double a13, b13, c13;
            double a23, b23, c23;
            double x, y, rad;

            // begin pre-calculations for linear system reduction
            a13 = 2 * (p1.X - p3.X);
            b13 = 2 * (p1.Y - p3.Y);
            c13 = p1.Y * p1.Y - p3.Y * p3.Y + (p1.X * p1.X - p3.X * p3.X);
            a23 = 2 * (p2.X - p3.X);
            b23 = 2 * (p2.Y - p3.Y);
            c23 = p2.Y * p2.Y - p3.Y * p3.Y + (p2.X * p2.X - p3.X * p3.X);

            // testsuite-suite to be certain we have three distinct points passed
            double smallNumber = 0.01;
            if ((System.Math.Abs(a13) < smallNumber && System.Math.Abs(b13) < smallNumber)
                    || (System.Math.Abs(a23) < smallNumber && System.Math.Abs(b23) < smallNumber))
            {
                // // points too close so set to default circle
                x = 0;
                y = 0;
                rad = 0;
            }
            else
            {
                // everything is acceptable do the y calculation
                y = (a13 * c23 - a23 * c13) / (a13 * b23 - a23 * b13);
                // x calculation
                // choose best formula for calculation
                if (System.Math.Abs(a13) > System.Math.Abs(a23))
                {
                    x = (c13 - b13 * y) / a13;
                }
                else
                {
                    x = (c23 - b23 * y) / a23;
                }
                // radius calculation
                rad = System.Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y));
            }

            NetTopologySuite.Geometries.Point center = Geometries[0] as NetTopologySuite.Geometries.Point;
            center.X = x;
            center.Y = y;

            Radii = (float)rad;
        }

        /// <summary>
        /// Horizontal, <see cref="Shape.Width"/>
        /// </summary>
        public float Cx => Components.W;

        /// <summary>
        /// Vertical, <see cref="Shape.Height"/>
        /// </summary>
        public float Cy => Components.Z;

        /// <summary>
        /// <inheritdoc/>
        /// Relative to <see cref="NetTopologySuite.Geometries.Geometry.Centroid"/>
        /// </summary>
        public float Radius => Components.X;

        /// <summary>
        /// </summary>
        public float Center => Components.Y;

        /// <summary>
        /// Relative to <see cref="Cx"/>, <see cref="Cy"/> and with respect to <see cref="Radius"/>
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="midAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        public CoordinateSequences.Arc CreateCircularArc(float startAngle, float midAngle, float endAngle) => new CoordinateSequences.Arc(new[]
        {
            Cx + Radius * System.MathF.Cos(startAngle), Cy + Radius * System.MathF.Sin(startAngle),
            Cx + Radius * System.MathF.Cos(midAngle), Cy + Radius * System.MathF.Sin(midAngle),
            Cx + Radius * System.MathF.Cos(endAngle), Cy + Radius * System.MathF.Sin(endAngle),
        });

        /// <summary>
        /// From the given set of, (usually 6)
        /// </summary>
        /// <param name="angles"></param>
        /// <returns></returns>
        public CoordinateSequences.Arc CreateCircularArc(params float[] angles) => new CoordinateSequences.Arc(angles);

        /// <summary>
        /// Is NOT.
        /// </summary>
        public override bool IsRectangle => false;

        /// <summary>
        /// Shift the center of the circle by delta X and delta Y
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public void Shift(double deltaX, double deltaY)
        {
            //These need to return a new circle
            Centroid.X = Centroid.X + deltaX;
            Centroid.Y = Centroid.Y + deltaY;
        }

        /// <summary>
        /// Move the circle to a new center
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(double x, double y)
        {
            //These need to return a new circle
            Centroid.X = x;
            Centroid.Y = y;
        }

        /// <summary>
        /// given a an arc defined from p1 to p2 existing on this circle, returns the height of the
        /// arc. This height is defined as the distance from the center of a chord defined by (p1,p2) 
        /// and the outer edge of the circle.
        /// </summary>
        /// <param name="arc"></param>
        /// <returns></returns>
        public double CalculateArcHeight(CoordinateSequences.Arc arc)
        {
            NetTopologySuite.Geometries.Coordinate chordCenterPt = arc.ChordCenter;
            double dist = arc.DistanceFromCenter(chordCenterPt);
            if (CoordinateSequences.Arc.ComputeAngle(arc).Radians > System.Math.PI)
            {
                return Radius + dist;
            }
            else
            {
                return Radius - dist;
            }
        }

        /// <summary>
        /// Given a set of angles produce the correspoding points which would otherwise represent the coordiantes
        /// </summary>
        /// <remarks>
        /// Could give Coordinates, Points or CoordianteSequence back.
        /// </remarks>
        /// <param name="angles"></param>
        /// <returns></returns>
        public double[] SamplePoints(params double[] angles)
        {
            double[] result = new double[angles.Length * 2];
            int i = 0;
            foreach (double angle in angles)
            {
                result[i++] = Cx + Radius * System.Math.Cos(angle);
                result[i++] = Cy + Radius * System.Math.Sin(angle);
            }

            return result;
        }

        /// <summary>
        /// <see cref="SamplePoints(double[])"/>.
        /// The results are half as long because each coordinate is packed.
        /// </summary>
        /// <param name="angles"></param>
        /// <returns></returns>
        public System.Numerics.Vector4[] SamplePoints(params float[] angles)
        {
            var result = new System.Numerics.Vector4 [angles.Length];
            int i = 0;
            foreach (var angle in angles)
            {
                result[i++] = new(Cx + Radius * System.MathF.Cos(angle), Cy + Radius * System.MathF.Sin(angle), float.NaN, float.NaN);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="midAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        public CoordinateSequences.Arc GetArc(double startAngle, double midAngle, double endAngle)
        {
            return new CoordinateSequences.Arc(new NetTopologySuite.Geometries.Coordinate(Cx + Radius * System.Math.Cos(startAngle), Cy + Radius * System.Math.Sin(startAngle)),
                   new NetTopologySuite.Geometries.Coordinate(Cx + Radius * System.Math.Cos(midAngle), Cy + Radius * System.Math.Sin(midAngle)),
                new NetTopologySuite.Geometries.Coordinate(Cx + Radius * System.Math.Cos(endAngle), Cy + Radius * System.Math.Sin(endAngle)));
        }

        /// <summary>
        /// Given 2 points defining an arc on the circle, interpolates the circle into a collection of
        /// points that provide connected chords that approximate the arc based on the tolerance value.
        /// The tolerance value specifies the maximum distance between a chord and the circle.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="tolerence"></param>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Coordinate> LinearizeArc(
            NetTopologySuite.Geometries.Coordinate p1, NetTopologySuite.Geometries.Coordinate p2, NetTopologySuite.Geometries.Coordinate p3, double tolerence)
        {
            CoordinateSequences.Arc arc = new(p1, p2, p3);

            foreach(var vec in arc.Linearize(tolerence, Factory))
            {
                yield return new(vec.X, vec.Y) { 
                    Z = vec.Z,
                    M = vec.W 
                };
            }
        }
    }
}

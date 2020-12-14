//https://github.com/prestodb/presto/blob/master/presto-geospatial-toolkit/src/main/java/com/facebook/presto/geospatial/Rectangle.java
namespace CurvedTopologyExperiment.Geometry.Shapes
{
    //TODO, make Square and then Derive from from Square so both have the same layout besides Height Width
    public class Rectangle : Shape
    {
        public static readonly Rectangle Infinitite = new Rectangle(
            double.NegativeInfinity,
            double.NegativeInfinity,
            double.PositiveInfinity,
            double.PositiveInfinity);

        public Rectangle(NetTopologySuite.Geometries.GeometryFactoryEx factory, Manifold manifold)
            : base(factory, manifold) { }

        public Rectangle(
                double xMin,
                double yMin,
                double xMax,
                double yMax): this(GeometryHelper.GeometryFactory, Manifold.None)
        {
            //checkArgument(xMin <= xMax, "xMin is greater than xMax");
            //checkArgument(yMin <= yMax, "yMin is greater than yMax");
            Components = new System.Numerics.Vector4((float)xMin, (float)yMin, (float)xMax, (float)yMax);
        }

        public float XMin => Components.X;
        public float YMin => Components.Y;
        public float XMax => Components.Z; 
        public float YMax => Components.W;

        public new double Height => YMax - YMin;

        public bool Intersects(Rectangle other)
        {
            return this.XMin <= other.XMax && this.XMax >= other.XMin &&
                this.YMin <= other.YMax && this.YMax >= other.YMin;
        }

        public Rectangle Intersection(Rectangle other)
        {
            if (!Intersects(other))
            {
                return null;
            }

            return new Rectangle(System.Math.Max(this.XMin, other.XMin), System.Math.Max(this.YMin, other.YMin), System.Math.Min(this.XMax, other.XMax), System.Math.Min(this.YMax, other.YMax));
        }

        public Rectangle Merge(Rectangle other)
        {
            return new Rectangle(System.Math.Min(this.XMin, other.XMin), System.Math.Min(this.YMin, other.YMin), System.Math.Max(this.XMax, other.XMax), System.Math.Max(this.YMax, other.YMax));
        }

        public bool Contains(double x, double y)=> XMin <= x && x <= XMax
                    && YMin <= y && y <= YMax;

        /**
         * Returns if this Rectangle contains only a single point.
         *
         * @return if xMax==xMin and yMax==yMin
         */
        public bool IsPointlike=> XMin == XMax && YMin == YMax;

        public override bool IsRectangle => true;
    }
}

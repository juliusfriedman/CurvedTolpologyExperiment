using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;

namespace CurvedTopologyExperiment.Geometry.Curved
{
    /// <summary>
    /// Not in SqlServer, Shapes / Figures?
    /// </summary>
    public class CircularRing : LinearRing,
        ICurvedGeometry<LinearRing>, CurvedRing
    {
        internal protected static CoordinateSequence FakeRing2d =
            new CoordinateArraySequence(
                    new Coordinate[] {
                        new Coordinate(0, 0),
                        new Coordinate(0, 1),
                        new Coordinate(1, 1),
                        new Coordinate(0, 0)
                    });

        CircularString curved;

        public CircularRing(CoordinateSequence points, GeometryFactory factory, double tolerance)
            :base(FakeRing2d, factory)
        {
            curved = new CircularString(points, factory, tolerance);
            if (!curved.IsClosed)
            {
                throw new System.ArgumentException("Start and end point are not matching, this is not a ring");
            }
        }

        public CircularRing(System.Numerics.Vector4[] controlPoints, GeometryFactory factory, double tolerance)
            : base(FakeRing2d, factory)
        {
            curved = new CircularString(controlPoints, factory, tolerance);
            if (!curved.IsClosed)
            {
                throw new System.ArgumentException("Start and end point are not matching, this is not a ring");
            }
        }

        public double Tolerance => curved.Tolerance;

        public int CoordinatesDimension => curved.CoordinatesDimension;

        bool IGeometry.IsGeometryCollection => false;

        public LinearRing Linearize() => Linearize(Tolerance);

        public LinearRing Linearize(double tolerance)
        {
            CoordinateSequence cs = curved.GetLinearizedCoordinateSequence(tolerance);
            return Factory.CreateLinearRing(cs);
        }

        public string ToCurvedText()
        {
            throw new System.NotImplementedException();
        }

        NetTopologySuite.Geometries.Geometry ICurvedGeometry.Linearize() => Linearize();

        NetTopologySuite.Geometries.Geometry ICurvedGeometry.Linearize(double tolerance) => Linearize(tolerance);
    }
}

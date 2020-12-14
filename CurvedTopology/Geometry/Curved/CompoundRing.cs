using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CurvedTopologyExperiment.Geometry.Curved
{
    /// <summary>
    /// Not in SqlServer, Shapes/Figures?
    /// </summary>
    public class CompoundRing : LinearRing,
         ICompoundCurvedGeometry<LinearRing>, CurvedRing
    {
        CompoundCurve curved;

        public CompoundRing(List<LineString> components, GeometryFactory factory, double tolerance)
            : this(new CompoundCurve(components, factory, tolerance))
        {
        }

        public CompoundRing(CompoundCurve curved) :
            base(CircularRing.FakeRing2d, curved.Factory)
        {
            this.curved = curved;

            // check closed
            if (!curved.IsClosed)
            {
                throw new System.ArgumentException("The components do not form a closed ring");
            }
        }

        public IEnumerable<LineString> Components => curved.Components;

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

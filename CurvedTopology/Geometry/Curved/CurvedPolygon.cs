using NetTopologySuite.Geometries;
using CurvedTopologyExperiment.Geometry.Curved;
using System.Collections.Generic;
using System.Text;

namespace CurvedTopologyExperiment.Geometry
{
    /// <summary>
    /// A subclass of <see cref="Polygon"/> that can host also curves and will linearize if needed
    /// </summary>
    public class CurvePolygon : Polygon, ICurvedGeometry<Polygon>, IPolygonal
    {
        private double tolerance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shell"></param>
        /// <param name="holes"></param>
        /// <param name="factory"></param>
        /// <param name="tolerance"></param>
        public CurvePolygon(LinearRing shell, List<LinearRing> holes, GeometryFactory factory, double tolerance)
                : base(shell, holes.ToArray(), factory)
        {
            this.tolerance = tolerance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shell"></param>
        /// <param name="holes"></param>
        /// <param name="factory"></param>
        /// <param name="tolerance"></param>
        public CurvePolygon(LinearRing shell, LinearRing[] holes, GeometryFactory factory, double tolerance)
                : base(shell, holes, factory)
        {
            this.tolerance = tolerance;
        }

        /// <summary>
        /// I think this is correct
        /// </summary>
        /// <param name="circularString"></param>
        public CurvePolygon(CircularString circularString) 
            : base(circularString.Factory.CreateLinearRing(circularString.Coordinates), circularString.Factory)
        {

        }

        /// <summary>
        /// I think this is correct
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="factory"></param>
        public CurvePolygon(CoordinateSequence coords, GeometryFactory factory)
            : base(factory.CreateLinearRing(coords), factory)
        {

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public Polygon Linearize()=> Linearize(tolerance);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public Polygon Linearize(double tolerance)
        {
            int numHoles = InteriorRings.Length;
            LinearRing shell = Linearize(tolerance, (LinearRing)ExteriorRing);
            LinearRing[] holes = new LinearRing[numHoles];
            for (int k = 0; k < numHoles; ++k)
            {
                LinearRing hole = (LinearRing)InteriorRings[k];
                hole = Linearize(tolerance, hole);
                holes[k] = hole;
            }
            return Factory.CreatePolygon(shell, holes);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tolerance"></param>
        /// <param name="hole"></param>
        /// <returns></returns>
        private LinearRing Linearize(double tolerance, LinearRing hole)
        {
            if (hole is ICurvedLineGeometry<LinearRing> cg)
            {
                hole = cg.Linearize(tolerance);
            }
            return hole;
        }

        public override string GeometryType => "CurvedPolygon";

        public override OgcGeometryType OgcGeometryType => OgcGeometryType.CurvePolygon;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public string ToCurvedText()
        {
            StringBuilder sb = new StringBuilder("CURVEPOLYGON ");
            if (IsEmpty)
            {
                sb.Append(" EMTPY");
            }
            else
            {
                sb.Append("(");
                WriteRing(sb, ExteriorRing);
                int holeNum = InteriorRings.Length;
                for (int k = 0; k < holeNum; ++k)
                {
                    sb.Append(", ");
                    LineString component = InteriorRings[k];
                    WriteRing(sb, component);
                }
                sb.Append(")");
            }
            return sb.ToString();
        }

        private void WriteRing(StringBuilder sb, LineString component)
        {
            if (component is ICurvedGeometry<LineString> curved)
            {
                sb.Append(curved.ToCurvedText());
            }
            else
            {
                sb.Append("(");
                CoordinateSequence cs = component.CoordinateSequence;
                for (int i = 0; i < cs.Count; ++i)
                {
                    sb.Append(cs.GetX(i) + " " + cs.GetY(i));
                    //TODO: ZM?
                    if (i < cs.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(")");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public double Tolerance => tolerance;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int CoordinatesDimension => (int)Dimension.Surface;

        /// <summary>
        /// Has its own type.
        /// </summary>
        bool IGeometry.IsGeometryCollection => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public CurvePolygon Clone(double?tolerance = null)=> new CurvePolygon(Shell, Holes, Factory, tolerance ?? this.tolerance);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToCurvedText();

        NetTopologySuite.Geometries.Geometry ICurvedGeometry.Linearize() => Linearize();

        NetTopologySuite.Geometries.Geometry ICurvedGeometry.Linearize(double tolerance) => Linearize(tolerance);
    }
}
